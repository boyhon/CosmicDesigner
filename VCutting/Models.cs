using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Runtime.CompilerServices;

namespace VCutting;

public static class WindowTitleFormatter
{
    public static string Format(string? path,bool modified=false)=>$"{(modified?"*":"")}{(string.IsNullOrWhiteSpace(path)?"Untitled":System.IO.Path.GetFileName(path))} - VCutting";
}

public enum MeasurementUnit{Millimeter,Centimeter,Meter}
public enum UnitChangeMode{PreserveNumbers,PreservePhysicalSize}
public static class MeasurementUnits
{
    public static string Symbol(this MeasurementUnit unit)=>unit switch{MeasurementUnit.Millimeter=>"mm",MeasurementUnit.Meter=>"m",_=>"cm"};
    public static double Metres(this MeasurementUnit unit)=>unit switch{MeasurementUnit.Millimeter=>.001,MeasurementUnit.Meter=>1,_=>.01};
    public static int DxfCode(this MeasurementUnit unit)=>unit switch{MeasurementUnit.Millimeter=>4,MeasurementUnit.Meter=>6,_=>5};
    public static bool TryParse(string? value,out MeasurementUnit unit){unit=value?.ToLowerInvariant() switch{"mm"=>MeasurementUnit.Millimeter,"m"=>MeasurementUnit.Meter,_=>MeasurementUnit.Centimeter};return value is not null&&(value.Equals("mm",StringComparison.OrdinalIgnoreCase)||value.Equals("cm",StringComparison.OrdinalIgnoreCase)||value.Equals("m",StringComparison.OrdinalIgnoreCase));}
}

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
    public ArcSegment? QuarterCircleArc => Shape=="QuarterCircle"?Geometry.OfType<ArcSegment>().FirstOrDefault():null;
    public ArcSegment? SemicircleArc => Shape=="Semicircle"?Geometry.OfType<ArcSegment>().FirstOrDefault():null;
    public double LowerLeftX => CenterX-Width/2;
    public double LowerLeftY => CenterY-Height/2;
    public double Width { get; set; } = 5;
    public double Height { get; set; } = 5;
    public int Sides { get; set; } = 7;
    public int StarStep { get; set; } = 2;
    public double Radius { get; set; } = 2.5;
    public double Rotation { get; set; }
    public string EntityType=>Shape;
    public string Layer=>"L";
    public bool Closed=>true;
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
    public string ParentContourId { get; init; } = "OUTER";
    public string Layer { get; init; } = "L";
    public string EntityType { get; init; } = "LINE";
    public GeometrySegment Geometry { get; init; } = new LineSegment(0,0,0,0);
}

public sealed partial class VCuttingDocument : INotifyPropertyChanged
{
    bool _preserveOuterContour;
    public MaterialModel Material { get; } = new() { Id = "MATERIAL" };
    public ContourObject OuterContour { get; } = new() { Id = "OUTER", Kind = ContourKind.Outer };
    public ObservableCollection<ContourObject> InnerContours { get; } = [];
    public ObservableCollection<BendObject> Bends { get; } = [];
    public ObservableCollection<SectionSegment> WSegments { get; } = [];
    public ObservableCollection<SectionSegment> HSegments { get; } = [];
    public ObservableCollection<CutOperation> Cuts { get; } = [];
    public ObservableCollection<SlitOperation> Slits { get; } = [];
    public ObservableCollection<MicroJoint> MicroJoints { get; } = [];
    public SectionViewState WView { get; } = new();
    public SectionViewState HView { get; } = new();
    public MeasurementUnit Unit { get; private set; }=MeasurementUnit.Centimeter;
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? Changed;

