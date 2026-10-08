using System.Windows;
using System.Windows.Media;

namespace VCutting;

public readonly record struct FlatViewportTransform(double Scale,Point Origin)
{
    public Point ToScreen(Point design)=>new(Origin.X+design.X*Scale,Origin.Y-design.Y*Scale);
    public Point ToDesign(Point screen)=>new((screen.X-Origin.X)/Scale,(Origin.Y-screen.Y)/Scale);
}

public static class FlatViewportEngine
{
    public static FlatViewportTransform Calculate(double viewportWidth,double viewportHeight,double materialWidth,double materialHeight,bool showRuler,double zoom,Vector pan)
    {
        var margin=showRuler?42d:24d;
        var fit=Math.Min(Math.Max(1,viewportWidth-2*margin)/Math.Max(.01,materialWidth),Math.Max(1,viewportHeight-2*margin)/Math.Max(.01,materialHeight));
        var scale=fit*Math.Clamp(zoom,.2,20);
        return new(scale,new Point((viewportWidth-materialWidth*scale)/2+pan.X,(viewportHeight+materialHeight*scale)/2+pan.Y));
    }

    public static Vector PanKeepingAnchor(FlatViewportTransform before,FlatViewportTransform after,Point screenAnchor)
    {
        var design=before.ToDesign(screenAnchor);
        var moved=after.ToScreen(design);
        return screenAnchor-moved;
    }

    public static double GridStep(double scale)
    {
        var desired=70/Math.Max(.0001,scale);var power=Math.Pow(10,Math.Floor(Math.Log10(desired)));var n=desired/power;return(n<2?2:n<5?5:10)*power;
    }

    public static double DisplayStep(double scale,double baseStep,double minimumPixels)
    {
        if(!double.IsFinite(baseStep)||baseStep<=0)return GridStep(scale);
        var required=Math.Max(1,minimumPixels/Math.Max(.0001,baseStep*scale));
        var power=Math.Pow(10,Math.Floor(Math.Log10(required)));var normalized=required/power;
        var multiplier=normalized<=1?1:normalized<=2?2:normalized<=5?5:10;
        return baseStep*multiplier*power;
    }
}

public readonly record struct HoleDragGeometry(double CenterX,double CenterY,double Width,double Height,IReadOnlyList<Point>? TriangleVertices=null,double SemicircleStartDegrees=0,IReadOnlyList<Point>? ContourVertices=null);

public static class HoleDragEngine
{
    public static CutOperation CreatePreview(string shape,HoleDragGeometry geometry)
    {
        var cut=new CutOperation{Shape=shape,CenterX=geometry.CenterX,CenterY=geometry.CenterY,Width=geometry.Width,Height=geometry.Height,Sides=7};
        if(shape=="Ellipse"&&geometry.ContourVertices is {Count:48} ellipse)
        {
            for(var i=0;i<ellipse.Count;i++){var a=ellipse[i];var b=ellipse[(i+1)%ellipse.Count];cut.Geometry.Add(new LineSegment(a.X,a.Y,b.X,b.Y));}
        }
        else if(shape=="Triangle"&&geometry.TriangleVertices is {Count:3} vertices)
        {
            for(var i=0;i<3;i++){var a=vertices[i];var b=vertices[(i+1)%3];cut.Geometry.Add(new LineSegment(a.X,a.Y,b.X,b.Y));}
        }
        else { if(shape is "Semicircle" or "QuarterCircle")cut.Geometry.Add(new ArcSegment(0,0,1,geometry.SemicircleStartDegrees,geometry.SemicircleStartDegrees+(shape=="QuarterCircle"?90:180))); HoleGeometryEngine.Rebuild(cut); }
        return cut;
    }
    public static bool Supports(string? shape)=>shape is "Circle" or "Ellipse" or "Semicircle" or "QuarterCircle" or "Triangle" or "Rectangle" or "Diamond" or "Parallelogram";

