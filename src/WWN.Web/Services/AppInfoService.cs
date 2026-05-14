namespace WWN.Web.Services;

public class AppInfoService
{
    public string Branch { get; }
    public string Environment { get; }
    public DateTime StartupTime { get; }
    public string Version { get; }
    public string Channel { get; }

    public AppInfoService(IConfiguration configuration)
    {
        Branch = System.Environment.GetEnvironmentVariable("BRANCH")
            ?? configuration["Version:Branch"]
            ?? "main";

        Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Production";

        StartupTime = DateTime.UtcNow;

        var versionBase = configuration["Version:Base"] ?? "1.0";
        var buildNumber = System.Environment.GetEnvironmentVariable("APP_BUILD_NUMBER");
        Version = string.IsNullOrWhiteSpace(buildNumber)
            ? $"{versionBase}-dev"
            : $"{versionBase}.{buildNumber}";

        Channel = Branch.Equals("main", StringComparison.OrdinalIgnoreCase) ? "stable" : "beta";
    }
}
