using System.Windows;

namespace VCutting;

public sealed class SlitOperation : DesignObject
{
    public string Shape { get; init; } = "Line";
    public List<GeometrySegment> Geometry { get; } = [];
}

public sealed record SlitPlacementEventArgs(string Shape, IReadOnlyList<GeometrySegment> Geometry);

/// <summary>Open cutting paths. Three-point arcs retain their signed sweep in metadata.</summary>
public static class SlitDrawingEngine
{
    public static bool TryCreate(string shape, IReadOnlyList<Point> points, out IReadOnlyList<GeometrySegment> geometry)
    {
        geometry = [];
        if (points.Any(p => !double.IsFinite(p.X) || !double.IsFinite(p.Y))) return false;
        if (shape == "Arc")
        {
            if (points.Count != 3) return false;
            var a = points[0]; var b = points[1]; var c = points[2];
            var u = b - a; var v = c - a;
            var cross = u.X * v.Y - u.Y * v.X;
            if (u.Length < 1e-6 || v.Length < 1e-6 || (c-b).Length < 1e-6 || Math.Abs(cross) < 1e-8 * u.Length * v.Length) return false;
            var cx = a.X + (u.LengthSquared * v.Y - v.LengthSquared * u.Y) / (2 * cross);
            var cy = a.Y + (u.X * v.LengthSquared - v.X * u.LengthSquared) / (2 * cross);
            var radius = (a - new Point(cx, cy)).Length;
            double Angle(Point p) => Math.Atan2(p.Y-cy, p.X-cx) * 180 / Math.PI;
            var start = Angle(a); var end = Angle(c);
            if (cross > 0) { while (end <= start) end += 360; }
            else { while (end >= start) end -= 360; }
            if (!double.IsFinite(radius) || !double.IsFinite(cx) || !double.IsFinite(cy)) return false;
            geometry = [new ArcSegment(cx, cy, radius, start, end)];
            return true;
        }
        if (shape is not ("Line" or "Polyline") || points.Count < 2 || (shape == "Line" && points.Count != 2)) return false;
        var lines = new List<GeometrySegment>();
        for (var i = 1; i < points.Count; i++)
        {
            var a = points[i-1]; var b = points[i];
            if ((b-a).Length < 1e-6) return false;
            lines.Add(new LineSegment(a.X, a.Y, b.X, b.Y));
        }
        geometry = lines;
        return true;
    }

    public static IEnumerable<Point> Sample(GeometrySegment segment)
    {
        if (segment is LineSegment l) return [new(l.X1,l.Y1), new(l.X2,l.Y2)];
        if (segment is ArcSegment a)
        {
            var count = Math.Max(1, (int)Math.Ceiling(Math.Abs(a.EndDegrees-a.StartDegrees)/5));
            return Enumerable.Range(0,count+1).Select(i => {
                var angle = (a.StartDegrees+(a.EndDegrees-a.StartDegrees)*i/count)*Math.PI/180;
                return new Point(a.Cx+a.Radius*Math.Cos(angle), a.Cy+a.Radius*Math.Sin(angle));
            });
        }
        return [];
    }

    public static bool Fits(IReadOnlyList<GeometrySegment> geometry, double width, double height)
    {
        if (geometry.Count == 0) return false;
        foreach (var segment in geometry)
        {
            var points = Sample(segment).ToList();
            if (segment is ArcSegment arc)
                for (var angle = -720; angle <= 720; angle += 90)
                    if (angle >= Math.Min(arc.StartDegrees,arc.EndDegrees) && angle <= Math.Max(arc.StartDegrees,arc.EndDegrees))
                        points.Add(new(arc.Cx+arc.Radius*Math.Cos(angle*Math.PI/180),arc.Cy+arc.Radius*Math.Sin(angle*Math.PI/180)));
            if (points.Count == 0 || points.Any(p => !double.IsFinite(p.X) || !double.IsFinite(p.Y) || p.X < -1e-8 || p.Y < -1e-8 || p.X > width+1e-8 || p.Y > height+1e-8)) return false;
        }
        return true;
    }
}
