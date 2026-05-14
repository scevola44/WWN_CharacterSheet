using WWN.Web.Services;

namespace WWN.Web.Endpoints;

public static class DiagnosticsEndpoints
{
    public static void MapDiagnosticsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/diagnostics")
            .WithName("Diagnostics");

        group.MapGet("/info", GetInfo)
            .Produces<InfoResponse>()
            .WithName("GetAppInfo")
            .WithSummary("Get application diagnostics information");
    }

    private static IResult GetInfo(AppInfoService appInfo)
    {
        return Results.Ok(new InfoResponse(
            appInfo.Branch,
            appInfo.Environment,
            appInfo.StartupTime,
            appInfo.Version,
            appInfo.Channel
        ));
    }

    public record InfoResponse(
        string Branch,
        string Environment,
        DateTime StartupTime,
        string Version,
        string Channel
    );
}
