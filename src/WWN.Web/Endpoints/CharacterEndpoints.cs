using WWN.Application.DTOs;
using WWN.Application.Services;

namespace WWN.Web.Endpoints;

public static class CharacterEndpoints
{
    public static void MapCharacterEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/characters").WithTags("Characters");

        group.MapGet("/", async (CharacterService svc, CancellationToken ct) =>
        {
            var list = await svc.ListCharactersAsync(ct);
            return Results.Ok(list);
        });

        group.MapPost("/", async (CreateCharacterRequest req, CharacterService svc, CancellationToken ct) =>
        {
            var id = await svc.CreateCharacterAsync(req, ct);
            return Results.Created($"/api/characters/{id}", new { id });
        });

        group.MapGet("/{id:guid}", async (Guid id, CharacterService svc, CancellationToken ct) =>
        {
            var dto = await svc.GetCharacterAsync(id, ct);
            return dto is not null ? Results.Ok(dto) : Results.NotFound();
        });

        group.MapDelete("/{id:guid}", async (Guid id, CharacterService svc, CancellationToken ct) =>
        {
            await svc.DeleteCharacterAsync(id, ct);
            return Results.NoContent();
        });

        group.MapPut("/{id:guid}/attributes/{attr}", async (Guid id, string attr,
            UpdateAttributeRequest req, CharacterService svc, CancellationToken ct) =>
        {
            var dto = await svc.UpdateAttributeAsync(id, attr, req.Score, ct);
            return Results.Ok(dto);
        });

        group.MapPut("/{id:guid}/skills/{skill}", async (Guid id, string skill,
            UpdateSkillRequest req, CharacterService svc, CancellationToken ct) =>
        {
            var dto = await svc.UpdateSkillAsync(id, skill, req.Level, ct);
            return Results.Ok(dto);
        });

        group.MapPost("/{id:guid}/skills/custom", async (Guid id,
            AddCustomSkillRequest req, CharacterService svc, CancellationToken ct) =>
        {
            var dto = await svc.AddCustomSkillAsync(id, req.Name, req.Level, ct);
            return Results.Ok(dto);
        });

        group.MapPut("/{id:guid}/hp", async (Guid id,
            SetHpRequest req, CharacterService svc, CancellationToken ct) =>
        {
            var dto = await svc.SetHpAsync(id, req.MaxHitPoints, req.CurrentHitPoints, ct);
            return Results.Ok(dto);
        });

        group.MapPut("/{id:guid}/level", async (Guid id,
            SetLevelRequest req, CharacterService svc, CancellationToken ct) =>
        {
            var dto = await svc.SetLevelAsync(id, req.Level, ct);
            return Results.Ok(dto);
        });

        group.MapPost("/{id:guid}/foci", async (Guid id,
            AddFocusRequest req, CharacterService svc, CancellationToken ct) =>
        {
            var dto = await svc.AddFocusAsync(id, req, ct);
            return Results.Ok(dto);
        });

        group.MapDelete("/{id:guid}/foci/{focusId:guid}", async (Guid id, Guid focusId,
            CharacterService svc, CancellationToken ct) =>
        {
            await svc.RemoveFocusAsync(id, focusId, ct);
            return Results.NoContent();
        });

        group.MapPost("/{id:guid}/foci/{focusId:guid}/upgrade", async (Guid id, Guid focusId,
            UpgradeFocusRequest req, CharacterService svc, CancellationToken ct) =>
        {
            var dto = await svc.UpgradeFocusAsync(id, focusId, req, ct);
            return Results.Ok(dto);
        });

        group.MapPatch("/{id:guid}/foci/{focusId:guid}/conditional", async (Guid id, Guid focusId,
            SetFocusConditionalRequest req, CharacterService svc, CancellationToken ct) =>
        {
            var dto = await svc.SetFocusConditionalAsync(id, focusId, req.Active, ct);
            return Results.Ok(dto);
        });

        group.MapPost("/{id:guid}/items", async (Guid id,
            AddItemRequest req, CharacterService svc, CancellationToken ct) =>
        {
            var dto = await svc.AddItemAsync(id, req, ct);
            return Results.Ok(dto);
        });

        group.MapDelete("/{id:guid}/items/{itemId:guid}", async (Guid id, Guid itemId,
            CharacterService svc, CancellationToken ct) =>
        {
            await svc.RemoveItemAsync(id, itemId, ct);
            return Results.NoContent();
        });

        group.MapPut("/{id:guid}/items/{itemId:guid}/slot", async (Guid id, Guid itemId,
            ChangeSlotRequest req, CharacterService svc, CancellationToken ct) =>
        {
            var dto = await svc.ChangeSlotAsync(id, itemId, req.SlotType, ct);
            return Results.Ok(dto);
        });

        group.MapPut("/{id:guid}/items/{itemId:guid}/attack-config", async (Guid id, Guid itemId,
            UpdateWeaponAttackConfigRequest req, CharacterService svc, CancellationToken ct) =>
        {
            var dto = await svc.UpdateWeaponAttackConfigAsync(id, itemId, req.Skill, req.Attribute, ct);
            return Results.Ok(dto);
        });

        group.MapPut("/{id:guid}/notes", async (Guid id,
            UpdateNotesRequest req, CharacterService svc, CancellationToken ct) =>
        {
            var dto = await svc.UpdateNotesAsync(id, req.Notes, ct);
            return Results.Ok(dto);
        });
    }
}
