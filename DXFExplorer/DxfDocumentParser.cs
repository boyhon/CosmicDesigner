using System.Globalization;
using System.IO;
using System.Text;

namespace DXFExplorer;

public static class DxfDocumentParser
{
    private const double Epsilon = 0.000001;
    private static readonly HashSet<string> StructuralEntities = new(StringComparer.OrdinalIgnoreCase)
        { "SECTION", "ENDSEC", "EOF", "SEQEND" };

    public static DxfDocument Parse(string path)
    {
        var document = new DxfDocument { FilePath = path };
        document.Log.Add($"[{DateTime.Now:HH:mm:ss}] File loaded: {Path.GetFileName(path)}");
        var groups = ReadGroups(path, document.Log);
        ParseHeader(groups, document);
        ParseEntities(groups, document);
        DetectDuplicateLines(document);
        foreach (var entity in document.Entities) entity.ExpandBounds(document.Bounds);
        document.Log.Add($"[{DateTime.Now:HH:mm:ss}] Bounds calculated");
        return document;
    }

    private static List<DxfGroup> ReadGroups(string path, List<string> log)
    {
        using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        Span<byte> marker = stackalloc byte[22];
        var markerLength = stream.Read(marker);
        stream.Position = 0;
        if (Encoding.ASCII.GetString(marker[..markerLength]).StartsWith("AutoCAD Binary DXF", StringComparison.Ordinal))
            throw new NotSupportedException("바이너리 DXF는 현재 지원하지 않습니다.");

        var result = new List<DxfGroup>();
        using var reader = new StreamReader(stream, Encoding.Latin1, true);
        var lineNumber = 0;
        while (reader.ReadLine() is { } codeLine)
        {
            lineNumber++;
            var valueLine = reader.ReadLine();
            lineNumber++;
            if (valueLine is null)
            {
                log.Add($"Unexpected EOF at line {lineNumber}");
                break;
            }
            if (!int.TryParse(codeLine.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var code))
            {
                log.Add($"Invalid group code at line {lineNumber - 1}: {codeLine}");
                continue;
            }
            result.Add(new DxfGroup(code, valueLine.Trim()));
        }
        return result;
    }

    private static void ParseHeader(IReadOnlyList<DxfGroup> groups, DxfDocument document)
    {
        var section = "";
        for (var i = 0; i < groups.Count; i++)
        {
            if (groups[i].Code == 0 && groups[i].Value.Equals("SECTION", StringComparison.OrdinalIgnoreCase) &&
                i + 1 < groups.Count && groups[i + 1].Code == 2)
            {
                section = groups[++i].Value;
                continue;
            }
            if (groups[i].Code == 0 && groups[i].Value.Equals("ENDSEC", StringComparison.OrdinalIgnoreCase))
                section = "";
            if (!section.Equals("HEADER", StringComparison.OrdinalIgnoreCase) || groups[i].Code != 9) continue;
            var value = i + 1 < groups.Count ? groups[i + 1].Value : "";
            switch (groups[i].Value.ToUpperInvariant())
            {
                case "$ACADVER": document.Header.AcadVersion = value; break;
                case "$DWGCODEPAGE": document.Header.CodePage = value; break;
                case "$MEASUREMENT": document.Header.Measurement = value == "1" ? "Metric (1)" : value == "0" ? "Imperial (0)" : value; break;
                case "$INSUNITS": document.Header.InsUnits = value; break;
            }
        }
        document.Log.Add($"[{DateTime.Now:HH:mm:ss}] Header parsed");
    }

