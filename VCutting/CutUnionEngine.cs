using System.Windows;
using System.Windows.Media;
using C = VCutting.CurvedBoundaryCutEngine;

namespace VCutting;

public static class CutContourEngine
{
    public static bool Contains(CutOperation cut,Point point)=>cut.Shape=="RegularStar"?RegularStarGeometry.Contains(cut.Geometry,point):Contains(cut.Geometry,point);
    public static IReadOnlyList<IReadOnlyList<GeometrySegment>> Loops(IReadOnlyList<GeometrySegment> geometry)
    {
        if(geometry.Count==1&&geometry[0] is CircleSegment c&&double.IsFinite(c.Radius)&&c.Radius>0)
            return [new GeometrySegment[]{new ArcSegment(c.Cx,c.Cy,c.Radius,0,180),new ArcSegment(c.Cx,c.Cy,c.Radius,180,360)}];
        if(geometry.Count==0||geometry.Any(s=>s is not (LineSegment or ArcSegment)))return [];
        var loops=new List<IReadOnlyList<GeometrySegment>>();var loop=new List<GeometrySegment>();
        foreach(var s in geometry)
        {
            var a=C.At(s,0);var b=C.At(s,1);
            if(!double.IsFinite(a.X)||!double.IsFinite(a.Y)||!double.IsFinite(b.X)||!double.IsFinite(b.Y)||s is ArcSegment arc&&(!double.IsFinite(arc.Radius)||arc.Radius<=0||Math.Abs(arc.EndDegrees-arc.StartDegrees)>360))return [];
            if(loop.Count>0&&(C.At(loop[^1],1)-a).Length>1e-6)return [];
            loop.Add(s);
            if((b-C.At(loop[0],0)).Length<=1e-6){if(loop.Count<2||Math.Abs(C.SignedArea(loop))<1e-10)return [];loops.Add(loop.ToList());loop.Clear();}
        }
        return loop.Count==0?loops:[];
    }
    public static PathGeometry Path(IReadOnlyList<GeometrySegment> geometry)
    {
        var path=new PathGeometry{FillRule=FillRule.EvenOdd};foreach(var loop in Loops(geometry))foreach(var f in C.Path(loop).Figures)path.Figures.Add(f);return path;
    }
    public static bool Contains(IReadOnlyList<GeometrySegment> geometry,Point p)
    {
        var inside=false;
        foreach(var s in geometry)
        {
            if(s is CircleSegment c){if((p-new Point(c.Cx,c.Cy)).Length<c.Radius)inside=!inside;continue;}
            if(s is LineSegment l){Cross(new(l.X1,l.Y1),new(l.X2,l.Y2));continue;}
            if(s is not ArcSegment a)continue;
            var ts=new List<double>{0,1};
            foreach(var angle in new[]{90d,270d})if(C.ArcParameter(a,new(a.Cx+a.Radius*Math.Cos(angle*Math.PI/180),a.Cy+a.Radius*Math.Sin(angle*Math.PI/180)),out var t))ts.Add(t);
            foreach(var piece in C.Split(a,ts).Cast<ArcSegment>())
            {
                var start=C.At(piece,0);var end=C.At(piece,1);if((start.Y>p.Y)==(end.Y>p.Y))continue;
                var v=(p.Y-piece.Cy)/piece.Radius;if(v< -1||v>1)continue;
                var angle=Math.Asin(Math.Clamp(v,-1,1));
                foreach(var tAngle in new[]{angle,Math.PI-angle})
                {
                    var q=new Point(piece.Cx+piece.Radius*Math.Cos(tAngle),p.Y);
                    if(C.ArcParameter(piece,q,out _)&&q.X>p.X){inside=!inside;break;}
                }
            }
        }
        return inside;
        void Cross(Point a,Point b){if((a.Y>p.Y)!=(b.Y>p.Y)&&p.X<(b.X-a.X)*(p.Y-a.Y)/(b.Y-a.Y)+a.X)inside=!inside;}
    }
    public static IEnumerable<Point> Extrema(IReadOnlyList<GeometrySegment> geometry)
    {
        foreach(var s in geometry)
        {
            yield return C.At(s,0);yield return C.At(s,1);
            if(s is ArcSegment a)foreach(var angle in new[]{0d,90d,180d,270d})
            {var p=new Point(a.Cx+a.Radius*Math.Cos(angle*Math.PI/180),a.Cy+a.Radius*Math.Sin(angle*Math.PI/180));if(C.ArcParameter(a,p,out _))yield return p;}
        }
    }
}

