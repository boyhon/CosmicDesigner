using System.IO;
using System.Collections.ObjectModel;

namespace DXFExplorer;

public sealed class FolderNode
{
    public string Name { get; }
    public string FullPath { get; }
    public ObservableCollection<FolderNode> Children { get; } = [];
    public bool IsExpanded { get; set; }
    public bool IsSelected { get; set; }
    public bool HasLoadedChildren { get; set; }

    public FolderNode(string name, string fullPath, bool addPlaceholder = true)
    {
        Name = name;
        FullPath = fullPath;
        if (addPlaceholder)
            Children.Add(new FolderNode("불러오는 중...", string.Empty, false));
    }
}

public sealed class DxfFileItem
{
    public required string Name { get; init; }
    public required string FullPath { get; init; }
    public required long Size { get; init; }
    public required DateTime Modified { get; init; }
    public string ModifiedText => Modified.ToString("yyyy-MM-dd tt h:mm");
    public string TypeText => "DXF 파일";
    public string SizeText => FormatSize(Size);

    public static string FormatSize(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB"];
        double value = bytes;
        var unit = 0;
        while (value >= 1024 && unit < units.Length - 1) { value /= 1024; unit++; }
        return unit == 0 ? $"{value:0} {units[unit]}" : $"{value:0.#} {units[unit]}";
    }
}

public sealed record DetailRow(string Label, string Value);

public sealed class DxfDetails
{
    public string FileSize { get; set; } = "-";
    public string Version { get; set; } = "-";
    public string AutoCadGeneration { get; set; } = "-";
    public string CodePage { get; set; } = "-";
    public string CoordinateDimension { get; set; } = "-";
    public int TotalEntities { get; set; }
    public string MainEntities { get; set; } = "-";
    public string UsedLayers { get; set; } = "-";
    public string EntitiesByLayer { get; set; } = "-";
    public string CoordinateBounds { get; set; } = "-";
    public string MaximumSize { get; set; } = "-";
    public string Measurement { get; set; } = "-";
    public string InsUnits { get; set; } = "-";
    public string? Warning { get; set; }

    public IReadOnlyList<DetailRow> ToRows() =>
    [
        new("파일 크기", FileSize),
        new("DXF 버전", Version),
        new("대응 AutoCAD 세대", AutoCadGeneration),
        new("문자 코드 선언", CodePage),
        new("좌표 차원", CoordinateDimension),
        new("전체 Entity", TotalEntities.ToString("N0")),
        new("주요 Entity", MainEntities),
        new("실제 사용 레이어", UsedLayers),
        new("레이어별 Entity", EntitiesByLayer),
        new("전체 형상 좌표 범위", CoordinateBounds),
        new("좌표상 최대 크기", MaximumSize),
        new("$MEASUREMENT", Measurement),
        new("$INSUNITS", InsUnits)
    ];
}