    private static void ParseEntities(IReadOnlyList<DxfGroup> groups, DxfDocument document)
    {
        var section = "";
        for (var i = 0; i < groups.Count; i++)
        {
            var group = groups[i];
            if (group.Code == 0 && group.Value.Equals("SECTION", StringComparison.OrdinalIgnoreCase) &&
                i + 1 < groups.Count && groups[i + 1].Code == 2)
            {
                section = groups[++i].Value;
                continue;
            }
            if (group.Code == 0 && group.Value.Equals("ENDSEC", StringComparison.OrdinalIgnoreCase))
            {
                section = "";
                continue;
            }
            if (!section.Equals("ENTITIES", StringComparison.OrdinalIgnoreCase) || group.Code != 0) continue;
            var type = group.Value.ToUpperInvariant();
            if (StructuralEntities.Contains(type)) continue;
            var start = i;
            var end = i + 1;
            while (end < groups.Count && groups[end].Code != 0) end++;
            var source = groups.Skip(start).Take(end - start).ToList();
            document.EntityCounts[type] = document.EntityCounts.GetValueOrDefault(type) + 1;
            GeometryEntity? entity = type switch
            {
                "LINE" => CreateLine(source, document.Entities.Count + 1, document.Log),
                "CIRCLE" => CreateCircle(source, document.Entities.Count + 1, document.Log),
                "ARC" => CreateArc(source, document.Entities.Count + 1, document.Log),
                _ => null
            };
            if (entity is null)
            {
                if (type is not ("LINE" or "CIRCLE" or "ARC"))
                {
                    document.UnsupportedCounts[type] = document.UnsupportedCounts.GetValueOrDefault(type) + 1;
                    document.Log.Add($"Unsupported entity: {type}");
                }
            }
            else
            {
                document.Entities.Add(entity);
                document.Layers.Add(entity.LayerName);
                document.LayerCounts[entity.LayerName] = document.LayerCounts.GetValueOrDefault(entity.LayerName) + 1;
            }
            i = end - 1;
        }
        document.Log.Add($"[{DateTime.Now:HH:mm:ss}] Layers parsed: {document.Layers.Count}");
        document.Log.Add($"[{DateTime.Now:HH:mm:ss}] Entities parsed: {document.Entities.Count}");
    }

    private static GeometryLine? CreateLine(IReadOnlyList<DxfGroup> source, int id, List<string> log)
    {
        if (!TryDouble(source, 10, out var x1) || !TryDouble(source, 20, out var y1) ||
            !TryDouble(source, 11, out var x2) || !TryDouble(source, 21, out var y2))
        {
            log.Add($"Invalid coordinate: LINE #{id}");
            return null;
        }
        return new GeometryLine
        {
            Id = id, EntityType = "LINE", LayerName = Value(source, 8, "0"), OriginalEntity = source,
            Start = new DxfPoint(x1, y1, Double(source, 30)), End = new DxfPoint(x2, y2, Double(source, 31))
        };
    }

    private static GeometryCircle? CreateCircle(IReadOnlyList<DxfGroup> source, int id, List<string> log)
    {
        if (!TryDouble(source, 10, out var x) || !TryDouble(source, 20, out var y) ||
            !TryDouble(source, 40, out var radius) || radius < 0)
        {
            log.Add($"Invalid coordinate: CIRCLE #{id}");
            return null;
        }
        return new GeometryCircle
        {
            Id = id, EntityType = "CIRCLE", LayerName = Value(source, 8, "0"), OriginalEntity = source,
            Center = new DxfPoint(x, y, Double(source, 30)), Radius = radius
        };
    }

    private static GeometryArc? CreateArc(IReadOnlyList<DxfGroup> source, int id, List<string> log)
    {
        if (!TryDouble(source, 10, out var x) || !TryDouble(source, 20, out var y) ||
            !TryDouble(source, 40, out var radius) || radius < 0 ||
            !TryDouble(source, 50, out var startAngle) || !TryDouble(source, 51, out var endAngle))
        {
            log.Add($"Invalid ARC: missing or invalid center, radius, or angle (#{id})");
            return null;
        }
        return new GeometryArc
        {
            Id = id, EntityType = "ARC", LayerName = Value(source, 8, "0"), OriginalEntity = source,
            Center = new DxfPoint(x, y, Double(source, 30)), Radius = radius,
            StartAngle = startAngle, EndAngle = endAngle
        };
    }
    private static void DetectDuplicateLines(DxfDocument document)
    {
        var lines = document.Entities.OfType<GeometryLine>().ToList();
        for (var i = 0; i < lines.Count; i++)
        for (var j = 0; j < i; j++)
        {
            var same = Near(lines[i].Start, lines[j].Start) && Near(lines[i].End, lines[j].End);
            var reverse = Near(lines[i].Start, lines[j].End) && Near(lines[i].End, lines[j].Start);
            if (!same && !reverse) continue;
            lines[i].IsDuplicate = true;
            lines[i].DuplicateOf = lines[j].Id;
            break;
        }
    }

    private static bool Near(DxfPoint a, DxfPoint b) =>
        Math.Abs(a.X - b.X) <= Epsilon && Math.Abs(a.Y - b.Y) <= Epsilon && Math.Abs(a.Z - b.Z) <= Epsilon;
    private static string Value(IEnumerable<DxfGroup> groups, int code, string fallback) =>
        groups.FirstOrDefault(x => x.Code == code).Value ?? fallback;
    private static double Double(IEnumerable<DxfGroup> groups, int code) =>
        TryDouble(groups, code, out var value) ? value : 0;
    private static bool TryDouble(IEnumerable<DxfGroup> groups, int code, out double value)
    {
        var text = groups.FirstOrDefault(x => x.Code == code).Value;
        return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}
