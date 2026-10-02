using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Runtime.CompilerServices;

namespace CosmicDesigner;

public enum SectionAxis { W, H }
public enum BendDirection { Up, Down, Left, Right }
public enum CutKind { Corner, Edge, Hole }
public enum ContourKind { Outer, Inner }

public abstract class DesignObject : INotifyPropertyChanged
{
    string _id = "";
    int _sequence;
    public string Id { get => _id; set => Set(ref _id, value); }
    public int Sequence { get => _sequence; set => Set(ref _sequence, value); }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    { if (EqualityComparer<T>.Default.Equals(field, value)) return false; field = value; PropertyChanged?.Invoke(this, new(name)); return true; }
}

public sealed class MaterialModel : DesignObject
{
    double _width = 300, _height = 300, _thickness = .2;
    public double Width { get => _width; internal set => Set(ref _width, value); }
    public double Height { get => _height; internal set => Set(ref _height, value); }
    public double Thickness { get => _thickness; internal set => Set(ref _thickness, value); }
}

public sealed class SectionSegment : DesignObject
{
    double _length;
    public SectionAxis Axis { get; init; }
    public int Index { get; init; }
    public double Length { get => _length; set => Set(ref _length, Math.Max(0.01, value)); }
}

public sealed class BendObject : DesignObject
{
    double _position, _bendAngle = 90, _vCutAngle = 90, _vCutDepth, _residualThickness;
    public SectionAxis Axis { get; init; }
    BendDirection _direction;
    public BendDirection Direction { get => _direction; set => Set(ref _direction, value); }
    public double Position { get => _position; set => Set(ref _position, Math.Max(0, value)); }
    public double BendAngle { get => _bendAngle; internal set => Set(ref _bendAngle, value); }
    public double VCutAngle { get => _vCutAngle; internal set => Set(ref _vCutAngle, value); }
    public double VCutDepth { get => _vCutDepth; internal set => Set(ref _vCutDepth, value); }
    public double ResidualThickness { get => _residualThickness; internal set => Set(ref _residualThickness, value); }
    public string Layer => Direction is BendDirection.Up or BendDirection.Left ? "V" : "V1";
}

public sealed class ContourObject : DesignObject
{
    public ContourKind Kind { get; init; }
    public List<GeometrySegment> Segments { get; } = [];
}
public sealed class CutOperation : DesignObject
{
    public CutKind Kind { get; init; }
    public string Shape { get; set; } = "Circle";
    public string ParentContourId { get; set; } = "OUTER";
    public double CenterX { get; set; }
    public double CenterY { get; set; }
    public double Width { get; set; } = 5;
    public double Height { get; set; } = 5;
    public int Sides { get; set; } = 7;
    public List<GeometrySegment> Geometry { get; } = [];
}
public sealed class MicroJoint : DesignObject
{
    public string ParentContourId { get; set; } = "OUTER";
    public double Position { get; set; }
    public double Width { get; set; } = 1;
    public bool Enabled { get; set; } = true;
}
public sealed class SectionViewState
{
    public bool Dimensions { get; set; } = true;
    public bool BentMode { get; set; }
    public int RotationQuarterTurns { get; set; }
}

public abstract record GeometrySegment;
public sealed record LineSegment(double X1, double Y1, double X2, double Y2) : GeometrySegment;
public sealed record ArcSegment(double Cx, double Cy, double Radius, double StartDegrees, double EndDegrees) : GeometrySegment;
public sealed record CircleSegment(double Cx, double Cy, double Radius) : GeometrySegment;
public sealed class GeometryObject : DesignObject
{
    public string Layer { get; init; } = "L";
    public string EntityType { get; init; } = "LINE";
    public GeometrySegment Geometry { get; init; } = new LineSegment(0,0,0,0);
}

public sealed class CosmicDesignerDocument : INotifyPropertyChanged
{
    bool _preserveOuterContour;
    public MaterialModel Material { get; } = new() { Id = "MATERIAL" };
    public ContourObject OuterContour { get; } = new() { Id = "OUTER", Kind = ContourKind.Outer };
    public ObservableCollection<ContourObject> InnerContours { get; } = [];
    public ObservableCollection<BendObject> Bends { get; } = [];
    public ObservableCollection<SectionSegment> WSegments { get; } = [];
    public ObservableCollection<SectionSegment> HSegments { get; } = [];
    public ObservableCollection<CutOperation> Cuts { get; } = [];
    public ObservableCollection<MicroJoint> MicroJoints { get; } = [];
    public SectionViewState WView { get; } = new();
    public SectionViewState HView { get; } = new();
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? Changed;

