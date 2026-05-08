using WWN.Application.DTOs;
using WWN.Application.Services;

namespace WWN.Web.Endpoints;

public static class ArtSourceEndpoints
{
    public static void MapArtSourceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/art-sources").WithTags("ArtSources");

        group.MapGet("/", async (ArtSourceService svc, CancellationToken ct) =>
            Results.Ok(await svc.ListAsync(ct)));

        group.MapPost("/", async (CreateArtSourceRequest req, ArtSourceService svc, CancellationToken ct) =>
        {
            try
            {
                var dto = await svc.CreateAsync(req, ct);
                return Results.Created($"/api/art-sources/{dto.Id}", dto);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(ex.Message);
            }
        }).RequireAuthorization();

        group.MapPut("/{id:int}", async (int id, UpdateArtSourceRequest req, ArtSourceService svc, CancellationToken ct) =>
        {
            var dto = await svc.UpdateAsync(id, req, ct);
            return dto is null ? Results.NotFound() : Results.Ok(dto);
        }).RequireAuthorization();

        group.MapDelete("/{id:int}", async (int id, ArtSourceService svc, CancellationToken ct) =>
        {
            var result = await svc.DeleteAsync(id, ct);
            return result switch
            {
                null => Results.NotFound(),
                false => Results.Conflict("This art source is referenced by one or more arts."),
                true => Results.NoContent()
            };
        }).RequireAuthorization();
    }
}
