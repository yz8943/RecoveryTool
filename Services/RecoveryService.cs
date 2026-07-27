using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text;
using Serilog;

namespace RecoveryTool.Services;

public enum ResetResult
{
    DirectStarted,
    SettingsOpened
}

public sealed class RecoveryService
{
    private readonly AppConfiguration _configuration;
    private readonly ILogger _log;

    public RecoveryService(AppConfiguration configuration, ILogger log)
    {
        _configuration = configuration;
        _log = log;
    }

    public Task<ResetResult> ResetAsync()
    {
        EnsureWindows();
        EnsureAdministrator();
        _log.Information("开始 Windows Reset 流程");

        var winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        var sysDir = Environment.GetFolderPath(Environment.SpecialFolder.System);
        var candidatePaths = new[]
        {
            Path.Combine(sysDir, "systemreset.exe"),
            Path.Combine(winDir, "System32", "systemreset.exe"),
            Path.Combine(winDir, "SysWOW64", "systemreset.exe")
        };

        foreach (var path in candidatePaths)
        {
            if (File.Exists(path))
            {
                try
                {
                    StartProcess(path, "-factoryreset");
                    _log.Information("已通过 {Path} 启动 Windows Reset", path);
                    return Task.FromResult(ResetResult.DirectStarted);
                }
                catch (Exception ex)
                {
                    _log.Warning(ex, "尝试启动 {Path} 失败，尝试其他路径或降级方案", path);
                }
            }
        }

        _log.Warning("未检测到有效的 systemreset.exe 或启动失败，准备调起系统恢复设置页面");
        try
        {
            StartProcess("ms-settings:recovery", "");
            _log.Information("已调起 Windows 设置 Recovery 页面");
            return Task.FromResult(ResetResult.SettingsOpened);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "无法调起系统恢复设置页面");
            throw new InvalidOperationException("未找到 systemreset.exe 组件，且无法调起系统恢复设置页面。请手动打开『设置 -> 系统 -> 恢复』进行重置。", ex);
        }
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

        var system32 = Environment.GetFolderPath(Environment.SpecialFolder.System);
        var powerShellPath = Path.Combine(system32, "WindowsPowerShell", "v1.0", "powershell.exe");
        if (!File.Exists(powerShellPath))
        {
            powerShellPath = "powershell.exe";
        }

        var safePath = isoPath.Replace("'", "''");
        var script = "$isoPath = '" + safePath + "'\r\n" +
            "Mount-DiskImage -ImagePath $isoPath -StorageType ISO -PassThru | Get-Volume | ForEach-Object {\r\n" +
            "  $setup = Join-Path ($_.DriveLetter + ':') 'setup.exe'\r\n" +
            "  Start-Process $setup -Verb RunAs\r\n" +
            "}\r\n";
        var encodedCommand = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));
        StartProcess(powerShellPath, $"-NoProfile -ExecutionPolicy Bypass -EncodedCommand {encodedCommand}");
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
