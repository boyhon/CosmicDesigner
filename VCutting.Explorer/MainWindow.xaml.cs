using VCutting.Viewer;
using DXFHelp;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VCutting.Explorer;

public partial class MainWindow : Window
{
    private sealed class ExplorerSettings
    {
        public string? LastDirectory { get; set; }
    }

    // CR-069: preserve existing Explorer user preferences.
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "DXFExplorer", "settings.json");

    private readonly ObservableCollection<FolderNode> _roots = [];
    private readonly List<string> _history = [];
    private List<DxfFileItem> _allFiles = [];
    private int _historyIndex = -1;
    private string _currentPath = "";
    private CancellationTokenSource? _analysisCts;

    public MainWindow()
    {
        InitializeComponent();
        FolderTree.ItemsSource = _roots;
        LoadRoots();
        Closed += (_, _) => SaveSettings();
        PreviewKeyDown += (_, e) => { if (e.Key == Key.F1) { HelpLauncher.Open(this, "VCutting.Explorer_help.html"); e.Handled = true; } };
        var start = LoadLastDirectory() ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        NavigateTo(Directory.Exists(start) ? start : Environment.CurrentDirectory);
    }

    private static string? LoadLastDirectory()
    {
        try
        {
            if (!File.Exists(SettingsPath)) return null;
            var settings = JsonSerializer.Deserialize<ExplorerSettings>(File.ReadAllText(SettingsPath));
            return !string.IsNullOrWhiteSpace(settings?.LastDirectory) && Directory.Exists(settings.LastDirectory)
                ? settings.LastDirectory
                : null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            return null;
        }
    }

    private void SaveSettings()
    {
        if (string.IsNullOrWhiteSpace(_currentPath) || !Directory.Exists(_currentPath)) return;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            var json = JsonSerializer.Serialize(new ExplorerSettings { LastDirectory = _currentPath },
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsPath, json);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // 설정 저장 실패가 프로그램 종료를 막지 않도록 한다.
        }
    }

    private void LoadRoots()
    {
        _roots.Clear();
        foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
            _roots.Add(new FolderNode($"{drive.Name} ({drive.VolumeLabel})", drive.RootDirectory.FullName));
    }

    private static void LoadChildren(FolderNode node)
    {
        if (node.HasLoadedChildren) return;
        node.Children.Clear();
        try
        {
            foreach (var path in Directory.EnumerateDirectories(node.FullPath).Where(CanShowDirectory)
                         .OrderBy(Path.GetFileName, StringComparer.CurrentCultureIgnoreCase))
                node.Children.Add(new FolderNode(Path.GetFileName(path), path));
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException) { }
        node.HasLoadedChildren = true;
    }

    private static bool CanShowDirectory(string path)
    {
        try
        {
            var attributes = File.GetAttributes(path);
            return !attributes.HasFlag(FileAttributes.Hidden) && !attributes.HasFlag(FileAttributes.System);
        }
        catch { return false; }
    }

    private void NavigateTo(string path, bool addHistory = true)
    {
        try
        {
            path = Path.GetFullPath(path);
            if (!Directory.Exists(path)) throw new DirectoryNotFoundException();
            _currentPath = path; AddressBox.Text = path;
            _allFiles = Directory.EnumerateFiles(path, "*.dxf").Select(p => new FileInfo(p))
                .Select(f => new DxfFileItem { Name = f.Name, FullPath = f.FullName, Size = f.Length, Modified = f.LastWriteTime })
                .OrderBy(f => f.Name, StringComparer.CurrentCultureIgnoreCase).ToList();
            ApplyFilter();
            SelectedFileName.Text = "DXF 파일을 선택하세요";
            DetailItems.ItemsSource = null; LoadingText.Text = "";
            ViewButton.IsEnabled = false; SimulateButton.IsEnabled = false;
            if (addHistory)
            {
                if (_historyIndex < _history.Count - 1) _history.RemoveRange(_historyIndex + 1, _history.Count - _historyIndex - 1);
                _history.Add(path); _historyIndex = _history.Count - 1;
            }
            UpdateNavigationButtons();
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
        {
            MessageBox.Show(this, "이 폴더에 접근할 수 없습니다.", "폴더 열기", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void ApplyFilter()
    {
        var query = SearchBox.Text.Trim();
        var files = string.IsNullOrEmpty(query) ? _allFiles :
            _allFiles.Where(f => f.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase)).ToList();
        FileList.ItemsSource = files; StatusText.Text = $"{files.Count:N0}개 DXF 파일";
    }

    private void UpdateNavigationButtons()
    {
        BackButton.IsEnabled = _historyIndex > 0;
        ForwardButton.IsEnabled = _historyIndex >= 0 && _historyIndex < _history.Count - 1;
    }

    private void FolderTreeItem_Expanded(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is TreeViewItem { DataContext: FolderNode node }) LoadChildren(node);
    }

    private void FolderTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (e.NewValue is FolderNode node && !string.IsNullOrEmpty(node.FullPath)) NavigateTo(node.FullPath);
    }

    private async void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _analysisCts?.Cancel();
        if (FileList.SelectedItem is not DxfFileItem file)
        {
            ViewButton.IsEnabled = false;
            SimulateButton.IsEnabled = false;
            return;
        }
        var cts = _analysisCts = new CancellationTokenSource();
        SelectedFileName.Text = file.Name; DetailItems.ItemsSource = null;
        LoadingText.Text = "DXF 정보를 분석하는 중…";
        ViewButton.IsEnabled = true;
        SimulateButton.IsEnabled = true;
        try
        {
            var details = await Task.Run(() => DxfParser.Parse(file.FullPath), cts.Token);
            if (cts.IsCancellationRequested) return;
            LoadingText.Text = details.Warning ?? ""; DetailItems.ItemsSource = details.ToRows();
            StatusText.Text = $"{_allFiles.Count:N0}개 DXF 파일  |  1개 선택  |  {file.SizeText}";
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { LoadingText.Text = $"파일을 분석할 수 없습니다: {ex.Message}"; }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e) { if (_historyIndex > 0) NavigateTo(_history[--_historyIndex], false); }
    private void ForwardButton_Click(object sender, RoutedEventArgs e) { if (_historyIndex < _history.Count - 1) NavigateTo(_history[++_historyIndex], false); }
    private void UpButton_Click(object sender, RoutedEventArgs e) { var parent = Directory.GetParent(_currentPath); if (parent is not null) NavigateTo(parent.FullName); }
    private void RefreshButton_Click(object sender, RoutedEventArgs e) { LoadRoots(); NavigateTo(_currentPath, false); }
    private void AddressBox_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) NavigateTo(AddressBox.Text); }
    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) { if (FileList is not null) ApplyFilter(); }
    private void HelpButton_Click(object sender, RoutedEventArgs e) => HelpLauncher.Open(this, "VCutting.Explorer_help.html");
    private void FileList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => OpenSelectedWithSystem();

    private void OpenSelectedWithSystem()
    {
        if (FileList.SelectedItem is not DxfFileItem file) return;
        try { Process.Start(new ProcessStartInfo(file.FullPath) { UseShellExecute = true }); }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "DXF 열기", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private void ViewButton_Click(object sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem is not DxfFileItem file) return;
        var viewer = Path.Combine(AppContext.BaseDirectory, "VCutting.Viewer.exe");
        if (!File.Exists(viewer))
        {
            MessageBox.Show(this, "VCutting.Viewer.exe가 같은 폴더에 없습니다.", "작업도 보기", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        Process.Start(new ProcessStartInfo(viewer, $"\"{file.FullPath}\"") { UseShellExecute = true });
    }

    private void SimulateButton_Click(object sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem is not DxfFileItem file) return;
        var simulator = Path.Combine(AppContext.BaseDirectory, "VCutting.Simulator.exe");
        if (!File.Exists(simulator))
        {
            MessageBox.Show(this, "VCutting.Simulator.exe가 같은 폴더에 없습니다.", "모의 절곡", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        Process.Start(new ProcessStartInfo(simulator, $"\"{file.FullPath}\"") { UseShellExecute = true });
    }
}
