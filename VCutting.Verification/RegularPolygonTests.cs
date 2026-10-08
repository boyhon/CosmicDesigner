using System.IO;
using System.Windows;
using VCutting;
using VCutting.Viewer;
using Localization=VCutting.Localization;

static class RegularPolygonTests
{
    static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
    static void Eq(double a,double b)=>Check(Math.Abs(a-b)<1e-6,$"regular polygon {a} != {b}");
    static VCuttingDocument Doc(out CutOperation cut,int sides=6,double radius=20,double angle=0){var d=new VCuttingDocument();cut=d.AddRegularPolygon(new(150,150,sides,radius,angle))!;Check(cut is not null,"regular polygon created");return d;}
    static IReadOnlyList<Point> Points(CutOperation cut)=>RegularPolygonGeometry.Vertices(RegularPolygonGeometry.Parameters(cut));
    public static void Triangle(){var points=RegularPolygonGeometry.Vertices(new(0,0,3,10,0));Check(points.Count==3,"triangle vertices");foreach(var p in points)Eq(10,((Vector)p).Length);}
    public static void Square(){var p=RegularPolygonGeometry.Vertices(new(0,0,4,10,0));var expected=new[]{new Point(10,0),new Point(0,10),new Point(-10,0),new Point(0,-10)};for(var i=0;i<4;i++)Eq(0,(p[i]-expected[i]).Length);}
    public static void EqualEdges(){var p=RegularPolygonGeometry.Vertices(new(0,0,6,20,27));var side=(p[1]-p[0]).Length;for(var i=0;i<p.Count;i++)Eq(side,(p[(i+1)%p.Count]-p[i]).Length);}
    public static void RadiusDistance(){foreach(var sides in new[]{3,4,5,6,8,64,512})foreach(var angle in new[]{0d,45,179,359}){var p=RegularPolygonGeometry.Vertices(new(100,50,sides,10,angle));Check(p.Count==sides,"all side counts");foreach(var v in p)Eq(10,(v-new Point(100,50)).Length);}}
    public static void ChangeSides(){var d=Doc(out var c);var p=RegularPolygonGeometry.Parameters(c);Check(d.TryUpdateRegularPolygon(c,p with {Sides=8}),"side edit");Check(c.Geometry.Count==8&&Points(c).Count==8,"full vertex regeneration");Eq(p.Radius,c.Radius);Eq(p.CenterX,c.CenterX);Eq(p.Rotation,c.Rotation);}
    public static void ChangeRadius(){var d=Doc(out var c);var p=RegularPolygonGeometry.Parameters(c);Check(d.TryUpdateRegularPolygon(c,p with {Radius=40}),"radius edit");foreach(var v in Points(c))Eq(40,(v-new Point(c.CenterX,c.CenterY)).Length);Check(c.Sides==6&&c.Rotation==0,"other radius properties unchanged");}
    public static void ChangeRotation(){var d=Doc(out var c,4,50);Check(d.TryUpdateRegularPolygon(c,RegularPolygonGeometry.Parameters(c) with {Rotation=45}),"rotation edit");Eq(150+50/Math.Sqrt(2),Points(c)[0].X);Eq(150+50/Math.Sqrt(2),Points(c)[0].Y);Eq(50,c.Radius);Check(c.Sides==4,"rotation sides unchanged");}
    public static void ChangeCenter(){var p=new RegularPolygonParameters(0,0,5,20,30);var before=RegularPolygonGeometry.Vertices(p);var after=RegularPolygonGeometry.Vertices(p with {CenterX=100,CenterY=50});for(var i=0;i<5;i++){Eq(100,after[i].X-before[i].X);Eq(50,after[i].Y-before[i].Y);}var d=Doc(out var c,5,20,30);var param=RegularPolygonGeometry.Parameters(c);d.UpdateCut(c,160,140,c.Width,c.Height,c.Sides);Eq(160,c.CenterX);Eq(140,c.CenterY);Eq(param.Radius,c.Radius);Eq(param.Rotation,c.Rotation);}
    public static void Export()
    {
        var d=Doc(out var c,7,20,19);var path=Path.Combine(Path.GetTempPath(),"regular-polygon-"+Guid.NewGuid().ToString("N")+".dxf");
        try{VCuttingDxfSerializer.Save(d,path);var lines=File.ReadAllLines(path);var start=Array.IndexOf(lines,"LWPOLYLINE");Check(start>0,"LWPOLYLINE exported");var pairs=new List<(int Code,string Value)>();for(var i=start+1;i+1<lines.Length;i+=2){if(lines[i]=="0")break;pairs.Add((int.Parse(lines[i]),lines[i+1]));}Check(pairs.Single(p=>p.Code==90).Value=="7"&&pairs.Single(p=>p.Code==70).Value=="1"&&pairs.Single(p=>p.Code==8).Value=="L","DXF count/closed/layer");Check(pairs.Count(p=>p.Code==10)==7&&pairs.Count(p=>p.Code==20)==7,"no repeated last vertex");Check(pairs.Any(p=>p.Code==100&&p.Value=="AcDbPolyline"),"DXF subclass");var loaded=VCuttingDxfSerializer.Load(path);Check(RegularPolygonGeometry.Parameters(c)==RegularPolygonGeometry.Parameters(loaded.Cuts.Single()),"parameter metadata roundtrip");var parsed=DxfDocumentParser.Parse(path);Check(parsed.UnsupportedCounts.Count==0&&parsed.EntityCounts["LWPOLYLINE"]==1,"plain parser supports output");var imported=GeneralDxfImportEngine.Import(path).Document;Check(imported.Cuts.Count==1&&imported.Cuts[0].Shape=="Imported Contour"&&imported.Cuts[0].Geometry.Count==7,"generic import never infers regular");}
        finally{File.Delete(path);}
    }
    public static void Invalid()
    {
        var d=Doc(out var c);var p=RegularPolygonGeometry.Parameters(c);var original=c.Geometry.ToList();var changed=0;d.Changed+=(_,_)=>changed++;
        var oversize=p with {Radius=500};Check(!oversize.ClampCenter(300,300).Fits(300,300),"oversized movement safely rejects rather than throwing");
        foreach(var bad in new[]{p with {Radius=0},p with {Radius=-1},p with {Sides=2},p with {Sides=513},p with {CenterX=double.NaN},p with {CenterY=double.PositiveInfinity},p with {Radius=double.PositiveInfinity},p with {Rotation=double.NaN},p with {CenterX=0},p with {Radius=1000}})Check(!d.TryUpdateRegularPolygon(c,bad)&&!d.CanUpdateRegularPolygon(c,bad),"invalid rejected");Check(original.SequenceEqual(c.Geometry)&&changed==0,"invalid no mutation/dirty");Check(d.TryUpdateRegularPolygon(c,p)&&changed==0,"same value no mutation");Check(d.AddRegularPolygon(p with {Sides=2}) is null,"invalid creation");Check(d.TryUpdateRegularPolygon(c,p with {Rotation=-45}),"angle normalized");Eq(315,c.Rotation);
    }
    static void FlatInteraction()
    {
        Exception? error=null;var thread=new Thread(()=>{try{
            var doc=new VCuttingDocument();var view=new FlatDesignerView{Document=doc,ActiveHoleShape="RegularPolygon",RegularPolygonSides=6};view.Measure(new Size(900,700));view.Arrange(new Rect(0,0,900,700));
            var type=typeof(FlatDesignerView);var click=type.GetMethod("HandleRegularPolygonClick",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)!;
            CutOperation? created=null;view.RegularPolygonPlacementRequested+=(_,p)=>created=doc.AddRegularPolygon(p);
            click.Invoke(view,[new Point(150,150)]);var state=(RegularPolygonDrawingState)type.GetField("_regularDrawing",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)!.GetValue(view)!;state.Move(new(180,170));view.RegularPolygonSides=8;
            using(var dc=new System.Windows.Media.DrawingVisual().RenderOpen())type.GetMethod("OnRender",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)!.Invoke(view,[dc]);
            click.Invoke(view,[new Point(180,170)]);Check(created is not null&&created.Sides==8,"actual Flat creation event and live sides");view.ActiveHoleShape=null;view.SelectedObject=created;
            var transform=FlatViewportEngine.Calculate(900,700,300,300,true,1,new Vector());var hit=type.GetMethod("Hit",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)!;
            foreach(var edge in created!.Geometry.Cast<LineSegment>()){object?[] args=[transform.ToScreen(new((edge.X1+edge.X2)/2,(edge.Y1+edge.Y2)/2)),null];Check(ReferenceEquals(hit.Invoke(view,args),created),"every Flat edge selects whole logical polygon");}
            view.ActiveHoleShape="RegularPolygon";click.Invoke(view,[new Point(150,150)]);Check(view.HandleRegularPolygonKey(System.Windows.Input.Key.Escape)&&view.ActiveHoleShape is null&&state.Center is null,"actual Flat ESC cancels");
        }catch(Exception e){error=e;}});thread.SetApartmentState(ApartmentState.STA);thread.Start();thread.Join();if(error is not null)throw new InvalidOperationException("Flat regular interaction: "+error.Message,error);
    }
    public static void Integration()
    {
        FlatInteraction();var state=new RegularPolygonDrawingState();Check(state.Sides==6,"default sides");Check(!state.Click(new(150,150),300,300,out _),"center starts preview");state.Move(new(180,180));var preview=state.Preview;state.Sides=8;Check(state.Preview.Sides==8,"live sides preview");Check(state.Click(new(180,180),300,300,out var p),"second click completes");var d=new VCuttingDocument();var history=new UndoRedoManager();history.Record(d);var c=d.AddRegularPolygon(p)!;Check(Points(c).SequenceEqual(RegularPolygonGeometry.Vertices(p)),"shared preview/creation calculator");Eq(45,c.Rotation);Check(state.Center is null,"creation clears preview");
        Check(history.Undo(d)!.Cuts.Count==0,"create undo");var before=DocumentSnapshot.Capture(d);history.Record(d);d.TryUpdateRegularPolygon(c,p with {Sides=6});var undo=history.Undo(d)!;Check(undo.Cuts.Single().Sides==8,"property undo");var redo=history.Redo(undo)!;Check(redo.Cuts.Single().Sides==6,"property redo");
        d.UpdateCut(c,160,160,c.Width,c.Height,c.Sides);Check(c.Sides==6&&c.Radius==p.Radius&&c.Rotation==p.Rotation,"body move preserves parameters");var oldRadius=c.Radius;d.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);Eq(oldRadius*10,c.Radius);Eq(1600,c.CenterX);Check(d.InnerContours.Single().Segments.SequenceEqual(c.Geometry),"inner contour after units");Check(BentSurfaceEngine.Build(d).Triangles.Count>0,"regular 3D");var mask=FlatMaterialMaskEngine.Build(d,new(1,new(0,3000)));Check(!mask.FillContains(new Point(1600,1400)),"flat cut");var restored=before.Restore();Check(restored.Cuts.Single().Shape=="RegularPolygon"&&RegularPolygonGeometry.Parameters(restored.Cuts.Single())==p,"snapshot parameters");history.Record(restored);Check(restored.DeleteObject(restored.Cuts.Single())&&restored.Cuts.Count==0&&restored.InnerContours.Count==0,"whole delete");Check(history.Undo(restored)!.Cuts.Count==1,"delete undo");
        state.Click(new(100,100),300,300,out _);state.Cancel();Check(state.Center is null,"cancel preview");state.Sides=2;Check(!state.Click(new(100,100),300,300,out _)&&state.Center is null,"invalid sides no preview");state.Sides=6;state.Click(new(10,10),300,300,out _);Check(!state.Click(new(100,10),300,300,out _)&&state.Center is not null,"outside preview refused without losing center");
        var merge=Doc(out var regular,6,30);merge.AddHole("Circle",160,150);Check(merge.CanMergeInternalCuts(regular)&&merge.TryMergeInternalCuts(regular)&&regular.Shape=="Compound","regular internal merge");var boundary=new VCuttingDocument();var bc=boundary.AddRegularPolygon(new(20,150,4,20,0))!;Check(boundary.CanMergeBoundaryCut(bc)&&boundary.TryMergeBoundaryCut(bc),"regular outer merge");var fillet=Doc(out var fc,6,30);Check(fillet.TryFilletContourCorner(fc.Id,0,2,out _)&&fc.Shape=="Compound","fillet transitions modified primitive");
        foreach(var lang in new[]{"en","ko"}){Localization.SetLanguage(lang);Check(Localization.Kind("RegularPolygon")!="RegularPolygon"&&Localization.Text("polygon.draw")!="polygon.draw","localized regular tool");}Localization.SetLanguage("en");
    }
}