    public VCuttingDocument() { Reset(); }
    public void Reset()
    {
        _preserveOuterContour=false;
        Bends.Clear(); WSegments.Clear(); HSegments.Clear(); Cuts.Clear(); Slits.Clear(); MicroJoints.Clear(); InnerContours.Clear();
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
    public SlitOperation? AddSlit(string shape,IReadOnlyList<GeometrySegment> geometry)
    {
        if(!SlitDrawingEngine.Fits(geometry,Material.Width,Material.Height))return null;
        var number=1;while(Slits.Any(s=>s.Id==$"SL{number:000}"))number++;
        var slit=new SlitOperation{Id=$"SL{number:000}",Sequence=NextSequence(),Shape=shape};
        slit.Geometry.AddRange(geometry);Slits.Add(slit);Recalculate();return slit;
    }
    public CutOperation AddHole(string shape,double x,double y)
    {
        var number=1;while(Cuts.Any(c=>c.Id==$"H{number:000}"))number++;var cut=new CutOperation{Id=$"H{number:000}",Sequence=NextSequence(),Kind=CutKind.Hole,Shape=shape,CenterX=Math.Clamp(x,2.5,Math.Max(2.5,Material.Width-2.5)),CenterY=Math.Clamp(y,2.5,Math.Max(2.5,Material.Height-2.5)),Width=5,Height=5,Sides=7};HoleGeometryEngine.Rebuild(cut);Cuts.Add(cut);var contour=new ContourObject{Id=cut.Id,Kind=ContourKind.Inner};contour.Segments.AddRange(cut.Geometry);InnerContours.Add(contour);Recalculate();return cut;
    }
    public void UpdateCut(CutOperation cut,double x,double y,double width,double height,int sides){if(RotatedRectangleGeometry.IsParametric(cut)){var p=(RotatedRectangleGeometry.Parameters(cut) with {CenterX=x,CenterY=y,Width=width,Height=height}).ClampCenter(Material.Width,Material.Height);TryUpdateRectangle(cut,p);return;}if(cut.Shape=="RegularStar"){var value=RegularStarGeometry.Parameters(cut);TryUpdateRegularStar(cut,value with {Polygon=(value.Polygon with {CenterX=x,CenterY=y}).ClampCenter(Material.Width,Material.Height)});return;}if(cut.Shape=="RegularPolygon"){var p=(RegularPolygonGeometry.Parameters(cut) with {CenterX=x,CenterY=y}).ClampCenter(Material.Width,Material.Height);TryUpdateRegularPolygon(cut,p);return;}if(cut.Shape=="Compound"){UpdateCompoundClamped(cut,x,y,width,height);return;}if(cut.Shape=="Ellipse"&&cut.Geometry.Count>0&&cut.Geometry.All(g=>g is LineSegment)){UpdateEllipse(cut,x,y,width,height);return;}if(cut.Shape=="QuarterCircle"){var r=Math.Abs(width-cut.Width)>.000001?width:height;width=height=Math.Clamp(r,.1,Math.Min(Material.Width,Material.Height));}if(cut.Shape=="Semicircle"){var vertical=(cut.Geometry.OfType<ArcSegment>().FirstOrDefault()?.StartDegrees??0) is 0 or 180;var radius=Math.Abs(width-cut.Width)>.000001?(vertical?width/2:width):(vertical?height:height/2);radius=Math.Clamp(radius,.1,Math.Min(vertical?Material.Width/2:Material.Width,vertical?Material.Height:Material.Height/2));width=vertical?radius*2:radius;height=vertical?radius:radius*2;}cut.Width=Math.Max(.1,width);cut.Height=Math.Max(.1,height);cut.CenterX=Math.Clamp(x,cut.Width/2,Math.Max(cut.Width/2,Material.Width-cut.Width/2));cut.CenterY=Math.Clamp(y,cut.Height/2,Math.Max(cut.Height/2,Material.Height-cut.Height/2));cut.Sides=Math.Max(3,sides);HoleGeometryEngine.Rebuild(cut);var contour=InnerContours.FirstOrDefault(c=>c.Id==cut.Id);if(contour is not null){contour.Segments.Clear();contour.Segments.AddRange(cut.Geometry);}Recalculate();}
    public CutOperation? AddRegularPolygon(RegularPolygonParameters value)
    {
        if(!value.Fits(Material.Width,Material.Height))return null;
        var number=1;while(Cuts.Any(c=>c.Id==$"H{number:000}"))number++;
        var cut=new CutOperation{Id=$"H{number:000}",Sequence=NextSequence(),Kind=CutKind.Hole,Shape="RegularPolygon"};
        SetRegularPolygon(cut,value);Cuts.Add(cut);var contour=new ContourObject{Id=cut.Id,Kind=ContourKind.Inner};contour.Segments.AddRange(cut.Geometry);InnerContours.Add(contour);Recalculate();return cut;
    }
    static void SetRegularPolygon(CutOperation cut,RegularPolygonParameters value){var p=value.Normalized;cut.CenterX=p.CenterX;cut.CenterY=p.CenterY;cut.Sides=p.Sides;cut.Radius=p.Radius;cut.Rotation=p.Rotation;RegularPolygonGeometry.Rebuild(cut);}
    public bool CanUpdateRegularPolygon(CutOperation cut,RegularPolygonParameters value)=>Cuts.Contains(cut)&&cut.Shape=="RegularPolygon"&&value.Fits(Material.Width,Material.Height);
    public bool TryUpdateRegularPolygon(CutOperation cut,RegularPolygonParameters value)
    {
        if(!CanUpdateRegularPolygon(cut,value))return false;if(RegularPolygonGeometry.Parameters(cut)==value.Normalized)return true;
        SetRegularPolygon(cut,value);var inner=InnerContours.FirstOrDefault(c=>c.Id==cut.Id);if(inner is not null){inner.Segments.Clear();inner.Segments.AddRange(cut.Geometry);}Recalculate();return true;
    }
    public CutOperation? AddRegularStar(RegularStarParameters value)
    {
        if(!value.Fits(Material.Width,Material.Height))return null;
        var number=1;while(Cuts.Any(c=>c.Id==$"H{number:000}"))number++;var cut=new CutOperation{Id=$"H{number:000}",Sequence=NextSequence(),Kind=CutKind.Hole,Shape="RegularStar"};Cuts.Add(cut);InnerContours.Add(new ContourObject{Id=cut.Id,Kind=ContourKind.Inner});
        SetRegularStar(cut,value);SyncStarContour(cut);Recalculate();return cut;
    }
    static void SetRegularStar(CutOperation cut,RegularStarParameters value){var p=value.Normalized;cut.CenterX=p.Polygon.CenterX;cut.CenterY=p.Polygon.CenterY;cut.Sides=p.Polygon.Sides;cut.Radius=p.Polygon.Radius;cut.Rotation=p.Polygon.Rotation;cut.StarStep=p.Step;RegularStarGeometry.Rebuild(cut);}
    void SyncStarContour(CutOperation cut){var inner=InnerContours.FirstOrDefault(c=>c.Id==cut.Id);if(inner is not null){inner.Segments.Clear();inner.Segments.AddRange(cut.Geometry);}}
    public bool CanUpdateRegularStar(CutOperation cut,RegularStarParameters value)=>Cuts.Contains(cut)&&cut.Shape=="RegularStar"&&value.Fits(Material.Width,Material.Height);
    public bool TryUpdateRegularStar(CutOperation cut,RegularStarParameters value){if(!CanUpdateRegularStar(cut,value))return false;if(RegularStarGeometry.Parameters(cut)==value.Normalized)return true;SetRegularStar(cut,value);SyncStarContour(cut);Recalculate();return true;}
    public bool CanUpdateQuarterCircle(CutOperation cut,double x,double y,double radius)
    {
        if(!Cuts.Contains(cut)||cut.QuarterCircleArc is not ArcSegment arc||!double.IsFinite(x)||!double.IsFinite(y)||!double.IsFinite(radius)||radius<.1)return false;
        var sx=arc.StartDegrees is 0 or 270?1:-1;var sy=arc.StartDegrees is 0 or 90?1:-1;
        return Math.Min(x,x+sx*radius)>=-1e-9&&Math.Min(y,y+sy*radius)>=-1e-9&&Math.Max(x,x+sx*radius)<=Material.Width+1e-9&&Math.Max(y,y+sy*radius)<=Material.Height+1e-9;
    }
    public bool TryUpdateQuarterCircle(CutOperation cut,double x,double y,double radius)
    {
        if(!CanUpdateQuarterCircle(cut,x,y,radius)||cut.QuarterCircleArc is not ArcSegment arc)return false;
        var sx=arc.StartDegrees is 0 or 270?1:-1;var sy=arc.StartDegrees is 0 or 90?1:-1;
        ApplyCutContour(cut,HoleDragEngine.CreatePreview("QuarterCircle",new(x+sx*radius/2,y+sy*radius/2,radius,radius,null,arc.StartDegrees)).Geometry);return true;
    }
    public bool CanUpdateSemicircle(CutOperation cut,double centerX,double centerY,double radius)
    {
        if(!Cuts.Contains(cut)||cut.SemicircleArc is not ArcSegment arc||!double.IsFinite(centerX)||!double.IsFinite(centerY)||!double.IsFinite(radius)||radius<.1)return false;
        var vertical=arc.StartDegrees is 0 or 180;
        var x=centerX+(arc.StartDegrees==270?radius/2:arc.StartDegrees==90?-radius/2:0);
        var y=centerY+(arc.StartDegrees==0?radius/2:arc.StartDegrees==180?-radius/2:0);
        var width=vertical?radius*2:radius;var height=vertical?radius:radius*2;
        return x-width/2>=-1e-9&&y-height/2>=-1e-9&&x+width/2<=Material.Width+1e-9&&y+height/2<=Material.Height+1e-9;
    }
    public bool TryUpdateSemicircle(CutOperation cut,double centerX,double centerY,double radius)
    {
        if(!CanUpdateSemicircle(cut,centerX,centerY,radius)||cut.SemicircleArc is not ArcSegment arc)return false;
        var vertical=arc.StartDegrees is 0 or 180;
        var geometry=new HoleDragGeometry(centerX+(arc.StartDegrees==270?radius/2:arc.StartDegrees==90?-radius/2:0),centerY+(arc.StartDegrees==0?radius/2:arc.StartDegrees==180?-radius/2:0),vertical?radius*2:radius,vertical?radius:radius*2,null,arc.StartDegrees);
        ApplyCutContour(cut,HoleDragEngine.CreatePreview("Semicircle",geometry).Geometry);
        return true;
    }
    public bool CanResizeCircleRadius(CutOperation cut,double radius)
        => Cuts.Contains(cut)&&cut.Shape=="Circle"&&double.IsFinite(radius)&&radius>=.05
            &&radius<=cut.CenterX&&radius<=cut.CenterY
            &&radius<=Material.Width-cut.CenterX&&radius<=Material.Height-cut.CenterY;
    public bool TryResizeCircleRadius(CutOperation cut,double radius)
    {
        if(!CanResizeCircleRadius(cut,radius))return false;
        UpdateCut(cut,cut.CenterX,cut.CenterY,radius*2,radius*2,cut.Sides);
        return true;
    }
    public bool TryResizeTriangleLowerLeft(CutOperation cut,double width,double height)
    {
        if(cut.Shape!="Triangle"||!double.IsFinite(width)||!double.IsFinite(height)||width<.1||height<.1)return false;
        var x=cut.LowerLeftX;var y=cut.LowerLeftY;
        if(x+width>Material.Width||y+height>Material.Height)return false;
        // Axis scaling preserves arbitrary vertices; circular fillets cannot be scaled to ellipses.
        if(cut.Geometry.Count!=3||cut.Geometry.Any(g=>g is not LineSegment))return false;
        var sx=width/cut.Width;var sy=height/cut.Height;
        var resized=cut.Geometry.Cast<LineSegment>().Select(l=>(GeometrySegment)new LineSegment(
            x+(l.X1-x)*sx,y+(l.Y1-y)*sy,x+(l.X2-x)*sx,y+(l.Y2-y)*sy)).ToList();
        ApplyCutContour(cut,resized);
        return true;
    }
    public void MoveTriangleLowerLeft(CutOperation cut,double x,double y)
    {
        if(cut.Shape!="Triangle"||!double.IsFinite(x)||!double.IsFinite(y))return;
        var dx=Math.Clamp(x,0,Math.Max(0,Material.Width-cut.Width))-cut.LowerLeftX;
        var dy=Math.Clamp(y,0,Math.Max(0,Material.Height-cut.Height))-cut.LowerLeftY;
        if(dx==0&&dy==0)return;
        var moved=cut.Geometry.Select<GeometrySegment,GeometrySegment>(g=>g switch
        {
            LineSegment l=>new LineSegment(l.X1+dx,l.Y1+dy,l.X2+dx,l.Y2+dy),
            ArcSegment a=>new ArcSegment(a.Cx+dx,a.Cy+dy,a.Radius,a.StartDegrees,a.EndDegrees),
            CircleSegment c=>new CircleSegment(c.Cx+dx,c.Cy+dy,c.Radius),
            _=>g
        }).ToList();
        ApplyCutContour(cut,moved);
    }
    public void UpdateTriangleVertices(CutOperation cut,IReadOnlyList<Point> vertices){if(cut.Shape!="Triangle"||vertices.Count!=3)return;var points=vertices.Select(p=>new Point(Math.Clamp(p.X,0,Material.Width),Math.Clamp(p.Y,0,Material.Height))).ToList();cut.Geometry.Clear();for(var i=0;i<3;i++){var a=points[i];var b=points[(i+1)%3];cut.Geometry.Add(new LineSegment(a.X,a.Y,b.X,b.Y));}var minX=points.Min(p=>p.X);var maxX=points.Max(p=>p.X);var minY=points.Min(p=>p.Y);var maxY=points.Max(p=>p.Y);cut.CenterX=(minX+maxX)/2;cut.CenterY=(minY+maxY)/2;cut.Width=Math.Max(.1,maxX-minX);cut.Height=Math.Max(.1,maxY-minY);var contour=InnerContours.FirstOrDefault(c=>c.Id==cut.Id);if(contour is not null){contour.Segments.Clear();contour.Segments.AddRange(cut.Geometry);}Recalculate();}
    public void UpdateBend(BendObject bend, double position, BendDirection direction, int sequence)
    { bend.Position=Math.Clamp(position,0,bend.Axis==SectionAxis.W?Material.Width:Material.Height);bend.Direction=direction;bend.Sequence=Math.Max(1,sequence);RebuildSegments(bend.Axis);Recalculate(); }
    public bool DeleteObject(DesignObject item)
    {
        if(item is SlitOperation slit&&Slits.Remove(slit)){Recalculate();return true;}
        if(item is BendObject bend&&Bends.Remove(bend)){RebuildSegments(bend.Axis);Recalculate();return true;}
        if(item is CutOperation cut&&Cuts.Remove(cut)){var contour=InnerContours.FirstOrDefault(x=>x.Id==cut.Id);if(contour is not null)InnerContours.Remove(contour);Recalculate();return true;}
        if(item is MicroJoint joint&&MicroJoints.Remove(joint)){Recalculate();return true;}
        if(item is GeometryObject geometry&&OuterContourEditEngine.TryDeleteLine(OuterContour.Segments,OuterContour.Segments.IndexOf(geometry.Geometry),out var updated)){OuterContour.Segments.Clear();OuterContour.Segments.AddRange(updated);_preserveOuterContour=true;Recalculate();return true;}
        return false;
    }
    public bool TryMergeBoundaryCut(CutOperation cut)
    {
        if(!TryGetBoundaryMerge(cut,out var contour))return false;Cuts.Remove(cut);var inner=InnerContours.FirstOrDefault(x=>x.Id==cut.Id);if(inner is not null)InnerContours.Remove(inner);OuterContour.Segments.Clear();OuterContour.Segments.AddRange(contour);_preserveOuterContour=true;Recalculate();return true;
    }
    public bool CanMergeBoundaryCut(CutOperation cut)=>TryGetBoundaryMerge(cut,out _);
    public bool CanMergeInternalCuts(CutOperation cut)=>CutUnionEngine.TryBuild(this,cut,out _,out _);
    public IReadOnlyList<CutOperation> InternalMergeGroup(CutOperation cut)=>CutUnionEngine.TryBuild(this,cut,out var members,out _)?members:[];
    public bool TryMergeInternalCuts(CutOperation selected)
    {
        if(!CutUnionEngine.TryBuild(this,selected,out var members,out var contour))return false;
        foreach(var cut in members.Where(c=>!ReferenceEquals(c,selected))){Cuts.Remove(cut);foreach(var inner in InnerContours.Where(c=>c.Id==cut.Id).ToList())InnerContours.Remove(inner);}
        selected.Shape="Compound";ApplyCutContour(selected,contour);return true;
    }
    public bool CanUpdateCompound(CutOperation cut,double x,double y,double width,double height)
    {
        if(!Cuts.Contains(cut)||cut.Shape!="Compound"||CutContourEngine.Loops(cut.Geometry).Count==0||!double.IsFinite(x)||!double.IsFinite(y)||!double.IsFinite(width)||!double.IsFinite(height)||width<.1||height<.1)return false;
        if(cut.Geometry.Any(g=>g is ArcSegment)&&Math.Abs(width/cut.Width-height/cut.Height)>1e-7)return false;
        return x-width/2>=-1e-9&&y-height/2>=-1e-9&&x+width/2<=Material.Width+1e-9&&y+height/2<=Material.Height+1e-9;
    }
    public bool TryUpdateCompound(CutOperation cut,double x,double y,double width,double height)
    {
        if(!CanUpdateCompound(cut,x,y,width,height))return false;
        var sx=width/cut.Width;var sy=height/cut.Height;
        Point Map(double px,double py)=>new(x+(px-cut.CenterX)*sx,y+(py-cut.CenterY)*sy);
        var contour=cut.Geometry.Select(g=>{
            if(g is ArcSegment arc){var p=Map(arc.Cx,arc.Cy);return (GeometrySegment)(arc with {Cx=p.X,Cy=p.Y,Radius=arc.Radius*sx});}
            var l=(LineSegment)g;var a=Map(l.X1,l.Y1);var b=Map(l.X2,l.Y2);return new LineSegment(a.X,a.Y,b.X,b.Y);
        }).ToList();ApplyCutContour(cut,contour);return true;
    }
    void UpdateCompoundClamped(CutOperation cut,double x,double y,double width,double height)
    {
        if(!double.IsFinite(x)||!double.IsFinite(y)||!double.IsFinite(width)||!double.IsFinite(height))return;
        if(cut.Geometry.Any(g=>g is ArcSegment)){
            var wx=Math.Abs(width-cut.Width)>1e-8;var hy=Math.Abs(height-cut.Height)>1e-8;
            var scale=wx&&hy?Math.Min(width/cut.Width,height/cut.Height):wx?width/cut.Width:hy?height/cut.Height:1;
            scale=Math.Clamp(scale,Math.Max(.1/cut.Width,.1/cut.Height),Math.Min(Material.Width/cut.Width,Material.Height/cut.Height));width=cut.Width*scale;height=cut.Height*scale;
        }else{width=Math.Clamp(width,.1,Material.Width);height=Math.Clamp(height,.1,Material.Height);}
        x=Math.Clamp(x,width/2,Material.Width-width/2);y=Math.Clamp(y,height/2,Material.Height-height/2);TryUpdateCompound(cut,x,y,width,height);
    }
    public void UpdateOuterContourLine(IReadOnlyList<GeometrySegment> source,int index,LineSegment replacement){var updated=OuterContourEditEngine.UpdateConnectedLine(source,index,replacement);OuterContour.Segments.Clear();OuterContour.Segments.AddRange(updated);_preserveOuterContour=true;Recalculate();}
    public bool TryFilletOuterCorner(int vertexIndex,double radius){if(!OuterContourFilletEngine.TryApply(OuterContour.Segments,vertexIndex,radius,out var contour))return false;OuterContour.Segments.Clear();OuterContour.Segments.AddRange(contour);_preserveOuterContour=true;Recalculate();return true;}
    public bool TryResizeOuterFillet(ArcSegment arc,double radius,out ArcSegment? updated){updated=null;var index=OuterContour.Segments.IndexOf(arc);if(index<0||!OuterContourFilletEngine.TryResize(OuterContour.Segments,index,radius,out var contour))return false;OuterContour.Segments.Clear();OuterContour.Segments.AddRange(contour);updated=OuterContour.Segments.ElementAtOrDefault(index) as ArcSegment;_preserveOuterContour=true;Recalculate();return updated is not null;}
    public bool TryFilletContourCorner(string contourId,int vertexIndex,double radius,out ArcSegment? updated){updated=null;if(contourId=="OUTER"){if(!TryFilletOuterCorner(vertexIndex,radius))return false;updated=OuterContour.Segments.ElementAtOrDefault(vertexIndex) as ArcSegment;return updated is not null;}var cut=Cuts.FirstOrDefault(item=>item.Id==contourId);if(cut is null||cut.Shape=="RegularStar"||cut.Shape=="Compound"&&CutContourEngine.Loops(cut.Geometry).Count>1||!OuterContourFilletEngine.TryApply(cut.Geometry,vertexIndex,radius,out var contour))return false;ApplyCutContour(cut,contour);updated=cut.Geometry.ElementAtOrDefault(vertexIndex) as ArcSegment;return updated is not null;}
    public bool TryResizeFillet(string contourId,ArcSegment arc,double radius,out ArcSegment? updated){updated=null;if(contourId=="OUTER")return TryResizeOuterFillet(arc,radius,out updated);var cut=Cuts.FirstOrDefault(item=>item.Id==contourId);if(cut is null||cut.Shape=="RegularStar"||cut.Shape=="Compound"&&CutContourEngine.Loops(cut.Geometry).Count>1)return false;var index=cut.Geometry.IndexOf(arc);if(index<0||!OuterContourFilletEngine.TryResize(cut.Geometry,index,radius,out var contour))return false;ApplyCutContour(cut,contour);updated=cut.Geometry.ElementAtOrDefault(index) as ArcSegment;return updated is not null;}
    public bool CanUpdateEllipse(CutOperation cut,EllipseProperties target)
        => Cuts.Contains(cut)&&EllipsePropertiesEngine.TryGet(cut,out _)&&target.Fits(Material.Width,Material.Height);
    public bool TryUpdateEllipse(CutOperation cut,EllipseProperties target)
    {
        if(!CanUpdateEllipse(cut,target)||!EllipsePropertiesEngine.TryGet(cut,out var old))return false;
        target=target with {RotationDegrees=EllipseProperties.NormalizeAngle(target.RotationDegrees)};
        if(Math.Abs(old.CenterX-target.CenterX)<1e-9&&Math.Abs(old.CenterY-target.CenterY)<1e-9&&Math.Abs(old.MajorLength-target.MajorLength)<1e-9&&Math.Abs(old.MinorLength-target.MinorLength)<1e-9&&Math.Abs(old.RotationDegrees-target.RotationDegrees)<1e-9)return true;
        var points=EllipsePropertiesEngine.Transform(cut,old,target);
        var contour=points.Select((p,i)=>{var q=points[(i+1)%points.Count];return (GeometrySegment)new LineSegment(p.X,p.Y,q.X,q.Y);}).ToList();
        ApplyCutContour(cut,contour);return true;
    }
    public bool ApplyEllipseContour(CutOperation cut,IReadOnlyList<Point> points)
    {
        if(!Cuts.Contains(cut)||cut.Shape!="Ellipse"||points.Count!=48||points.Any(p=>!double.IsFinite(p.X)||!double.IsFinite(p.Y)||p.X<0||p.Y<0||p.X>Material.Width||p.Y>Material.Height))return false;
        var contour=points.Select((p,i)=>{var q=points[(i+1)%points.Count];return (GeometrySegment)new LineSegment(p.X,p.Y,q.X,q.Y);}).ToList();
        ApplyCutContour(cut,contour);return true;
    }
    void UpdateEllipse(CutOperation cut,double x,double y,double width,double height)
    {
        if(!double.IsFinite(x)||!double.IsFinite(y)||!double.IsFinite(width)||!double.IsFinite(height))return;
        width=Math.Clamp(width,.1,Material.Width);height=Math.Clamp(height,.1,Material.Height);
        x=Math.Clamp(x,width/2,Material.Width-width/2);y=Math.Clamp(y,height/2,Material.Height-height/2);
        var sx=width/cut.Width;var sy=height/cut.Height;
        Point Map(double px,double py)=>new(x+(px-cut.CenterX)*sx,y+(py-cut.CenterY)*sy);
        var contour=cut.Geometry.Cast<LineSegment>().Select(l=>{var a=Map(l.X1,l.Y1);var b=Map(l.X2,l.Y2);return (GeometrySegment)new LineSegment(a.X,a.Y,b.X,b.Y);}).ToList();
        ApplyCutContour(cut,contour);
    }
    void ApplyCutContour(CutOperation cut,IReadOnlyList<GeometrySegment> contour){if(cut.Shape is "RegularPolygon" or "RegularStar" or "Rectangle")cut.Shape="Compound";cut.Geometry.Clear();cut.Geometry.AddRange(contour);var points=(cut.Shape=="Compound"?CutContourEngine.Extrema(contour):contour.SelectMany(SampleGeometry)).ToList();if(points.Count>0){var minX=points.Min(p=>p.X);var maxX=points.Max(p=>p.X);var minY=points.Min(p=>p.Y);var maxY=points.Max(p=>p.Y);cut.CenterX=(minX+maxX)/2;cut.CenterY=(minY+maxY)/2;cut.Width=Math.Max(.1,maxX-minX);cut.Height=Math.Max(.1,maxY-minY);}var inner=InnerContours.FirstOrDefault(item=>item.Id==cut.Id);if(inner is not null){inner.Segments.Clear();inner.Segments.AddRange(cut.Geometry);}Recalculate();}
    static IEnumerable<Point> SampleGeometry(GeometrySegment geometry){if(geometry is LineSegment line)return[new(line.X1,line.Y1),new(line.X2,line.Y2)];if(geometry is ArcSegment arc)return Enumerable.Range(0,17).Select(i=>{var angle=(arc.StartDegrees+(arc.EndDegrees-arc.StartDegrees)*i/16)*Math.PI/180;return new Point(arc.Cx+arc.Radius*Math.Cos(angle),arc.Cy+arc.Radius*Math.Sin(angle));});if(geometry is CircleSegment circle)return[new(circle.Cx-circle.Radius,circle.Cy-circle.Radius),new(circle.Cx+circle.Radius,circle.Cy+circle.Radius)];return[];}
    public bool UpdateClippedSectionSegment(SectionAxis axis,SectionGeometry geometry,int segmentIndex,double newLength,double selector)
    {
        if(segmentIndex<0||segmentIndex>=geometry.SegmentLengths.Count||newLength<=0)return false;var total=axis==SectionAxis.W?Material.Width:Material.Height;var delta=newLength-geometry.SegmentLengths[segmentIndex];if(Math.Abs(delta)<1e-9)return true;var moveEnd=geometry.EndPosition<total-1e-6;var boundary=moveEnd?geometry.EndPosition:geometry.StartPosition;var boundaryDelta=moveEnd?delta:-delta;var newBoundary=boundary+boundaryDelta;if(newBoundary<0||newBoundary>total)return false;if(!OuterContourEditEngine.TryMoveSectionBoundary(OuterContour.Segments,axis,selector,boundary,boundaryDelta,out var contour))return false;
        if(moveEnd){for(var i=segmentIndex;i<geometry.Bends.Count;i++)geometry.Bends[i].Position+=delta;}
        else{for(var i=0;i<Math.Min(segmentIndex,geometry.Bends.Count);i++)geometry.Bends[i].Position-=delta;}
        OuterContour.Segments.Clear();OuterContour.Segments.AddRange(contour);_preserveOuterContour=true;RebuildSegments(axis);Recalculate();return true;
    }
    bool TryGetBoundaryMerge(CutOperation cut,out IReadOnlyList<GeometrySegment> contour){contour=[];if(!Cuts.Contains(cut))return false;if(cut.Shape=="Compound")return CutContourEngine.Loops(cut.Geometry).Count==1&&CurvedBoundaryCutEngine.TrySubtract(OuterContour.Segments,cut.Geometry,out contour);if(cut.Shape=="RegularPolygon")return BoundaryCutEngine.TrySubtractPolygon(OuterContour.Segments,cut.Geometry,out contour);if(cut.Shape=="QuarterCircle")return CurvedBoundaryCutEngine.TrySubtractQuarterCircle(OuterContour.Segments,cut.Geometry,out contour);if(cut.Shape=="Semicircle")return CurvedBoundaryCutEngine.TrySubtractSemicircle(OuterContour.Segments,cut.Geometry,out contour);if(string.Equals(cut.Shape,"Rectangle",StringComparison.OrdinalIgnoreCase)){if(cut.Rotation!=0)return BoundaryCutEngine.TrySubtractPolygon(OuterContour.Segments,cut.Geometry,out contour);var rect=new Rect(cut.CenterX-cut.Width/2,cut.CenterY-cut.Height/2,cut.Width,cut.Height);return BoundaryCutEngine.TrySubtractRectangle(OuterContour.Segments,rect,out contour);}return (string.Equals(cut.Shape,"Triangle",StringComparison.OrdinalIgnoreCase)||string.Equals(cut.Shape,"Parallelogram",StringComparison.OrdinalIgnoreCase))&&BoundaryCutEngine.TrySubtractPolygon(OuterContour.Segments,cut.Geometry,out contour);}
    public void UpdateSegment(SectionSegment segment,double length)
    {
        if(IsBasicRectangle())_preserveOuterContour=false;
        segment.Length=Math.Max(.01,length);var list=segment.Axis==SectionAxis.W?WSegments:HSegments;var bends=Bends.Where(b=>b.Axis==segment.Axis).OrderBy(b=>b.Position).ToList();double p=0;
        for(var i=0;i<bends.Count;i++){p+=list[i].Length;bends[i].Position=p;}Recalculate();
    }
    public void ChangeThickness(double thickness) { if (thickness <= 0) throw new ArgumentOutOfRangeException(nameof(thickness)); Material.Thickness = thickness; Recalculate(); }
    public void ConfigureNew(double width,double height,double thickness,MeasurementUnit unit){if(width<=0||height<=0||thickness<=0)throw new ArgumentOutOfRangeException();Unit=unit;Bends.Clear();Cuts.Clear();InnerContours.Clear();MicroJoints.Clear();WSegments.Clear();HSegments.Clear();WSegments.Add(new(){Id="W-S001",Axis=SectionAxis.W,Index=0,Length=width});HSegments.Add(new(){Id="H-S001",Axis=SectionAxis.H,Index=0,Length=height});Material.Thickness=thickness;_preserveOuterContour=false;Recalculate();}
    public void ChangeUnit(MeasurementUnit unit,UnitChangeMode mode)
    {
        if(unit==Unit)return;if(mode==UnitChangeMode.PreserveNumbers){Unit=unit;PropertyChanged?.Invoke(this,new(null));Changed?.Invoke(this,EventArgs.Empty);return;}var factor=Unit.Metres()/unit.Metres();foreach(var s in WSegments.Concat(HSegments))s.Length*=factor;foreach(var b in Bends)b.Position*=factor;foreach(var c in Cuts){c.CenterX*=factor;c.CenterY*=factor;c.Width*=factor;c.Height*=factor;if(c.Shape is "RegularPolygon" or "RegularStar")c.Radius*=factor;for(var i=0;i<c.Geometry.Count;i++)c.Geometry[i]=ScaleGeometry(c.Geometry[i],factor);var inner=InnerContours.FirstOrDefault(x=>x.Id==c.Id);if(inner is not null){inner.Segments.Clear();inner.Segments.AddRange(c.Geometry);}}foreach(var slit in Slits)for(var i=0;i<slit.Geometry.Count;i++)slit.Geometry[i]=ScaleGeometry(slit.Geometry[i],factor);foreach(var j in MicroJoints){j.Position*=factor;j.Width*=factor;}for(var i=0;i<OuterContour.Segments.Count;i++)OuterContour.Segments[i]=ScaleGeometry(OuterContour.Segments[i],factor);Material.Thickness*=factor;Unit=unit;_preserveOuterContour=true;Recalculate();
    }
    public void Recalculate() { BendCalculationEngine.Recalculate(this); RebuildOuterContour(); PropertyChanged?.Invoke(this, new(null)); Changed?.Invoke(this, EventArgs.Empty); }
    internal void ApplyImportedGeometry(double width,double height,IEnumerable<GeometrySegment> outer,IEnumerable<CutOperation> cuts,IEnumerable<(SectionAxis Axis,double Position,BendDirection Direction)> bends,double thickness=.2)
    {
        Bends.Clear();Cuts.Clear();InnerContours.Clear();MicroJoints.Clear();WSegments.Clear();HSegments.Clear();
        var bendList=bends.ToList();
        WSegments.Add(new(){Id="W-S001",Axis=SectionAxis.W,Index=0,Length=Math.Max(.01,width)});
        HSegments.Add(new(){Id="H-S001",Axis=SectionAxis.H,Index=0,Length=Math.Max(.01,height)});
        Material.Thickness=thickness;Material.Width=Math.Max(.01,width);Material.Height=Math.Max(.01,height);
        foreach(var b in bendList.OrderBy(x=>x.Axis).ThenBy(x=>x.Position))AddBend(b.Axis,b.Position,b.Direction);
        Cuts.Clear();InnerContours.Clear();foreach(var cut in cuts){Cuts.Add(cut);var contour=new ContourObject{Id=cut.Id,Kind=ContourKind.Inner};contour.Segments.AddRange(cut.Geometry);InnerContours.Add(contour);}
        _preserveOuterContour=true;OuterContour.Segments.Clear();OuterContour.Segments.AddRange(outer);Recalculate();
    }
    internal void PreserveOuterGeometry(IEnumerable<GeometrySegment> geometry){_preserveOuterContour=true;OuterContour.Segments.Clear();OuterContour.Segments.AddRange(geometry);}
    internal void SetUnit(MeasurementUnit unit)=>Unit=unit;
    public int NextSequence() => Bends.Count + Cuts.Count + Slits.Count + MicroJoints.Count + 1;
    void RebuildSegments(SectionAxis axis)
    {
        var list=axis==SectionAxis.W?WSegments:HSegments;var currentFlat=axis==SectionAxis.W?Material.Width:Material.Height;var total=list.Count>0?list.Sum(s=>s.Length):currentFlat;var positions=Bends.Where(b=>b.Axis==axis).Select(b=>Math.Clamp(b.Position,0,total)).Order().ToList();list.Clear();double last=0;var i=1;
        foreach(var p in positions){list.Add(new(){Id=$"{axis}-S{i:000}",Axis=axis,Index=i-1,Length=Math.Max(.01,p-last)});last=p;i++;}
        list.Add(new(){Id=$"{axis}-S{i:000}",Axis=axis,Index=i-1,Length=Math.Max(.01,total-last)});
    }
    bool IsBasicRectangle()
    {
        var w=Material.Width;var h=Material.Height;
        var expected=new LineSegment[]{new(0,0,w,0),new(w,0,w,h),new(w,h,0,h),new(0,h,0,0)};
        if(OuterContour.Segments.Count!=4)return false;
        return expected.All(edge=>OuterContour.Segments.OfType<LineSegment>().Any(line=>
            (Same(line.X1,edge.X1)&&Same(line.Y1,edge.Y1)&&Same(line.X2,edge.X2)&&Same(line.Y2,edge.Y2))||
            (Same(line.X2,edge.X1)&&Same(line.Y2,edge.Y1)&&Same(line.X1,edge.X2)&&Same(line.Y1,edge.Y2))));
        static bool Same(double a,double b)=>Math.Abs(a-b)<1e-6;
    }
    void RebuildOuterContour()
    {
        if(_preserveOuterContour&&OuterContour.Segments.Count>0){var closed=OuterContourEditEngine.CloseGaps(OuterContour.Segments).ToList();OuterContour.Segments.Clear();OuterContour.Segments.AddRange(closed);return;}
        OuterContour.Segments.Clear(); var w = Material.Width; var h = Material.Height;
        OuterContour.Segments.AddRange([new LineSegment(0,0,w,0), new LineSegment(w,0,w,h), new LineSegment(w,h,0,h), new LineSegment(0,h,0,0)]);
    }
    static GeometrySegment ScaleGeometry(GeometrySegment geometry,double scale)=>geometry switch{LineSegment l=>new LineSegment(l.X1*scale,l.Y1*scale,l.X2*scale,l.Y2*scale),CircleSegment c=>new CircleSegment(c.Cx*scale,c.Cy*scale,c.Radius*scale),ArcSegment a=>new ArcSegment(a.Cx*scale,a.Cy*scale,a.Radius*scale,a.StartDegrees,a.EndDegrees),_=>geometry};
}
