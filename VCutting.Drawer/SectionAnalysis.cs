using VCutting.Viewer;
namespace VCutting.Drawer;
public enum SectionAxis { X, Y }
public sealed record SectionProjection(SectionAxis Axis,double Position,SectionModel Model);
public static class SectionAnalyzer
{
 public static SectionProjection Analyze(DrawingDocument source,SectionAxis axis,double position)
 {
  var document=axis==SectionAxis.Y?source.ToDxfDocument():Swap(source);
  return new(axis,position,SectionModel.Create(document,position));
 }
 static DxfDocument Swap(DrawingDocument source)
 {
  var d=new DxfDocument{FilePath=""};
  foreach(var e in source.Entities){GeometryEntity g=e switch{EditorLine l=>new GeometryLine{Id=l.Id,EntityType="LINE",LayerName=l.Layer,OriginalEntity=[],Start=P(l.Start),End=P(l.End)},EditorCircle c=>new GeometryCircle{Id=c.Id,EntityType="CIRCLE",LayerName=c.Layer,OriginalEntity=[],Center=P(c.Center),Radius=c.Radius},EditorArc a=>SwapArc(a),_=>throw new NotSupportedException()};d.Entities.Add(g);d.Layers.Add(g.LayerName);g.ExpandBounds(d.Bounds);}return d;
 }
 static GeometryArc SwapArc(EditorArc a)=>new(){Id=a.Id,EntityType="ARC",LayerName=a.Layer,OriginalEntity=[],Center=P(a.Center),Radius=a.Radius,StartAngle=90-a.EndAngle,EndAngle=90-a.StartAngle};
 static DxfPoint P(DxfPoint p)=>new(p.Y,p.X,p.Z);
}
