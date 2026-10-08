using System.Windows.Media;

namespace VCutting.Viewer;

public sealed record DxfPoint(double X, double Y, double Z = 0);

public sealed class DxfBounds
{
    public double MinX { get; private set; } = double.PositiveInfinity;
    public double MinY { get; private set; } = double.PositiveInfinity;
    public double MaxX { get; private set; } = double.NegativeInfinity;
    public double MaxY { get; private set; } = double.NegativeInfinity;
    public bool IsEmpty => double.IsInfinity(MinX);
    public double Width => IsEmpty ? 0 : MaxX - MinX;
    public double Height => IsEmpty ? 0 : MaxY - MinY;

    public void Include(double x, double y)
    {
        MinX = Math.Min(MinX, x); MinY = Math.Min(MinY, y);
        MaxX = Math.Max(MaxX, x); MaxY = Math.Max(MaxY, y);
    }
}

public abstract class GeometryEntity
{
    public required int Id { get; init; }
    public required string EntityType { get; init; }
    public required string LayerName { get; init; }
    public required IReadOnlyList<DxfGroup> OriginalEntity { get; init; }
    public bool IsDuplicate { get; set; }
    public int? DuplicateOf { get; set; }
    public abstract void ExpandBounds(DxfBounds bounds);
}

public sealed class GeometryLine : GeometryEntity
{
    public required DxfPoint Start { get; init; }
    public required DxfPoint End { get; init; }
    public override void ExpandBounds(DxfBounds bounds)
    {
        bounds.Include(Start.X, Start.Y);
        bounds.Include(End.X, End.Y);
    }
}

public sealed class GeometryCircle : GeometryEntity
{
    public required DxfPoint Center { get; init; }
    public required double Radius { get; init; }
    public override void ExpandBounds(DxfBounds bounds)
    {
        bounds.Include(Center.X - Radius, Center.Y - Radius);
        bounds.Include(Center.X + Radius, Center.Y + Radius);
    }
}

public sealed class GeometryArc : GeometryEntity
{
    private const double AngleEpsilon = 1e-9;
    public required DxfPoint Center { get; init; }
    public required double Radius { get; init; }
    public required double StartAngle { get; init; }
    public required double EndAngle { get; init; }
    public double SweepAngle
    {
        get
        {
            var sweep = NormalizeAngle(EndAngle) - NormalizeAngle(StartAngle);
            if (sweep < 0) sweep += 360;
            return sweep;
        }
    }
    public DxfPoint StartPoint => PointAt(StartAngle);
    public DxfPoint EndPoint => PointAt(EndAngle);

    public static double NormalizeAngle(double angle)
    {
        var normalized = angle % 360;
        return normalized < 0 ? normalized + 360 : normalized;
    }

    public bool ContainsAngle(double testAngle)
    {
        var relative = NormalizeAngle(testAngle) - NormalizeAngle(StartAngle);
        if (relative < 0) relative += 360;
        return relative <= SweepAngle + AngleEpsilon;
    }

    public DxfPoint PointAt(double angle)
    {
        var radians = angle * Math.PI / 180.0;
        return new DxfPoint(Center.X + Radius * Math.Cos(radians), Center.Y + Radius * Math.Sin(radians), Center.Z);
    }

    public override void ExpandBounds(DxfBounds bounds)
    {
        var start = StartPoint;
        var end = EndPoint;
        bounds.Include(start.X, start.Y);
        bounds.Include(end.X, end.Y);
        foreach (var angle in new[] { 0d, 90d, 180d, 270d })
        {
            if (!ContainsAngle(angle)) continue;
            var point = PointAt(angle);
            bounds.Include(point.X, point.Y);
        }
    }
}
public readonly record struct DxfGroup(int Code, string Value);

public sealed class DxfHeader
{
    public string AcadVersion { get; set; } = "선언 없음";
    public string CodePage { get; set; } = "선언 없음";
    public string Measurement { get; set; } = "선언 없음";
    public string InsUnits { get; set; } = "선언 없음";
}

public sealed class DxfDocument
{
    public required string FilePath { get; init; }
    public DxfHeader Header { get; } = new();
    public List<GeometryEntity> Entities { get; } = [];
    public HashSet<string> Layers { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> EntityCounts { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> LayerCounts { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> UnsupportedCounts { get; } = new(StringComparer.OrdinalIgnoreCase);
    public DxfBounds Bounds { get; } = new();
    public List<string> Log { get; } = [];
}

public enum ManufacturingOperation { Cut, BendOrVGroove, AlternateVGroove, Other }

public sealed class LayerStyle
{
    public required string LayerName { get; init; }
    public required string DisplayName { get; init; }
    public required ManufacturingOperation Operation { get; init; }
    public required Color Color { get; init; }
    public double LineWidth { get; init; } = 1.5;
    public bool Visible { get; set; } = true;
}

public sealed class LayerStyleProvider
{
    private readonly Dictionary<string, LayerStyle> _styles = new(StringComparer.OrdinalIgnoreCase)
    {
        ["-L-"] = new() { LayerName = "-L-", DisplayName = "절단 형상", Operation = ManufacturingOperation.Cut, Color = Colors.Black, LineWidth = 1.8 },
        ["-V-"] = new() { LayerName = "-V-", DisplayName = "V 가공/절곡선", Operation = ManufacturingOperation.BendOrVGroove, Color = Color.FromRgb(220, 38, 38), LineWidth = 1.7 },
        ["-V1-"] = new() { LayerName = "-V1-", DisplayName = "별도 V 가공선", Operation = ManufacturingOperation.AlternateVGroove, Color = Color.FromRgb(37, 99, 235), LineWidth = 1.7 }
    };

    private static readonly Color[] Palette =
    [
        Color.FromRgb(71, 85, 105), Color.FromRgb(5, 150, 105),
        Color.FromRgb(147, 51, 234), Color.FromRgb(234, 88, 12)
    ];

    public LayerStyle Get(string layer)
    {
        if (_styles.TryGetValue(layer, out var style)) return style;
        var color = Palette[(layer.GetHashCode(StringComparison.OrdinalIgnoreCase) & int.MaxValue) % Palette.Length];
        return _styles[layer] = new LayerStyle
        {
            LayerName = layer, DisplayName = layer, Operation = ManufacturingOperation.Other, Color = color
        };
    }
}
