using System.IO;
using System.Text.Json;

namespace RecoveryTool.Services;

public sealed class AppConfiguration
{
    public string DefaultMode { get; set; } = "Reset";
    public string ISOFolder { get; set; } = "ISO";
    public bool EnableCloudReset { get; set; } = true;
    public bool EnableUSBInstall { get; set; } = true;
    public string RootPath { get; private set; } = AppContext.BaseDirectory;
    public string IsoFolderPath => Path.Combine(RootPath, ISOFolder);
    public string LogsPath => Path.Combine(RootPath, "Logs");

    public static AppConfiguration Load(string root)
    {
        var path = Path.Combine(root, "Config", "config.json");
        var config = File.Exists(path)
            ? JsonSerializer.Deserialize<AppConfiguration>(File.ReadAllText(path)) ?? new()
            : new();
        config.RootPath = root;
        Directory.CreateDirectory(config.IsoFolderPath);
        return config;
    }
}
