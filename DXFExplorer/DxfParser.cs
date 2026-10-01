using System.IO;
using System.Globalization;
using System.Text;

namespace DXFExplorer;

public static class DxfParser
{
    private readonly record struct Pair(int Code, string Value);
    private sealed class Bounds
    {
        public double MinX = double.PositiveInfinity, MinY = double.PositiveInfinity, MinZ = double.PositiveInfinity;
        public double MaxX = double.NegativeInfinity, MaxY = double.NegativeInfinity, MaxZ = double.NegativeInfinity;
        public bool HasX, HasY, HasZ;
        public void Add(int code, double value)
        {
            var axis = (Math.Abs(code) % 100) / 10 - 1;
            if (axis == 0) { MinX = Math.Min(MinX, value); MaxX = Math.Max(MaxX, value); HasX = true; }
            else if (axis == 1) { MinY = Math.Min(MinY, value); MaxY = Math.Max(MaxY, value); HasY = true; }
            else if (axis == 2) { MinZ = Math.Min(MinZ, value); MaxZ = Math.Max(MaxZ, value); HasZ = true; }
        }
    }

    public static DxfDetails Parse(string path)
    {
        var info = new FileInfo(path);
        var result = new DxfDetails { FileSize = DxfFileItem.FormatSize(info.Length) };
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        Span<byte> marker = stackalloc byte[22];
        var markerLength = stream.Read(marker);
        stream.Position = 0;
        if (Encoding.ASCII.GetString(marker[..markerLength]).StartsWith("AutoCAD Binary DXF", StringComparison.Ordinal))
        {
            result.Warning = "바이너리 DXF는 현재 메타데이터 분석을 지원하지 않습니다.";
            result.Version = "바이너리 DXF";
            return result;
        }

        var pairs = ReadPairs(stream).ToList();
        string section = string.Empty;
        var variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var entities = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var layers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var bounds = new Bounds();
        var currentEntity = string.Empty;
        var currentLayer = "0";
        var entityOpen = false;

        void CommitEntity()
        {
            if (!entityOpen) return;
            entities[currentEntity] = entities.GetValueOrDefault(currentEntity) + 1;
            layers[currentLayer] = layers.GetValueOrDefault(currentLayer) + 1;
            entityOpen = false;
        }

        for (var i = 0; i < pairs.Count; i++)
        {
            var p = pairs[i];
            if (p.Code == 0 && p.Value.Equals("SECTION", StringComparison.OrdinalIgnoreCase) &&
                i + 1 < pairs.Count && pairs[i + 1].Code == 2)
            {
                CommitEntity();
                section = pairs[++i].Value.ToUpperInvariant();
                continue;
            }
            if (p.Code == 0 && p.Value.Equals("ENDSEC", StringComparison.OrdinalIgnoreCase))
            {
                CommitEntity();
                section = string.Empty;
                continue;
            }
            if (section == "HEADER" && p.Code == 9)
            {
                var name = p.Value;
                if (i + 1 < pairs.Count)
                    variables[name] = pairs[i + 1].Value;
                continue;
            }
            if (section != "ENTITIES") continue;
            if (p.Code == 0)
            {
                CommitEntity();
                if (p.Value is not ("SEQEND" or "ENDSEC" or "EOF"))
                {
                    currentEntity = p.Value.ToUpperInvariant();
                    currentLayer = "0";
                    entityOpen = true;
                }
                continue;
            }
            if (!entityOpen) continue;
            if (p.Code == 8) currentLayer = p.Value;
            if (IsCoordinateCode(p.Code) &&
                double.TryParse(p.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var coordinate))
                bounds.Add(p.Code, coordinate);
        }
        CommitEntity();

        var acadVer = variables.GetValueOrDefault("$ACADVER", "-");
        result.Version = acadVer;
        result.AutoCadGeneration = GetAutoCadGeneration(acadVer);
        result.CodePage = variables.GetValueOrDefault("$DWGCODEPAGE", "선언 없음");
        result.CoordinateDimension = bounds.HasZ && (Math.Abs(bounds.MinZ) > 1e-12 || Math.Abs(bounds.MaxZ) > 1e-12) ? "3D (X, Y, Z)" : "2D (X, Y)";
        result.TotalEntities = entities.Values.Sum();
        result.MainEntities = entities.Count == 0 ? "-" : string.Join(", ", entities.OrderByDescending(x => x.Value).Take(8).Select(x => $"{x.Key} {x.Value:N0}"));
        result.UsedLayers = layers.Count == 0 ? "-" : string.Join(", ", layers.OrderBy(x => x.Key).Select(x => x.Key));
        result.EntitiesByLayer = layers.Count == 0 ? "-" : string.Join(Environment.NewLine, layers.OrderByDescending(x => x.Value).ThenBy(x => x.Key).Select(x => $"{x.Key}: {x.Value:N0}"));
        result.CoordinateBounds = bounds.HasX
            ? $"X: {F(bounds.MinX)} ~ {F(bounds.MaxX)}{Environment.NewLine}Y: {F(bounds.MinY)} ~ {F(bounds.MaxY)}" +
              (bounds.HasZ ? $"{Environment.NewLine}Z: {F(bounds.MinZ)} ~ {F(bounds.MaxZ)}" : "")
            : "좌표 없음";
        if (bounds.HasX)
        {
            var dx = bounds.MaxX - bounds.MinX;
            var dy = bounds.MaxY - bounds.MinY;
            var dz = bounds.HasZ ? bounds.MaxZ - bounds.MinZ : 0;
            result.MaximumSize = bounds.HasZ ? $"X {F(dx)} × Y {F(dy)} × Z {F(dz)} (최대 {F(Math.Max(dx, Math.Max(dy, dz)))})"
                                             : $"X {F(dx)} × Y {F(dy)} (최대 {F(Math.Max(dx, dy))})";
        }
        result.Measurement = MeasurementName(variables.GetValueOrDefault("$MEASUREMENT"));
        result.InsUnits = InsUnitsName(variables.GetValueOrDefault("$INSUNITS"));
        return result;
    }

