using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WWN.Infrastructure.Identity;

namespace WWN.Web.Endpoints;

public static class AuthEndpoints
{
    private const string AdminRole = "Admin";

    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async (
            RegisterRequest req,
            UserManager<AppUser> userManager,
            IConfiguration configuration) =>
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

            return Results.Ok(new { message = "Registration successful." });
        });

        group.MapPost("/login", async (
            LoginRequest req,
            UserManager<AppUser> userManager,
            IConfiguration configuration) =>
        {
            var user = await userManager.FindByEmailAsync(req.Email);
            if (user is null || !await userManager.CheckPasswordAsync(user, req.Password))
                return Results.Unauthorized();

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
}

public record RegisterRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password);

public record LoginRequest(
    [property: Required] string Email,
    [property: Required] string Password);
