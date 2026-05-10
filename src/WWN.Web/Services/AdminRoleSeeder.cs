using Microsoft.AspNetCore.Identity;
using WWN.Infrastructure.Identity;

namespace WWN.Web.Services;

public class AdminRoleSeeder(
    RoleManager<IdentityRole> roleManager,
    UserManager<AppUser> userManager,
    IConfiguration configuration)
{
    private const string AdminRole = "Admin";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!await roleManager.RoleExistsAsync(AdminRole))
            await roleManager.CreateAsync(new IdentityRole(AdminRole));

        var emails = (configuration["AdminEmails"] ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var email in emails)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is not null && !await userManager.IsInRoleAsync(user, AdminRole))
                await userManager.AddToRoleAsync(user, AdminRole);
        }
    }
}
