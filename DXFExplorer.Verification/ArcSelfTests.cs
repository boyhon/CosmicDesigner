using DXFExplorer;

internal static class ArcSelfTests
{
    private const double Epsilon = 0.000001;

    public static void Run()
    {
        Equal(270, GeometryArc.NormalizeAngle(-90), "normalize -90");
        Equal(0, GeometryArc.NormalizeAngle(360), "normalize 360");
        Equal(90, GeometryArc.NormalizeAngle(450), "normalize 450");

        var left = Arc(40, 10, 10, 180, 270);
        Point(30, 10, left.StartPoint, "180° start");
        Point(40, 0, left.EndPoint, "270° end");
        True(left.ContainsAngle(180) && left.ContainsAngle(225) && left.ContainsAngle(270), "180→270 inclusion");
        True(!left.ContainsAngle(0) && !left.ContainsAngle(90), "180→270 exclusion");
        Bounds(left, 30, 0, 40, 10, "quarter bounds");

        var crossing = Arc(260, 10, 10, 270, 0);
        Point(260, 0, crossing.StartPoint, "270° start");
        Point(270, 10, crossing.EndPoint, "0° end");
        True(crossing.ContainsAngle(270) && crossing.ContainsAngle(350) && crossing.ContainsAngle(0), "270→0 inclusion");
        True(!crossing.ContainsAngle(90) && !crossing.ContainsAngle(180), "270→0 exclusion");
        Bounds(crossing, 260, 0, 270, 10, "cross-zero bounds");

        var half = Arc(0, 0, 5, 0, 180);
        Bounds(half, -5, 0, 5, 5, "half arc bounds");

        var tenCrossing = Arc(0, 0, 1, 350, 10);
        True(tenCrossing.ContainsAngle(0) && !tenCrossing.ContainsAngle(180), "350→10 inclusion");
        var rendered = ArcRendering.CreateGeometry(Arc(20, 110, 20, 90, 180), p => new System.Windows.Point(p.X, p.Y));
        Equal(0, rendered.Bounds.Left, "render model minX"); Equal(110, rendered.Bounds.Top, "render model minY");
        Equal(20, rendered.Bounds.Right, "render model maxX"); Equal(130, rendered.Bounds.Bottom, "render model maxY");
        Console.WriteLine("ARC unit checks: PASS");

        var analyzed = DxfLineAnalyzer.Analyze(new[] { "0", "SECTION", "2", "ENTITIES", "0", "ARC", "8", "V", "10", "100", "20", "200", "40", "50", "50", "0", "51", "90" });
        Text("새로운 DXF SECTION 시작", analyzed[1].Description, "section value");
        Text("ARC Entity 시작", analyzed[5].Description, "entity value");
        Text("중심 X 좌표 Group Code", analyzed[8].Description, "ARC center X code");
        Text("중심 X 좌표 = 100", analyzed[9].Description, "ARC center X value");
        Text("반지름 = 50", analyzed[13].Description, "ARC radius value");
        Text("시작 각도 = 0°", analyzed[15].Description, "ARC start angle value");
        Text("종료 각도 = 90°", analyzed[17].Description, "ARC end angle value");
        Console.WriteLine("DXF line analyzer checks: PASS");
    }

    private static GeometryArc Arc(double x, double y, double r, double start, double end) => new()
    {
        Id = 1, EntityType = "ARC", LayerName = "-L-", OriginalEntity = [],
        Center = new DxfPoint(x, y), Radius = r, StartAngle = start, EndAngle = end
    };

    private static void Bounds(GeometryArc arc, double minX, double minY, double maxX, double maxY, string name)
    {
        var bounds = new DxfBounds();
        arc.ExpandBounds(bounds);
        Equal(minX, bounds.MinX, name + " minX"); Equal(minY, bounds.MinY, name + " minY");
        Equal(maxX, bounds.MaxX, name + " maxX"); Equal(maxY, bounds.MaxY, name + " maxY");
    }

    private static void Point(double x, double y, DxfPoint actual, string name)
    {
        Equal(x, actual.X, name + " X"); Equal(y, actual.Y, name + " Y");
    }

    private static void Equal(double expected, double actual, string name)
    {
        if (Math.Abs(expected - actual) > Epsilon)
            throw new InvalidOperationException($"{name}: expected {expected}, actual {actual}");
    }

    private static void Text(string expected, string actual, string name)
    {
        if (!string.Equals(expected, actual, StringComparison.Ordinal))
            throw new InvalidOperationException($"{name}: expected '{expected}', actual '{actual}'");
    }

    private static void True(bool value, string name)
    {
        if (!value) throw new InvalidOperationException(name);
    }
}
