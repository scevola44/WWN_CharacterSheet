using WWN.Application.DTOs;
using WWN.Application.Services;

namespace WWN.Web.Endpoints;

public static class SpellEndpoints
{
    public static void MapSpellEndpoints(this WebApplication app)
    {
        var spellGroup = app.MapGroup("/api/spells").WithTags("Spells");

        spellGroup.MapGet("/", async (SpellService svc, CancellationToken ct) =>
        {
            var spells = await svc.ListSpellsAsync(ct);
            return Results.Ok(spells);
        });

        spellGroup.MapGet("/{id:guid}", async (Guid id, SpellService svc, CancellationToken ct) =>
        {
            var spell = await svc.GetSpellAsync(id, ct);
            return spell is not null ? Results.Ok(spell) : Results.NotFound();
        });

        spellGroup.MapPost("/", async (CreateSpellRequest req, SpellService svc, CancellationToken ct) =>
        {
            var dto = await svc.CreateSpellAsync(req, ct);
            return Results.Created($"/api/spells/{dto.Id}", dto);
        });

        spellGroup.MapPut("/{id:guid}", async (Guid id, UpdateSpellRequest req, SpellService svc, CancellationToken ct) =>
        {
            var dto = await svc.UpdateSpellAsync(id, req, ct);
            return dto is not null ? Results.Ok(dto) : Results.NotFound();
        });

        spellGroup.MapDelete("/{id:guid}", async (Guid id, SpellService svc, CancellationToken ct) =>
        {
            await svc.DeleteSpellAsync(id, ct);
            return Results.NoContent();
        });

        var charSpellGroup = app.MapGroup("/api/characters/{charId:guid}/spells").WithTags("Character Spells");

        charSpellGroup.MapGet("/", async (Guid charId, CharacterService charSvc, CancellationToken ct) =>
        {
            var character = await charSvc.GetCharacterAsync(charId, ct);
            return character is not null ? Results.Ok(character) : Results.NotFound();
        });

        charSpellGroup.MapPost("/{spellId:guid}", async (Guid charId, Guid spellId,
            CharacterSpellService svc, CancellationToken ct) =>
        {
            var knownSpell = await svc.LearnSpellAsync(charId, spellId, ct);
            return Results.Created($"/api/characters/{charId}/spells/{spellId}", knownSpell);
        });

        charSpellGroup.MapDelete("/{spellId:guid}", async (Guid charId, Guid spellId,
            CharacterSpellService svc, CancellationToken ct) =>
        {
            await svc.ForgetSpellAsync(charId, spellId, ct);
            return Results.NoContent();
        });

        charSpellGroup.MapPost("/use-slot", async (Guid charId,
            UseSpellSlotRequest req, CharacterSpellService svc, CancellationToken ct) =>
        {
            var dto = await svc.UseSpellSlotAsync(charId, req.SpellLevel, ct);
            return Results.Ok(dto);
        });

        charSpellGroup.MapPost("/restore-slots", async (Guid charId,
            CharacterSpellService svc, CancellationToken ct) =>
        {
            var dto = await svc.RestoreSpellSlotsAsync(charId, ct);
            return Results.Ok(dto);
        });
    }
}
