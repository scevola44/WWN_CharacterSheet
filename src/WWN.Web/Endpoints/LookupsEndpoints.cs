using Microsoft.Net.Http.Headers;
using WWN.Application.Services;

namespace WWN.Web.Endpoints;

public static class LookupsEndpoints
{
    public static void MapLookupsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/lookups", (LookupsService svc, HttpContext ctx) =>
        {
            var etag = svc.ETag;

            // Honor If-None-Match for cheap revalidation.
            var ifNoneMatch = ctx.Request.Headers.IfNoneMatch.ToString();
            if (!string.IsNullOrEmpty(ifNoneMatch) && ifNoneMatch == etag)
            {
                ctx.Response.Headers[HeaderNames.ETag] = etag;
                return Results.StatusCode(StatusCodes.Status304NotModified);
            }

            ctx.Response.Headers[HeaderNames.ETag] = etag;
            ctx.Response.Headers[HeaderNames.CacheControl] = "public, max-age=300";
            return Results.Ok(svc.GetAll());
        }).WithTags("Lookups");
    }
}
