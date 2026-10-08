namespace VCutting.Viewer;

public sealed class SectionSelectionState
{
    public double CenterX { get; }
    public double MinX { get; }
    public double MaxX { get; }
    public double SectionX { get; private set; }

    private SectionSelectionState(double minX, double maxX)
    {
        MinX = minX;
        MaxX = maxX;
        CenterX = (minX + maxX) / 2;
        SectionX = CenterX;
    }

    public static SectionSelectionState Create(DxfDocument document)
    {
        var material = document.Entities
            .Where(entity => entity.LayerName.Trim('-', ' ').Equals("L", StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (material.Count == 0) material = document.Entities.ToList();
        var bounds = new DxfBounds();
        foreach (var entity in material) entity.ExpandBounds(bounds);
        if (bounds.IsEmpty) throw new InvalidOperationException("DXF에 유효한 형상이 없습니다.");
        return new SectionSelectionState(bounds.MinX, bounds.MaxX);
    }

    public bool SetSectionX(double value)
    {
        var clamped = Math.Clamp(value, MinX, MaxX);
        if (Math.Abs(clamped - SectionX) <= 1e-9) return false;
        SectionX = clamped;
        return true;
    }

    public bool Reset() => SetSectionX(CenterX);
}
