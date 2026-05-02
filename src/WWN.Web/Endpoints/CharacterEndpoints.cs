using System.Security.Claims;
using WWN.Application.DTOs;
using WWN.Application.Services;

namespace WWN.Web.Endpoints;

public static class CharacterEndpoints
{
    public static void MapCharacterEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/characters")
            .WithTags("Characters")
            .RequireAuthorization();

        group.MapGet("/", async (ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var list = await svc.ListCharactersAsync(userId, ct);
            return Results.Ok(list);
        });

        group.MapPost("/", async (CreateCharacterRequest req, ClaimsPrincipal principal,
            CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var id = await svc.CreateCharacterAsync(req, userId, ct);
            return Results.Created($"/api/characters/{id}", new { id });
        });

        group.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal principal,
            CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.GetCharacterAsync(id, userId, ct);
            return dto is not null ? Results.Ok(dto) : Results.NotFound();
        });

        group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal principal,
            CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await svc.DeleteCharacterAsync(id, userId, ct);
            return Results.NoContent();
        });

        group.MapPut("/{id:guid}/attributes/{attr}", async (Guid id, string attr,
            UpdateAttributeRequest req, ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.UpdateAttributeAsync(id, userId, attr, req.Score, ct);
            return Results.Ok(dto);
        });

        group.MapPut("/{id:guid}/skills/{skill}", async (Guid id, string skill,
            UpdateSkillRequest req, ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.UpdateSkillAsync(id, userId, skill, req.Level, ct);
            return Results.Ok(dto);
        });

        group.MapPost("/{id:guid}/skills/custom", async (Guid id,
            AddCustomSkillRequest req, ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.AddCustomSkillAsync(id, userId, req.Name, req.Level, ct);
            return Results.Ok(dto);
        });

        group.MapPut("/{id:guid}/hp", async (Guid id,
            SetHpRequest req, ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.SetHpAsync(id, userId, req.MaxHitPoints, req.CurrentHitPoints, ct);
            return Results.Ok(dto);
        });

        group.MapPut("/{id:guid}/strain", async (Guid id,
            SetStrainRequest req, ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.SetStrainAsync(id, userId, req.CurrentStrain, ct);
            return Results.Ok(dto);
        });

        group.MapPut("/{id:guid}/level", async (Guid id,
            SetLevelRequest req, ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.SetLevelAsync(id, userId, req.Level, ct);
            return Results.Ok(dto);
        });

        group.MapPost("/{id:guid}/foci", async (Guid id,
            AddFocusRequest req, ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.AddFocusAsync(id, userId, req, ct);
            return Results.Ok(dto);
        });

        group.MapDelete("/{id:guid}/foci/{focusId:guid}", async (Guid id, Guid focusId,
            ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await svc.RemoveFocusAsync(id, userId, focusId, ct);
            return Results.NoContent();
        });

        group.MapPost("/{id:guid}/foci/{focusId:guid}/upgrade", async (Guid id, Guid focusId,
            UpgradeFocusRequest req, ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.UpgradeFocusAsync(id, userId, focusId, req, ct);
            return Results.Ok(dto);
        });

        group.MapPatch("/{id:guid}/foci/{focusId:guid}/conditional", async (Guid id, Guid focusId,
            SetFocusConditionalRequest req, ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.SetFocusConditionalAsync(id, userId, focusId, req.Active, ct);
            return Results.Ok(dto);
        });

        group.MapPost("/{id:guid}/items", async (Guid id,
            AddItemRequest req, ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.AddItemAsync(id, userId, req, ct);
            return Results.Ok(dto);
        });

        group.MapDelete("/{id:guid}/items/{itemId:guid}", async (Guid id, Guid itemId,
            ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await svc.RemoveItemAsync(id, userId, itemId, ct);
            return Results.NoContent();
        });

        group.MapPut("/{id:guid}/items/{itemId:guid}/slot", async (Guid id, Guid itemId,
            ChangeSlotRequest req, ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.ChangeSlotAsync(id, userId, itemId, req.SlotType, ct);
            return Results.Ok(dto);
        });

        group.MapPut("/{id:guid}/items/{itemId:guid}", async (Guid id, Guid itemId,
            UpdateItemRequest req, ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.UpdateItemAsync(id, userId, itemId, req, ct);
            return Results.Ok(dto);
        });

        group.MapPut("/{id:guid}/items/{itemId:guid}/attack-config", async (Guid id, Guid itemId,
            UpdateWeaponAttackConfigRequest req, ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.UpdateWeaponAttackConfigAsync(id, userId, itemId, req.Skill, req.Attribute, ct);
            return Results.Ok(dto);
        });

        group.MapPut("/{id:guid}/notes", async (Guid id,
            UpdateNotesRequest req, ClaimsPrincipal principal, CharacterService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.UpdateNotesAsync(id, userId, req.Notes, ct);
            return Results.Ok(dto);
        });
    }
}
