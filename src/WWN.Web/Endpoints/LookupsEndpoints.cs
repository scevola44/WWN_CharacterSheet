using Microsoft.Net.Http.Headers;
using WWN.Application.Services;

namespace WWN.Web.Endpoints;

public static class LookupsEndpoints
{
    public static void MapLookupsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/lookups", async (LookupsService svc, HttpContext ctx, CancellationToken ct) =>
        {
            var dto = await svc.GetAllAsync(ct);
            var etag = LookupsService.ComputeETag(dto);

            var ifNoneMatch = ctx.Request.Headers.IfNoneMatch.ToString();
            if (!string.IsNullOrEmpty(ifNoneMatch) && ifNoneMatch == etag)
            {
                ctx.Response.Headers[HeaderNames.ETag] = etag;
                return Results.StatusCode(StatusCodes.Status304NotModified);
            }

            ctx.Response.Headers[HeaderNames.ETag] = etag;
            ctx.Response.Headers[HeaderNames.CacheControl] = "private, max-age=30";
            return Results.Ok(dto);
        }).WithTags("Lookups");
    }
}
