using DXFExplorer;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DXFSimulater;

public sealed class TopView : FrameworkElement
{
    private const double SelectorHitTolerance = 8;
    private DxfDocument? _document;
    private SectionModel? _section;
    private int? _selectedId;
    private double _scale = 1;
    private Vector _offset;
    private bool _isDraggingSelector;
    private bool _isSelectorHovered;
    private Point? _panStart;
    private Vector _panOrigin;

    public event EventHandler<GeometryEntity>? BendSelected;
    public event EventHandler<DxfPoint>? CoordinateChanged;
    public event Action<double>? SectionXChanged;

    public TopView() { ClipToBounds = true; Focusable = true; }
    public void SetDocument(DxfDocument document, SectionModel section) { _document = document; _section = section; Fit(); }
    public void SetSection(SectionModel section) { _section = section; InvalidateVisual(); }
    public void SetSelected(int? entityId) { _selectedId = entityId; InvalidateVisual(); }

    public void Fit()
    {
        if (_document?.Bounds is not { IsEmpty: false } bounds || ActualWidth <= 0 || ActualHeight <= 0) return;
        _scale = Math.Min(Math.Max(ActualWidth - 70, 10) / Math.Max(bounds.Width, .001), Math.Max(ActualHeight - 70, 10) / Math.Max(bounds.Height, .001));
        _offset = new((ActualWidth - bounds.Width * _scale) / 2, (ActualHeight - bounds.Height * _scale) / 2);
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc)
    {
        dc.DrawRectangle(Brushes.White, null, new Rect(RenderSize));
        if (_document?.Bounds is not { IsEmpty: false } bounds) { DrawText(dc, "DXF 파일을 열어 주세요.", new(30, 30), Brushes.SlateGray); return; }
        foreach (var entity in _document.Entities)
        {
            var brush = IsLayer(entity, "V1") ? Brushes.DodgerBlue : IsLayer(entity, "V") ? Brushes.Crimson : Brushes.SlateGray;
            var pen = new Pen(entity.Id == _selectedId ? Brushes.Gold : brush, entity.Id == _selectedId ? 5 : 1.7);
            switch (entity)
            {
                case GeometryLine line: dc.DrawLine(pen, ToScreen(line.Start), ToScreen(line.End)); break;
                case GeometryCircle circle: dc.DrawEllipse(null, pen, ToScreen(circle.Center), circle.Radius * _scale, circle.Radius * _scale); break;
                case GeometryArc arc:
                    dc.DrawGeometry(null, pen, ArcRendering.CreateGeometry(arc, ToScreen)); break;
            }
        }
        if (_section is null) return;
        var x = ToScreen(new DxfPoint(_section.SectionX, bounds.MinY)).X;
        var active = _isDraggingSelector || _isSelectorHovered;
        var selectorPen = new Pen(active ? Brushes.SeaGreen : Brushes.MediumSeaGreen, active ? 3 : 1.5) { DashStyle = DashStyles.Dash };
        dc.DrawLine(selectorPen, new(x, 0), new(x, ActualHeight));
        DrawText(dc, $"X = {_section.SectionX:0.000}", new(Math.Clamp(x + 7, 4, Math.Max(4, ActualWidth - 105)), 8), active ? Brushes.SeaGreen : Brushes.SlateGray);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        if (_document is null) return;
        var pivot = e.GetPosition(this); var oldScale = _scale;
        _scale = Math.Clamp(_scale * (e.Delta > 0 ? 1.15 : 1 / 1.15), .00001, 100000);
        var ratio = _scale / oldScale;
        _offset = new(pivot.X - (pivot.X - _offset.X) * ratio, pivot.Y - (pivot.Y - _offset.Y) * ratio);
        InvalidateVisual(); e.Handled = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (_document is null) return;
        var screen = e.GetPosition(this); var dxf = ToDxf(screen);
        CoordinateChanged?.Invoke(this, dxf);
        if (_isDraggingSelector)
        {
            SectionXChanged?.Invoke(dxf.X); Cursor = Cursors.SizeWE; e.Handled = true; return;
        }
        if (_panStart is { } start && e.RightButton == MouseButtonState.Pressed)
        {
            _offset = _panOrigin + (screen - start); InvalidateVisual(); Cursor = Cursors.Hand; e.Handled = true; return;
        }
        var hovered = IsNearSelector(screen.X);
        if (hovered != _isSelectorHovered) { _isSelectorHovered = hovered; InvalidateVisual(); }
        Cursor = hovered ? Cursors.SizeWE : Cursors.Arrow;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        if (_document is null) return;
        var screen = e.GetPosition(this);
        if (IsNearSelector(screen.X))
        {
            Focus(); CaptureMouse(); _isDraggingSelector = true; _isSelectorHovered = true; Cursor = Cursors.SizeWE; InvalidateVisual(); e.Handled = true; return;
        }
        var point = ToDxf(screen); var tolerance = 9 / Math.Max(_scale, .0001);
        var hit = _document.Entities.Where(entity => IsLayer(entity, "V") || IsLayer(entity, "V1")).OfType<GeometryLine>()
            .Select(entity => (Entity: entity, Distance: Distance(point, entity.Start, entity.End))).Where(item => item.Distance <= tolerance).OrderBy(item => item.Distance).FirstOrDefault();
        if (hit.Entity is not null) BendSelected?.Invoke(this, hit.Entity);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        if (!_isDraggingSelector) return;
        _isDraggingSelector = false; ReleaseMouseCapture(); _isSelectorHovered = IsNearSelector(e.GetPosition(this).X); Cursor = _isSelectorHovered ? Cursors.SizeWE : Cursors.Arrow; InvalidateVisual(); e.Handled = true;
    }

    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) { if (_document is null) return; Focus(); CaptureMouse(); _panStart = e.GetPosition(this); _panOrigin = _offset; Cursor = Cursors.Hand; e.Handled = true; }
    protected override void OnMouseRightButtonUp(MouseButtonEventArgs e) { if (_panStart is null) return; _panStart = null; ReleaseMouseCapture(); Cursor = IsNearSelector(e.GetPosition(this).X) ? Cursors.SizeWE : Cursors.Arrow; e.Handled = true; }
    protected override void OnRenderSizeChanged(SizeChangedInfo info) { base.OnRenderSizeChanged(info); if (info.PreviousSize.Width == 0 || info.PreviousSize.Height == 0) Fit(); }

    private bool IsNearSelector(double screenX) => _section is not null && Math.Abs(screenX - ToScreen(new DxfPoint(_section.SectionX, 0)).X) <= SelectorHitTolerance;
    private Point ToScreen(DxfPoint point) { var bounds = _document!.Bounds; return new(_offset.X + (point.X - bounds.MinX) * _scale, _offset.Y + (bounds.MaxY - point.Y) * _scale); }
    private DxfPoint ToDxf(Point point) { var bounds = _document!.Bounds; return new(bounds.MinX + (point.X - _offset.X) / _scale, bounds.MaxY - (point.Y - _offset.Y) / _scale); }
    private static bool IsLayer(GeometryEntity entity, string layer) => entity.LayerName.Trim('-', ' ').Equals(layer, StringComparison.OrdinalIgnoreCase);
    private static double Distance(DxfPoint p, DxfPoint a, DxfPoint b) { var dx = b.X - a.X; var dy = b.Y - a.Y; if (dx == 0 && dy == 0) return Math.Sqrt((p.X - a.X) * (p.X - a.X) + (p.Y - a.Y) * (p.Y - a.Y)); var t = Math.Clamp(((p.X - a.X) * dx + (p.Y - a.Y) * dy) / (dx * dx + dy * dy), 0, 1); var px = p.X - a.X - t * dx; var py = p.Y - a.Y - t * dy; return Math.Sqrt(px * px + py * py); }
    private static void DrawText(DrawingContext dc, string value, Point point, Brush brush) => dc.DrawText(new FormattedText(value, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 13, brush, 1), point);
}
