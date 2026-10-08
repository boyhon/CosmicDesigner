using System.Windows;

namespace VCutting;

public readonly record struct EllipseProperties(double CenterX,double CenterY,double MajorLength,double MinorLength,double RotationDegrees)
{
    public static double NormalizeAngle(double angle){var value=((angle%180)+180)%180;return value<1e-10||180-value<1e-10?0:value;}
    public bool IsValid=>double.IsFinite(CenterX)&&double.IsFinite(CenterY)&&double.IsFinite(MajorLength)&&double.IsFinite(MinorLength)&&double.IsFinite(RotationDegrees)&&MinorLength>=.1&&MajorLength>=MinorLength;
    public bool Fits(double width,double height)
    {
        if(!IsValid)return false;
        var t=RotationDegrees*Math.PI/180;var a=MajorLength/2;var b=MinorLength/2;
        var ex=Math.Sqrt(a*a*Math.Cos(t)*Math.Cos(t)+b*b*Math.Sin(t)*Math.Sin(t));
        var ey=Math.Sqrt(a*a*Math.Sin(t)*Math.Sin(t)+b*b*Math.Cos(t)*Math.Cos(t));
        return double.IsFinite(ex)&&double.IsFinite(ey)&&CenterX-ex>=-1e-9&&CenterY-ey>=-1e-9&&CenterX+ex<=width+1e-9&&CenterY+ey<=height+1e-9;
    }
}

public static class EllipsePropertiesEngine
{
    // Legacy ellipses are sampled uniformly in parameter, including after affine resize.
    // Accept only a closed, undeformed ellipse; never silently repair an edited contour.
    public static bool TryGet(CutOperation cut,out EllipseProperties value)
    {
        value=default;if(cut.Shape!="Ellipse"||cut.Geometry.Count!=48||cut.Geometry.Any(g=>g is not LineSegment))return false;
        var lines=cut.Geometry.Cast<LineSegment>().ToList();
        var points=lines.Select(l=>new Point(l.X1,l.Y1)).ToList();
        if(lines.Any(l=>!double.IsFinite(l.X1)||!double.IsFinite(l.Y1)||!double.IsFinite(l.X2)||!double.IsFinite(l.Y2)))return false;
        var cx=points.Average(p=>p.X);var cy=points.Average(p=>p.Y);
        var xx=points.Average(p=>(p.X-cx)*(p.X-cx));var yy=points.Average(p=>(p.Y-cy)*(p.Y-cy));var xy=points.Average(p=>(p.X-cx)*(p.Y-cy));
        var diff=Math.Sqrt((xx-yy)*(xx-yy)+4*xy*xy);var hi=(xx+yy+diff)/2;var lo=(xx+yy-diff)/2;
        if(!double.IsFinite(hi)||!double.IsFinite(lo)||lo<=0)return false;
        var a=Math.Sqrt(2*hi);var b=Math.Sqrt(2*lo);
        var t=diff<=Math.Max(xx+yy,1e-20)*1e-10?Math.Atan2(points[0].Y-cy,points[0].X-cx):Math.Atan2(2*xy,xx-yy)/2;
        var cs=Math.Cos(t);var sn=Math.Sin(t);
        // Project directly rather than subtracting nearly equal eigenvalues for thin ellipses.
        a=Math.Sqrt(2*points.Average(p=>Math.Pow((p.X-cx)*cs+(p.Y-cy)*sn,2)));
        b=Math.Sqrt(2*points.Average(p=>Math.Pow(-(p.X-cx)*sn+(p.Y-cy)*cs,2)));
        var tolerance=Math.Max(a,1)*1e-7;
        for(int i=0;i<48;i++)
        {
            var p=points[i];var dx=p.X-cx;var dy=p.Y-cy;var u=(dx*cs+dy*sn)/a;var v=(-dx*sn+dy*cs)/b;
            if(Math.Abs(u*u+v*v-1)>1e-6||(new Point(lines[i].X2,lines[i].Y2)-points[(i+1)%48]).Length>tolerance)return false;
            // Guard against repeated/reordered points masquerading as a valid ellipse.
            var q=points[(i+1)%48];var qu=((q.X-cx)*cs+(q.Y-cy)*sn)/a;var qv=(-(q.X-cx)*sn+(q.Y-cy)*cs)/b;
            if(Math.Abs(u*qu+v*qv-Math.Cos(Math.PI*2/48))>1e-6)return false;
        }
        var minor=2*b;var major=2*a;
        if(Math.Abs(minor-.1)<1e-8)minor=.1;
        if(Math.Abs(major-minor)<1e-8)major=minor;
        value=new(cx,cy,major,minor,EllipseProperties.NormalizeAngle(t*180/Math.PI));return value.IsValid;
    }

    public static IReadOnlyList<Point> Transform(CutOperation cut,EllipseProperties old,EllipseProperties target)
    {
        var t=old.RotationDegrees*Math.PI/180;var n=target.RotationDegrees*Math.PI/180;
        return cut.Geometry.Cast<LineSegment>().Select(l=>
        {
            var dx=l.X1-old.CenterX;var dy=l.Y1-old.CenterY;
            var u=(dx*Math.Cos(t)+dy*Math.Sin(t))*target.MajorLength/old.MajorLength;
            var v=(-dx*Math.Sin(t)+dy*Math.Cos(t))*target.MinorLength/old.MinorLength;
            return new Point(target.CenterX+u*Math.Cos(n)-v*Math.Sin(n),target.CenterY+u*Math.Sin(n)+v*Math.Cos(n));
        }).ToList();
    }
}
