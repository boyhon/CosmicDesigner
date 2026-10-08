using System.Windows;

namespace VCutting;

public static class SlitHitTesting
{
    public static SlitOperation? Find(IEnumerable<SlitOperation> slits, FlatViewportTransform transform, Point screen, double tolerance)
    {
        if(!double.IsFinite(transform.Scale)||transform.Scale<=0||!double.IsFinite(tolerance)||tolerance<0)return null;
        var point=transform.ToDesign(screen);var best=tolerance/transform.Scale;SlitOperation? selected=null;
        foreach(var slit in slits.Reverse())
            foreach(var segment in slit.Geometry)
            {
                var distance=Distance(segment,point);
                if(distance>best||selected is not null&&distance>=best)continue;
                best=distance;selected=slit;
            }
        return selected;
    }
    public static double Distance(GeometrySegment segment, Point point)
    {
        if(segment is LineSegment l)
        {
            var start=new Point(l.X1,l.Y1);var delta=new Vector(l.X2-l.X1,l.Y2-l.Y1);
            var t=delta.LengthSquared<1e-20?0:Math.Clamp(Vector.Multiply(point-start,delta)/delta.LengthSquared,0,1);
            return (point-(start+delta*t)).Length;
        }
        if(segment is ArcSegment a)
        {
            var offset=point-new Point(a.Cx,a.Cy);var sweep=a.EndDegrees-a.StartDegrees;
            var angle=Math.Atan2(offset.Y,offset.X)*180/Math.PI;
            var travel=sweep>=0?Normalize(angle-a.StartDegrees):Normalize(a.StartDegrees-angle);
            if(travel<=Math.Abs(sweep)+1e-9||Math.Abs(sweep)>=360)return Math.Abs(offset.Length-a.Radius);
            Point At(double degrees)=>new(a.Cx+a.Radius*Math.Cos(degrees*Math.PI/180),a.Cy+a.Radius*Math.Sin(degrees*Math.PI/180));
            return Math.Min((point-At(a.StartDegrees)).Length,(point-At(a.EndDegrees)).Length);
        }
        return double.PositiveInfinity;
    }
    static double Normalize(double degrees)=>(degrees%360+360)%360;
}
