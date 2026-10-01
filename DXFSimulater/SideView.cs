using DXFExplorer;
using System.Windows;
using System.Windows.Media;

namespace DXFSimulater;

public sealed class SideView : FrameworkElement
{
    private const double HorizontalMargin = 25;
    private const double VerticalMargin = 35;

    public BendSimulation? Simulation { get; set; }
    public SectionModel? Section { get; set; }
    public int? SelectedEntityId { get; set; }

    protected override void OnRender(DrawingContext dc)
    {
        dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(248, 250, 252)), null, new Rect(RenderSize));
        var points = Simulation?.Points;
        if (points is null || points.Count < 2) return;

        var minPosition = points.Min(p => p.AlongSection);
        var maxPosition = points.Max(p => p.AlongSection);
        var minOffset = points.Min(p => p.Offset);
        var maxOffset = points.Max(p => p.Offset);
        var scale = Math.Min(
            Math.Max(ActualWidth - HorizontalMargin * 2, 10) / Math.Max(maxOffset - minOffset, 1),
            Math.Max(ActualHeight - VerticalMargin * 2, 10) / Math.Max(maxPosition - minPosition, 1));
        var centerOffset = (minOffset + maxOffset) / 2;
        var centerPosition = (minPosition + maxPosition) / 2;

        // DXF Y (AlongSection) grows upward, while WPF screen Y grows downward.
        // Offset is the horizontal displacement seen in the side projection.
        Point Screen(SectionPoint point) => new(
            ActualWidth / 2 + (point.Offset - centerOffset) * scale,
            ActualHeight / 2 - (point.AlongSection - centerPosition) * scale);

        for (var i = 0; i < points.Count - 1; i++)
        {
            var isFixed = i == Section?.FixedSegmentIndex;
            dc.DrawLine(new Pen(isFixed ? Brushes.Black : Brushes.SteelBlue, isFixed ? 5 : 4), Screen(points[i]), Screen(points[i + 1]));
        }

        if (Section is null) return;
        for (var i = 0; i < Section.Hinges.Count; i++)
        {
            var hinge = Section.Hinges[i];
            var brush = hinge.Direction == BendDirection.Left ? Brushes.Crimson : Brushes.DodgerBlue;
            dc.DrawEllipse(hinge.EntityId == SelectedEntityId ? Brushes.Gold : Brushes.White,
                new Pen(brush, 2), Screen(points[i + 1]), 6, 6);
        }
    }
}
