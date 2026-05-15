using System.Security.Claims;
using WWN.Application.DTOs;
using WWN.Application.Services;

namespace WWN.Web.Endpoints;

public static class SpellEndpoints
{
    private const string AdminRole = "Admin";

    public static void MapSpellEndpoints(this IEndpointRouteBuilder app)
    {
        var spellGroup = app.MapGroup("/api/spells").WithTags("Spells");

        spellGroup.MapGet("/", async (ClaimsPrincipal principal, SpellService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var spells = await svc.ListSpellsAsync(userId, ct);
            return Results.Ok(spells);
        });

        spellGroup.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal principal, SpellService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var spell = await svc.GetSpellAsync(id, userId, ct);
            return spell is not null ? Results.Ok(spell) : Results.NotFound();
        });

        spellGroup.MapPost("/", async (CreateSpellRequest req, ClaimsPrincipal principal, SpellService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.CreateSpellAsync(req, userId, ct);
            return Results.Created($"/api/spells/{dto.Id}", dto);
        }).RequireAuthorization();

        spellGroup.MapPut("/{id:guid}", async (Guid id, UpdateSpellRequest req, ClaimsPrincipal principal, SpellService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = principal.IsInRole(AdminRole);
            var dto = await svc.UpdateSpellAsync(id, req, userId, isAdmin, ct);
            return dto is not null ? Results.Ok(dto) : Results.NotFound();
        }).RequireAuthorization();

        spellGroup.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal principal, SpellService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = principal.IsInRole(AdminRole);
            await svc.DeleteSpellAsync(id, userId, isAdmin, ct);
            return Results.NoContent();
        }).RequireAuthorization();

        var charSpellGroup = app.MapGroup("/api/characters/{charId:guid}/spells")
            .WithTags("Character Spells")
            .RequireAuthorization();

        charSpellGroup.MapGet("/", async (Guid charId, ClaimsPrincipal principal,
            CharacterIdentityService charSvc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var character = await charSvc.GetCharacterAsync(charId, userId, ct);
            return character is not null ? Results.Ok(character) : Results.NotFound();
        });

        charSpellGroup.MapPost("/{spellId:guid}", async (Guid charId, Guid spellId,
            ClaimsPrincipal principal, CharacterSpellService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var knownSpell = await svc.LearnSpellAsync(charId, userId, spellId, ct);
            return Results.Created($"/api/characters/{charId}/spells/{spellId}", knownSpell);
        });

        charSpellGroup.MapDelete("/{spellId:guid}", async (Guid charId, Guid spellId,
            ClaimsPrincipal principal, CharacterSpellService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await svc.ForgetSpellAsync(charId, userId, spellId, ct);
            return Results.NoContent();
        });

        charSpellGroup.MapPost("/use-slot", async (Guid charId,
            UseSpellSlotRequest req, ClaimsPrincipal principal, CharacterSpellService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.UseSpellSlotAsync(charId, userId, req.SpellLevel, ct);
            return Results.Ok(dto);
        });

        charSpellGroup.MapPost("/restore-slots", async (Guid charId,
            ClaimsPrincipal principal, CharacterSpellService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.RestoreSpellSlotsAsync(charId, userId, ct);
            return Results.Ok(dto);
        });
    }
}
