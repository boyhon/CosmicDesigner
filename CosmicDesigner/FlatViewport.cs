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
}

public readonly record struct SectionViewportState(double Zoom,Vector Pan);

public static class SectionViewportEngine
{
    public static Matrix Transform(double width,double height,SectionViewportState state)
    {
        var center=new Point(width/2,height/2);var matrix=Matrix.Identity;matrix.Translate(-center.X,-center.Y);matrix.Scale(Math.Clamp(state.Zoom,.4,5),Math.Clamp(state.Zoom,.4,5));matrix.Translate(center.X+state.Pan.X,center.Y+state.Pan.Y);return matrix;
    }

    public static SectionViewportState ZoomAt(double width,double height,SectionViewportState state,Point anchor,double factor)
    {
        var before=Transform(width,height,state);var inverse=before;if(inverse.HasInverse)inverse.Invert();var design=inverse.Transform(anchor);var zoom=Math.Clamp(state.Zoom*factor,.4,5);var provisional=new SectionViewportState(zoom,state.Pan);var moved=Transform(width,height,provisional).Transform(design);return provisional with{Pan=state.Pan+(anchor-moved)};
    }

    public static bool IsInsideFlatMaterial(SectionAxis axis,Point point,double width,double height)
    {
        var body=axis==SectionAxis.W?new Rect(55,height/2-9,Math.Max(1,width-110),18):new Rect(width/2-9,55,18,Math.Max(1,height-110));return body.Contains(point);
    }
}
