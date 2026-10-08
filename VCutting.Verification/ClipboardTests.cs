using System.IO;
using System.Text.Json;
using System.Windows;
using VCutting;

static class ClipboardTests
{
    static void Check(bool ok,string why){if(!ok)throw new InvalidOperationException("clipboard: "+why);}
    static void Eq(double a,double b)=>Check(Math.Abs(a-b)<1e-6,$"{a}!={b}");
    static DesignObject Paste(VCuttingDocument d,string text,int step=1)
    {
        Check(ShapeClipboard.TryPrepare(d,text,step,out var p,out var j),"prepare");return ShapeClipboard.Insert(d,p!,j);
    }
    public static void Run()
    {
        foreach(var name in new[]{"Circle","Ellipse","Semicircle","QuarterCircle","Triangle","Rectangle","Diamond","Parallelogram","Polygon","RegularPolygon","RegularStar"})
        {
            var d=new VCuttingDocument();var source=d.AddHole(name,150,150);if(name=="Rectangle")Check(d.TryUpdateRectangle(source,new(150,150,30,20,45)),"rotated fixture");
            var events=0;d.Changed+=(_,_)=>events++;Check(ShapeClipboard.TryCapture(d,source,out var text)&&events==0,"copy clean "+name);var geometry=source.Geometry.ToArray();
            var history=new UndoRedoManager();history.Record(d);var pasted=(CutOperation)Paste(d,text);
            Check(pasted.Id!=source.Id&&pasted.Sequence!=source.Sequence&&pasted.Shape==source.Shape,"identity/type "+name);Check(source.Geometry.SequenceEqual(geometry)&&!ReferenceEquals(source.Geometry,pasted.Geometry),"source independent "+name);
            Eq(source.Width,pasted.Width);Eq(source.Height,pasted.Height);Eq(source.Rotation,pasted.Rotation);Eq(source.CenterX+1,pasted.CenterX);Eq(source.CenterY-1,pasted.CenterY);
            Check(d.InnerContours.Single(c=>c.Id==pasted.Id).Segments.SequenceEqual(pasted.Geometry),"inner cache");var undo=history.Undo(d)!;Check(undo.Cuts.Count==1,"one undo");var redo=history.Redo(undo)!;Check(redo.Cuts.Count==2&&redo.Cuts[1].Geometry.SequenceEqual(pasted.Geometry),"redo");
            Check(ShapeClipboard.TryCapture(d,pasted,out var secondGeneration)&&ShapeClipboard.TryPrepare(d,secondGeneration,1,out _,out _),"copy pasted shape "+name);
            var again=(CutOperation)Paste(d,text,2);Eq(source.CenterX+2,again.CenterX);Check(d.Cuts.Select(c=>c.Id).Distinct().Count()==3,"repeat ID");
            var path=Path.Combine(Path.GetTempPath(),"clipboard-"+Guid.NewGuid()+".dxf");try{VCuttingDxfSerializer.Save(d,path);var load=VCuttingDxfSerializer.Load(path);Check(load.Cuts.Count==3&&load.Cuts[1].Geometry.SequenceEqual(pasted.Geometry),"DXF "+name);}finally{File.Delete(path);}
            source.Geometry.Clear();d.DeleteObject(source);var afterDelete=(CutOperation)Paste(d,text);Check(afterDelete.Geometry.Count>0,"snapshot persists "+name);
        }
        var origin=new VCuttingDocument();var r=origin.AddHole("Rectangle",100,100);origin.UpdateCut(r,100,100,20,10,7);origin.MicroJoints.Add(new(){Id="MJ001",ParentContourId=r.Id,Position=4,Width=.2});
        Check(ShapeClipboard.TryCapture(origin,new GeometryObject{ParentContourId=r.Id,Geometry=r.Geometry[0]},out var payload),"child copies whole");
        var dest=new VCuttingDocument();dest.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);var converted=(CutOperation)Paste(dest,payload);Eq(200,converted.Width);Eq(100,converted.Height);Eq(1010,converted.CenterX);Eq(990,converted.CenterY);Eq(40,dest.MicroJoints.Single().Position);Eq(2,dest.MicroJoints.Single().Width);Check(dest.MicroJoints[0].ParentContourId==converted.Id,"joint parent remap");Paste(dest,payload,2);Check(dest.MicroJoints.Select(j=>j.Id).Distinct().Count()==2,"joint IDs");
        foreach(var unit in new[]{MeasurementUnit.Centimeter,MeasurementUnit.Meter}){var target=new VCuttingDocument();target.ChangeUnit(unit,UnitChangeMode.PreservePhysicalSize);var c=(CutOperation)Paste(target,payload);Eq(.2,c.Width*unit.Metres());}
        var slit=origin.AddSlit("Arc",[new ArcSegment(120,120,15,350,370)])!;Check(ShapeClipboard.TryCapture(origin,slit,out var arcText),"ARC capture");var arcCopy=(SlitOperation)Paste(origin,arcText);var a=(ArcSegment)arcCopy.Geometry[0];Eq(350,a.StartDegrees);Eq(370,a.EndDegrees);Eq(121,a.Cx);Check(origin.Cuts.Count==1,"slit not filled");
        var line=origin.AddSlit("Polyline",[new LineSegment(30,30,40,30),new LineSegment(40,30,40,40)])!;Check(ShapeClipboard.TryCapture(origin,line,out var lineText),"polyline copy");Check(((SlitOperation)Paste(origin,lineText)).Geometry.Count==2,"polyline whole");
        Check(ShapeClipboard.TryCapture(origin,new GeometryObject{Geometry=origin.OuterContour.Segments[0]},out var outerText),"outer segment copy");var outer=origin.OuterContour.Segments.ToArray();Paste(origin,outerText);Check(origin.OuterContour.Segments.SequenceEqual(outer),"outer not mutated");
        var compound=new VCuttingDocument();var c1=compound.AddHole("Circle",140,150);compound.UpdateCut(c1,140,150,40,40,7);var c2=compound.AddHole("Circle",160,150);compound.UpdateCut(c2,160,150,40,40,7);Check(compound.TryMergeInternalCuts(c1),"compound fixture");Check(ShapeClipboard.TryCapture(compound,c1,out var union),"compound copy");var cp=(CutOperation)Paste(compound,union);Check(cp.Shape=="Compound"&&cp.Geometry.OfType<ArcSegment>().Any(),"exact compound arcs");Check(BentSurfaceEngine.Build(compound).Triangles.Count>0,"compound mesh");
        var tiny=new VCuttingDocument();tiny.ConfigureNew(10,10,.2,MeasurementUnit.Centimeter);var before=DocumentSnapshot.Capture(tiny);Check(!ShapeClipboard.TryPrepare(tiny,payload,1,out _,out _)&&DocumentSnapshot.Capture(tiny).Cuts.Count==before.Cuts.Count,"oversize atomic");
        Check(ShapeClipboard.TryPrepare(origin,payload,100,out var edge,out _),"edge clamp");Check(((CutOperation)edge!).CenterX+((CutOperation)edge).Width/2<=origin.Material.Width,"clamped bounds");
        Check(!ShapeClipboard.TryCapture(origin,origin.Material,out _)&&!ShapeClipboard.TryCapture(origin,origin.OuterContour,out _),"nondrawable selection");
        foreach(var invalid in new string?[]{null,"","hello","{",new('x',ShapeClipboard.MaxLength+1),payload.Replace("\"Version\":1","\"Version\":2"),payload.Replace("\"Unit\":1","\"Unit\":99"),payload.Replace("\"Geometry\":[","\"Geometry\":null,\"Ignored\":["),payload.Replace("\"Width\":20","\"Width\":-20")})Check(!ShapeClipboard.TryPrepare(origin,invalid,1,out _,out _),"bad clipboard");
        Check(ShapeClipboard.TryRead(payload,out var dto),"valid JSON");Check(!ShapeClipboard.TryRead(JsonSerializer.Serialize(dto! with {Geometry=[new("LINE",0,0,0,0,0)]}),out _),"degenerate");Check(!ShapeClipboard.TryRead(JsonSerializer.Serialize(dto! with {Rotation=55}),out _),"parametric mismatch");
        Check(!ShapeClipboard.TryRead(JsonSerializer.Serialize(dto! with {Geometry=[new("CIRCLE",1,1,0,0,-5)]}),out _),"invalid radius");Check(!ShapeClipboard.TryRead(JsonSerializer.Serialize(dto! with {Joints=null!}),out _),"null joints");
        r.Sequence=99;var ordered=(CutOperation)Paste(origin,payload);Check(ordered.Sequence>99,"sequence remains unique after reordering");
        foreach(var (name,segments) in new (string,GeometrySegment[])[]{("Imported Circle",[new CircleSegment(50,60,10)]),("Imported Contour",[new LineSegment(50,50,70,50),new LineSegment(70,50,70,60),new LineSegment(70,60,50,60),new LineSegment(50,60,50,50)]),("Imported ARC",[new ArcSegment(50,60,10,0,90)]),("Imported LINE",[new LineSegment(50,60,70,80)])})
        {
            var imported=new CutOperation{Id="I001",Shape=name};imported.Geometry.AddRange(segments);var doc=new VCuttingDocument();doc.Cuts.Add(imported);
            Check(ShapeClipboard.TryCapture(doc,imported,out var importedText),"imported capture "+name);var copy=Paste(doc,importedText);
            Check(copy is SlitOperation==(name is "Imported ARC" or "Imported LINE"),"imported open/closed classification");
            if(copy is CutOperation closed){Check(closed.Shape is "Circle" or "Compound","imported closed editable");Check(closed.CenterX>40&&closed.CenterY>40,"derived imported bounds");}
            Check(imported.Geometry.SequenceEqual(segments),"imported source preserved");
        }
    }
}
