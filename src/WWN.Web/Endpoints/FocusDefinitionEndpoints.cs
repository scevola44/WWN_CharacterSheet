using System.Security.Claims;
using WWN.Application.DTOs;
using WWN.Application.Services;

namespace WWN.Web.Endpoints;

public static class FocusDefinitionEndpoints
{
    private const string AdminRole = "Admin";

    public static void MapFocusDefinitionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/focus-definitions").WithTags("Focus Definitions");

        group.MapGet("/", async (ClaimsPrincipal principal, FocusDefinitionService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var foci = await svc.ListAsync(userId, ct);
            return Results.Ok(foci);
        });

        group.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal principal, FocusDefinitionService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var fd = await svc.GetAsync(id, userId, ct);
            return fd is not null ? Results.Ok(fd) : Results.NotFound();
        });

        group.MapPost("/", async (CreateFocusDefinitionRequest req, ClaimsPrincipal principal, FocusDefinitionService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await svc.CreateAsync(req, userId, ct);
            return Results.Created($"/api/focus-definitions/{dto.Id}", dto);
        }).RequireAuthorization();

        group.MapPut("/{id:guid}", async (Guid id, UpdateFocusDefinitionRequest req, ClaimsPrincipal principal, FocusDefinitionService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = principal.IsInRole(AdminRole);
            var dto = await svc.UpdateAsync(id, req, userId, isAdmin, ct);
            return dto is not null ? Results.Ok(dto) : Results.NotFound();
        }).RequireAuthorization();

        group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal principal, FocusDefinitionService svc, CancellationToken ct) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = principal.IsInRole(AdminRole);
            await svc.DeleteAsync(id, userId, isAdmin, ct);
            return Results.NoContent();
        }).RequireAuthorization();
    }
}
