using System.IO;
using System.Windows;
using VCutting;

static class AngleEditingTests
{
    static void Check(bool ok,string why){if(!ok)throw new InvalidOperationException(why);}
    static void Eq(double a,double b)=>Check(Math.Abs(a-b)<1e-6,$"angle edit {a} != {b}");
    public static void Arcs()
    {
        var d=new VCuttingDocument();var slit=d.AddSlit("Arc",[new ArcSegment(150,150,20,350,370)])!;var arc=(ArcSegment)slit.Geometry[0];var history=new UndoRedoManager();history.Record(d);
        Check(d.TryEditArc(slit.Id,arc,350,400,out var updated),"independent slit start/end edit");Eq(350,updated.StartDegrees);Eq(400,updated.EndDegrees);Eq(20,updated.Radius);Eq(150,updated.Cx);Check(d.Cuts.Count==0&&slit.Geometry.Count==1,"slit never closed or filled");
        Check(((ArcSegment)history.Undo(d)!.Slits.Single().Geometry[0]).EndDegrees==370,"arc undo");Check(((ArcSegment)history.Redo(new VCuttingDocument())!.Slits.Single().Geometry[0]).EndDegrees==400,"arc redo");
        var events=0;d.Changed+=(_,_)=>events++;Check(d.TryEditArc(slit.Id,updated,350,400,out _)&&events==0,"arc no-op clean");
        foreach(var (start,end) in new[]{(0d,0d),(0d,360d),(double.NaN,30d),(0d,double.PositiveInfinity)})Check(!d.TryEditArc(slit.Id,updated,start,end,out _)&&events==0,"bad angle atomic rejection");
        Check(d.TryEditArc(slit.Id,updated,20,-40,out var clockwise)&&clockwise.EndDegrees-clockwise.StartDegrees== -60,"clockwise signed sweep");
        var outside=new VCuttingDocument();var small=outside.AddSlit("Arc",[new ArcSegment(5,150,10,-90,90)])!;Check(!outside.TryEditArc(small.Id,(ArcSegment)small.Geometry[0],90,270,out _),"arc cardinal outside bounds rejected");
        var outer=new VCuttingDocument();Check(outer.TryFilletContourCorner("OUTER",0,10,out var fillet),"outer arc fixture");var sweep=fillet!.EndDegrees-fillet.StartDegrees;Check(outer.TryEditArc("OUTER",fillet,fillet.StartDegrees+Math.Sign(sweep)*5,fillet.EndDegrees-Math.Sign(sweep)*5,out var outerArc),"closed outer angle edit");Check(CutContourEngine.Loops(outer.OuterContour.Segments).Count==1,"outer remains closed");Eq(fillet.Radius,outerArc.Radius);
        var hole=new VCuttingDocument();var c=hole.AddHole("Rectangle",150,150);hole.UpdateCut(c,150,150,60,50,7);Check(hole.TryFilletContourCorner(c.Id,0,5,out var innerArc),"inner arc fixture");sweep=innerArc!.EndDegrees-innerArc.StartDegrees;Check(hole.TryEditArc(c.Id,innerArc,innerArc.StartDegrees+Math.Sign(sweep)*5,innerArc.EndDegrees-Math.Sign(sweep)*5,out _),"closed inner arc edit");Check(c.Shape=="Compound"&&CutContourEngine.Loops(c.Geometry).Count==1&&hole.InnerContours.Single().Segments.SequenceEqual(c.Geometry),"compound cache closure");
        var touching=new ArcSegment(150,150,20,0,180);var neighbor=new ArcSegment(150,150,20,180,360);Check(ArcEditing.TryBuild([touching,neighbor],touching,10,170,true,300,300,out _,out var joined)&&joined.Count==4&&joined.Count(g=>g is LineSegment)==2,"adjacent arcs retain original neighbor and reconnect explicitly");Check(joined.Contains(neighbor)&&CutContourEngine.Loops(joined).Count==1,"ARC neighbor unchanged and loop valid");
        var path=Path.Combine(Path.GetTempPath(),"arc-angles-"+Guid.NewGuid()+".dxf");try{VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path);Check(loaded.Slits[0].Geometry.SequenceEqual(slit.Geometry),"signed arc metadata roundtrip");Check(File.ReadAllText(path).Contains("ARC"),"native ARC export");}finally{File.Delete(path);}
        d.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);var scaled=(ArcSegment)d.Slits[0].Geometry[0];Eq(clockwise.Radius*10,scaled.Radius);Eq(clockwise.StartDegrees,scaled.StartDegrees);Eq(clockwise.EndDegrees,scaled.EndDegrees);Check(BentSurfaceEngine.Build(outer).Triangles.Count>0&&BentSurfaceEngine.Build(hole).Triangles.Count>0,"edited ARC 3D builds");
    }
    public static void Rectangles()
    {
        var d=new VCuttingDocument();var c=d.AddHole("Rectangle",150,150);d.UpdateCut(c,150,150,60,30,7);var p=RotatedRectangleGeometry.Parameters(c);var history=new UndoRedoManager();history.Record(d);Check(d.TryUpdateRectangle(c,p with {Rotation=45}),"rectangle rotate");Eq(60,c.Width);Eq(30,c.Height);Eq(45,c.Rotation);
        var vertices=RotatedRectangleGeometry.Vertices(RotatedRectangleGeometry.Parameters(c));Eq(60,(vertices[1]-vertices[0]).Length);Eq(30,(vertices[2]-vertices[1]).Length);Eq(0,Vector.Multiply(vertices[1]-vertices[0],vertices[2]-vertices[1]));Check(RotatedRectangleGeometry.IsParametric(c)&&d.InnerContours.Single().Segments.SequenceEqual(c.Geometry),"rectangle caches sync");
        Check(history.Undo(d)!.Cuts[0].Rotation==0,"rotation undo");Check(history.Redo(new VCuttingDocument())!.Cuts[0].Rotation==45,"rotation redo");
        var events=0;d.Changed+=(_,_)=>events++;Check(d.TryUpdateRectangle(c,RotatedRectangleGeometry.Parameters(c))&&events==0,"rectangle no-op clean");var before=c.Geometry.ToList();foreach(var bad in new[]{p with {Rotation=double.NaN},p with {Width=0},p with {Height=-1},p with {CenterX=0},p with {Width=1000}})Check(!d.TryUpdateRectangle(c,bad),"invalid rotation/dimension/bounds reject");Check(events==0&&before.SequenceEqual(c.Geometry),"invalid no mutation");
        d.UpdateCut(c,160,140,70,40,7);Eq(45,c.Rotation);Eq(70,c.Width);Eq(40,c.Height);Eq(160,c.CenterX);Check(d.TryUpdateRectangle(c,RotatedRectangleGeometry.Parameters(c) with {Rotation=-30}),"normalized negative angle");Eq(330,c.Rotation);
        var path=Path.Combine(Path.GetTempPath(),"rectangle-rotation-"+Guid.NewGuid()+".dxf");try{VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path).Cuts.Single();Check(RotatedRectangleGeometry.Parameters(c)==RotatedRectangleGeometry.Parameters(loaded)&&c.Geometry.SequenceEqual(loaded.Geometry),"rectangle metadata geometry roundtrip");}finally{File.Delete(path);}
        var snapshot=DocumentSnapshot.Capture(d);var restored=snapshot.Restore();Eq(c.Rotation,restored.Cuts[0].Rotation);Check(c.Geometry.SequenceEqual(restored.Cuts[0].Geometry),"rotation snapshot");d.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);Eq(700,c.Width);Eq(400,c.Height);Eq(330,c.Rotation);Check(RotatedRectangleGeometry.IsParametric(c),"units preserve rotated shape");
        var mask=FlatMaterialMaskEngine.Build(restored,new(1,new(0,300)));Check(!mask.FillContains(new Point(160,160)),"rotated flat mask center empty");Check(BentSurfaceEngine.Build(restored).Triangles.Count>0,"rotated mesh");
        var merge=new VCuttingDocument();var mc=merge.AddHole("Rectangle",150,150);merge.UpdateCut(mc,150,150,40,30,7);merge.TryUpdateRectangle(mc,RotatedRectangleGeometry.Parameters(mc) with {Rotation=35});merge.AddHole("Circle",150,150);Check(merge.TryMergeInternalCuts(mc)&&mc.Shape=="Compound","rotated internal merge");
        var fillet=new VCuttingDocument();var fc=fillet.AddHole("Rectangle",150,150);fillet.UpdateCut(fc,150,150,40,30,7);fillet.TryUpdateRectangle(fc,RotatedRectangleGeometry.Parameters(fc) with {Rotation=25});Check(fillet.TryFilletContourCorner(fc.Id,0,3,out _)&&fc.Shape=="Compound","rotated fillet conversion");
        var boundary=new VCuttingDocument();var bc=boundary.AddHole("Rectangle",30,150);boundary.UpdateCut(bc,30,150,40,40,7);boundary.TryUpdateRectangle(bc,RotatedRectangleGeometry.Parameters(bc) with {Rotation=45});var target=RotatedRectangleGeometry.Parameters(bc);target=target with {CenterX=RotatedRectangleGeometry.Bounds(bc).Width/2};Check(boundary.TryUpdateRectangle(bc,target),"touching rotation fits");Check(boundary.CanMergeBoundaryCut(bc)&&boundary.TryMergeBoundaryCut(bc),"rotated outer merge");
        LegacyRectangles();HitRotatedRectangle();
    }
    static void LegacyRectangles()
    {
        var d=new VCuttingDocument();var c=d.AddHole("Rectangle",150,150);var path=Path.Combine(Path.GetTempPath(),"legacy-rectangle-"+Guid.NewGuid()+".dxf");
        try
        {
            VCuttingDxfSerializer.Save(d,path);File.WriteAllText(path,File.ReadAllText(path).Replace(",\"Rotation\":0", ""));
            var loaded=VCuttingDxfSerializer.Load(path).Cuts.Single();Check(loaded.Rotation==0&&c.Geometry.SequenceEqual(loaded.Geometry)&&RotatedRectangleGeometry.IsParametric(loaded),"legacy missing rotation retains axis-aligned shape");
            Check(d.TryFilletContourCorner(c.Id,0,1,out _),"legacy rounded fixture");c.Shape="Rectangle";
            VCuttingDxfSerializer.Save(d,path);loaded=VCuttingDxfSerializer.Load(path).Cuts.Single();Check(c.Geometry.SequenceEqual(loaded.Geometry)&&loaded.Geometry.Any(g=>g is ArcSegment),"legacy modified rectangle cache not rebuilt as plain rectangle");
            Check(DocumentSnapshot.Capture(d).Restore().Cuts[0].Geometry.SequenceEqual(c.Geometry),"legacy modified rectangle history preserved");
        }finally{File.Delete(path);}
    }
    static void HitRotatedRectangle()
    {
        Exception? error=null;var thread=new Thread(()=>{try{var d=new VCuttingDocument();var c=d.AddHole("Rectangle",150,150);d.UpdateCut(c,150,150,80,20,7);d.TryUpdateRectangle(c,RotatedRectangleGeometry.Parameters(c) with {Rotation=45});var view=new FlatDesignerView{Document=d};view.Measure(new Size(900,700));view.Arrange(new Rect(0,0,900,700));var transform=FlatViewportEngine.Calculate(900,700,300,300,true,1,new Vector());var method=typeof(FlatDesignerView).GetMethod("Hit",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)!;object?[] center=[transform.ToScreen(new(150,150)),null];Check(ReferenceEquals(method.Invoke(view,center),c),"rotated body selects");object?[] empty=[transform.ToScreen(new(178,122)),null];Check(method.Invoke(view,empty) is null,"bounding-box empty corner not selected");}catch(Exception e){error=e;}});thread.SetApartmentState(ApartmentState.STA);thread.Start();thread.Join();if(error is not null)throw new InvalidOperationException("rotated hit test",error);
    }
}
