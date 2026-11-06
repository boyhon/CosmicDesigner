using System.Windows;

namespace VCutting;

public enum BendContactKind { Interior, EndpointTouch, SharedEndpoint, Overlap }

public readonly record struct BendContact(Point Position, BendContactKind Kind);

public static class BendContactEngine
{
    const double Tolerance = 1e-6;

    public static IReadOnlyList<BendContact> Find(VCuttingDocument document)
    {
        var segments = document.Bends.SelectMany(bend => EffectiveSegments(document, bend)).ToList();
        var contacts = new List<BendContact>();
        for (var i = 0; i < segments.Count; i++)
        for (var j = i + 1; j < segments.Count; j++)
        {
            var first = segments[i];
            var second = segments[j];
            if (ReferenceEquals(first.Bend, second.Bend)) continue;
            BendContact? contact = first.Bend.Axis == second.Bend.Axis
                ? Parallel(first, second)
                : Cross(first.Bend.Axis == SectionAxis.W ? first : second,
                        first.Bend.Axis == SectionAxis.H ? first : second);
            if (contact is { } value && !contacts.Any(existing =>
                (existing.Position - value.Position).Length <= Tolerance))
                contacts.Add(value);
        }
        return contacts;
    }

    static IEnumerable<EffectiveBend> EffectiveSegments(VCuttingDocument document, BendObject bend)
    {
        var vertical = bend.Axis == SectionAxis.W;
        var extent = vertical ? document.Material.Height : document.Material.Width;
        var crossExtent = vertical ? document.Material.Width : document.Material.Height;
        if (bend.Position < 0 || bend.Position > crossExtent || extent <= Tolerance) yield break;

        Point At(double station) => vertical ? new(bend.Position, station) : new(station, bend.Position);
        var a = At(0);
        var b = At(extent);
        var line = new LineSegment(a.X, a.Y, b.X, b.Y);
        var stops = new List<double> { 0, 1 };
        foreach (var segment in document.OuterContour.Segments.SelectMany(segment => segment is CircleSegment circle
            ? new GeometrySegment[] { new ArcSegment(circle.Cx, circle.Cy, circle.Radius, 0, 180), new ArcSegment(circle.Cx, circle.Cy, circle.Radius, 180, 360) }
            : [segment]).Concat(
            document.Cuts.SelectMany(cut => CutContourEngine.Loops(cut.Geometry).SelectMany(loop => loop))))
            CurvedBoundaryCutEngine.AllIntersections(line, segment, stops, []);

        var sorted = stops.Where(t => t >= 0 && t <= 1).Distinct().Order().ToArray();
        double? activeStart = null;
        var activeEnd = 0.0;
        for (var i = 1; i < sorted.Length; i++)
        {
            var start = sorted[i - 1] * extent;
            var end = sorted[i] * extent;
            if (end - start <= Tolerance) continue;
            var middle = At((start + end) / 2);
            if (CutContourEngine.Contains(document.OuterContour.Segments, middle) &&
                !document.Cuts.Any(cut => CutContourEngine.Contains(cut, middle)))
            {
                activeStart ??= start;
                activeEnd = end;
            }
            else if (activeStart is double firstStation)
            {
                yield return new(bend, firstStation, activeEnd);
                activeStart = null;
            }
        }
        if (activeStart is double finalStation) yield return new(bend, finalStation, activeEnd);
    }

    static BendContact? Cross(EffectiveBend vertical, EffectiveBend horizontal)
    {
        var x = vertical.Bend.Position;
        var y = horizontal.Bend.Position;
        if (!vertical.Contains(y) || !horizontal.Contains(x)) return null;
        var firstEnd = vertical.IsEnd(y);
        var secondEnd = horizontal.IsEnd(x);
        return new(new(x, y), firstEnd && secondEnd ? BendContactKind.SharedEndpoint :
            firstEnd || secondEnd ? BendContactKind.EndpointTouch : BendContactKind.Interior);
    }

    static BendContact? Parallel(EffectiveBend first, EffectiveBend second)
    {
        if (Math.Abs(first.Bend.Position - second.Bend.Position) > Tolerance) return null;
        var start = Math.Max(first.Start, second.Start);
        var end = Math.Min(first.End, second.End);
        if (end < start - Tolerance) return null;
        var station = (start + end) / 2;
        var position = first.Bend.Axis == SectionAxis.W
            ? new Point(first.Bend.Position, station) : new Point(station, first.Bend.Position);
        return new(position, end - start > Tolerance ? BendContactKind.Overlap : BendContactKind.SharedEndpoint);
    }

    readonly record struct EffectiveBend(BendObject Bend, double Start, double End)
    {
        public bool Contains(double value) => value >= Start - Tolerance && value <= End + Tolerance;
        public bool IsEnd(double value) => Math.Abs(value - Start) <= Tolerance || Math.Abs(value - End) <= Tolerance;
    }
}
