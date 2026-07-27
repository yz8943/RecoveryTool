using System.IO;
using Serilog;

namespace RecoveryTool.Services;

public static class AppServices
{
    public static AppConfiguration Configuration { get; private set; } = null!;
    public static RecoveryService Recovery { get; private set; } = null!;
    public static ILogger Log { get; private set; } = null!;

    public static void Initialize()
    {
        var root = AppContext.BaseDirectory;
        Configuration = AppConfiguration.Load(root);
        Directory.CreateDirectory(Configuration.LogsPath);
        Log = new LoggerConfiguration().MinimumLevel.Information()
            .WriteTo.File(Path.Combine(Configuration.LogsPath, "Recovery.log"), rollingInterval: RollingInterval.Infinite, shared: true)
            .CreateLogger();
        Recovery = new RecoveryService(Configuration, Log);
    }

    public static void Shutdown() => Serilog.Log.CloseAndFlush();
}
