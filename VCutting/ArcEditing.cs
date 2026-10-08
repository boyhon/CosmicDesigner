using System.Windows;
using C=VCutting.CurvedBoundaryCutEngine;

namespace VCutting;
public static class ArcEditing
{
    public static bool TryBuild(IReadOnlyList<GeometrySegment> source,ArcSegment arc,double start,double end,bool closed,double width,double height,out ArcSegment updated,out IReadOnlyList<GeometrySegment> result)
    {
        updated=arc;result=[];var sweep=end-start;
        if(!double.IsFinite(arc.Cx)||!double.IsFinite(arc.Cy)||!double.IsFinite(arc.Radius)||arc.Radius<=0||!double.IsFinite(start)||!double.IsFinite(end)||!double.IsFinite(sweep)||Math.Abs(sweep)<=1e-7||Math.Abs(sweep)>=360||!source.Contains(arc))return false;
        var value=arc with {StartDegrees=RegularPolygonParameters.Normalize(start),EndDegrees=RegularPolygonParameters.Normalize(start)+sweep};
        var loops=closed?CutContourEngine.Loops(source):new IReadOnlyList<GeometrySegment>[] {source};if(loops.Count==0)return false;
        var output=new List<GeometrySegment>();
        foreach(var loop in loops)
        {
            var index=loop.ToList().IndexOf(arc);if(index<0){output.AddRange(loop);continue;}
            var segments=loop.ToList();var before=index>0?index-1:closed?segments.Count-1:-1;var after=index+1<segments.Count?index+1:closed?0:-1;
            var a=C.At(value,0);var b=C.At(value,1);
            if(before>=0&&segments[before] is LineSegment prev)segments[before]=prev with {X2=a.X,Y2=a.Y};
            if(after>=0&&segments[after] is LineSegment next)segments[after]=next with {X1=b.X,Y1=b.Y};
            for(var i=0;i<segments.Count;i++)
            {
                if(i!=index){output.Add(segments[i]);continue;}
                if(before>=0&&segments[before] is not LineSegment){var p=C.At(segments[before],1);if((a-p).Length>1e-8)output.Add(new LineSegment(p.X,p.Y,a.X,a.Y));}
                output.Add(value);
                if(after>=0&&segments[after] is not LineSegment){var p=C.At(segments[after],0);if((p-b).Length>1e-8)output.Add(new LineSegment(b.X,b.Y,p.X,p.Y));}
            }
        }
        if(output.Any(g=>g is LineSegment l&&new Vector(l.X2-l.X1,l.Y2-l.Y1).Length<1e-7))return false;
        if(CutContourEngine.Extrema(output).Any(p=>!double.IsFinite(p.X)||!double.IsFinite(p.Y)||p.X< -1e-8||p.Y< -1e-8||p.X>width+1e-8||p.Y>height+1e-8))return false;
        if(closed)
        {
            var newLoops=CutContourEngine.Loops(output);if(newLoops.Count!=loops.Count)return false;
            for(var i=0;i<loops.Count;i++)if(Math.Sign(C.SignedArea(newLoops[i]))!=Math.Sign(C.SignedArea(loops[i])))return false;
            foreach(var loop in newLoops)for(var i=0;i<loop.Count;i++)for(var j=i+1;j<loop.Count;j++)
            {
                var ts=new List<double>();var us=new List<double>();C.AllIntersections(loop[i],loop[j],ts,us);
                if(j==i+1){if(ts.Any(t=>Math.Abs(t-1)>1e-6)||us.Any(t=>Math.Abs(t)>1e-6))return false;}
                else if(i==0&&j==loop.Count-1){if(ts.Any(t=>Math.Abs(t)>1e-6)||us.Any(t=>Math.Abs(t-1)>1e-6))return false;}
                else if(ts.Count>0)return false;
            }
            for(var i=0;i<newLoops.Count;i++)for(var j=i+1;j<newLoops.Count;j++)foreach(var first in newLoops[i])foreach(var second in newLoops[j]){var ts=new List<double>();var us=new List<double>();C.AllIntersections(first,second,ts,us);if(ts.Count>0)return false;}
        }
        updated=value;result=output;return true;
    }
}
public sealed partial class VCuttingDocument
{
    public bool CanEditArc(string parentId,ArcSegment arc,double start,double end,out ArcSegment updated,out IReadOnlyList<GeometrySegment> result)
    {
        IReadOnlyList<GeometrySegment>? source=parentId=="OUTER"?OuterContour.Segments:Cuts.FirstOrDefault(c=>c.Id==parentId)?.Geometry??Slits.FirstOrDefault(s=>s.Id==parentId)?.Geometry;
        updated=arc;result=[];return source is not null&&ArcEditing.TryBuild(source,arc,start,end,parentId=="OUTER"||Cuts.Any(c=>c.Id==parentId),Material.Width,Material.Height,out updated,out result);
    }
    public bool TryEditArc(string parentId,ArcSegment arc,double start,double end,out ArcSegment updated)
    {
        if(!CanEditArc(parentId,arc,start,end,out updated,out var result))return false;if(arc==updated)return true;
        if(parentId=="OUTER"){OuterContour.Segments.Clear();OuterContour.Segments.AddRange(result);_preserveOuterContour=true;Recalculate();}
        else if(Cuts.FirstOrDefault(c=>c.Id==parentId) is CutOperation cut){cut.Shape="Compound";ApplyCutContour(cut,result);}
        else if(Slits.FirstOrDefault(s=>s.Id==parentId) is SlitOperation slit){slit.Geometry.Clear();slit.Geometry.AddRange(result);Recalculate();}
        return true;
    }
}
