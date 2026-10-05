using System.Diagnostics;
using System.IO;
using System.Windows;

namespace CosmicDesigner;

public static class UserHelp
{
    static readonly HashSet<string> Pages = ["index.html", "getting-started.html", "reference/shortcuts.html", "troubleshooting/common-errors.html"];

    public static string ResolvePath(string page, string baseDirectory)
    {
        if (!Pages.Contains(page)) throw new ArgumentException("Unknown help page", nameof(page));
        return Path.GetFullPath(Path.Combine(baseDirectory, "help", page));
    }

    public static void Open(Window owner, string page)
    {
        var path = ResolvePath(page, AppContext.BaseDirectory);
        if (!File.Exists(path))
        {
            MessageBox.Show(owner, "도움말 파일을 찾을 수 없습니다. 프로그램 배포 폴더의 help 디렉터리를 확인하거나 다시 설치하세요.", "도움말", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        try { Process.Start(new ProcessStartInfo(path) { UseShellExecute = true }); }
        catch (Exception)
        {
            MessageBox.Show(owner, "도움말을 열 수 없습니다. HTML 파일의 기본 브라우저 연결을 확인하고 help 폴더에서 문서를 직접 열어 보세요.", "도움말", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
