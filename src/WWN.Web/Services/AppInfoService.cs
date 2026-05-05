namespace WWN.Web.Services;

public class AppInfoService
{
    public string Branch { get; }
    public string Environment { get; }
    public DateTime StartupTime { get; }

    public AppInfoService(IConfiguration configuration)
    {
        Branch = System.Environment.GetEnvironmentVariable("BRANCH")
            ?? configuration["Version:Branch"]
            ?? "main";

        Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Production";

        StartupTime = DateTime.UtcNow;
    }
}