public static class CutUnionEngine
{
    const double Tolerance=1e-6;
    public static bool TryBuild(VCuttingDocument document,CutOperation selected,out IReadOnlyList<CutOperation> members,out IReadOnlyList<GeometrySegment> contour)
    {
        members=[];contour=[];if(!document.Cuts.Contains(selected))return false;
        var outer=CutContourEngine.Loops(document.OuterContour.Segments).SelectMany(l=>l).ToList();if(outer.Count==0)return false;
        var nodes=new List<(CutOperation Cut,List<GeometrySegment> Segments,PathGeometry Path)>();
        foreach(var cut in document.Cuts)
        {
            var loops=CutContourEngine.Loops(cut.Geometry);if(loops.Count==0)continue;
            var segments=loops.SelectMany(l=>l).ToList();var path=CutContourEngine.Path(segments);if(cut.Shape=="RegularStar")path.FillRule=FillRule.Nonzero;
            if(!Internal(outer,segments))continue;
            nodes.Add((cut,segments,path));
        }
        var seed=nodes.FindIndex(n=>ReferenceEquals(n.Cut,selected));if(seed<0)return false;
        var included=new HashSet<int>{seed};var queue=new Queue<int>();queue.Enqueue(seed);
        while(queue.Count>0)
        {
            var i=queue.Dequeue();for(int j=0;j<nodes.Count;j++)if(!included.Contains(j)&&Connected(nodes[i].Segments,nodes[j].Segments,nodes[i].Path,nodes[j].Path)){included.Add(j);queue.Enqueue(j);}
        }
        if(included.Count<2)return false;
        var group=included.Order().Select(i=>nodes[i]).ToList();var paths=group.Select(n=>n.Segments).ToList();
        if(!TryUnion(paths,group.Select(n=>n.Cut.Shape=="RegularStar").ToArray(),out contour))return false;
        members=group.Select(n=>n.Cut).ToList();return true;
    }
    static bool Internal(IReadOnlyList<GeometrySegment> outer,IReadOnlyList<GeometrySegment> segments)
    {
        foreach(var s in segments)
        {
            if(!CutContourEngine.Contains(outer,C.At(s,.5)))return false;
            foreach(var edge in outer){var a=new List<double>();var b=new List<double>();C.AllIntersections(s,edge,a,b);if(a.Count>0)return false;}
        }
        return true;
    }
    static bool Connected(IReadOnlyList<GeometrySegment> a,IReadOnlyList<GeometrySegment> b,PathGeometry pa,PathGeometry pb)
    {
        // Area overlap includes containment and identical objects; shared boundary length is also mergeable.
        if(Geometry.Combine(pa,pb,GeometryCombineMode.Intersect,Transform.Identity,1e-7,ToleranceType.Absolute).GetArea(1e-7,ToleranceType.Absolute)>1e-10)return true;
        foreach(var first in a)foreach(var second in b)
        {
            var ts=new List<double>{0,1};var us=new List<double>{0,1};C.AllIntersections(first,second,ts,us);
            foreach(var piece in C.Split(first,ts))if(C.OnBoundary([second],C.At(piece,.5))&&(C.At(piece,1)-C.At(piece,0)).Length>Tolerance)return true;
        }
        return false;
    }
    static bool TryUnion(IReadOnlyList<List<GeometrySegment>> paths,IReadOnlyList<bool> solidStars,out IReadOnlyList<GeometrySegment> contour)
    {
        contour=[];var sources=paths.SelectMany((path,owner)=>path.Select(s=>(Segment:s,Owner:owner,Ts:new List<double>{0,1}))).ToList();
        for(int i=0;i<sources.Count;i++)for(int j=i+1;j<sources.Count;j++)C.AllIntersections(sources[i].Segment,sources[j].Segment,sources[i].Ts,sources[j].Ts);
        var points=sources.SelectMany(s=>CutContourEngine.Extrema([s.Segment])).ToList();var span=Math.Max(points.Max(p=>p.X)-points.Min(p=>p.X),points.Max(p=>p.Y)-points.Min(p=>p.Y));
        var offset=Math.Max(1e-7,span*1e-8);var pieces=new List<GeometrySegment>();
        bool Filled(Point p)=>paths.Select((path,i)=>solidStars[i]?RegularStarGeometry.Contains(path,p):CutContourEngine.Contains(path,p)).Any(inside=>inside);
        foreach(var source in sources)foreach(var piece in C.Split(source.Segment,source.Ts))
        {
            var mid=C.At(piece,.5);var tangent=Tangent(piece,.5);if(tangent.Length<=1e-12)return false;tangent.Normalize();var normal=new Vector(-tangent.Y,tangent.X)*offset;
            var left=Filled(mid+normal);var right=Filled(mid-normal);if(left==right)continue;
            var kept=left?piece:C.Reverse(piece);
            if(!pieces.Any(p=>Same(p,kept)))pieces.Add(kept);
        }
        if(pieces.Count<2)return false;
        var ordered=new List<GeometrySegment>();var remaining=pieces.ToList();
        while(remaining.Count>0)
        {
            var loop=new List<GeometrySegment>{remaining[0]};remaining.RemoveAt(0);
            while((C.At(loop[^1],1)-C.At(loop[0],0)).Length>Tolerance)
            {
                var end=C.At(loop[^1],1);var next=remaining.Select((s,i)=>(s,i)).Where(n=>(C.At(n.s,0)-end).Length<=Tolerance).ToList();
                if(next.Count!=1)return false;loop.Add(next[0].s);remaining.RemoveAt(next[0].i);if(loop.Count>pieces.Count)return false;
            }
            if(Math.Abs(C.SignedArea(loop))<=1e-10)return false;ordered.AddRange(loop);
        }
        if(CutContourEngine.Loops(ordered).Count==0)return false;contour=ordered;return true;
    }
    static Vector Tangent(GeometrySegment s,double t)
    {
        if(s is LineSegment l)return new(l.X2-l.X1,l.Y2-l.Y1);
        var a=(ArcSegment)s;var angle=(a.StartDegrees+(a.EndDegrees-a.StartDegrees)*t)*Math.PI/180;
        return new Vector(-Math.Sin(angle),Math.Cos(angle))*Math.Sign(a.EndDegrees-a.StartDegrees);
    }
    static bool Same(GeometrySegment a,GeometrySegment b)=>(C.At(a,0)-C.At(b,0)).Length<=Tolerance&&(C.At(a,1)-C.At(b,1)).Length<=Tolerance&&(C.At(a,.5)-C.At(b,.5)).Length<=Tolerance;
}
