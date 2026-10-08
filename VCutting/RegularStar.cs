using System.Windows;

namespace VCutting;

public readonly record struct RegularStarParameters(RegularPolygonParameters Polygon,int Step)
{
    public bool IsValid=>Polygon.IsValid&&Polygon.Sides>=5&&Step>=2&&Step<=(Polygon.Sides-1)/2;
    public bool Fits(double width,double height)=>IsValid&&Polygon.Fits(width,height);
    public RegularStarParameters Normalized=>this with {Polygon=Polygon.Normalized};
}
public static class RegularStarGeometry
{
    public static bool Contains(IReadOnlyList<GeometrySegment> geometry,Point point)
    {
        var winding=0;
        foreach(var line in geometry.OfType<LineSegment>())
        {
            var cross=(line.X2-line.X1)*(point.Y-line.Y1)-(point.X-line.X1)*(line.Y2-line.Y1);
            if(line.Y1<=point.Y&&line.Y2>point.Y&&cross>0)winding++;
            else if(line.Y1>point.Y&&line.Y2<=point.Y&&cross<0)winding--;
        }
        return winding!=0;
    }
    public static RegularStarParameters Parameters(CutOperation cut)=>new(RegularPolygonGeometry.Parameters(cut),cut.StarStep);
    public static IReadOnlyList<Point> Profile(RegularStarParameters value)
    {
        if(!value.IsValid)return [];
        var p=value.Polygon.Normalized;var tips=RegularPolygonGeometry.Vertices(p);
        // Adjacent tips' outward skip edges intersect on their angular bisector.
        // Their supporting line is R*cos(K*pi/N) from the center.
        var inner=p.Radius*Math.Cos(value.Step*Math.PI/p.Sides)/Math.Cos((value.Step-1)*Math.PI/p.Sides);
        var points=new List<Point>(2*p.Sides);
        for(var i=0;i<p.Sides;i++)
        {
            points.Add(tips[i]);var angle=(p.Rotation+(i+.5)*360/p.Sides)*Math.PI/180;
            points.Add(new(p.CenterX+inner*Math.Cos(angle),p.CenterY+inner*Math.Sin(angle)));
        }
        return points;
    }
    // A non-coprime step makes several cycles, never a synthetic bridge between them.
    public static IReadOnlyList<IReadOnlyList<Point>> Cycles(RegularStarParameters value)
    {
        if(!value.IsValid)return [];
        var vertices=RegularPolygonGeometry.Vertices(value.Polygon);var visited=new bool[vertices.Count];var cycles=new List<IReadOnlyList<Point>>();
        for(var start=0;start<vertices.Count;start++)
        {
            if(visited[start])continue;var cycle=new List<Point>();var i=start;
            do{visited[i]=true;cycle.Add(vertices[i]);i=(i+value.Step)%vertices.Count;}while(i!=start);
            cycles.Add(cycle);
        }
        return cycles;
    }
    public static void Rebuild(CutOperation cut)
    {
        var points=Profile(Parameters(cut));cut.Geometry.Clear();if(points.Count==0)return;
        cut.Rotation=RegularPolygonParameters.Normalize(cut.Rotation);var bounds=RegularPolygonGeometry.Bounds(cut);cut.Width=bounds.Width;cut.Height=bounds.Height;
        for(var i=0;i<points.Count;i++){var a=points[i];var b=points[(i+1)%points.Count];cut.Geometry.Add(new LineSegment(a.X,a.Y,b.X,b.Y));}
    }
}
