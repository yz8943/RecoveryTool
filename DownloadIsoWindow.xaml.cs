using System.Diagnostics;
using System.Windows;

namespace RecoveryTool;

public partial class DownloadIsoWindow : Window
{
    private const string DownloadUrl = "https://www.microsoft.com/zh-cn/software-download/windows11";

    public DownloadIsoWindow()
    {
        InitializeComponent();
    }

    private void OpenDownloadPage_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo(DownloadUrl) { UseShellExecute = true });
    }

    private void ContinueButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
