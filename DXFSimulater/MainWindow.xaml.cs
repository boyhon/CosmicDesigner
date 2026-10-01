using DXFExplorer;
using DXFHelp;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace DXFSimulater;

public partial class MainWindow : Window
{
    private DxfDocument? _document;
    private SectionSelectionState? _sectionSelection;
    private SectionModel? _section;
    private BendSimulation? _simulation;
    private int? _selected;

    public MainWindow()
    {
        InitializeComponent();
        Icon = BitmapFrame.Create(new Uri("pack://application:,,,/cnc_vgroove_icon.ico"));
        PreviewKeyDown += (_, e) => { if (e.Key == Key.F1) { HelpLauncher.Open(this, "DXFSimulator_help.html"); e.Handled = true; } };
        TopViewport.BendSelected += BendSelected;
        TopViewport.SectionXChanged += SetSectionX;
        TopViewport.CoordinateChanged += (_, point) => CoordinateText.Text = $"X {point.X:0.000}   Y {point.Y:0.000}";
        var args = Environment.GetCommandLineArgs();
        if (args.Length > 1 && File.Exists(args[1])) Load(args[1]);
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Filter = "DXF 파일 (*.dxf)|*.dxf", CheckFileExists = true };
        if (dialog.ShowDialog(this) == true) Load(dialog.FileName);
    }

    private void Load(string path)
    {
        try
        {
            _document = DxfDocumentParser.Parse(path);
            _sectionSelection = SectionSelectionState.Create(_document);
            _section = SectionModel.Create(_document, _sectionSelection.SectionX);
            _simulation = new BendSimulation(_section);
            TopViewport.SetDocument(_document, _section);
            SideViewport.Section = _section;
            SideViewport.Simulation = _simulation;
            FileText.Text = Path.GetFileName(path);
            StatusText.Text = _section.Hinges.Count == 0 ? "현재 단면에 절곡 가공선이 없습니다." : "단면선을 드래그하거나 V/V1 가공선을 클릭하세요.";
            RefreshViews();
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "DXFSimulater", MessageBoxButton.OK, MessageBoxImage.Error); }
    }

    private void SetSectionX(double requestedX)
    {
        if (_document is null || _sectionSelection is null) return;
        if (!_sectionSelection.SetSectionX(requestedX)) return;
        _section = SectionModel.Create(_document, _sectionSelection.SectionX);
        _simulation = new BendSimulation(_section); // A new section always starts flat.
        _selected = null;
        TopViewport.SetSection(_section);
        TopViewport.SetSelected(null);
        SideViewport.Section = _section;
        SideViewport.Simulation = _simulation;
        SelectionText.Text = "";
        StatusText.Text = _section.Hinges.Count == 0 ? "현재 단면에 절곡 가공선이 없습니다." : $"X={_section.SectionX:0.000} 단면을 다시 계산했습니다.";
        RefreshViews();
    }

    private void BendSelected(object? sender, GeometryEntity entity)
    {
        _selected = entity.Id; TopViewport.SetSelected(entity.Id);
        var hinge = _section?.Hinges.FirstOrDefault(h => h.EntityId == entity.Id);
        if (hinge is null)
        {
            SelectionText.Text = $"{entity.LayerName} #{entity.Id}\n현재 X={_section?.SectionX:0.000} 단면과 교차하지 않습니다.";
            StatusText.Text = "선택한 가공선은 현재 단면의 시뮬레이션 대상이 아닙니다.";
            RefreshViews(); return;
        }
        _simulation!.Bend(entity.Id);
        SelectionText.Text = $"{hinge.Layer} #{hinge.EntityId}\nY = {hinge.PositionY:0.###}\n방향: {(hinge.Direction == BendDirection.Left ? "V 방향" : "V1 방향")}\n각도: {hinge.AngleDegrees:0}°";
        StatusText.Text = $"{_simulation.History.Count}번째 절곡을 적용했습니다.";
        RefreshViews();
    }

    private void ResetSection_Click(object sender, RoutedEventArgs e)
    {
        if (_sectionSelection is null) return;
        var center = _sectionSelection.CenterX;
        _sectionSelection.Reset();
        SetSectionXAfterStateChange(center);
    }

    private void SetSectionXAfterStateChange(double sectionX)
    {
        if (_document is null) return;
        _section = SectionModel.Create(_document, sectionX); _simulation = new BendSimulation(_section); _selected = null;
        TopViewport.SetSection(_section); TopViewport.SetSelected(null); SideViewport.Section = _section; SideViewport.Simulation = _simulation;
        SelectionText.Text = ""; StatusText.Text = "단면을 중앙 위치로 복원했습니다."; RefreshViews();
    }

    private void Undo_Click(object sender, RoutedEventArgs e) { if (_simulation?.Undo() == true) { StatusText.Text = "마지막 절곡을 취소했습니다."; RefreshViews(); } }
    private void Reset_Click(object sender, RoutedEventArgs e) { _simulation?.Reset(); _selected = null; TopViewport.SetSelected(null); SelectionText.Text = ""; StatusText.Text = "현재 단면의 절곡 상태를 초기화했습니다."; RefreshViews(); }
    private void Fit_Click(object sender, RoutedEventArgs e) => TopViewport.Fit();
    private void Help_Click(object sender, RoutedEventArgs e) => HelpLauncher.Open(this, "DXFSimulator_help.html");

    private void RefreshViews()
    {
        if (_section is not null && _sectionSelection is not null)
            BoundsText.Text = $"현재 단면 X: {_section.SectionX:0.000}\n기본 centerX: {_sectionSelection.CenterX:0.000}\n선택 범위: {_sectionSelection.MinX:0.###} ~ {_sectionSelection.MaxX:0.###}\nY 범위: {_section.MinY:0.###} ~ {_section.MaxY:0.###}\nSegment: {_section.Segments.Count}개\nHinge: {_section.Hinges.Count}개";
        SideViewport.SelectedEntityId = _selected; SideViewport.InvalidateVisual();
        HistoryList.ItemsSource = _simulation?.History.Select(item => $"{item.Sequence}. {item.Layer}  Y={item.HingePositionY:0.###}  {item.AngleDegrees:0}°").ToList();
    }
}

