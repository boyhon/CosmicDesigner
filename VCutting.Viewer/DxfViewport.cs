using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace VCutting.Viewer;

public sealed class DxfViewport : FrameworkElement
{
    private DxfDocument? _document;
    private LayerStyleProvider? _styles;
    private double _scale = 1;
    private Vector _translation;
    private Point? _dragStart;
    private Vector _dragOrigin;
    private const double MarginSize = 48;

    public bool ShowBounds { get; set; } = true;
    public double Scale => _scale;
    public event EventHandler<DxfPoint>? CoordinateChanged;

    public DxfViewport()
    {
        Focusable = true;
        ClipToBounds = true;
        Background = Brushes.White;
    }

    public Brush Background { get; set; }

    public void SetDocument(DxfDocument document, LayerStyleProvider styles)
    {
        _document = document;
        _styles = styles;
        Fit();
    }

    public void Fit()
    {
        if (_document?.Bounds is not { IsEmpty: false } bounds || ActualWidth <= 0 || ActualHeight <= 0) return;
        var width = Math.Max(bounds.Width, 0.001);
        var height = Math.Max(bounds.Height, 0.001);
        _scale = Math.Min(Math.Max(ActualWidth - MarginSize * 2, 10) / width,
                          Math.Max(ActualHeight - MarginSize * 2, 10) / height);
        _translation = new Vector(
            (ActualWidth - bounds.Width * _scale) / 2,
            (ActualHeight - bounds.Height * _scale) / 2);
        InvalidateVisual();
    }

    public void Zoom(double factor, Point? center = null)
    {
        if (_document is null) return;
        var pivot = center ?? new Point(ActualWidth / 2, ActualHeight / 2);
        var oldScale = _scale;
        _scale = Math.Clamp(_scale * factor, 0.00001, 100000);
        var ratio = _scale / oldScale;
        _translation = new Vector(
            pivot.X - (pivot.X - _translation.X) * ratio,
            pivot.Y - (pivot.Y - _translation.Y) * ratio);
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc)
    {
        dc.DrawRectangle(Background, null, new Rect(RenderSize));
        if (_document?.Bounds is not { IsEmpty: false } bounds || _styles is null)
        {
            var text = MakeText("DXF 파일을 열면 판금 작업도가 표시됩니다.", 16, Brushes.SlateGray);
            dc.DrawText(text, new Point((ActualWidth - text.Width) / 2, (ActualHeight - text.Height) / 2));
            return;
        }

        foreach (var entity in _document.Entities)
        {
            var style = _styles.Get(entity.LayerName);
            if (!style.Visible) continue;
            var pen = new Pen(new SolidColorBrush(style.Color), style.LineWidth);
            pen.Freeze();
            switch (entity)
            {
                case GeometryLine line:
                    dc.DrawLine(pen, ToScreen(line.Start), ToScreen(line.End));
                    break;
                case GeometryCircle circle:
                    dc.DrawEllipse(null, pen, ToScreen(circle.Center), circle.Radius * _scale, circle.Radius * _scale);
                    break;
                case GeometryArc arc:
                    dc.DrawGeometry(null, pen, ArcRendering.CreateGeometry(arc, ToScreen));
                    break;
            }
        }

        if (ShowBounds)
        {
            var topLeft = ToScreen(new DxfPoint(bounds.MinX, bounds.MaxY));
            var rect = new Rect(topLeft.X, topLeft.Y, bounds.Width * _scale, bounds.Height * _scale);
            var boundPen = new Pen(Brushes.Gray, 1) { DashStyle = DashStyles.Dash };
            dc.DrawRectangle(null, boundPen, rect);
            var sizeText = MakeText($"{bounds.Width:0.###} × {bounds.Height:0.###}", 13, Brushes.DimGray);
            dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(220, 255, 255, 255)), null,
                new Rect(rect.Left, rect.Bottom + 6, sizeText.Width + 12, sizeText.Height + 6));
            dc.DrawText(sizeText, new Point(rect.Left + 6, rect.Bottom + 9));
        }
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        if (sizeInfo.PreviousSize.Width == 0 || sizeInfo.PreviousSize.Height == 0) Fit();
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        Zoom(e.Delta > 0 ? 1.15 : 1 / 1.15, e.GetPosition(this));
        e.Handled = true;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        Focus();
        CaptureMouse();
        _dragStart = e.GetPosition(this);
        _dragOrigin = _translation;
        Cursor = Cursors.Hand;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        var p = e.GetPosition(this);
        if (_dragStart is { } start && e.LeftButton == MouseButtonState.Pressed)
        {
            _translation = _dragOrigin + (p - start);
            InvalidateVisual();
        }
        if (_document?.Bounds is { IsEmpty: false })
            CoordinateChanged?.Invoke(this, ToDxf(p));
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        ReleaseMouseCapture();
        _dragStart = null;
        Cursor = Cursors.Arrow;
    }

    private Point ToScreen(DxfPoint point)
    {
        var bounds = _document!.Bounds;
        return new Point(
            _translation.X + (point.X - bounds.MinX) * _scale,
            _translation.Y + (bounds.MaxY - point.Y) * _scale);
    }

    private DxfPoint ToDxf(Point point)
    {
        var bounds = _document!.Bounds;
        return new DxfPoint(
            bounds.MinX + (point.X - _translation.X) / _scale,
            bounds.MaxY - (point.Y - _translation.Y) / _scale);
    }

    private static FormattedText MakeText(string text, double size, Brush brush) =>
        new(text, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            new Typeface("Segoe UI"), size, brush, 1);
}