    public CosmicDesignerDocument() { Reset(); }
    public void Reset()
    {
        _preserveOuterContour=false;
        Bends.Clear(); WSegments.Clear(); HSegments.Clear(); Cuts.Clear(); MicroJoints.Clear(); InnerContours.Clear();
        WSegments.Add(new() { Id = "W-S001", Axis = SectionAxis.W, Length = 300 });
        HSegments.Add(new() { Id = "H-S001", Axis = SectionAxis.H, Length = 300 });
        Material.Width = Material.Height = 300; Material.Thickness = .2;
        WView.Dimensions=HView.Dimensions=true;WView.BentMode=HView.BentMode=false;WView.RotationQuarterTurns=HView.RotationQuarterTurns=0;
        RebuildOuterContour(); Recalculate();
    }
    public BendObject AddBend(SectionAxis axis, double position, BendDirection direction)
    {
        var bend = new BendObject { Id = $"B{Bends.Count + 1:000}", Sequence = NextSequence(), Axis = axis, Position = position, Direction = direction };
        Bends.Add(bend); RebuildSegments(axis); Recalculate(); return bend;
    }
    public CutOperation AddHole(string shape,double x,double y)
    {
        var cut=new CutOperation{Id=$"H{Cuts.Count+1:000}",Sequence=NextSequence(),Kind=CutKind.Hole,Shape=shape,CenterX=Math.Clamp(x,2.5,Math.Max(2.5,Material.Width-2.5)),CenterY=Math.Clamp(y,2.5,Math.Max(2.5,Material.Height-2.5)),Width=5,Height=5,Sides=7};HoleGeometryEngine.Rebuild(cut);Cuts.Add(cut);var contour=new ContourObject{Id=cut.Id,Kind=ContourKind.Inner};contour.Segments.AddRange(cut.Geometry);InnerContours.Add(contour);Recalculate();return cut;
    }
    public void UpdateCut(CutOperation cut,double x,double y,double width,double height,int sides){cut.Width=Math.Max(.1,width);cut.Height=Math.Max(.1,height);cut.CenterX=Math.Clamp(x,cut.Width/2,Math.Max(cut.Width/2,Material.Width-cut.Width/2));cut.CenterY=Math.Clamp(y,cut.Height/2,Math.Max(cut.Height/2,Material.Height-cut.Height/2));cut.Sides=Math.Max(3,sides);HoleGeometryEngine.Rebuild(cut);var contour=InnerContours.FirstOrDefault(c=>c.Id==cut.Id);if(contour is not null){contour.Segments.Clear();contour.Segments.AddRange(cut.Geometry);}Recalculate();}
    public void UpdateBend(BendObject bend, double position, BendDirection direction, int sequence)
    { bend.Position=Math.Clamp(position,0,bend.Axis==SectionAxis.W?Material.Width:Material.Height);bend.Direction=direction;bend.Sequence=Math.Max(1,sequence);RebuildSegments(bend.Axis);Recalculate(); }
    public bool DeleteObject(DesignObject item)
    {
        if(item is BendObject bend&&Bends.Remove(bend)){RebuildSegments(bend.Axis);Recalculate();return true;}
        if(item is CutOperation cut&&Cuts.Remove(cut)){var contour=InnerContours.FirstOrDefault(x=>x.Id==cut.Id);if(contour is not null)InnerContours.Remove(contour);Recalculate();return true;}
        if(item is MicroJoint joint&&MicroJoints.Remove(joint)){Recalculate();return true;}
        if(item is GeometryObject geometry&&OuterContour.Segments.Remove(geometry.Geometry)){_preserveOuterContour=true;Recalculate();return true;}
        return false;
    }
    public bool TryMergeBoundaryCut(CutOperation cut)
    {
        if(!TryGetBoundaryMerge(cut,out var contour))return false;Cuts.Remove(cut);var inner=InnerContours.FirstOrDefault(x=>x.Id==cut.Id);if(inner is not null)InnerContours.Remove(inner);OuterContour.Segments.Clear();OuterContour.Segments.AddRange(contour);_preserveOuterContour=true;Recalculate();return true;
    }
    public bool CanMergeBoundaryCut(CutOperation cut)=>TryGetBoundaryMerge(cut,out _);
    public void UpdateOuterContourLine(IReadOnlyList<GeometrySegment> source,int index,LineSegment replacement){var updated=OuterContourEditEngine.UpdateConnectedLine(source,index,replacement);OuterContour.Segments.Clear();OuterContour.Segments.AddRange(updated);_preserveOuterContour=true;Recalculate();}
    bool TryGetBoundaryMerge(CutOperation cut,out IReadOnlyList<GeometrySegment> contour){contour=[];if(!string.Equals(cut.Shape,"Rectangle",StringComparison.OrdinalIgnoreCase))return false;var rect=new Rect(cut.CenterX-cut.Width/2,cut.CenterY-cut.Height/2,cut.Width,cut.Height);return BoundaryCutEngine.TrySubtractRectangle(OuterContour.Segments,rect,out contour);}
    public void UpdateSegment(SectionSegment segment,double length)
    {
        segment.Length=Math.Max(.01,length);var list=segment.Axis==SectionAxis.W?WSegments:HSegments;var bends=Bends.Where(b=>b.Axis==segment.Axis).OrderBy(b=>b.Position).ToList();double p=0;
        for(var i=0;i<bends.Count;i++){p+=list[i].Length;bends[i].Position=p;}Recalculate();
    }
    public void ChangeThickness(double thickness) { if (thickness <= 0) throw new ArgumentOutOfRangeException(nameof(thickness)); Material.Thickness = thickness; Recalculate(); }
    public void Recalculate() { BendCalculationEngine.Recalculate(this); RebuildOuterContour(); PropertyChanged?.Invoke(this, new(null)); Changed?.Invoke(this, EventArgs.Empty); }
    internal void ApplyImportedGeometry(double width,double height,IEnumerable<GeometrySegment> outer,IEnumerable<CutOperation> cuts,IEnumerable<(SectionAxis Axis,double Position,BendDirection Direction)> bends,double thickness=.2)
    {
        Bends.Clear();Cuts.Clear();InnerContours.Clear();MicroJoints.Clear();WSegments.Clear();HSegments.Clear();
        var bendList=bends.ToList();
        WSegments.Add(new(){Id="W-S001",Axis=SectionAxis.W,Index=0,Length=Math.Max(.01,width+bendList.Count(x=>x.Axis==SectionAxis.W)*thickness/2)});
        HSegments.Add(new(){Id="H-S001",Axis=SectionAxis.H,Index=0,Length=Math.Max(.01,height+bendList.Count(x=>x.Axis==SectionAxis.H)*thickness/2)});
        Material.Thickness=thickness;Material.Width=Math.Max(.01,width);Material.Height=Math.Max(.01,height);
        foreach(var b in bendList.OrderBy(x=>x.Axis).ThenBy(x=>x.Position))AddBend(b.Axis,b.Position,b.Direction);
        Cuts.Clear();InnerContours.Clear();foreach(var cut in cuts){Cuts.Add(cut);var contour=new ContourObject{Id=cut.Id,Kind=ContourKind.Inner};contour.Segments.AddRange(cut.Geometry);InnerContours.Add(contour);}
        _preserveOuterContour=true;OuterContour.Segments.Clear();OuterContour.Segments.AddRange(outer);Recalculate();
    }
    internal void PreserveOuterGeometry(IEnumerable<GeometrySegment> geometry){_preserveOuterContour=true;OuterContour.Segments.Clear();OuterContour.Segments.AddRange(geometry);}
    public int NextSequence() => Bends.Count + Cuts.Count + MicroJoints.Count + 1;
    void RebuildSegments(SectionAxis axis)
    {
        var list=axis==SectionAxis.W?WSegments:HSegments;var currentFlat=axis==SectionAxis.W?Material.Width:Material.Height;var total=list.Count>0?list.Sum(s=>s.Length):currentFlat;var positions=Bends.Where(b=>b.Axis==axis).Select(b=>Math.Clamp(b.Position,0,total)).Order().ToList();list.Clear();double last=0;var i=1;
        foreach(var p in positions){list.Add(new(){Id=$"{axis}-S{i:000}",Axis=axis,Index=i-1,Length=Math.Max(.01,p-last)});last=p;i++;}
        list.Add(new(){Id=$"{axis}-S{i:000}",Axis=axis,Index=i-1,Length=Math.Max(.01,total-last)});
    }
    void RebuildOuterContour()
    {
        if(_preserveOuterContour&&OuterContour.Segments.Count>0)return;
        OuterContour.Segments.Clear(); var w = Material.Width; var h = Material.Height;
        OuterContour.Segments.AddRange([new LineSegment(0,0,w,0), new LineSegment(w,0,w,h), new LineSegment(w,h,0,h), new LineSegment(0,h,0,0)]);
    }
}
