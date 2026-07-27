using Microsoft.Win32;

namespace RecoveryTool.Services;

public static class SystemInfoService
{
    public static string GetWindowsVersion()
    {
        using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
        var product = key?.GetValue("ProductName")?.ToString() ?? "Windows";
        var display = key?.GetValue("DisplayVersion")?.ToString();
        return string.IsNullOrWhiteSpace(display) ? product : $"{product} {display}";
    }
}
