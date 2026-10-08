using System.Diagnostics;
using System.IO;
using System.Windows;

namespace VCutting;

public static class UserHelp
{
    static readonly HashSet<string> Pages = ["index.html", "getting-started.html", "reference/shortcuts.html", "troubleshooting/common-errors.html"];

    public static string ResolvePath(string page, string baseDirectory)
    {
        if (!Pages.Contains(page)) throw new ArgumentException(Localization.Text("ui.0190"), nameof(page));
        return Path.GetFullPath(Path.Combine(baseDirectory, "help", page));
    }

    public static void Open(Window owner, string page)
    {
        var path = ResolvePath(page, AppContext.BaseDirectory);
        if (!File.Exists(path))
        {
            MessageBox.Show(owner, Localization.Text("ui.0191"), Localization.Text("ui.0192"), MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        try { Process.Start(new ProcessStartInfo(path) { UseShellExecute = true }); }
        catch (Exception)
        {
            MessageBox.Show(owner, Localization.Text("ui.0193"), Localization.Text("ui.0194"), MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
