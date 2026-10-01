using DXFExplorer;

internal static class SimulationSelfTests
{
    public static void Run()
    {
        VerticalSectionAndFlatState();
        VariableSectionChangesHinges();
        SectionSelectionClampsAndResets();
        IntersectionsAreSortedAndCenterSegmentIsFixed();
        DirectionsAreOpposite();
        SequentialUndoAndResetWork();
        Console.WriteLine("Simulation self-tests passed.");
    }

    private static void VerticalSectionAndFlatState()
    {
        var model = BuildDocument(false); var simulation = new BendSimulation(model);
        Assert(Near(model.SectionX, 50), "initial sectionX=centerX");
        Assert(Near(model.MinY, 0) && Near(model.MaxY, 400), "Y section range");
        Assert(simulation.Points.Count == 2 && simulation.Points.All(p => Near(p.Offset, 0)), "flat Y section");
        Assert(Near(simulation.Points[^1].AlongSection - simulation.Points[0].AlongSection, 400), "section length");
    }

    private static void VariableSectionChangesHinges()
    {
        var document = Document(true, partialBends: true);
        var center = SectionModel.Create(document, 50); var left = SectionModel.Create(document, 20); var right = SectionModel.Create(document, 80);
        Assert(center.Hinges.Count == 2, "center intersects V and V1");
        Assert(left.Hinges.Count == 1 && left.Hinges[0].Layer == "V", "left section only intersects V");
        Assert(right.Hinges.Count == 1 && right.Hinges[0].Layer == "V1", "right section only intersects V1");
        var bent = new BendSimulation(center); bent.Bend(center.Hinges[0].EntityId);
        var changed = new BendSimulation(left);
        Assert(changed.History.Count == 0 && changed.Points.All(p => Near(p.Offset, 0)), "section change starts flat");
    }

    private static void SectionSelectionClampsAndResets()
    {
        var state = SectionSelectionState.Create(Document(false));
        Assert(Near(state.CenterX, 50) && Near(state.SectionX, 50), "selection initial center");
        state.SetSectionX(-100); Assert(Near(state.SectionX, 0), "left clamp");
        state.SetSectionX(1000); Assert(Near(state.SectionX, 100), "right clamp");
        state.Reset(); Assert(Near(state.SectionX, 50), "selection reset");
    }

    private static void IntersectionsAreSortedAndCenterSegmentIsFixed()
    {
        var model = BuildDocument(true);
        Assert(model.Hinges.Count == 2, "two hinges");
        Assert(Near(model.Hinges[0].PositionY, 100) && Near(model.Hinges[1].PositionY, 300), "hinges sorted by intersection Y");
        Assert(model.FixedSegmentIndex == 1 && model.Segments[1].IsFixed, "centerY fixed segment");
        Assert(Near(model.Segments[1].StartY, 100) && Near(model.Segments[1].EndY, 300), "Y segments");
    }

    private static void DirectionsAreOpposite()
    {
        var model = BuildDocument(true);
        var v = new BendSimulation(model); v.Bend(model.Hinges[0].EntityId);
        var v1 = new BendSimulation(model); v1.Bend(model.Hinges[1].EntityId);
        Assert(Math.Abs(v.Points[0].Offset) > 1 && Math.Abs(v1.Points[^1].Offset) > 1, "both ends bend");
        Assert(Math.Sign(v.Points[0].Offset) != Math.Sign(v1.Points[^1].Offset), "V and V1 opposite in side view");
        Assert(Near(v.Points[1].AlongSection, 100) && Near(v.Points[1].Offset, 0), "fixed hinge pivot");
    }

    private static void SequentialUndoAndResetWork()
    {
        var model = BuildDocument(true); var simulation = new BendSimulation(model);
        simulation.Bend(model.Hinges[0].EntityId); var afterFirst = simulation.Points.ToArray();
        simulation.Bend(model.Hinges[1].EntityId); Assert(simulation.History.Count == 2, "sequential history");
        Assert(simulation.Undo() && simulation.History.Count == 1 && Same(afterFirst, simulation.Points), "undo");
        simulation.Reset(); Assert(simulation.History.Count == 0 && simulation.Points.All(p => Near(p.Offset, 0)), "reset");
    }

    private static SectionModel BuildDocument(bool withBends) => SectionModel.Create(Document(withBends));
    private static DxfDocument Document(bool withBends, bool partialBends = false)
    {
        var d = new DxfDocument { FilePath = "test.dxf" };
        Add(d, 1, "L", 0, 0, 100, 0); Add(d, 2, "L", 100, 0, 100, 400);
        Add(d, 3, "L", 100, 400, 0, 400); Add(d, 4, "L", 0, 400, 0, 0);
        if (withBends)
        {
            Add(d, 5, "V", 0, 100, partialBends ? 60 : 100, 100);
            Add(d, 6, "V1", partialBends ? 40 : 0, 300, 100, 300);
        }
        foreach (var entity in d.Entities) entity.ExpandBounds(d.Bounds); return d;
    }
    private static void Add(DxfDocument d, int id, string layer, double x1, double y1, double x2, double y2) => d.Entities.Add(new GeometryLine { Id = id, EntityType = "LINE", LayerName = layer, OriginalEntity = [], Start = new(x1, y1), End = new(x2, y2) });
    private static bool Same(IReadOnlyList<SectionPoint> a, IReadOnlyList<SectionPoint> b) => a.Count == b.Count && a.Zip(b).All(x => Near(x.First.AlongSection, x.Second.AlongSection) && Near(x.First.Offset, x.Second.Offset));
    private static bool Near(double a, double b) => Math.Abs(a - b) < 1e-8;
    private static void Assert(bool condition, string name) { if (!condition) throw new InvalidOperationException($"Simulation test failed: {name}"); }
}