    public static bool TryCalculate(string shape,Point start,Point end,double materialWidth,double materialHeight,out HoleDragGeometry geometry)
    {
        geometry=default;if(!Supports(shape)||!double.IsFinite(start.X)||!double.IsFinite(start.Y)||!double.IsFinite(end.X)||!double.IsFinite(end.Y)||!double.IsFinite(materialWidth)||!double.IsFinite(materialHeight)||materialWidth<=0||materialHeight<=0)return false;
        start=new(Math.Clamp(start.X,0,materialWidth),Math.Clamp(start.Y,0,materialHeight));
        end=new(Math.Clamp(end.X,0,materialWidth),Math.Clamp(end.Y,0,materialHeight));
        var dx=end.X-start.X;var dy=end.Y-start.Y;
        if(shape=="Ellipse")
        {
            var length=Math.Sqrt(dx*dx+dy*dy);if(length<.2||!double.IsFinite(length))return false;
            var ux=dx/length;var uy=dy/length;var a=length/2;var b=a/2;
            var cx=(start.X+end.X)/2;var cy=(start.Y+end.Y)/2;
            var ex=Math.Sqrt(a*a*ux*ux+b*b*uy*uy);var ey=Math.Sqrt(a*a*uy*uy+b*b*ux*ux);
            var scale=Math.Min(1,Math.Min(Math.Min(cx,materialWidth-cx)/ex,Math.Min(cy,materialHeight-cy)/ey));
            a*=scale;b*=scale;if(b<.05)return false;
            var points=Enumerable.Range(0,48).Select(i=>{var t=i*Math.PI*2/48;return new Point(cx+a*Math.Cos(t)*ux-b*Math.Sin(t)*uy,cy+a*Math.Cos(t)*uy+b*Math.Sin(t)*ux);}).ToList();
            geometry=new(cx,cy,points.Max(p=>p.X)-points.Min(p=>p.X),points.Max(p=>p.Y)-points.Min(p=>p.Y),ContourVertices:points);return true;
        }
        if(shape=="QuarterCircle")
        {
            var radius=Math.Min(Math.Abs(dx),Math.Abs(dy));if(radius<.1)return false;
            var angle=dx>0?(dy>0?0:270):(dy>0?90:180);
            geometry=new(start.X+Math.Sign(dx)*radius/2,start.Y+Math.Sign(dy)*radius/2,radius,radius,null,angle);return true;
        }
        if(shape=="Semicircle")
        {
            var vertical=Math.Abs(dy)>=Math.Abs(dx);var positive=vertical?dy>=0:dx>=0;
            var radius=vertical?Math.Abs(dy):Math.Abs(dx);
            radius=Math.Min(radius,vertical?Math.Min(start.X,materialWidth-start.X):Math.Min(start.Y,materialHeight-start.Y));
            radius=Math.Min(radius,vertical?(positive?materialHeight-start.Y:start.Y):(positive?materialWidth-start.X:start.X));
            if(radius<.1)return false;
            var angle=vertical?(positive?0:180):(positive?270:90);
            geometry=new(start.X+(vertical?0:(positive?1:-1)*radius/2),start.Y+(vertical?(positive?1:-1)*radius/2:0),vertical?radius*2:radius,vertical?radius:radius*2,null,angle);
            return true;
        }
        if(shape=="Circle")
        {
            var size=Math.Min(Math.Abs(dx),Math.Abs(dy));if(size<.1)return false;
            end=new(start.X+Math.Sign(dx)*size,start.Y+Math.Sign(dy)*size);
        }
        var left=Math.Min(start.X,end.X);var right=Math.Max(start.X,end.X);var bottom=Math.Min(start.Y,end.Y);var top=Math.Max(start.Y,end.Y);
        if(right-left<.1||top-bottom<.1)return false;
        IReadOnlyList<Point>? vertices=shape=="Triangle"?[new(left,start.Y),new(right,start.Y),new((left+right)/2,end.Y)]:null;
        geometry=new((left+right)/2,(bottom+top)/2,right-left,top-bottom,vertices);return true;
    }
}

public static class FlatMaterialMaskEngine
{
    public static Geometry Build(VCuttingDocument document,FlatViewportTransform transform)
    {
        var outer=new StreamGeometry{FillRule=FillRule.EvenOdd};using(var context=outer.Open())AddContour(context,document.OuterContour.Segments,transform);
        Geometry result=outer;
        foreach(var cut in document.Cuts){var hole=new StreamGeometry{FillRule=cut.Shape=="RegularStar"?FillRule.Nonzero:FillRule.EvenOdd};using(var context=hole.Open()){
            if(cut.Geometry.Count==1&&cut.Geometry[0] is CircleSegment)AddContour(context,cut.Geometry,transform);
            else {var loops=CutContourEngine.Loops(cut.Geometry);if(loops.Count==0&&cut.Shape!="Compound")AddContour(context,cut.Geometry,transform);else foreach(var loop in loops)AddContour(context,loop,transform);}
        }result=Geometry.Combine(result,hole,GeometryCombineMode.Exclude,System.Windows.Media.Transform.Identity,.001,ToleranceType.Absolute);}
        result.Freeze();return result;
    }

