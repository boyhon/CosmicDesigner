using System.Windows;
using System.Windows.Media;

namespace VCutting;

// Preserve analytic LINE/ARC geometry while subtracting a boundary cut.
public static class CurvedBoundaryCutEngine
{
    const double Tolerance=1e-6;
    public static bool TrySubtractQuarterCircle(IReadOnlyList<GeometrySegment> outer,IReadOnlyList<GeometrySegment> cut,out IReadOnlyList<GeometrySegment> result)
    {
        result=[];
        if(cut.Count!=3||cut[0] is not ArcSegment arc||cut[1] is not LineSegment||cut[2] is not LineSegment||arc.Radius<=Tolerance||Math.Abs(arc.EndDegrees-arc.StartDegrees-90)>Tolerance)return false;
        for(var i=0;i<cut.Count;i++)if((At(cut[i],1)-At(cut[(i+1)%cut.Count],0)).Length>Tolerance)return false;
        var overlap=cut.OfType<LineSegment>().Any(radius=>outer.OfType<LineSegment>().Any(line=>
        {
            var p=At(radius,0);var edge=At(radius,1)-p;if(edge.Length<=Tolerance)return false;
            var a=At(line,0);var b=At(line,1);
            if(Math.Abs(Vector.CrossProduct(a-p,edge))/edge.Length>Tolerance||Math.Abs(Vector.CrossProduct(b-p,edge))/edge.Length>Tolerance)return false;
            var t0=Vector.Multiply(a-p,edge)/edge.LengthSquared;var t1=Vector.Multiply(b-p,edge)/edge.LengthSquared;
            return Math.Min(1,Math.Max(t0,t1))-Math.Max(0,Math.Min(t0,t1))>Tolerance;
        }));
        return overlap&&TrySubtract(outer,cut,out result);
    }
    public static bool TrySubtractSemicircle(IReadOnlyList<GeometrySegment> outer,IReadOnlyList<GeometrySegment> cut,out IReadOnlyList<GeometrySegment> result)
    {
        result=[];
        if(cut.Count!=2||cut[0] is not ArcSegment arc||cut[1] is not LineSegment diameter||Math.Abs(arc.EndDegrees-arc.StartDegrees-180)>Tolerance||arc.Radius<=Tolerance)return false;
        if((At(arc,1)-At(diameter,0)).Length>Tolerance||(At(diameter,1)-At(arc,0)).Length>Tolerance)return false;
        var p=At(diameter,0);var edge=At(diameter,1)-p;
        var overlap=outer.OfType<LineSegment>().Any(line=>
        {
            var a=At(line,0);var b=At(line,1);
            if(Math.Abs(Vector.CrossProduct(a-p,edge))/edge.Length>Tolerance||Math.Abs(Vector.CrossProduct(b-p,edge))/edge.Length>Tolerance)return false;
            var t0=Vector.Multiply(a-p,edge)/edge.LengthSquared;var t1=Vector.Multiply(b-p,edge)/edge.LengthSquared;
            return Math.Min(1,Math.Max(t0,t1))-Math.Max(0,Math.Min(t0,t1))>Tolerance;
        });
        return overlap&&TrySubtract(outer,cut,out result);
    }
    public static bool TrySubtract(IReadOnlyList<GeometrySegment> outer,IReadOnlyList<GeometrySegment> cut,out IReadOnlyList<GeometrySegment> result)
    {
        result=[];
        if(outer.Count<3||cut.Count<2||outer.Any(s=>s is not (LineSegment or ArcSegment))||cut.Any(s=>s is not (LineSegment or ArcSegment)))return false;
        var outerGeometry=Path(outer);var cutGeometry=Path(cut);
        var outerSplits=outer.Select(_=>new List<double>{0,1}).ToArray();var cutSplits=cut.Select(_=>new List<double>{0,1}).ToArray();
        for(var i=0;i<outer.Count;i++)for(var j=0;j<cut.Count;j++)
            AllIntersections(outer[i],cut[j],outerSplits[i],cutSplits[j]);
        var pieces=new List<GeometrySegment>();var removed=false;
        for(var i=0;i<outer.Count;i++)foreach(var piece in Split(outer[i],outerSplits[i]))
        {
            var mid=At(piece,.5);
            if(OnBoundary(cut,mid)||cutGeometry.FillContains(mid,Tolerance,ToleranceType.Absolute)){removed=true;continue;}
            pieces.Add(piece);
        }
        var reverse=SignedArea(outer)*SignedArea(cut)>0;
        for(var i=0;i<cut.Count;i++)foreach(var piece in Split(cut[i],cutSplits[i]))
        {
            var mid=At(piece,.5);
            if(OnBoundary(outer,mid)||!outerGeometry.FillContains(mid,Tolerance,ToleranceType.Absolute))continue;
            pieces.Add(reverse?Reverse(piece):piece);
        }
        if(!removed||pieces.Count<3)return false;
        var ordered=new List<GeometrySegment>{pieces[0]};pieces.RemoveAt(0);
        while(pieces.Count>0)
        {
            var end=At(ordered[^1],1);
            if((end-At(ordered[0],0)).Length<=Tolerance)return false; // More than one loop, including internal holes.
            var matches=pieces.Select((s,i)=>(s,i)).Where(x=>(At(x.s,0)-end).Length<=Tolerance).ToList();
            if(matches.Count!=1)return false;
            ordered.Add(matches[0].s);pieces.RemoveAt(matches[0].i);
        }
        if((At(ordered[^1],1)-At(ordered[0],0)).Length>Tolerance||Math.Abs(SignedArea(ordered))<=Tolerance)return false;
        if(Math.Abs(SignedArea(outer))-Math.Abs(SignedArea(ordered))<=Tolerance)return false;
        result=ordered;return true;
    }
    internal static Point At(GeometrySegment s,double t)=>s switch
    {
        LineSegment l=>new(l.X1+(l.X2-l.X1)*t,l.Y1+(l.Y2-l.Y1)*t),
        ArcSegment a=>new(a.Cx+a.Radius*Math.Cos((a.StartDegrees+(a.EndDegrees-a.StartDegrees)*t)*Math.PI/180),a.Cy+a.Radius*Math.Sin((a.StartDegrees+(a.EndDegrees-a.StartDegrees)*t)*Math.PI/180)),
        _=>throw new ArgumentException(Localization.Text("ui.0000"))
    };
    internal static GeometrySegment Reverse(GeometrySegment s)=>s switch{LineSegment l=>new LineSegment(l.X2,l.Y2,l.X1,l.Y1),ArcSegment a=>a with{StartDegrees=a.EndDegrees,EndDegrees=a.StartDegrees},_=>throw new ArgumentException()};
    internal static IEnumerable<GeometrySegment> Split(GeometrySegment s,List<double> parameters)
    {
        var ts=parameters.Where(t=>t>=-Tolerance&&t<=1+Tolerance).Select(t=>Math.Clamp(t,0,1)).Order().ToList();
        var unique=new List<double>();foreach(var t in ts)if(unique.Count==0||t-unique[^1]>1e-9)unique.Add(t);
        for(var i=0;i<unique.Count-1;i++)
        {
            var t0=unique[i];var t1=unique[i+1];if((At(s,t0)-At(s,t1)).Length<=Tolerance)continue;
            if(t0==0&&t1==1){yield return s;continue;}
            if(s is ArcSegment a)yield return a with{StartDegrees=a.StartDegrees+(a.EndDegrees-a.StartDegrees)*t0,EndDegrees=a.StartDegrees+(a.EndDegrees-a.StartDegrees)*t1};
            else{var p=At(s,t0);var q=At(s,t1);yield return new LineSegment(p.X,p.Y,q.X,q.Y);}
        }
    }
    internal static void AllIntersections(GeometrySegment first,GeometrySegment second,List<double> ft,List<double> st)
    {
        if(second is LineSegment line){Intersections(first,line,ft,st);return;}
        if(first is LineSegment firstLine){Intersections(second,firstLine,st,ft);return;}
        var a=(ArcSegment)first;var b=(ArcSegment)second;
        var centerA=new Point(a.Cx,a.Cy);var centerB=new Point(b.Cx,b.Cy);var delta=centerB-centerA;var distance=delta.Length;
        void Add(Point point){if(ArcParameter(a,point,out var t)&&ArcParameter(b,point,out var u)){ft.Add(t);st.Add(u);}}
        if(distance<=Tolerance){if(Math.Abs(a.Radius-b.Radius)<=Tolerance)foreach(var point in new[]{At(a,0),At(a,1),At(b,0),At(b,1)})Add(point);return;}
        if(distance>a.Radius+b.Radius+Tolerance||distance<Math.Abs(a.Radius-b.Radius)-Tolerance)return;
        var x=(a.Radius*a.Radius-b.Radius*b.Radius+distance*distance)/(2*distance);var heightSquared=a.Radius*a.Radius-x*x;
        if(heightSquared< -Tolerance)return;
        var unit=delta/distance;var midpoint=centerA+unit*x;var offset=new Vector(-unit.Y,unit.X)*Math.Sqrt(Math.Max(0,heightSquared));
        Add(midpoint+offset);Add(midpoint-offset);
    }
    static void Intersections(GeometrySegment outer,LineSegment cut,List<double> ot,List<double> ct)
    {
        var p=At(cut,0);var d=At(cut,1)-p;
        if(d.LengthSquared<=Tolerance*Tolerance)return;
        if(outer is LineSegment line)
        {
            var q=At(line,0);var e=At(line,1)-q;var cross=Vector.CrossProduct(e,d);
            if(Math.Abs(cross)>1e-10)
            {
                var t=Vector.CrossProduct(p-q,d)/cross;var u=Vector.CrossProduct(p-q,e)/cross;
                if(t>=-Tolerance&&t<=1+Tolerance&&u>=-Tolerance&&u<=1+Tolerance){ot.Add(t);ct.Add(u);}
            }
            else if(e.LengthSquared>Tolerance*Tolerance&&Math.Abs(Vector.CrossProduct(p-q,e))/e.Length<=Tolerance)
            {
                foreach(var point in new[]{q,q+e,p,p+d})
                {
                    var t=Vector.Multiply(point-q,e)/e.LengthSquared;var u=Vector.Multiply(point-p,d)/d.LengthSquared;
                    if(t>=-Tolerance&&t<=1+Tolerance&&u>=-Tolerance&&u<=1+Tolerance){ot.Add(t);ct.Add(u);}
                }
            }
        }
        else if(outer is ArcSegment arc)
        {
            var f=p-new Point(arc.Cx,arc.Cy);var aa=d.LengthSquared;var bb=2*Vector.Multiply(f,d);var cc=f.LengthSquared-arc.Radius*arc.Radius;
            var discriminant=bb*bb-4*aa*cc;if(discriminant<0)return;
            foreach(var u in new[]{(-bb-Math.Sqrt(discriminant))/(2*aa),(-bb+Math.Sqrt(discriminant))/(2*aa)})
            {
                if(u< -Tolerance||u>1+Tolerance)continue;var point=p+d*u;
                if(ArcParameter(arc,point,out var t)){ot.Add(t);ct.Add(u);}
            }
        }
    }
    internal static bool ArcParameter(ArcSegment a,Point p,out double t)
    {
        var angle=Math.Atan2(p.Y-a.Cy,p.X-a.Cx)*180/Math.PI;var sweep=a.EndDegrees-a.StartDegrees;t=0;if(Math.Abs(sweep)<1e-10)return false;
        var turns=Math.Round((a.StartDegrees-angle)/360);angle+=turns*360;
        foreach(var shifted in new[]{angle,angle+360,angle-360}){var candidate=(shifted-a.StartDegrees)/sweep;if(candidate>=-Tolerance&&candidate<=1+Tolerance){t=Math.Clamp(candidate,0,1);return true;}}
        return false;
    }
    internal static bool OnBoundary(IReadOnlyList<GeometrySegment> segments,Point p)=>segments.Any(s=>
    {
        if(s is ArcSegment a)return Math.Abs((p-new Point(a.Cx,a.Cy)).Length-a.Radius)<=Tolerance&&ArcParameter(a,p,out _);
        var start=At(s,0);var edge=At(s,1)-start;if(edge.LengthSquared<=Tolerance*Tolerance)return (p-start).Length<=Tolerance;
        var t=Math.Clamp(Vector.Multiply(p-start,edge)/edge.LengthSquared,0,1);return (p-(start+edge*t)).Length<=Tolerance;
    });
    internal static PathGeometry Path(IReadOnlyList<GeometrySegment> segments)
    {
        var figure=new PathFigure{StartPoint=At(segments[0],0),IsClosed=true,IsFilled=true};
        foreach(var s in segments)
            if(s is ArcSegment a)figure.Segments.Add(new System.Windows.Media.ArcSegment(At(a,1),new Size(a.Radius,a.Radius),0,Math.Abs(a.EndDegrees-a.StartDegrees)>180,a.EndDegrees>=a.StartDegrees?SweepDirection.Clockwise:SweepDirection.Counterclockwise,true));
            else figure.Segments.Add(new System.Windows.Media.LineSegment(At(s,1),true));
        return new PathGeometry([figure]);
    }
    internal static double SignedArea(IEnumerable<GeometrySegment> segments)
    {
        var twiceArea=0d;
        foreach(var s in segments)
            if(s is LineSegment l)twiceArea+=l.X1*l.Y2-l.X2*l.Y1;
            else if(s is ArcSegment a){var start=a.StartDegrees*Math.PI/180;var end=a.EndDegrees*Math.PI/180;twiceArea+=a.Radius*a.Cx*(Math.Sin(end)-Math.Sin(start))+a.Radius*a.Cy*(Math.Cos(start)-Math.Cos(end))+a.Radius*a.Radius*(end-start);}
        return twiceArea/2;
    }
}
