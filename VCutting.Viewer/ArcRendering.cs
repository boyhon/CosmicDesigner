using System.Windows;
using System.Windows.Media;

namespace VCutting.Viewer;

public static class ArcRendering
{
    public static Geometry CreateGeometry(GeometryArc arc, Func<DxfPoint, Point> toScreen)
    {
        var sweep = arc.SweepAngle;
        var steps = Math.Max(8, (int)Math.Ceiling(sweep / 5));
        var geometry = new StreamGeometry();
        using (var context = geometry.Open())
        {
            context.BeginFigure(toScreen(arc.StartPoint), false, false);
            for (var i = 1; i <= steps; i++)
                context.LineTo(toScreen(arc.PointAt(arc.StartAngle + sweep * i / steps)), true, false);
        }
        geometry.Freeze();
        return geometry;
    }
}
