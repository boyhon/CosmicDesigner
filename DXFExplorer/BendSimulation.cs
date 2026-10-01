namespace DXFExplorer;

public enum BendDirection { Left = -1, Right = 1 }
public sealed record SectionPoint(double AlongSection, double Offset);
public sealed record BendHinge(int Index, double PositionY, int EntityId, string Layer, BendDirection Direction, double AngleDegrees = BendSimulation.DefaultBendAngleDegrees);
public sealed record SectionSegment(int Index, double StartY, double EndY, bool IsFixed);
public sealed record BendOperation(int EntityId, string Layer, double HingePositionY, double AngleDegrees, BendDirection Direction, int Sequence);

public sealed class SectionModel
{
    public required double SectionX { get; init; }
    public required double MinY { get; init; }
    public required double MaxY { get; init; }
    public required double CenterY { get; init; }
    public required int FixedSegmentIndex { get; init; }
    public required IReadOnlyList<SectionSegment> Segments { get; init; }
    public required IReadOnlyList<BendHinge> Hinges { get; init; }

    public static SectionModel Create(DxfDocument document, double? sectionX = null, double tolerance = 1e-6)
    {
        var selection = SectionSelectionState.Create(document);
        var actualSectionX = Math.Clamp(sectionX ?? selection.CenterX, selection.MinX, selection.MaxX);
        var material = document.Entities.Where(entity => IsLayer(entity.LayerName, "L")).ToList();
        if (material.Count == 0) material = document.Entities.ToList();
        var materialBounds = new DxfBounds(); foreach (var entity in material) entity.ExpandBounds(materialBounds);

        // Prefer the actual intersections of the L boundary at this X. Fall back to
        // the material bounds for open/incomplete cutting geometry.
        var materialIntersections = material.SelectMany(entity => IntersectionsWithVerticalSection(entity, actualSectionX, tolerance, true)).OrderBy(y => y).ToList();
        var minY = materialIntersections.Count >= 2 ? materialIntersections.First() : materialBounds.MinY;
        var maxY = materialIntersections.Count >= 2 ? materialIntersections.Last() : materialBounds.MaxY;
        if (maxY - minY <= tolerance) { minY = materialBounds.MinY; maxY = materialBounds.MaxY; }

        var candidates = new List<(double Y, GeometryEntity Entity, BendDirection Direction)>();
        foreach (var entity in document.Entities)
        {
            BendDirection? direction = IsLayer(entity.LayerName, "V1") ? BendDirection.Right : IsLayer(entity.LayerName, "V") ? BendDirection.Left : null;
            if (direction is null) continue;
            candidates.AddRange(IntersectionsWithVerticalSection(entity, actualSectionX, tolerance).Select(y => (y, entity, direction.Value)));
        }

        var hinges = new List<BendHinge>();
        foreach (var item in candidates.OrderBy(item => item.Y))
        {
            if (item.Y < minY - tolerance || item.Y > maxY + tolerance || hinges.Any(h => Math.Abs(h.PositionY - item.Y) <= tolerance)) continue;
            hinges.Add(new BendHinge(hinges.Count, item.Y, item.Entity.Id, item.Entity.LayerName, item.Direction));
        }

        var cuts = new[] { minY }.Concat(hinges.Select(h => h.PositionY)).Concat([maxY]).ToArray();
        var centerY = (minY + maxY) / 2;
        var firstUpper = Array.FindIndex(cuts, 1, y => y >= centerY);
        var fixedIndex = Math.Clamp(firstUpper < 0 ? cuts.Length - 2 : firstUpper - 1, 0, cuts.Length - 2);
        return new SectionModel
        {
            SectionX = actualSectionX, MinY = minY, MaxY = maxY, CenterY = centerY, FixedSegmentIndex = fixedIndex,
            Segments = Enumerable.Range(0, cuts.Length - 1).Select(i => new SectionSegment(i, cuts[i], cuts[i + 1], i == fixedIndex)).ToList(), Hinges = hinges
        };
    }

