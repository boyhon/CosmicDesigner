using System.Windows;

namespace CosmicDesigner;

public readonly record struct CenterAlignmentGuide(bool Vertical,double Position,double First,double Second);
public static class CutAlignmentEngine
{
    public static Point Center(CutOperation cut)=>cut.SemicircleArc is { } semi?new(semi.Cx,semi.Cy):cut.QuarterCircleArc is { } quarter?new(quarter.Cx,quarter.Cy):new(cut.CenterX,cut.CenterY);
    public static IReadOnlyList<CenterAlignmentGuide> Find(CosmicDesignerDocument document,CutOperation cut,double scale,double tolerancePixels=1.5)
    {
        var center=Center(cut);var result=new List<CenterAlignmentGuide>();
        if(!double.IsFinite(scale)||scale<=0||!double.IsFinite(tolerancePixels)||tolerancePixels<0||!double.IsFinite(center.X)||!double.IsFinite(center.Y))return result;
        foreach(var vertical in new[]{true,false})
        {
            var station=vertical?center.X:center.Y;var cross=vertical?center.Y:center.X;
            var positions=document.OuterContour.Segments.OfType<LineSegment>().Where(l=>vertical?
                Math.Abs(l.X1-l.X2)<1e-7&&cross>=Math.Min(l.Y1,l.Y2)-1e-7&&cross<=Math.Max(l.Y1,l.Y2)+1e-7:
                Math.Abs(l.Y1-l.Y2)<1e-7&&cross>=Math.Min(l.X1,l.X2)-1e-7&&cross<=Math.Max(l.X1,l.X2)+1e-7)
                .Select(l=>vertical?l.X1:l.Y1)
                .Concat(document.Bends.Where(b=>b.Axis==(vertical?SectionAxis.W:SectionAxis.H)&&
                    ContourSectionEngine.MaterialIntervals(document.OuterContour.Segments,vertical?SectionAxis.H:SectionAxis.W,b.Position).Any(i=>cross>=i.Start&&cross<=i.End)).Select(b=>b.Position))
                .Where(double.IsFinite).Distinct().Order().ToList();
            var left=positions.Where(p=>p<=station).ToList();var right=positions.Where(p=>p>=station).ToList();
            if(left.Count==0||right.Count==0)continue;var a=left[^1];var b=right[0];if(b-a<1e-7)continue;
            var middle=a+(b-a)/2;
            if(Math.Abs(station-middle)*scale<=tolerancePixels)result.Add(new(vertical,middle,a,b));
        }
        return result;
    }
}
