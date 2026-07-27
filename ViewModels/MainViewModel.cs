using System.ComponentModel;
using System.Runtime.CompilerServices;
using RecoveryTool.Services;

namespace RecoveryTool.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private string _statusMessage = "准备就绪";
    public string ComputerName { get; private set; } = "-";
    public string WindowsVersion { get; private set; } = "-";
    public string ReinstallRecommendation { get; private set; } = "建议重装时选择与当前系统相同的 Windows 版本和语言。";
    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    public void Initialize()
    {
        ComputerName = Environment.MachineName;
        WindowsVersion = SystemInfoService.GetWindowsVersion();
        ReinstallRecommendation = $"重装建议：优先下载与当前系统相同的版本（{WindowsVersion}）镜像。";
        OnPropertyChanged(nameof(ComputerName));
        OnPropertyChanged(nameof(WindowsVersion));
        OnPropertyChanged(nameof(ReinstallRecommendation));
        AppServices.Log.Information("程序启动，计算机：{ComputerName}，系统：{WindowsVersion}", ComputerName, WindowsVersion);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new(name));
}
