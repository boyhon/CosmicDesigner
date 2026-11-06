using System.IO;
using VCutting;

internal static class SectionAndBendTests
{
    public static void Sections()
    {
        var document = new VCuttingDocument();
        document.ConfigureNew(150, 120, 2, MeasurementUnit.Millimeter);
        Check(SectionSelectionEngine.Resolve(document, SectionAxis.H, 200) == 75, "H guide");
        Check(SectionSelectionEngine.Resolve(document, SectionAxis.W, 200) == 60, "W guide");
        Check(SectionSelectionEngine.Resolve(document, SectionAxis.H, 40) == 40, "Keep valid guide");

        document.OuterContour.Segments.Clear();
        document.OuterContour.Segments.AddRange([
            new LineSegment(0, 0, 30, 0), new LineSegment(30, 0, 30, 30),
            new LineSegment(30, 30, 0, 30), new LineSegment(0, 30, 0, 0)]);
        Check(SectionSelectionEngine.Resolve(document, SectionAxis.H, 75) == 15, "Contour fallback");

        document.OuterContour.Segments.Clear();
        Check(SectionSelectionEngine.Resolve(document, SectionAxis.H, 75) is null, "Empty contour");
        document.ConfigureNew(150, 120, 2, MeasurementUnit.Millimeter);
        document.HView.Position = 40;
        document.WView.Position = 25;
        var restored = DocumentSnapshot.Capture(document).Restore();
        Check(restored.HView.Position == 40 && restored.WView.Position == 25, "Snapshot");

        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".dxf");
        try
        {
            VCuttingDxfSerializer.Save(document, path);
            var loaded = VCuttingDxfSerializer.Load(path);
            Check(loaded.HView.Position == 40 && loaded.WView.Position == 25, "DXF round trip");
        }
        finally { File.Delete(path); }
    }

    public static void Contacts()
    {
        foreach (var pair in new[] {
            (BendDirection.Up, BendDirection.Left), (BendDirection.Down, BendDirection.Right),
            (BendDirection.Up, BendDirection.Right) })
        {
            var document = new VCuttingDocument();
            document.ConfigureNew(150, 120, 2, MeasurementUnit.Millimeter);
            document.AddBend(SectionAxis.W, 50, pair.Item1);
            document.AddBend(SectionAxis.H, 40, pair.Item2);
            var contact = BendContactEngine.Find(document).Single();
            Check(contact.Kind == BendContactKind.Interior && contact.Position.X == 50 && contact.Position.Y == 40, "Crossing");

            Check(document.Cuts.Count == 0 && document.Slits.Count == 0 && document.Bends.Count == 2,
                "Contact detection does not modify geometry");
            document.AddSlit("Line", [new LineSegment(50, 40, 60, 50)]);
            Check(BendContactEngine.Find(document).Single().Kind == BendContactKind.Interior,
                "An unrelated slit does not dismiss the crossing");

            var cut = document.AddHole("Circle", 50, 40);
            document.UpdateCut(cut, 50, 40, 10, 10, 0);
            Check(BendContactEngine.Find(document).Count == 0, "Opening removes the crossing");
        }

        var endpoint = new VCuttingDocument();
        endpoint.ConfigureNew(150, 120, 2, MeasurementUnit.Millimeter);
        var vertical = endpoint.AddBend(SectionAxis.W, 50, BendDirection.Up);
        var horizontal = endpoint.AddBend(SectionAxis.H, 0, BendDirection.Left);
        Check(BendContactEngine.Find(endpoint).Single().Kind == BendContactKind.EndpointTouch, "Endpoint touch");
        vertical.Position = 0;
        Check(BendContactEngine.Find(endpoint).Single().Kind == BendContactKind.SharedEndpoint, "Shared endpoint");
        horizontal.Position = 125;
        Check(BendContactEngine.Find(endpoint).Count == 0, "Extension beyond the material");

        var overlap = new VCuttingDocument();
        overlap.ConfigureNew(150, 120, 2, MeasurementUnit.Millimeter);
        overlap.AddBend(SectionAxis.W, 50, BendDirection.Up);
        overlap.AddBend(SectionAxis.W, 50, BendDirection.Down);
        Check(BendContactEngine.Find(overlap).Single().Kind == BendContactKind.Overlap, "Overlap");
        overlap.AddBend(SectionAxis.H, 40, BendDirection.Left);
        Check(BendContactEngine.Find(overlap).Count(c => c.Kind == BendContactKind.Interior) == 1, "Deduplication");
    }

    static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
