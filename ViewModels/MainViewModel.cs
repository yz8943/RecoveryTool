using System.ComponentModel;
using System.Runtime.CompilerServices;
using RecoveryTool.Services;

namespace RecoveryTool.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private string _statusMessage = "准备就绪";
    public string ComputerName { get; private set; } = "-";
    public string WindowsVersion { get; private set; } = "-";
    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    public void Initialize()
    {
        ComputerName = Environment.MachineName;
        WindowsVersion = SystemInfoService.GetWindowsVersion();
        OnPropertyChanged(nameof(ComputerName));
        OnPropertyChanged(nameof(WindowsVersion));
        AppServices.Log.Information("程序启动，计算机：{ComputerName}，系统：{WindowsVersion}", ComputerName, WindowsVersion);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new(name));
}
