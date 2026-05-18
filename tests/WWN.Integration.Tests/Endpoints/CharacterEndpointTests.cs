using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WWN.Application.DTOs;
using WWN.Infrastructure.Persistence;

namespace WWN.Integration.Tests.Endpoints;

public class CharacterEndpointTests : IClassFixture<CharacterEndpointTests.CustomFactory>, IDisposable
{
    private class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "test-user-id") };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    private readonly HttpClient _client;
    private readonly SqliteConnection _connection;

    public class CustomFactory : WebApplicationFactory<Program>
    {
        public SqliteConnection Connection { get; }

        public CustomFactory()
        {
            // Environment variables are read by WebApplication.CreateBuilder immediately,
            // before Program.cs evaluates builder.Configuration["Jwt:Key"].
            // ConfigureAppConfiguration callbacks are deferred until app.Build() and arrive too late.
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
                // The real signing key lives in user-secrets / env vars; supply a deterministic
                // one here so Program.cs can boot during integration tests.
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "test-only-signing-key-must-be-at-least-32-chars-long"
                });
            });

            builder.ConfigureServices(services =>
            {
                // Override authentication with test handler instead of JWT
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", null);

                services.Configure<AuthenticationOptions>(o =>
                {
                    o.DefaultAuthenticateScheme = "Test";
                    o.DefaultChallengeScheme = "Test";
                });

                // Replace DbContext with in-memory SQLite
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<WwnDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<WwnDbContext>(opt =>
                    opt.UseSqlite(Connection));
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) Connection.Dispose();
            base.Dispose(disposing);
        }
    }

    public CharacterEndpointTests(CustomFactory factory)
    {
        _connection = factory.Connection;
        _client = factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    private async Task<Guid> CreateTestCharacter()
    {
        var req = new CreateCharacterRequest
        {
            Name = "Test Hero",
            Class = "Warrior",
            Attributes = new Dictionary<string, int>
            {
                ["Strength"] = 14,
                ["Dexterity"] = 12,
                ["Constitution"] = 10,
                ["Intelligence"] = 10,
                ["Wisdom"] = 10,
                ["Charisma"] = 10
            }
        };
        var response = await _client.PostAsJsonAsync("/api/characters", req);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<IdResponse>();
        return result!.Id;
    }

    [Fact]
    public async Task POST_CreateCharacter_Returns201()
    {
        var id = await CreateTestCharacter();
        id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GET_ListCharacters_ReturnsArray()
    {
        await CreateTestCharacter();
        var response = await _client.GetAsync("/api/characters");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<CharacterSummaryDto>>();
        list.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GET_GetCharacter_ReturnsDetailWithDerivedStats()
    {
        var id = await CreateTestCharacter();
        var response = await _client.GetAsync($"/api/characters/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<CharacterDetailDto>();
        dto.Should().NotBeNull();
        dto!.Name.Should().Be("Test Hero");
        dto.DerivedStats.Should().NotBeNull();
        dto.DerivedStats.ArmorClass.Should().BeGreaterThan(0);
        dto.Attributes.Should().HaveCount(6);
        dto.Skills.Should().HaveCount(21);
    }

    [Fact]
    public async Task GET_NonExistent_Returns404()
    {
        var response = await _client.GetAsync($"/api/characters/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PUT_UpdateAttribute_RecalculatesDerived()
    {
        var id = await CreateTestCharacter();
        var initial = await _client.GetFromJsonAsync<CharacterDetailDto>($"/api/characters/{id}");
        var initialAc = initial!.DerivedStats.ArmorClass;

        var response = await _client.PutAsJsonAsync($"/api/characters/{id}/attributes/Dexterity",
            new UpdateAttributeRequest { Score = 18 });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<CharacterDetailDto>();
        updated!.DerivedStats.ArmorClass.Should().BeGreaterThan(initialAc);
    }

    [Fact]
    public async Task POST_CreateCharacter_WithLevel_PersistsLevel()
    {
        var req = new CreateCharacterRequest
        {
            Name = "Veteran",
            Class = "Warrior",
            Level = 5,
            MaxHitPoints = 25,
            Attributes = new Dictionary<string, int>
            {
                ["Strength"] = 14,
                ["Dexterity"] = 12,
                ["Constitution"] = 12,
                ["Intelligence"] = 10,
                ["Wisdom"] = 10,
                ["Charisma"] = 10
            }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/characters", req);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var id = (await createResponse.Content.ReadFromJsonAsync<IdResponse>())!.Id;

        var dto = await _client.GetFromJsonAsync<CharacterDetailDto>($"/api/characters/{id}");
        dto!.Level.Should().Be(5);
    }

    [Fact]
    public async Task POST_LevelUp_IncrementsLevelAndAddsHp()
    {
        var id = await CreateTestCharacter();
        var before = await _client.GetFromJsonAsync<CharacterDetailDto>($"/api/characters/{id}");

        var response = await _client.PostAsJsonAsync($"/api/characters/{id}/levelup",
            new LevelUpRequest { HpGain = 5 });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<CharacterDetailDto>();
        dto!.Level.Should().Be(before!.Level + 1);
        dto.MaxHitPoints.Should().Be(before.MaxHitPoints + 5);
        dto.CurrentHitPoints.Should().Be(before.CurrentHitPoints + 5);
    }

    [Fact]
    public async Task PUT_SetHp_Returns200()
    {
        var id = await CreateTestCharacter();
        var response = await _client.PutAsJsonAsync($"/api/characters/{id}/hp",
            new SetHpRequest { MaxHitPoints = 20, CurrentHitPoints = 15 });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<CharacterDetailDto>();
        dto!.MaxHitPoints.Should().Be(20);
        dto.CurrentHitPoints.Should().Be(15);
    }

    [Fact]
    public async Task DELETE_Character_Returns204()
    {
        var id = await CreateTestCharacter();
        var response = await _client.DeleteAsync($"/api/characters/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task POST_AddWeapon_ShowsInInventory()
    {
        var id = await CreateTestCharacter();
        var addResponse = await _client.PostAsJsonAsync($"/api/characters/{id}/items",
            new AddItemRequest
            {
                Name = "Longsword",
                Encumbrance = 1,
                ItemType = "Weapon",
                DamageDieCount = 1,
                DamageDieSides = 8,
                AttributeModifier = "Strength",
                WeaponType = "Melee"
            });
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await addResponse.Content.ReadFromJsonAsync<CharacterDetailDto>();
        dto!.Inventory.Should().HaveCount(1);
        dto.Inventory[0].ItemType.Should().Be("Weapon");
    }

    private record IdResponse(Guid Id);
}
