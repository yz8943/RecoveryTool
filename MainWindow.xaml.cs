using System.Windows;
using RecoveryTool.Services;
using RecoveryTool.ViewModels;

namespace RecoveryTool;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
        _viewModel.Initialize();
    }

    private async void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "此操作将删除当前电脑上的用户数据和已安装软件，并启动 Windows 官方恢复流程。\n\n确定继续吗？",
            "确认重置系统", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            await AppServices.Recovery.ResetAsync();
            _viewModel.StatusMessage = "已启动 Windows 恢复流程";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "无法重置系统", MessageBoxButton.OK, MessageBoxImage.Error);
            _viewModel.StatusMessage = "操作失败";
        }
    }

    private async void InstallButton_Click(object sender, RoutedEventArgs e)
    {
        var guide = new DownloadIsoWindow { Owner = this };
        if (guide.ShowDialog() != true) return;

        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "选择 Windows ISO 镜像",
            Filter = "Windows ISO (*.iso)|*.iso|所有文件 (*.*)|*.*",
            InitialDirectory = AppServices.Configuration.IsoFolderPath
        };
        if (dialog.ShowDialog() != true) return;

        var result = MessageBox.Show(
            $"即将使用以下镜像启动 Windows 安装程序：\n\n{dialog.FileName}\n\n请确认已备份重要数据。继续吗？",
            "确认安装系统", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            await AppServices.Recovery.InstallFromIsoAsync(dialog.FileName);
            _viewModel.StatusMessage = "已启动 Windows 安装程序";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "无法安装系统", MessageBoxButton.OK, MessageBoxImage.Error);
            _viewModel.StatusMessage = "操作失败";
        }
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e) => Close();
}
