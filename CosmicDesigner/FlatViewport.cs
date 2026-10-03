using System.Windows;
using System.Windows.Media;

namespace CosmicDesigner;

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

public readonly record struct HoleDragGeometry(double CenterX,double CenterY,double Width,double Height);

public static class HoleDragEngine
{
    public static bool Supports(string? shape)=>shape is "Circle" or "Triangle" or "Rectangle";

    public static bool TryCalculate(string shape,Point start,Point end,double materialWidth,double materialHeight,out HoleDragGeometry geometry)
    {
        geometry=default;if(!Supports(shape))return false;
        start=new(Math.Clamp(start.X,0,materialWidth),Math.Clamp(start.Y,0,materialHeight));
        end=new(Math.Clamp(end.X,0,materialWidth),Math.Clamp(end.Y,0,materialHeight));
        var dx=end.X-start.X;var dy=end.Y-start.Y;
        if(shape=="Circle")
        {
            var size=Math.Min(Math.Abs(dx),Math.Abs(dy));if(size<.1)return false;
            end=new(start.X+Math.Sign(dx)*size,start.Y+Math.Sign(dy)*size);
        }
        var left=Math.Min(start.X,end.X);var right=Math.Max(start.X,end.X);var bottom=Math.Min(start.Y,end.Y);var top=Math.Max(start.Y,end.Y);
        if(right-left<.1||top-bottom<.1)return false;
        geometry=new((left+right)/2,(bottom+top)/2,right-left,top-bottom);return true;
    }
}

public static class FlatMaterialMaskEngine
{
    public static StreamGeometry Build(CosmicDesignerDocument document,FlatViewportTransform transform)
    {
        var geometry=new StreamGeometry{FillRule=FillRule.EvenOdd};using var context=geometry.Open();
        AddContour(context,document.OuterContour.Segments,transform);
        foreach(var cut in document.Cuts)AddContour(context,cut.Geometry,transform);
        geometry.Freeze();return geometry;
    }

    static void AddContour(StreamGeometryContext context,IReadOnlyList<GeometrySegment> segments,FlatViewportTransform transform)
    {
        if(segments.Count==0)return;
        if(segments.Count==1&&segments[0] is CircleSegment circle){var center=transform.ToScreen(new(circle.Cx,circle.Cy));var radius=Math.Abs(circle.Radius*transform.Scale);var right=new Point(center.X+radius,center.Y);var left=new Point(center.X-radius,center.Y);context.BeginFigure(right,true,true);context.ArcTo(left,new(radius,radius),0,false,SweepDirection.Clockwise,true,false);context.ArcTo(right,new(radius,radius),0,false,SweepDirection.Clockwise,true,false);return;}
        var start=Start(segments[0]);context.BeginFigure(transform.ToScreen(start),true,true);
        foreach(var segment in segments){if(segment is LineSegment line)context.LineTo(transform.ToScreen(new(line.X2,line.Y2)),true,false);else if(segment is ArcSegment arc){var end=transform.ToScreen(new(arc.Cx+arc.Radius*Math.Cos(arc.EndDegrees*Math.PI/180),arc.Cy+arc.Radius*Math.Sin(arc.EndDegrees*Math.PI/180)));context.ArcTo(end,new(Math.Abs(arc.Radius*transform.Scale),Math.Abs(arc.Radius*transform.Scale)),0,Math.Abs(arc.EndDegrees-arc.StartDegrees)>180,SweepDirection.Counterclockwise,true,false);}}
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
