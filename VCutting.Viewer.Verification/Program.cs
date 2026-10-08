using VCutting.Viewer;

if (args.Length == 1 && args[0] == "--self-test")
{
    ArcSelfTests.Run();
    SimulationSelfTests.Run();
    return 0;
}

if (args.Length == 0)
{
    Console.WriteLine("Usage: VCutting.Viewer.Verification <dxf files>");
    return 2;
}

var failed = false;
foreach (var path in args)
{
    try
    {
        var document = DxfDocumentParser.Parse(path);
        Console.WriteLine(Path.GetFileName(path));
        Console.WriteLine($"  LINE={document.EntityCounts.GetValueOrDefault("LINE")} CIRCLE={document.EntityCounts.GetValueOrDefault("CIRCLE")} ARC={document.EntityCounts.GetValueOrDefault("ARC")}");
        Console.WriteLine($"  Layers={string.Join(",", document.Layers.OrderBy(x => x))}");
        Console.WriteLine($"  Bounds={document.Bounds.Width:0.###} x {document.Bounds.Height:0.###}");
        Console.WriteLine($"  Duplicate lines={document.Entities.Count(x => x.IsDuplicate)} Unsupported={document.UnsupportedCounts.Values.Sum()}");
        if (document.Bounds.IsEmpty) failed = true;
    }
    catch (Exception ex)
    {
        failed = true;
        Console.Error.WriteLine($"{Path.GetFileName(path)}: {ex.Message}");
    }
}
return failed ? 1 : 0;


