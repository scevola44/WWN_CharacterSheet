using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WWN.Application.Email;
using WWN.Infrastructure.Persistence;

namespace WWN.Integration.Tests.Endpoints;

[Collection("WebHostShared")]
public class AuthEndpointTests : IClassFixture<AuthEndpointTests.CustomFactory>, IDisposable
{
    public sealed record CapturedEmail(string To, string Subject, string Body);

    public sealed class FakeEmailSender : IEmailSender
    {
        private readonly List<CapturedEmail> _sent = new();
        private readonly object _lock = new();

        public Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
        {
            lock (_lock) _sent.Add(new CapturedEmail(to, subject, htmlBody));
            return Task.CompletedTask;
        }

        public IReadOnlyList<CapturedEmail> Snapshot()
        {
            lock (_lock) return _sent.ToList();
        }
    }

    public class CustomFactory : WebApplicationFactory<Program>
    {
        public SqliteConnection Connection { get; }
        public FakeEmailSender EmailSender { get; } = new();

        public CustomFactory()
        {
            Environment.SetEnvironmentVariable("Jwt__Key", "integration-test-secret-key-long-enough-for-hmacsha256");
            Environment.SetEnvironmentVariable("Jwt__Issuer", "test-issuer");
            Environment.SetEnvironmentVariable("Jwt__Audience", "test-audience");
            Connection = new SqliteConnection("Data Source=:memory:");
            Connection.Open();
        }

        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "test-only-signing-key-must-be-at-least-32-chars-long",
                    ["AppUrls:WebBaseUrl"] = "http://localhost:5173"
                });
            });

            builder.ConfigureServices(services =>
            {
                // Replace SMTP sender with in-memory fake
                var emailDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailSender));
                if (emailDescriptor != null) services.Remove(emailDescriptor);
                services.AddSingleton<IEmailSender>(EmailSender);

                // Replace DbContext with in-memory SQLite
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<WwnDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<WwnDbContext>(opt => opt.UseSqlite(Connection));
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) Connection.Dispose();
            base.Dispose(disposing);
        }
    }

    private readonly CustomFactory _factory;
    private readonly HttpClient _client;

    public AuthEndpointTests(CustomFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public void Dispose() => _client.Dispose();

    private static string UniqueEmail() => $"user-{Guid.NewGuid():N}@example.test";

    private async Task<string> RegisterUser(string email, string password = "password123")
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register",
            new { email, password });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return email;
    }

    private CapturedEmail LastEmailContaining(string recipient, string pathFragment)
        => _factory.EmailSender.Snapshot()
            .Where(e => e.To == recipient && e.Body.Contains(pathFragment))
            .Last();

    private static (string UserId, string Token) ExtractLinkParams(string body, string path)
    {
        // Matches the encoded URL produced by AuthEndpoints.BuildLink, e.g.
        //   http://localhost:5173/confirm-email?userId=abc&token=xyz
        var pattern = new Regex(Regex.Escape(path) + @"\?([^""\s<]+)");
        var match = pattern.Match(body);
        match.Success.Should().BeTrue($"the email body should contain a {path} link");
        var query = HttpUtility.ParseQueryString(match.Groups[1].Value);
        return (query["userId"]!, query["token"]!);
    }

    [Fact]
    public async Task Register_SendsConfirmationEmail_AndLeavesUnconfirmed()
    {
        var email = UniqueEmail();
        await RegisterUser(email);

        var sent = _factory.EmailSender.Snapshot().SingleOrDefault(e => e.To == email);
        sent.Should().NotBeNull();
        sent!.Subject.Should().Contain("Confirm");
        sent.Body.Should().Contain("/confirm-email?");
    }

    [Fact]
    public async Task Login_BeforeConfirmation_Returns403WithCode()
    {
        var email = UniqueEmail();
        await RegisterUser(email);

        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new { email, password = "password123" });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        body!["error"].Should().Be("email_not_confirmed");
    }

    [Fact]
    public async Task ConfirmEmail_WithValidToken_AllowsLogin()
    {
        var email = UniqueEmail();
        await RegisterUser(email);

        var (userId, token) = ExtractLinkParams(LastEmailContaining(email, "/confirm-email").Body, "/confirm-email");
        var confirm = await _client.PostAsJsonAsync("/api/auth/confirm-email", new { userId, token });
        confirm.StatusCode.Should().Be(HttpStatusCode.OK);

        var login = await _client.PostAsJsonAsync("/api/auth/login",
            new { email, password = "password123" });
        login.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await login.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        payload!["token"].Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ConfirmEmail_WithBadToken_Returns400()
    {
        var email = UniqueEmail();
        await RegisterUser(email);
        var (userId, _) = ExtractLinkParams(LastEmailContaining(email, "/confirm-email").Body, "/confirm-email");

        var response = await _client.PostAsJsonAsync("/api/auth/confirm-email",
            new { userId, token = "not-a-real-token" });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ResendConfirmation_AlwaysReturns200_AndSendsForUnconfirmedUser()
    {
        var email = UniqueEmail();
        await RegisterUser(email);
        var initialCount = _factory.EmailSender.Snapshot().Count(e => e.To == email);

        var response = await _client.PostAsJsonAsync("/api/auth/resend-confirmation", new { email });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.EmailSender.Snapshot().Count(e => e.To == email).Should().Be(initialCount + 1);

        // Same endpoint also returns 200 for unknown emails (no leak).
        var unknown = await _client.PostAsJsonAsync("/api/auth/resend-confirmation",
            new { email = UniqueEmail() });
        unknown.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ForgotAndResetPassword_RoundTrip_AllowsLoginWithNewPassword()
    {
        var email = UniqueEmail();
        await RegisterUser(email);

        // Confirm first — forgot-password should only fire for confirmed users.
        var (confirmUserId, confirmToken) = ExtractLinkParams(LastEmailContaining(email, "/confirm-email").Body, "/confirm-email");
        var confirmResp = await _client.PostAsJsonAsync("/api/auth/confirm-email",
            new { userId = confirmUserId, token = confirmToken });
        confirmResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var forgot = await _client.PostAsJsonAsync("/api/auth/forgot-password", new { email });
        forgot.StatusCode.Should().Be(HttpStatusCode.OK);

        var (resetUserId, resetToken) = ExtractLinkParams(LastEmailContaining(email, "/reset-password").Body, "/reset-password");
        var newPassword = "newpassword456";
        var reset = await _client.PostAsJsonAsync("/api/auth/reset-password",
            new { userId = resetUserId, token = resetToken, newPassword });
        reset.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginOld = await _client.PostAsJsonAsync("/api/auth/login",
            new { email, password = "password123" });
        loginOld.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var loginNew = await _client.PostAsJsonAsync("/api/auth/login",
            new { email, password = newPassword });
        loginNew.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ForgotPassword_UnknownEmail_StillReturns200()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/forgot-password",
            new { email = UniqueEmail() });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
