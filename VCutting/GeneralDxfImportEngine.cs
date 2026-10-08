using VCutting.Viewer;
using System.IO;

namespace VCutting;

public sealed record DxfImportResult(VCuttingDocument Document,int OuterEntities,int CutObjects,int BendObjects,IReadOnlyList<string> Warnings);

public static class GeneralDxfImportEngine
{
    const double Tolerance=.001;
    public static DxfImportResult Import(string path,double defaultThickness=.2)
    {
        var source=DxfDocumentParser.Parse(path);
        var l=source.Entities.Where(e=>Layer(e.LayerName)=="L"&&!e.IsDuplicate).ToList();
        if(l.Count==0)throw new InvalidDataException(Localization.Text("ui.0013"));
        var bounds=new DxfBounds();foreach(var e in l)e.ExpandBounds(bounds);var unitScale=source.Header.InsUnits=="5"?1d:.1d;
        if(bounds.IsEmpty||bounds.Width<=Tolerance||bounds.Height<=Tolerance)throw new InvalidDataException(Localization.Text("ui.0014"));
        GeometrySegment Convert(GeometryEntity e)=>e switch{
            GeometryLine x=>new LineSegment((x.Start.X-bounds.MinX)*unitScale,(x.Start.Y-bounds.MinY)*unitScale,(x.End.X-bounds.MinX)*unitScale,(x.End.Y-bounds.MinY)*unitScale),
            GeometryCircle x=>new CircleSegment((x.Center.X-bounds.MinX)*unitScale,(x.Center.Y-bounds.MinY)*unitScale,x.Radius*unitScale),
            GeometryArc x=>new ArcSegment((x.Center.X-bounds.MinX)*unitScale,(x.Center.Y-bounds.MinY)*unitScale,x.Radius*unitScale,x.StartAngle,x.EndAngle),
            _=>throw new NotSupportedException(e.EntityType)};
        var chains=BuildClosedChains(l.OfType<GeometryLine>().ToList());
        var outerChain=chains.OrderByDescending(Area).FirstOrDefault();
        var warnings=new List<string>();List<GeometrySegment> outer;
        if(outerChain is null){warnings.Add(Localization.Text("ui.0015"));outer=[new LineSegment(0,0,bounds.Width,0),new LineSegment(bounds.Width,0,bounds.Width,bounds.Height),new LineSegment(bounds.Width,bounds.Height,0,bounds.Height),new LineSegment(0,bounds.Height,0,0)];}
        else outer=outerChain.Select(Convert).ToList();
        var outerIds=outerChain?.Select(x=>x.Id).ToHashSet()??[];
        var cuts=new List<CutOperation>();var sequence=1;
        foreach(var circle in l.OfType<GeometryCircle>())AddCut([Convert(circle)],"Imported Circle");
        foreach(var chain in chains.Where(x=>!ReferenceEquals(x,outerChain)))AddCut(chain.Select(Convert),"Imported Contour");
        foreach(var entity in l.Where(e=>!outerIds.Contains(e.Id)&&e is not GeometryCircle&&!(e is GeometryLine line&&chains.Any(c=>c.Contains(line)))))AddCut([Convert(entity)],$"Imported {entity.EntityType}");
        void AddCut(IEnumerable<GeometrySegment> geometry,string shape){var c=new CutOperation{Id=$"I{sequence:000}",Sequence=sequence++,Kind=CutKind.Hole,Shape=shape};c.Geometry.AddRange(geometry);cuts.Add(c);}
        var bends=new List<(SectionAxis,double,BendDirection)>();
        foreach(var line in source.Entities.OfType<GeometryLine>().Where(x=>Layer(x.LayerName) is "V" or "V1"&&!x.IsDuplicate)){
            var layer=Layer(line.LayerName);var dx=Math.Abs(line.End.X-line.Start.X);var dy=Math.Abs(line.End.Y-line.Start.Y);
            if(dx<=Tolerance&&dy>Tolerance)bends.Add((SectionAxis.W,((line.Start.X+line.End.X)/2-bounds.MinX)*unitScale,layer=="V"?BendDirection.Up:BendDirection.Down));
            else if(dy<=Tolerance&&dx>Tolerance)bends.Add((SectionAxis.H,((line.Start.Y+line.End.Y)/2-bounds.MinY)*unitScale,layer=="V"?BendDirection.Left:BendDirection.Right));
            else warnings.Add(Localization.Format("ui.0016", line.LayerName, line.Id));
        }
        var document=new VCuttingDocument();document.ApplyImportedGeometry(bounds.Width*unitScale,bounds.Height*unitScale,outer,cuts,bends,defaultThickness);
        if(source.UnsupportedCounts.Count>0)warnings.Add(Localization.Text("ui.0017")+string.Join(", ",source.UnsupportedCounts.Select(x=>$"{x.Key} {x.Value}")));
        return new(document,outer.Count,cuts.Count,bends.Count,warnings);
    }
    static string Layer(string value)=>value.Trim().Trim('-').Replace(" ","").ToUpperInvariant();
    static List<List<GeometryLine>> BuildClosedChains(List<GeometryLine> remaining)
    {
        var output=new List<List<GeometryLine>>();var pool=new List<GeometryLine>(remaining);
        while(pool.Count>0){var chain=new List<GeometryLine>{pool[0]};pool.RemoveAt(0);var start=chain[0].Start;var end=chain[0].End;
            while(!Near(start,end)){var index=pool.FindIndex(x=>Near(x.Start,end)||Near(x.End,end));if(index<0)break;var next=pool[index];pool.RemoveAt(index);if(Near(next.End,end))next=new GeometryLine{Id=next.Id,EntityType=next.EntityType,LayerName=next.LayerName,OriginalEntity=next.OriginalEntity,Start=next.End,End=next.Start};chain.Add(next);end=next.End;}
            if(chain.Count>=3&&Near(start,end))output.Add(chain);
        }return output;
    }
    static bool Near(DxfPoint a,DxfPoint b)=>Math.Abs(a.X-b.X)<=Tolerance&&Math.Abs(a.Y-b.Y)<=Tolerance;
    static double Area(List<GeometryLine> chain)=>Math.Abs(chain.Sum(x=>x.Start.X*x.End.Y-x.End.X*x.Start.Y))/2;
}