    private static bool IsLayer(string actual, string expected) => actual.Trim('-', ' ').Equals(expected, StringComparison.OrdinalIgnoreCase);
    private static IEnumerable<double> IntersectionsWithVerticalSection(GeometryEntity entity, double sectionX, double tolerance, bool collinearEndpoints = false)
    {
        if (entity is GeometryLine line)
        {
            var dx = line.End.X - line.Start.X;
            if (Math.Abs(dx) <= tolerance) { if (Math.Abs(sectionX - line.Start.X) <= tolerance) { if (collinearEndpoints) { yield return line.Start.Y; if (Math.Abs(line.End.Y - line.Start.Y) > tolerance) yield return line.End.Y; } else yield return (line.Start.Y + line.End.Y) / 2; } yield break; }
            var t = (sectionX - line.Start.X) / dx;
            if (t >= -tolerance && t <= 1 + tolerance) yield return line.Start.Y + t * (line.End.Y - line.Start.Y);
        }
        else if (entity is GeometryCircle circle)
        {
            var d = sectionX - circle.Center.X; var squared = circle.Radius * circle.Radius - d * d; if (squared < -tolerance) yield break;
            var dy = Math.Sqrt(Math.Max(0, squared)); yield return circle.Center.Y - dy; if (dy > tolerance) yield return circle.Center.Y + dy;
        }
        else if (entity is GeometryArc arc)
        {
            var d = sectionX - arc.Center.X; var squared = arc.Radius * arc.Radius - d * d; if (squared < -tolerance) yield break;
            var dy = Math.Sqrt(Math.Max(0, squared));
            foreach (var y in new[] { arc.Center.Y - dy, arc.Center.Y + dy }.Distinct())
            {
                var angle = Math.Atan2(y - arc.Center.Y, sectionX - arc.Center.X) * 180 / Math.PI;
                if (arc.ContainsAngle(angle)) yield return y;
            }
        }
    }
}

public sealed class BendSimulation
{
    public const double DefaultBendAngleDegrees = 90;
    private readonly SectionModel _model; private readonly List<BendOperation> _history = [];
    public IReadOnlyList<BendOperation> History => _history;
    public IReadOnlyList<SectionPoint> Points { get; private set; }
    public BendSimulation(SectionModel model) { _model = model; Points = CreateFlatPoints(); }
    public bool Bend(int entityId)
    {
        var hinge = _model.Hinges.FirstOrDefault(h => h.EntityId == entityId); if (hinge is null) return false;
        _history.Add(new BendOperation(hinge.EntityId, hinge.Layer, hinge.PositionY, hinge.AngleDegrees, hinge.Direction, _history.Count + 1)); Rebuild(); return true;
    }
    public bool Undo() { if (_history.Count == 0) return false; _history.RemoveAt(_history.Count - 1); Rebuild(); return true; }
    public void Reset() { _history.Clear(); Points = CreateFlatPoints(); }
    private IReadOnlyList<SectionPoint> CreateFlatPoints() => new[] { _model.MinY }.Concat(_model.Hinges.Select(h => h.PositionY)).Concat([_model.MaxY]).Select(y => new SectionPoint(y, 0)).ToArray();
    private void Rebuild()
    {
        var points = CreateFlatPoints().ToArray();
        foreach (var operation in _history)
        {
            var hingeIndex = _model.Hinges.ToList().FindIndex(h => h.EntityId == operation.EntityId); if (hingeIndex < 0) continue;
            var rotateLower = hingeIndex < _model.FixedSegmentIndex; var pivot = points[hingeIndex + 1];
            var angle = operation.AngleDegrees * Math.PI / 180 * (int)operation.Direction; if (rotateLower) angle = -angle;
            var range = rotateLower ? Enumerable.Range(0, hingeIndex + 1) : Enumerable.Range(hingeIndex + 2, points.Length - hingeIndex - 2);
            foreach (var i in range)
            {
                var along = points[i].AlongSection - pivot.AlongSection; var offset = points[i].Offset - pivot.Offset;
                points[i] = new SectionPoint(pivot.AlongSection + along * Math.Cos(angle) - offset * Math.Sin(angle), pivot.Offset + along * Math.Sin(angle) + offset * Math.Cos(angle));
            }
        }
        Points = points;
    }
}

