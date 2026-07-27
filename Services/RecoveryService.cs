using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text;
using Serilog;

namespace RecoveryTool.Services;

public sealed class RecoveryService
{
    private readonly AppConfiguration _configuration;
    private readonly ILogger _log;

    public RecoveryService(AppConfiguration configuration, ILogger log)
    {
        _configuration = configuration;
        _log = log;
    }

    public Task ResetAsync()
    {
        EnsureWindows();
        EnsureAdministrator();
        _log.Information("开始 Windows Reset");
        // systemreset.exe is the supported user-facing entry point for Reset this PC.
        StartProcess("systemreset.exe", "-factoryreset");
        _log.Information("Windows Reset 已启动");
        return Task.CompletedTask;
    }

    public Task InstallFromIsoAsync(string isoPath)
    {
        EnsureWindows();
        EnsureAdministrator();
        if (string.IsNullOrWhiteSpace(isoPath) || !File.Exists(isoPath))
            throw new FileNotFoundException("未找到系统镜像文件。", isoPath);
        if (!string.Equals(Path.GetExtension(isoPath), ".iso", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("请选择有效的 Windows ISO 文件。");

        _log.Information("开始 Windows ISO 安装：{IsoPath}", isoPath);
        // The PowerShell command is embedded so the published single-file EXE has no script dependency.
        var safePath = isoPath.Replace("'", "''");
        var script = "$isoPath = '" + safePath + "'\r\n" +
            "Mount-DiskImage -ImagePath $isoPath -StorageType ISO -PassThru | Get-Volume | ForEach-Object {\r\n" +
            "  $setup = Join-Path ($_.DriveLetter + ':') 'setup.exe'\r\n" +
            "  Start-Process $setup -Verb RunAs\r\n" +
            "}\r\n";
        var encodedCommand = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
        StartProcess("powershell.exe", $"-NoProfile -ExecutionPolicy Bypass -EncodedCommand {encodedCommand}");
        _log.Information("Windows 安装程序已启动");
        return Task.CompletedTask;
    }

    private static void EnsureWindows()
    {
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("RecoveryTool 只能在 Windows 10/11 上运行。");
    }

    private static void EnsureAdministrator()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            throw new UnauthorizedAccessException("请以管理员身份运行 RecoveryTool。");
    }

    private static void StartProcess(string fileName, string arguments)
    {
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            UseShellExecute = true,
            WorkingDirectory = AppContext.BaseDirectory
        });
        if (process is null) throw new InvalidOperationException($"无法启动 {fileName}。");
    }
}
