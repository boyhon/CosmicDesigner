using System.Diagnostics;
using System.IO;
using System.Windows;

namespace DXFHelp;

public static class HelpLauncher
{
    public static void Open(Window owner, string helpFileName)
    {
        var installedPath = Path.Combine(AppContext.BaseDirectory, helpFileName);
        var developmentPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "help-content", helpFileName));
        var path = File.Exists(installedPath) ? installedPath : developmentPath;
        if (!File.Exists(path))
        {
            MessageBox.Show(owner, $"도움말 파일을 찾을 수 없습니다.\n{installedPath}", "도움말", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show(owner, $"도움말을 열 수 없습니다.\n{ex.Message}", "도움말", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