    private static IEnumerable<Pair> ReadPairs(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.Latin1, true, 65536, leaveOpen: true);
        while (reader.ReadLine() is { } codeLine)
        {
            var valueLine = reader.ReadLine();
            if (valueLine is null) yield break;
            if (int.TryParse(codeLine.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var code))
                yield return new Pair(code, valueLine.Trim());
        }
    }

    private static bool IsCoordinateCode(int code) =>
        (code >= 10 && code <= 18) || (code >= 20 && code <= 28) || (code >= 30 && code <= 38) ||
        (code >= 110 && code <= 138) || (code >= 210 && code <= 238);

    private static string F(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);

    private static string MeasurementName(string? value) => value switch
    {
        "0" => "0 — 영국식(Imperial)",
        "1" => "1 — 미터법(Metric)",
        null or "" => "선언 없음",
        _ => value
    };

    private static string InsUnitsName(string? value)
    {
        string[] names = ["단위 없음", "인치", "피트", "마일", "밀리미터", "센티미터", "미터", "킬로미터",
            "마이크로인치", "밀", "야드", "옹스트롬", "나노미터", "마이크로미터", "데시미터", "데카미터",
            "헥토미터", "기가미터", "천문 단위", "광년", "파섹", "US Survey 피트", "US Survey 인치",
            "US Survey 야드", "US Survey 마일"];
        return int.TryParse(value, out var index) && index >= 0 && index < names.Length
            ? $"{index} — {names[index]}" : string.IsNullOrWhiteSpace(value) ? "선언 없음" : value;
    }

    private static string GetAutoCadGeneration(string version) => version switch
    {
        "AC1006" => "AutoCAD R10",
        "AC1009" => "AutoCAD R11/R12",
        "AC1012" => "AutoCAD R13",
        "AC1014" => "AutoCAD R14",
        "AC1015" => "AutoCAD 2000/2000i/2002",
        "AC1018" => "AutoCAD 2004/2005/2006",
        "AC1021" => "AutoCAD 2007/2008/2009",
        "AC1024" => "AutoCAD 2010/2011/2012",
        "AC1027" => "AutoCAD 2013–2017",
        "AC1032" => "AutoCAD 2018 이후",
        _ => version == "-" ? "확인 불가" : "알 수 없는 세대"
    };
}
