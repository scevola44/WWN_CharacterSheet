using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WWN.Infrastructure.Identity;

namespace WWN.Web.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async (
            RegisterRequest req,
            UserManager<AppUser> userManager) =>
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

            var token = GenerateJwtToken(user, configuration);
            return Results.Ok(new { token });
        });

        group.MapGet("/me", (ClaimsPrincipal principal) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = principal.FindFirstValue(ClaimTypes.Email);
            return Results.Ok(new { userId, email });
        }).RequireAuthorization();
    }

    private static string GenerateJwtToken(AppUser user, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
        };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);
