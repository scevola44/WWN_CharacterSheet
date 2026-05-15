using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WWN.Application.Email;
using WWN.Infrastructure.Email;
using WWN.Infrastructure.Identity;

namespace WWN.Web.Endpoints;

public static class AuthEndpoints
{
    private const string AdminRole = "Admin";
    private const string EmailNotConfirmedError = "email_not_confirmed";
    private const string GenericConfirmationResponse = "If an account with that email exists, a confirmation email has been sent.";
    private const string GenericPasswordResetResponse = "If an account with that email exists, a password reset email has been sent.";

    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async (
            RegisterRequest req,
            UserManager<AppUser> userManager,
            IEmailSender emailSender,
            IOptions<AppUrlsOptions> appUrls,
            IConfiguration configuration,
            ILogger<AuthEndpointsMarker> logger) =>
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return Results.BadRequest(new { message = "Email and password are required." });

            var user = new AppUser
            {
                UserName = req.Email,
                Email = req.Email,
            };

            var result = await userManager.CreateAsync(user, req.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return Results.BadRequest(new { message = "Registration failed.", errors });
            }

            await AssignAdminRoleIfConfigured(user, userManager, configuration);
            await SendConfirmationEmailAsync(user, userManager, emailSender, appUrls.Value, logger);

            return Results.Ok(new { message = "Registration successful. Please check your email to confirm your account." });
        });

        group.MapPost("/login", async (
            LoginRequest req,
            UserManager<AppUser> userManager,
            IConfiguration configuration) =>
        {
            var user = await userManager.FindByEmailAsync(req.Email);
            if (user is null || !await userManager.CheckPasswordAsync(user, req.Password))
                return Results.Unauthorized();

            if (!user.EmailConfirmed)
            {
                return Results.Json(
                    new { error = EmailNotConfirmedError, message = "Please confirm your email before signing in." },
                    statusCode: StatusCodes.Status403Forbidden);
            }

            await AssignAdminRoleIfConfigured(user, userManager, configuration);

            var token = await GenerateJwtTokenAsync(user, userManager, configuration);
            return Results.Ok(new { token });
        });

        group.MapGet("/me", (ClaimsPrincipal principal) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var isAdmin = principal.IsInRole(AdminRole);
            return Results.Ok(new { userId, email, isAdmin });
        }).RequireAuthorization();

        group.MapPost("/confirm-email", async (
            ConfirmEmailRequest req,
            UserManager<AppUser> userManager) =>
        {
            if (string.IsNullOrWhiteSpace(req.UserId) || string.IsNullOrWhiteSpace(req.Token))
                return Results.BadRequest(new { message = "Invalid confirmation link." });

            var user = await userManager.FindByIdAsync(req.UserId);
            if (user is null)
                return Results.BadRequest(new { message = "Invalid confirmation link." });

            if (!TryDecodeToken(req.Token, out var decoded))
                return Results.BadRequest(new { message = "Invalid confirmation link." });

            var result = await userManager.ConfirmEmailAsync(user, decoded);
            if (!result.Succeeded)
                return Results.BadRequest(new { message = "Invalid or expired confirmation link." });

            return Results.Ok(new { message = "Email confirmed. You can now sign in." });
        });

        group.MapPost("/resend-confirmation", async (
            ResendConfirmationRequest req,
            UserManager<AppUser> userManager,
            IEmailSender emailSender,
            IOptions<AppUrlsOptions> appUrls,
            ILogger<AuthEndpointsMarker> logger) =>
        {
            if (!string.IsNullOrWhiteSpace(req.Email))
            {
                var user = await userManager.FindByEmailAsync(req.Email);
                if (user is not null && !user.EmailConfirmed)
                {
                    await SendConfirmationEmailAsync(user, userManager, emailSender, appUrls.Value, logger);
                }
            }
            return Results.Ok(new { message = GenericConfirmationResponse });
        });

        group.MapPost("/forgot-password", async (
            ForgotPasswordRequest req,
            UserManager<AppUser> userManager,
            IEmailSender emailSender,
            IOptions<AppUrlsOptions> appUrls,
            ILogger<AuthEndpointsMarker> logger) =>
        {
            if (!string.IsNullOrWhiteSpace(req.Email))
            {
                var user = await userManager.FindByEmailAsync(req.Email);
                if (user is not null && user.EmailConfirmed)
                {
                    await SendPasswordResetEmailAsync(user, userManager, emailSender, appUrls.Value, logger);
                }
            }
            return Results.Ok(new { message = GenericPasswordResetResponse });
        });

        group.MapPost("/reset-password", async (
            ResetPasswordRequest req,
            UserManager<AppUser> userManager) =>
        {
            if (string.IsNullOrWhiteSpace(req.UserId) || string.IsNullOrWhiteSpace(req.Token) || string.IsNullOrWhiteSpace(req.NewPassword))
                return Results.BadRequest(new { message = "Invalid password reset request." });

            var user = await userManager.FindByIdAsync(req.UserId);
            if (user is null)
                return Results.BadRequest(new { message = "Invalid or expired password reset link." });

            if (!TryDecodeToken(req.Token, out var decoded))
                return Results.BadRequest(new { message = "Invalid or expired password reset link." });

            var result = await userManager.ResetPasswordAsync(user, decoded, req.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return Results.BadRequest(new { message = "Password reset failed.", errors });
            }

            return Results.Ok(new { message = "Password reset successful. You can now sign in." });
        });
    }

    private static async Task SendConfirmationEmailAsync(
        AppUser user,
        UserManager<AppUser> userManager,
        IEmailSender emailSender,
        AppUrlsOptions appUrls,
        ILogger logger)
    {
        try
        {
            var rawToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = EncodeToken(rawToken);
            var link = BuildLink(appUrls.WebBaseUrl, "/confirm-email", user.Id, encodedToken);
            var body = $"""
                <p>Welcome to WWN Character Sheet!</p>
                <p>Please confirm your account by clicking the link below:</p>
                <p><a href="{link}">Confirm my email</a></p>
                <p>If you didn't create an account, you can ignore this email.</p>
                """;
            await emailSender.SendAsync(user.Email!, "Confirm your WWN Character Sheet account", body);
        }
        catch (Exception ex)
        {
            // Don't fail the HTTP request if email delivery fails — the user can request a resend.
            logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
        }
    }

    private static async Task SendPasswordResetEmailAsync(
        AppUser user,
        UserManager<AppUser> userManager,
        IEmailSender emailSender,
        AppUrlsOptions appUrls,
        ILogger logger)
    {
        try
        {
            var rawToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = EncodeToken(rawToken);
            var link = BuildLink(appUrls.WebBaseUrl, "/reset-password", user.Id, encodedToken);
            var body = $"""
                <p>We received a request to reset the password for your WWN Character Sheet account.</p>
                <p>Click the link below to choose a new password:</p>
                <p><a href="{link}">Reset my password</a></p>
                <p>If you didn't request this, you can safely ignore this email.</p>
                """;
            await emailSender.SendAsync(user.Email!, "Reset your WWN Character Sheet password", body);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
        }
    }

    private static string BuildLink(string baseUrl, string path, string userId, string encodedToken)
    {
        var trimmedBase = baseUrl.TrimEnd('/');
        var query = QueryHelpers.AddQueryString(path, new Dictionary<string, string?>
        {
            ["userId"] = userId,
            ["token"] = encodedToken,
        });
        return $"{trimmedBase}{query}";
    }

    private static string EncodeToken(string rawToken)
        => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));

    private static bool TryDecodeToken(string encoded, out string decoded)
    {
        try
        {
            decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encoded));
            return true;
        }
        catch (FormatException)
        {
            decoded = string.Empty;
            return false;
        }
    }

    private static async Task AssignAdminRoleIfConfigured(
        AppUser user, UserManager<AppUser> userManager, IConfiguration configuration)
    {
        var configured = (configuration["AdminEmails"] ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (configured.Any(e => e.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
            && !await userManager.IsInRoleAsync(user, AdminRole))
        {
            await userManager.AddToRoleAsync(user, AdminRole);
        }
    }

    private static async Task<string> GenerateJwtTokenAsync(
        AppUser user, UserManager<AppUser> userManager, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Marker type for logger category names in the static endpoint handlers.
    private sealed class AuthEndpointsMarker { }
}

public record RegisterRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password);

public record LoginRequest(
    [property: Required] string Email,
    [property: Required] string Password);

public record ConfirmEmailRequest(
    [property: Required] string UserId,
    [property: Required] string Token);

public record ResendConfirmationRequest(
    [property: Required, EmailAddress] string Email);

public record ForgotPasswordRequest(
    [property: Required, EmailAddress] string Email);

public record ResetPasswordRequest(
    [property: Required] string UserId,
    [property: Required] string Token,
    [property: Required] string NewPassword);
