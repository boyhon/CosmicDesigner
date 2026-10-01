using System.Globalization;
using System.IO;
using System.Text;

namespace DXFExplorer;

public sealed record DxfLineInfo(int LineNumber, string RawText, string Description)
{
    public int? GroupCode { get; init; }
    public string? EntityType { get; init; }
    public string? SectionType { get; init; }
    public string? LayerName { get; init; }
}

public static class DxfLineAnalyzer
{
    private static readonly HashSet<string> EntityNames = new(StringComparer.OrdinalIgnoreCase)
        { "LINE", "LWPOLYLINE", "POLYLINE", "VERTEX", "ARC", "CIRCLE", "TEXT", "MTEXT" };

    public static IReadOnlyList<DxfLineInfo> Analyze(string path)
    {
        using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        Span<byte> marker = stackalloc byte[22];
        var markerLength = stream.Read(marker);
        stream.Position = 0;
        if (Encoding.ASCII.GetString(marker[..markerLength]).StartsWith("AutoCAD Binary DXF", StringComparison.Ordinal)) return [];
        using var reader = new StreamReader(stream, Encoding.Latin1, true);
        var lines = new List<string>();
        while (reader.ReadLine() is { } line) lines.Add(line);
        return Analyze(lines);
    }

    public static IReadOnlyList<DxfLineInfo> Analyze(IReadOnlyList<string> lines)
    {
        var result = new List<DxfLineInfo>(lines.Count);
        string? entity = null, section = null, layer = null;
        for (var index = 0; index < lines.Count; index += 2)
        {
            var codeText = lines[index];
            var hasValue = index + 1 < lines.Count;
            var valueText = hasValue ? lines[index + 1] : string.Empty;
            if (!int.TryParse(codeText.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var code))
            {
                result.Add(Create(index + 1, codeText, "올바른 Group Code가 아닙니다", null, entity, section, layer));
                if (hasValue) result.Add(Create(index + 2, valueText, "짝이 없는 원문 값", null, entity, section, layer));
                continue;
            }
            var meaning = DescribeCode(code, entity, section);
            result.Add(Create(index + 1, codeText, meaning + " Group Code", code, entity, section, layer));
            if (!hasValue) continue;
            var value = valueText.Trim();
            var valueDescription = DescribeValue(code, value, meaning, section);
            if (code == 0)
            {
                if (value.Equals("SECTION", StringComparison.OrdinalIgnoreCase)) entity = null;
                else if (value.Equals("ENDSEC", StringComparison.OrdinalIgnoreCase)) { entity = null; section = null; }
                else if (value.Equals("EOF", StringComparison.OrdinalIgnoreCase)) { entity = null; section = null; }
                else if (EntityNames.Contains(value)) entity = value.ToUpperInvariant();
                else if (value.Equals("SEQEND", StringComparison.OrdinalIgnoreCase)) entity = null;
            }
            else if (code == 2 && section is null) section = value.ToUpperInvariant();
            else if (code == 8) layer = value;
            result.Add(Create(index + 2, valueText, valueDescription, code, entity, section, layer));
        }
        return result;
    }

    private static DxfLineInfo Create(int number, string raw, string description, int? code, string? entity, string? section, string? layer) =>
        new(number, raw, description) { GroupCode = code, EntityType = entity, SectionType = section, LayerName = layer };

    private static string DescribeCode(int code, string? entity, string? section) => code switch
    {
        0 => "다음 줄이 DXF 객체 또는 구조 종류임을 나타내는", 2 => section is null ? "SECTION 이름을 나타내는" : "블록 또는 구조 이름을 나타내는", 8 => "레이어 이름을 나타내는",
        10 => entity switch { "LINE" => "시작점 X 좌표", "ARC" or "CIRCLE" => "중심 X 좌표", "TEXT" or "MTEXT" => "삽입점 X 좌표", "VERTEX" or "LWPOLYLINE" or "POLYLINE" => "정점 X 좌표", _ => "기준점 X 좌표" },
        20 => entity switch { "LINE" => "시작점 Y 좌표", "ARC" or "CIRCLE" => "중심 Y 좌표", "TEXT" or "MTEXT" => "삽입점 Y 좌표", "VERTEX" or "LWPOLYLINE" or "POLYLINE" => "정점 Y 좌표", _ => "기준점 Y 좌표" },
        30 => "기준점 Z 좌표", 11 => entity == "LINE" ? "끝점 X 좌표" : "두 번째 점 X 좌표", 21 => entity == "LINE" ? "끝점 Y 좌표" : "두 번째 점 Y 좌표", 31 => entity == "LINE" ? "끝점 Z 좌표" : "두 번째 점 Z 좌표",
        40 => entity is "ARC" or "CIRCLE" ? "반지름" : entity is "TEXT" or "MTEXT" ? "문자 높이" : "실수 값", 41 => entity == "LWPOLYLINE" ? "시작 폭" : "X 방향 배율 또는 실수 값", 42 => entity is "LWPOLYLINE" or "VERTEX" ? "호 돌출량(Bulge)" : "실수 값",
        50 => entity == "ARC" ? "시작 각도" : entity is "TEXT" or "MTEXT" ? "문자 회전 각도" : "각도", 51 => entity == "ARC" ? "종료 각도" : "각도", 1 => entity is "TEXT" or "MTEXT" ? "문자열" : "기본 문자열 값", 3 => entity == "MTEXT" ? "이어지는 문자열" : "추가 문자열 값",
        5 => "객체 핸들", 6 => "선 종류 이름", 62 => "색상 번호", 70 => entity is "LWPOLYLINE" or "POLYLINE" ? "폴리라인 플래그" : "정수 플래그", 90 => entity == "LWPOLYLINE" ? "정점 개수" : "32비트 정수 값", 100 => "객체 서브클래스 표식", 210 => "돌출 방향 X", 220 => "돌출 방향 Y", 230 => "돌출 방향 Z", _ => $"DXF 속성({code})을 나타내는"
    };

    private static string DescribeValue(int code, string value, string meaning, string? section)
    {
        if (code == 0) return value.ToUpperInvariant() switch { "SECTION" => "새로운 DXF SECTION 시작", "ENDSEC" => "현재 DXF SECTION 종료", "EOF" => "DXF 파일 끝", "SEQEND" => "연속 Entity 데이터 종료", var name when EntityNames.Contains(name) => $"{name} Entity 시작", _ => $"{value} 객체 시작" };
        if (code == 2 && section is null) return $"{value} SECTION";
        if (code == 8) return $"레이어 = {value}";
        if (code is 50 or 51) return $"{meaning} = {value}°";
        return $"{meaning} = {value}";
    }
}