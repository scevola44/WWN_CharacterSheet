using System.Security.Claims;
using WWN.Application.DTOs;
using WWN.Application.Services;

namespace WWN.Web.Endpoints;

public static class ArtEndpoints
{
    public static void MapArtEndpoints(this WebApplication app)
    {
        var artGroup = app.MapGroup("/api/arts").WithTags("Arts");

        artGroup.MapGet("/", async (ArtService svc, CancellationToken ct) =>
        {
            var arts = await svc.ListArtsAsync(ct);
            return Results.Ok(arts);
        });

        artGroup.MapGet("/{id:guid}", async (Guid id, ArtService svc, CancellationToken ct) =>
        {
            var art = await svc.GetArtAsync(id, ct);
            return art is not null ? Results.Ok(art) : Results.NotFound();
        });

        artGroup.MapPost("/", async (CreateArtRequest req, ArtService svc, CancellationToken ct) =>
        {
            var dto = await svc.CreateArtAsync(req, ct);
            return Results.Created($"/api/arts/{dto.Id}", dto);
        });

        artGroup.MapPut("/{id:guid}", async (Guid id, UpdateArtRequest req, ArtService svc, CancellationToken ct) =>
        {
            var dto = await svc.UpdateArtAsync(id, req, ct);
            return dto is not null ? Results.Ok(dto) : Results.NotFound();
        });

        artGroup.MapDelete("/{id:guid}", async (Guid id, ArtService svc, CancellationToken ct) =>
        {
            await svc.DeleteArtAsync(id, ct);
            return Results.NoContent();
        });

        var charArtGroup = app.MapGroup("/api/characters/{charId:guid}/arts")
            .WithTags("Character Arts")
            .RequireAuthorization();

        charArtGroup.MapPost("/{artId:guid}", async (Guid charId, Guid artId,
            ClaimsPrincipal principal, CharacterArtService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var knownArt = await svc.LearnArtAsync(charId, userId, artId, ct);
            return Results.Created($"/api/characters/{charId}/arts/{artId}", knownArt);
        });

        charArtGroup.MapDelete("/{artId:guid}", async (Guid charId, Guid artId,
            ClaimsPrincipal principal, CharacterArtService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await svc.ForgetArtAsync(charId, userId, artId, ct);
            return Results.NoContent();
        });

        charArtGroup.MapPost("/commit-effort", async (Guid charId,
            CommitEffortRequest req, ClaimsPrincipal principal, CharacterArtService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.CommitEffortAsync(charId, userId, req.Commitment, req.Amount, ct);
            return Results.Ok(dto);
        });

        charArtGroup.MapPost("/end-scene", async (Guid charId,
            ClaimsPrincipal principal, CharacterArtService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.EndSceneAsync(charId, userId, ct);
            return Results.Ok(dto);
        });

        charArtGroup.MapPost("/rest-day", async (Guid charId,
            ClaimsPrincipal principal, CharacterArtService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.RestForDayAsync(charId, userId, ct);
            return Results.Ok(dto);
        });

        charArtGroup.MapPost("/release-sustained", async (Guid charId,
            ReleaseSustainedRequest req, ClaimsPrincipal principal, CharacterArtService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.ReleaseSustainedAsync(charId, userId, req.Amount, ct);
            return Results.Ok(dto);
        });
    }
}
