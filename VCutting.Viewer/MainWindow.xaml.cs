using Microsoft.Win32;
using DXFHelp;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace VCutting.Viewer;

public partial class MainWindow : Window
{
    private sealed class LayerRow
    {
        public required string LayerName { get; init; }
        public required string Text { get; init; }
        public required Brush Brush { get; init; }
        public bool Visible { get; set; } = true;
    }

    private readonly LayerStyleProvider _styles = new();

    public MainWindow()
    {
        InitializeComponent();
        PreviewKeyDown += (_, e) => { if (e.Key == Key.F1) { HelpLauncher.Open(this, "VCutting.Viewer_help.html"); e.Handled = true; } };
        Viewport.CoordinateChanged += (_, point) =>
            CoordinateText.Text = $"X {point.X:0.000}    Y {point.Y:0.000}";
        var args = Environment.GetCommandLineArgs();
        if (args.Length > 1 && File.Exists(args[1])) _ = LoadFileAsync(args[1]);
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "판금 DXF 파일 열기", Filter = "DXF 파일 (*.dxf)|*.dxf|모든 파일 (*.*)|*.*", CheckFileExists = true
        };
        if (dialog.ShowDialog(this) == true) _ = LoadFileAsync(dialog.FileName);
    }

    private async Task LoadFileAsync(string path)
    {
        TitleText.Text = "DXF 분석 중…";
        CommandGrid.ItemsSource = null;
        try
        {
            var loaded = await Task.Run(() => (Document: DxfDocumentParser.Parse(path), Lines: DxfLineAnalyzer.Analyze(path)));
            var document = loaded.Document;
            PopulateDocument(document);
            CommandGrid.ItemsSource = loaded.Lines;
            Viewport.SetDocument(document, _styles);
            document.Log.Add($"[{DateTime.Now:HH:mm:ss}] Render completed");
            LogText.Text = string.Join(Environment.NewLine, document.Log);
            TitleText.Text = Path.GetFileName(path);
            UpdateScale();
        }
        catch (Exception ex)
        {
            TitleText.Text = "DXF 파일을 열 수 없습니다";
            MessageBox.Show(this, ex.Message, "VCutting.Viewer", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void PopulateDocument(DxfDocument document)
    {
        var h = document.Header;
        FileNameText.Text = Path.GetFileName(document.FilePath); PathText.Text = document.FilePath;
        VersionText.Text = h.AcadVersion; CodePageText.Text = h.CodePage;
        MeasurementText.Text = h.Measurement; InsUnitsText.Text = h.InsUnits;
        EntitySummaryText.Text = $"전체  {document.EntityCounts.Values.Sum():N0}" + Environment.NewLine +
            string.Join(Environment.NewLine, document.EntityCounts.OrderBy(x => x.Key).Select(x => $"{x.Key}  {x.Value:N0}"));
        var rows = document.Layers.OrderBy(x => x).Select(layer =>
        {
            var style = _styles.Get(layer); style.Visible = true;
            return new LayerRow { LayerName = layer, Text = $"{layer}  ·  {style.DisplayName} ({document.LayerCounts.GetValueOrDefault(layer):N0})", Brush = new SolidColorBrush(style.Color) };
        }).ToList();
        LayerItems.ItemsSource = rows;
        LegendItems.ItemsSource = rows.Select(row => new LayerRow
        {
            LayerName = row.LayerName, Text = $"{row.LayerName}   {_styles.Get(row.LayerName).DisplayName}", Brush = row.Brush
        }).Concat([new LayerRow { LayerName = "$BOUNDS", Text = "전체 작업 범위", Brush = Brushes.Gray }]);
        LayerCountText.Text = string.Join(Environment.NewLine, document.LayerCounts.OrderBy(x => x.Key).Select(x => $"{x.Key} : {x.Value:N0}"));
        UnsupportedText.Text = document.UnsupportedCounts.Count == 0 ? "" : "지원되지 않은 Entity" + Environment.NewLine +
            string.Join(Environment.NewLine, document.UnsupportedCounts.Select(x => $"{x.Key} : {x.Value:N0}"));
        SizeText.Text = document.Bounds.IsEmpty ? "좌표 없음" : $"{document.Bounds.Width:0.###} × {document.Bounds.Height:0.###}";
        BoundsStatusText.Text = document.Bounds.IsEmpty ? "Bounds —" :
            $"Bounds  X {document.Bounds.MinX:0.###}…{document.Bounds.MaxX:0.###}  Y {document.Bounds.MinY:0.###}…{document.Bounds.MaxY:0.###}";
    }

    private void LayerVisibilityChanged(object sender, RoutedEventArgs e)
    {
        if (Viewport is null) return;
        if (sender == BoundsCheckBox) Viewport.ShowBounds = BoundsCheckBox.IsChecked == true;
        else if (sender is CheckBox { Tag: string layer } box) _styles.Get(layer).Visible = box.IsChecked == true;
        Viewport.InvalidateVisual();
    }

    private void AllLayersButton_Click(object sender, RoutedEventArgs e)
    {
        BoundsCheckBox.IsChecked = true;
        if (LayerItems.ItemsSource is not IEnumerable<LayerRow> current) return;
        var rows = current.ToList();
        foreach (var row in rows) { row.Visible = true; _styles.Get(row.LayerName).Visible = true; }
        LayerItems.ItemsSource = null; LayerItems.ItemsSource = rows;
        Viewport.InvalidateVisual();
    }

    private void FitButton_Click(object sender, RoutedEventArgs e) { Viewport.Fit(); UpdateScale(); }
    private void ZoomInButton_Click(object sender, RoutedEventArgs e) { Viewport.Zoom(1.2); UpdateScale(); }
    private void ZoomOutButton_Click(object sender, RoutedEventArgs e) { Viewport.Zoom(1 / 1.2); UpdateScale(); }
    private void HelpButton_Click(object sender, RoutedEventArgs e) => HelpLauncher.Open(this, "VCutting.Viewer_help.html");
    private void UpdateScale() => ScaleText.Text = $"Scale {Viewport.Scale:0.###}";
}
