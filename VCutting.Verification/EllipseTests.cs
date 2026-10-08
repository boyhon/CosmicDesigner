using System.IO;
using System.Windows;
using System.Xml.Linq;
using VCutting;
using Localization = VCutting.Localization;

static class EllipseTests
{
    static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
    static void Eq(double a,double b)=>Check(Math.Abs(a-b)<1e-7,$"ellipse {a} != {b}");
    public static void Run()
    {
        foreach(var delta in new[]{new Vector(60,0),new Vector(0,60),new Vector(60,40),new Vector(-60,40),new Vector(-60,-40),new Vector(60,-40)})
        {
            var start=new Point(150,150);var end=start+delta;
            Check(HoleDragEngine.TryCalculate("Ellipse",start,end,300,300,out var g),"all ellipse directions");
            var pts=g.ContourVertices!;Check(pts.Count==48,"48 ellipse points");
            Eq((start.X+end.X)/2,g.CenterX);Eq((start.Y+end.Y)/2,g.CenterY);
            Eq(end.X,pts[0].X);Eq(end.Y,pts[0].Y);Eq(start.X,pts[24].X);Eq(start.Y,pts[24].Y);
            var major=(pts[0]-new Point(g.CenterX,g.CenterY)).Length;
            var minor=(pts[12]-new Point(g.CenterX,g.CenterY)).Length;Eq(2,major/minor);
            Eq(0,Vector.Multiply(pts[0]-new Point(g.CenterX,g.CenterY),pts[12]-new Point(g.CenterX,g.CenterY)));
            var preview=HoleDragEngine.CreatePreview("Ellipse",g);
            var d=new VCuttingDocument();var history=new UndoRedoManager();history.Record(d);
            var cut=d.AddHole("Ellipse",g.CenterX,g.CenterY);Check(d.ApplyEllipseContour(cut,pts),"create ellipse");
            Check(cut.Geometry.SequenceEqual(preview.Geometry)&&d.InnerContours.Single().Segments.SequenceEqual(cut.Geometry),"preview/create/inner exact");
            var restored=history.Undo(d)!;Check(restored.Cuts.Count==0,"undo single creation");
            Check(history.Redo(restored)!.Cuts.Single().Geometry.SequenceEqual(cut.Geometry),"redo tilt");
            var before=cut.Geometry.Cast<LineSegment>().ToList();d.UpdateCut(cut,cut.CenterX-10,cut.CenterY-5,cut.Width,cut.Height,cut.Sides);
            for(int i=0;i<48;i++){var line=(LineSegment)cut.Geometry[i];Eq(before[i].X1-10,line.X1);Eq(before[i].Y1-5,line.Y1);}
            var center=new Point(cut.CenterX,cut.CenterY);var oldW=cut.Width;var oldH=cut.Height;before=cut.Geometry.Cast<LineSegment>().ToList();
            d.UpdateCut(cut,center.X,center.Y,oldW*1.2,oldH*.8,cut.Sides);
            for(int i=0;i<48;i++){var line=(LineSegment)cut.Geometry[i];Eq(center.X+(before[i].X1-center.X)*1.2,line.X1);Eq(center.Y+(before[i].Y1-center.Y)*.8,line.Y1);}
            d.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);d.ChangeUnit(MeasurementUnit.Centimeter,UnitChangeMode.PreservePhysicalSize);
            Check(d.InnerContours.Single().Segments.SequenceEqual(cut.Geometry),"unit contour sync");
            var path=Path.Combine(Path.GetTempPath(),"ellipse-"+Guid.NewGuid().ToString("N")+".dxf");
            try{VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path);Check(loaded.Cuts.Single().Geometry.SequenceEqual(cut.Geometry),"DXF tilt roundtrip");Check(File.ReadAllText(path).Contains("\r\nLINE\r\n8\r\nL\r\n"),"manufacturing LINE export");}
            finally{File.Delete(path);}
            var t=new FlatViewportTransform(1,new(0,300));var mask=FlatMaterialMaskEngine.Build(d,t);
            Check(!mask.FillContains(t.ToScreen(center))&&mask.FillContains(t.ToScreen(new(10,10))),"flat material mask");
        }
        foreach(var pair in new[]{(new Point(2,150),new Point(30,290)),(new Point(10,290),new Point(290,299)),(new Point(-40,20),new Point(200,350))})
        {Check(HoleDragEngine.TryCalculate("Ellipse",pair.Item1,pair.Item2,300,300,out var g),"boundary calculation");Check(g.ContourVertices!.All(p=>p.X>=0&&p.Y>=0&&p.X<=300&&p.Y<=300),"bounds preserved");}
        Check(!HoleDragEngine.TryCalculate("Ellipse",new(0,0),new(100,0),300,300,out _),"boundary zero thickness rejected");
        Check(!HoleDragEngine.TryCalculate("Ellipse",new(10,10),new(10.01,10),300,300,out _),"tiny click rejected");
        Check(!HoleDragEngine.TryCalculate("Ellipse",new(double.NaN,10),new(20,30),300,300,out _),"nonfinite rejected");
        var legacy=new VCuttingDocument();var lc=legacy.AddHole("Ellipse",150,150);legacy.UpdateCut(lc,150,150,60,30,7);Eq(60,lc.Width);Eq(30,lc.Height);Check(lc.Geometry.Count==48,"legacy editable");
        var mesh=BentSurfaceEngine.Build(legacy);Check(mesh.Triangles.Count>0&&mesh.Edges.Count>=52,"3D contour");
        for(int i=0;i<mesh.Triangles.Count;i+=3){var a=mesh.Points[mesh.Triangles[i]];var b=mesh.Points[mesh.Triangles[i+1]];var c=mesh.Points[mesh.Triangles[i+2]];var x=(a.X+b.X+c.X)/3;var y=(a.Y+b.Y+c.Y)/3;Check(Math.Pow((x-150)/29,2)+Math.Pow((y-150)/14,2)>=1,"3D hole faces absent");}
        var root=new DirectoryInfo(AppContext.BaseDirectory);while(root!=null&&!File.Exists(Path.Combine(root.FullName,"VCutting.sln")))root=root.Parent;
        var xml=XDocument.Load(Path.Combine(root!.FullName,"VCutting","MainWindow.xaml"));var buttons=xml.Descendants().Where(e=>e.Name.LocalName=="Button"&&e.Attribute("Tag")!=null).ToList();
        var circle=buttons.FindIndex(b=>(string?)b.Attribute("Tag")=="Circle");Check((string?)buttons[circle+1].Attribute("Tag")=="Ellipse","ellipse beside circle");Check(buttons[circle+1].Descendants().Any(e=>e.Name.LocalName=="Ellipse"),"outline ellipse icon");
        Localization.SetLanguage("ko");Check(Localization.Text("hole.ellipse")=="타원","Korean tooltip");Localization.SetLanguage("en");Check(Localization.Text("hole.ellipse")=="Ellipse","English tooltip");
    }
}
