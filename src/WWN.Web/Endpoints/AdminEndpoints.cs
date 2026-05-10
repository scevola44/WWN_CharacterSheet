using WWN.Application.Services;

namespace WWN.Web.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin").WithTags("Admin").RequireAuthorization("AdminOnly");

        group.MapPost("/reload-lookups", async (
            FocusDefinitionSeeder focusSeeder,
            SpellDefinitionSeeder spellSeeder,
            ArtDefinitionSeeder artSeeder,
            ClassAbilitySeeder abilitySeeder,
            CancellationToken ct) =>
        {
            await focusSeeder.ForceReseedAsync(ct);
            await spellSeeder.ForceReseedAsync(ct);
            await artSeeder.ForceReseedAsync(ct);
            await abilitySeeder.ForceReseedAsync(ct);
            return Results.Ok(new { message = "Lookup tables reloaded." });
        });
    }
}
