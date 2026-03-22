using WWN.Application.DTOs;
using WWN.Application.Services;

namespace WWN.Web.Endpoints;

public static class FocusDefinitionEndpoints
{
    public static void MapFocusDefinitionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/focus-definitions").WithTags("Focus Definitions");

        group.MapGet("/", async (FocusDefinitionService svc, CancellationToken ct) =>
        {
            var foci = await svc.ListAsync(ct);
            return Results.Ok(foci);
        });

        group.MapGet("/{id:guid}", async (Guid id, FocusDefinitionService svc, CancellationToken ct) =>
        {
            var fd = await svc.GetAsync(id, ct);
            return fd is not null ? Results.Ok(fd) : Results.NotFound();
        });

        group.MapPost("/", async (CreateFocusDefinitionRequest req, FocusDefinitionService svc, CancellationToken ct) =>
        {
            var dto = await svc.CreateAsync(req, ct);
            return Results.Created($"/api/focus-definitions/{dto.Id}", dto);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateFocusDefinitionRequest req, FocusDefinitionService svc, CancellationToken ct) =>
        {
            var dto = await svc.UpdateAsync(id, req, ct);
            return dto is not null ? Results.Ok(dto) : Results.NotFound();
        });

        group.MapDelete("/{id:guid}", async (Guid id, FocusDefinitionService svc, CancellationToken ct) =>
        {
            await svc.DeleteAsync(id, ct);
            return Results.NoContent();
        });
    }
}