    static void AddContour(StreamGeometryContext context,IReadOnlyList<GeometrySegment> segments,FlatViewportTransform transform)
    {
        if(segments.Count==0)return;
        if(segments.Count==1&&segments[0] is CircleSegment circle){var center=transform.ToScreen(new(circle.Cx,circle.Cy));var radius=Math.Abs(circle.Radius*transform.Scale);var right=new Point(center.X+radius,center.Y);var left=new Point(center.X-radius,center.Y);context.BeginFigure(right,true,true);context.ArcTo(left,new(radius,radius),0,false,SweepDirection.Clockwise,true,false);context.ArcTo(right,new(radius,radius),0,false,SweepDirection.Clockwise,true,false);return;}
        var start=Start(segments[0]);context.BeginFigure(transform.ToScreen(start),true,true);
        foreach(var segment in segments){if(segment is LineSegment line)context.LineTo(transform.ToScreen(new(line.X2,line.Y2)),true,false);else if(segment is ArcSegment arc){var end=transform.ToScreen(new(arc.Cx+arc.Radius*Math.Cos(arc.EndDegrees*Math.PI/180),arc.Cy+arc.Radius*Math.Sin(arc.EndDegrees*Math.PI/180)));context.ArcTo(end,new(Math.Abs(arc.Radius*transform.Scale),Math.Abs(arc.Radius*transform.Scale)),0,Math.Abs(arc.EndDegrees-arc.StartDegrees)>180,arc.EndDegrees>=arc.StartDegrees?SweepDirection.Counterclockwise:SweepDirection.Clockwise,true,false);}}
    }

    static Point Start(GeometrySegment segment)=>segment switch{LineSegment line=>new(line.X1,line.Y1),ArcSegment arc=>new(arc.Cx+arc.Radius*Math.Cos(arc.StartDegrees*Math.PI/180),arc.Cy+arc.Radius*Math.Sin(arc.StartDegrees*Math.PI/180)),CircleSegment circle=>new(circle.Cx+circle.Radius,circle.Cy),_=>new()};
}

public readonly record struct SectionViewportState(double Zoom,Vector Pan);

public static class SectionViewportEngine
{
    public static double BentFitScale(double viewportWidth,double viewportHeight,double geometryWidth,double geometryHeight)
    {
        var availableWidth=Math.Max(1,viewportWidth-120);var availableHeight=Math.Max(1,viewportHeight-72);return Math.Min(1.5,Math.Min(availableWidth/Math.Max(1,geometryWidth),availableHeight/Math.Max(1,geometryHeight)));
    }

    public static Matrix Transform(double width,double height,SectionViewportState state)
    {
        var center=new Point(width/2,height/2);var matrix=Matrix.Identity;matrix.Translate(-center.X,-center.Y);matrix.Scale(Math.Clamp(state.Zoom,.4,5),Math.Clamp(state.Zoom,.4,5));matrix.Translate(center.X+state.Pan.X,center.Y+state.Pan.Y);return matrix;
    }

    public static SectionViewportState ZoomAt(double width,double height,SectionViewportState state,Point anchor,double factor)
    {
        var before=Transform(width,height,state);var inverse=before;if(inverse.HasInverse)inverse.Invert();var design=inverse.Transform(anchor);var zoom=Math.Clamp(state.Zoom*factor,.4,5);var provisional=new SectionViewportState(zoom,state.Pan);var moved=Transform(width,height,provisional).Transform(design);return provisional with{Pan=state.Pan+(anchor-moved)};
    }

    public static SectionViewportState CenteredZoom(SectionViewportState state,double factor)=>new(Math.Clamp(state.Zoom*factor,.4,5),new Vector());

