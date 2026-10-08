using System.Windows;

namespace VCutting;

public readonly record struct RegularPolygonParameters(double CenterX,double CenterY,int Sides,double Radius,double Rotation)
{
    public const int MaximumSides=512;
    public bool IsValid=>Sides>=3&&Sides<=MaximumSides&&double.IsFinite(CenterX)&&double.IsFinite(CenterY)&&double.IsFinite(Radius)&&Radius>0&&double.IsFinite(Rotation);
    public static double Normalize(double angle)=>((angle%360)+360)%360;
    public RegularPolygonParameters Normalized=>this with {Rotation=Normalize(Rotation)};
    public bool Fits(double width,double height)=>IsValid&&RegularPolygonGeometry.Vertices(this).All(p=>double.IsFinite(p.X)&&double.IsFinite(p.Y)&&p.X>=-1e-9&&p.Y>=-1e-9&&p.X<=width+1e-9&&p.Y<=height+1e-9);
    public RegularPolygonParameters ClampCenter(double width,double height)
    {
        var offsets=RegularPolygonGeometry.Vertices(this with {CenterX=0,CenterY=0});
        if(offsets.Count==0||offsets.Max(p=>p.X)-offsets.Min(p=>p.X)>width||offsets.Max(p=>p.Y)-offsets.Min(p=>p.Y)>height)return this;
        return this with {CenterX=Math.Clamp(CenterX,-offsets.Min(p=>p.X),width-offsets.Max(p=>p.X)),CenterY=Math.Clamp(CenterY,-offsets.Min(p=>p.Y),height-offsets.Max(p=>p.Y))};
    }
}
public static class RegularPolygonGeometry
{
    public static IReadOnlyList<Point> Vertices(RegularPolygonParameters value)
    {
        if(!value.IsValid)return [];
        var p=value.Normalized;return Enumerable.Range(0,p.Sides).Select(i=>{var a=(p.Rotation+i*360d/p.Sides)*Math.PI/180;return new Point(p.CenterX+p.Radius*Math.Cos(a),p.CenterY+p.Radius*Math.Sin(a));}).ToArray();
    }
    public static RegularPolygonParameters Parameters(CutOperation cut)=>new(cut.CenterX,cut.CenterY,cut.Sides,cut.Radius,cut.Rotation);
    public static void Rebuild(CutOperation cut)
    {
        var points=Vertices(Parameters(cut));cut.Geometry.Clear();if(points.Count<3)return;
        cut.Rotation=RegularPolygonParameters.Normalize(cut.Rotation);
        cut.Width=points.Max(p=>p.X)-points.Min(p=>p.X);cut.Height=points.Max(p=>p.Y)-points.Min(p=>p.Y);
        for(var i=0;i<points.Count;i++){var a=points[i];var b=points[(i+1)%points.Count];cut.Geometry.Add(new LineSegment(a.X,a.Y,b.X,b.Y));}
    }
    public static Rect Bounds(CutOperation cut){var points=Vertices(Parameters(cut));return points.Count==0?Rect.Empty:new Rect(new Point(points.Min(p=>p.X),points.Min(p=>p.Y)),new Point(points.Max(p=>p.X),points.Max(p=>p.Y)));}
}
public sealed class RegularPolygonDrawingState
{
    public Point? Center {get;private set;}
    public Point Hover {get;private set;}
    public int Sides {get;set;}=6;
    public void Cancel()=>Center=null;
    public void Move(Point point)=>Hover=point;
    public RegularPolygonParameters Preview
    {
        get{if(Center is not Point center)return default;var v=Hover-center;return new(center.X,center.Y,Sides,v.Length,Math.Atan2(v.Y,v.X)*180/Math.PI);}
    }
    public bool Click(Point point,double width,double height,out RegularPolygonParameters result)
    {
        result=default;if(Sides<3||Sides>RegularPolygonParameters.MaximumSides)return false;
        if(Center is null){if(!double.IsFinite(point.X)||!double.IsFinite(point.Y)||point.X<0||point.Y<0||point.X>width||point.Y>height)return false;Center=point;Hover=point;return false;}
        Move(point);var p=Preview;if(!p.Fits(width,height))return false;result=p.Normalized;Cancel();return true;
    }
}
