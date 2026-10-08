using System.Windows;

namespace VCutting;
public readonly record struct RectangleParameters(double CenterX,double CenterY,double Width,double Height,double Rotation)
{
    public bool IsValid=>double.IsFinite(CenterX)&&double.IsFinite(CenterY)&&double.IsFinite(Width)&&double.IsFinite(Height)&&double.IsFinite(Rotation)&&Width>=.1&&Height>=.1;
    public RectangleParameters Normalized=>this with {Rotation=RegularPolygonParameters.Normalize(Rotation)};
    public bool Fits(double width,double height)=>IsValid&&RotatedRectangleGeometry.Vertices(this).All(p=>p.X>=-1e-8&&p.Y>=-1e-8&&p.X<=width+1e-8&&p.Y<=height+1e-8);
    public RectangleParameters ClampCenter(double width,double height)
    {
        var offsets=RotatedRectangleGeometry.Vertices(this with {CenterX=0,CenterY=0});if(offsets.Count==0)return this;
        var x=offsets.Max(p=>p.X);var y=offsets.Max(p=>p.Y);if(2*x>width||2*y>height)return this;
        return this with {CenterX=Math.Clamp(CenterX,x,width-x),CenterY=Math.Clamp(CenterY,y,height-y)};
    }
}
public static class RotatedRectangleGeometry
{
    public static RectangleParameters Parameters(CutOperation cut)=>new(cut.CenterX,cut.CenterY,cut.Width,cut.Height,cut.Rotation);
    public static IReadOnlyList<Point> Vertices(RectangleParameters p)
    {
        if(!p.IsValid)return [];var a=p.Normalized.Rotation*Math.PI/180;var c=Math.Cos(a);var s=Math.Sin(a);
        return new[]{new Point(-p.Width/2,-p.Height/2),new Point(p.Width/2,-p.Height/2),new Point(p.Width/2,p.Height/2),new Point(-p.Width/2,p.Height/2)}.Select(v=>new Point(p.CenterX+c*v.X-s*v.Y,p.CenterY+s*v.X+c*v.Y)).ToArray();
    }
    public static bool IsParametric(CutOperation cut)
    {
        if(cut.Shape!="Rectangle"||cut.Geometry.Count!=4||cut.Geometry.Any(g=>g is not LineSegment))return false;
        var p=Vertices(Parameters(cut));if(p.Count!=4)return false;
        return cut.Geometry.Cast<LineSegment>().Select((l,i)=>(new Point(l.X1,l.Y1)-p[i]).Length<1e-6&&(new Point(l.X2,l.Y2)-p[(i+1)%4]).Length<1e-6).All(ok=>ok);
    }
    public static Rect Bounds(CutOperation cut){var p=Vertices(Parameters(cut));return p.Count==0?Rect.Empty:new Rect(new Point(p.Min(v=>v.X),p.Min(v=>v.Y)),new Point(p.Max(v=>v.X),p.Max(v=>v.Y)));}
    public static void Rebuild(CutOperation cut){var p=Vertices(Parameters(cut));cut.Geometry.Clear();for(var i=0;i<p.Count;i++){var a=p[i];var b=p[(i+1)%p.Count];cut.Geometry.Add(new LineSegment(a.X,a.Y,b.X,b.Y));}}
}
public sealed partial class VCuttingDocument
{
    public bool CanUpdateRectangle(CutOperation cut,RectangleParameters value)=>Cuts.Contains(cut)&&RotatedRectangleGeometry.IsParametric(cut)&&value.Fits(Material.Width,Material.Height);
    public bool TryUpdateRectangle(CutOperation cut,RectangleParameters value)
    {
        if(!CanUpdateRectangle(cut,value))return false;var p=value.Normalized;if(RotatedRectangleGeometry.Parameters(cut)==p)return true;
        cut.CenterX=p.CenterX;cut.CenterY=p.CenterY;cut.Width=p.Width;cut.Height=p.Height;cut.Rotation=p.Rotation;RotatedRectangleGeometry.Rebuild(cut);
        var inner=InnerContours.FirstOrDefault(c=>c.Id==cut.Id);if(inner is not null){inner.Segments.Clear();inner.Segments.AddRange(cut.Geometry);}Recalculate();return true;
    }
}