    public static bool IsInsideFlatMaterial(SectionAxis axis,Point point,double width,double height)
    {
        var body=axis==SectionAxis.W?new Rect(55,height/2-9,Math.Max(1,width-110),18):new Rect(width/2-9,55,18,Math.Max(1,height-110));return body.Contains(point);
    }
}

public sealed record SectionProfileEdges(List<Point> Plus,List<Point> Minus);
public static class SectionProfileEngine
{
    public static double HalfThickness(double thickness,double scale)=>thickness*scale/2;
    public static SectionProfileEdges Edges(IReadOnlyList<Point> center,double halfThickness)
    {
        var plus=new List<Point>();var minus=new List<Point>();
        for(var i=0;i<center.Count;i++){var offset=VertexOffset(center,i,halfThickness);plus.Add(center[i]+offset);minus.Add(center[i]-offset);}
        return new(plus,minus);
    }
    static Vector VertexOffset(IReadOnlyList<Point> points,int index,double distance)
    {
        static Vector Normal(Point a,Point b){var v=b-a;if(v.Length<1e-9)return new Vector(0,-1);v.Normalize();return new Vector(-v.Y,v.X);}
        if(index==0)return Normal(points[0],points[1])*distance;
        if(index==points.Count-1)return Normal(points[^2],points[^1])*distance;
        var n1=Normal(points[index-1],points[index]);var n2=Normal(points[index],points[index+1]);var sum=n1+n2;
        if(sum.Length<1e-6)return n2*distance;var denominator=Vector.Multiply(sum,n2);
        if(Math.Abs(denominator)<.15)return n2*distance;return sum*(distance/denominator);
    }
}

public sealed record SectionDimensionLayout(Point EdgeA,Point EdgeB,Point A,Point B,Point Label,Vector Direction,Vector Normal);
public static class SectionDimensionEngine
{
    public static IReadOnlyList<SectionDimensionLayout> Arrange(IReadOnlyList<(Point A,Point B)> edges,Rect bounds)
    {
        var layouts=edges.Select(edge=>Outside(edge.A,edge.B,bounds)).ToArray();
        var placed=new List<int>();
        foreach(var i in Enumerable.Range(0,edges.Count).OrderBy(i=>(edges[i].B-edges[i].A).Length))
        {
            var layout=layouts[i];
            var station=Vector.Multiply((Vector)layout.A,layout.Normal);
            foreach(var j in placed)
            {
                var other=layouts[j];
                if(Vector.Multiply(layout.Normal,other.Normal)<.999)continue;
                double Project(Point p)=>Vector.Multiply((Vector)p,layout.Direction);
                var lo=Math.Min(Project(layout.A),Project(layout.B));var hi=Math.Max(Project(layout.A),Project(layout.B));
                var otherLo=Math.Min(Project(other.A),Project(other.B));var otherHi=Math.Max(Project(other.A),Project(other.B));
                if(hi<otherLo-8||otherHi<lo-8)continue;
                station=Math.Max(station,Vector.Multiply((Vector)other.A,layout.Normal)+30);
            }
            var shift=layout.Normal*(station-Vector.Multiply((Vector)layout.A,layout.Normal));
            layouts[i]=layout with{A=layout.A+shift,B=layout.B+shift,Label=layout.Label+shift};
            placed.Add(i);
        }
        return layouts;
    }
    static SectionDimensionLayout Outside(Point a,Point b,Rect bounds)
    {
        var direction=b-a;if(direction.Length<1e-6)direction=new Vector(1,0);else direction.Normalize();
        var normal=new Vector(-direction.Y,direction.X);var midpoint=new Point((a.X+b.X)/2,(a.Y+b.Y)/2);
        var boxCenter=new Point(bounds.Left+bounds.Width/2,bounds.Top+bounds.Height/2);
        if(Vector.Multiply(midpoint-boxCenter,normal)<0)normal=-normal;
        var extreme=new[]{bounds.TopLeft,bounds.TopRight,bounds.BottomLeft,bounds.BottomRight}.Max(p=>Vector.Multiply((Vector)p,normal));
        var shift=Math.Max(30,extreme+34-Vector.Multiply((Vector)midpoint,normal));
        var da=a+normal*shift;var db=b+normal*shift;
        return new(a,b,da,db,new((da.X+db.X)/2,(da.Y+db.Y)/2),direction,normal);
    }
}
