using System.IO;
using System.Reflection;
using System.Windows;
using VCutting;
using VCutting.Viewer;
using Localization=VCutting.Localization;

static class RegularStarTests
{
    static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
    static void Eq(double a,double b)=>Check(Math.Abs(a-b)<1e-6,$"star {a} != {b}");
    static RegularStarParameters P(int n=5,int k=2)=>new(new(150,150,n,30,18),k);
    public static void Geometry()
    {
        foreach(var (n,k,loops) in new[]{(5,2,1),(6,2,2),(7,2,1),(7,3,1),(8,2,2),(8,3,1),(512,255,1)})
        {
            var p=P(n,k);var cycles=RegularStarGeometry.Cycles(p);Check(cycles.Count==loops&&cycles.Sum(c=>c.Count)==n,"gcd cycle topology");
            var vertices=RegularPolygonGeometry.Vertices(p.Polygon);var edge=2*p.Polygon.Radius*Math.Sin(Math.PI*k/n);
            foreach(var cycle in cycles)for(var i=0;i<cycle.Count;i++){Eq(edge,(cycle[(i+1)%cycle.Count]-cycle[i]).Length);Eq(30,(cycle[i]-new Point(150,150)).Length);Check(vertices.Contains(cycle[i]),"shares polygon calculator");}
        }
        var d=new VCuttingDocument();var c=d.AddRegularStar(P())!;var original=c.Geometry.ToList();var events=0;d.Changed+=(_,_)=>events++;
        foreach(var bad in new[]{P(4,2),P(5,1),P(6,3),P(513,2),P() with {Step=int.MaxValue},P() with {Polygon=P().Polygon with {Radius=0}},P() with {Polygon=P().Polygon with {Rotation=double.NaN}},P() with {Polygon=P().Polygon with {CenterX=0}}})
            Check(!d.CanUpdateRegularStar(c,bad)&&!d.TryUpdateRegularStar(c,bad)&&d.AddRegularStar(bad) is null,"invalid star rejected");
        Check(events==0&&original.SequenceEqual(c.Geometry),"rejection does not mutate");Check(d.TryUpdateRegularStar(c,P())&&events==0,"no-op unchanged");
        Check(!d.TryFilletContourCorner(c.Id,0,2,out _)&&!d.CanMergeBoundaryCut(c),"self intersecting primitive not fed to simple contour operations");
    }
    public static void Integration()
    {
        Interaction();foreach(var (n,k) in new[]{(5,2),(6,2),(7,3),(8,3)})
        {
            var d=new VCuttingDocument();var history=new UndoRedoManager();history.Record(d);var c=d.AddRegularStar(P(n,k))!;
            Check(d.Cuts.Count==1&&d.InnerContours.Single().Segments.SequenceEqual(c.Geometry),"one logical object");Check(history.Undo(d)!.Cuts.Count==0,"create undo");Check(history.Redo(new VCuttingDocument())!.Cuts.Single().StarStep==k,"create redo step");
            var before=DocumentSnapshot.Capture(d);history.Record(d);var edited=P(n,k) with {Polygon=P(n,k).Polygon with {Radius=35,Rotation=-45}};
            Check(d.TryUpdateRegularStar(c,edited)&&c.Rotation==315,"property regeneration");Check(RegularStarGeometry.Parameters(history.Undo(d)!.Cuts.Single())==P(n,k),"property undo");
            var path=Path.Combine(Path.GetTempPath(),"star-"+Guid.NewGuid()+".dxf");try
            {
                VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path).Cuts.Single();Check(RegularStarGeometry.Parameters(c)==RegularStarGeometry.Parameters(loaded)&&c.Geometry.SequenceEqual(loaded.Geometry),"DXF metadata and geometry roundtrip");
                var parsed=DxfDocumentParser.Parse(path);Check(parsed.UnsupportedCounts.Count==0&&parsed.EntityCounts["LWPOLYLINE"]==RegularStarGeometry.Cycles(edited).Count,"one closed DXF polyline per cycle");
                var imported=GeneralDxfImportEngine.Import(path).Document;Check(imported.Cuts.Sum(c=>c.Geometry.Count)==n,"plain import retains every star segment");
            }finally{File.Delete(path);}
            d.UpdateCut(c,180,160,c.Width,c.Height,c.Sides);Eq(35,c.Radius);Eq(315,c.Rotation);Check(c.StarStep==k&&c.Sides==n,"move preserves shape");
            d.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);Eq(350,c.Radius);Eq(1800,c.CenterX);Check(RegularStarGeometry.Cycles(RegularStarGeometry.Parameters(c)).Sum(x=>x.Count)==n,"units retain topology");
            var restored=before.Restore();Check(restored.Cuts.Single().Shape=="RegularStar"&&RegularStarGeometry.Parameters(restored.Cuts.Single())==P(n,k),"snapshot parametric star");
            history.Record(restored);Check(restored.DeleteObject(restored.Cuts.Single())&&restored.InnerContours.Count==0,"delete whole object");Check(history.Undo(restored)!.Cuts.Single().StarStep==k,"delete undo");
        }
        foreach(var language in new[]{"en","ko"}){Localization.SetLanguage(language);Check(Localization.Kind("RegularStar")!="RegularStar"&&Localization.Text("star.instructions")!="star.instructions","localized star");}Localization.SetLanguage("en");
    }
    static void Interaction()
    {
        Exception? error=null;var thread=new Thread(()=>{try
        {
            var doc=new VCuttingDocument();var view=new FlatDesignerView{Document=doc,ActiveHoleShape="RegularStar",RegularPolygonSides=6,RegularStarStep=2};view.Measure(new Size(900,700));view.Arrange(new Rect(0,0,900,700));
            var flags=BindingFlags.Instance|BindingFlags.NonPublic;var click=typeof(FlatDesignerView).GetMethod("HandleRegularPolygonClick",flags)!;CutOperation? cut=null;view.RegularStarPlacementRequested+=(_,p)=>cut=doc.AddRegularStar(p);
            click.Invoke(view,[new Point(150,150)]);var state=(RegularPolygonDrawingState)typeof(FlatDesignerView).GetField("_regularDrawing",flags)!.GetValue(view)!;state.Move(new(180,150));
            using(var dc=new System.Windows.Media.DrawingVisual().RenderOpen())typeof(FlatDesignerView).GetMethod("OnRender",flags)!.Invoke(view,[dc]);
            view.RegularStarStep=3;click.Invoke(view,[new Point(180,150)]);Check(cut is null&&state.Center is not null,"invalid step retains preview");view.RegularStarStep=2;click.Invoke(view,[new Point(180,150)]);Check(cut?.Shape=="RegularStar"&&cut.Geometry.Count==6,"actual event creates hexagram");
            view.ActiveHoleShape=null;view.SelectedObject=cut;var transform=FlatViewportEngine.Calculate(900,700,300,300,true,1,new Vector());
            foreach(var edge in cut!.Geometry.Cast<LineSegment>()){object?[] args=[transform.ToScreen(new((edge.X1+edge.X2)/2,(edge.Y1+edge.Y2)/2)),null];Check(ReferenceEquals(typeof(FlatDesignerView).GetMethod("Hit",flags)!.Invoke(view,args),cut),"each crossing line selects whole object");}
            view.ActiveHoleShape="RegularStar";click.Invoke(view,[new Point(150,150)]);Check(view.HandleRegularPolygonKey(System.Windows.Input.Key.Escape)&&state.Center is null&&view.ActiveHoleShape is null,"star ESC cancel");
        }catch(Exception e){error=e;}});thread.SetApartmentState(ApartmentState.STA);thread.Start();thread.Join();if(error is not null)throw new InvalidOperationException("star UI event",error);
    }
    public static void Material()
    {
        foreach(var (n,k) in new[]{(5,2),(6,2),(7,3),(8,2),(8,3)})
        {
            var d=new VCuttingDocument();var c=d.AddRegularStar(P(n,k))!;var mask=FlatMaterialMaskEngine.Build(d,new(1,new(0,300)));var mesh=BentSurfaceEngine.Build(d);Check(mesh.Triangles.Count>0,"star mesh built");
            for(var x=125.25;x<175;x+=3)for(var y=125.125;y<175;y+=3)
            {
                var p=new Point(x,y);if(c.Geometry.Min(edge=>SlitHitTesting.Distance(edge,p))<.02)continue;Check(mask.FillContains(new Point(x,300-y),1e-7,System.Windows.Media.ToleranceType.Absolute)!=CutContourEngine.Contains(c.Geometry,p),$"Flat parity agrees with analytical cycles n={n} k={k} p={p} mask={mask.FillContains(new Point(x,300-y),1e-7,System.Windows.Media.ToleranceType.Absolute)} cut={CutContourEngine.Contains(c.Geometry,p)}");
            }
            for(var i=0;i<mesh.Triangles.Count;i+=3)
            {
                var a=mesh.Points[mesh.Triangles[i]];var b=mesh.Points[mesh.Triangles[i+1]];var e=mesh.Points[mesh.Triangles[i+2]];
                // Unbent mesh projects back to the same document coordinates.
                Check(!CutContourEngine.Contains(c.Geometry,new((a.X+b.X+e.X)/3,(a.Y+b.Y+e.Y)/3)),"3D does not fill star cut areas");
            }
            var circle=d.AddHole("Circle",150,150);d.UpdateCut(circle,150,150,100,100,7);Check(d.CanMergeInternalCuts(c)&&d.TryMergeInternalCuts(c)&&c.Shape=="Compound"&&d.Cuts.Count==1,"containing circle merges full crossing star safely");Check(CutContourEngine.Contains(c.Geometry,new(150,150)),"circle union fills central island");
        }
    }
    public static void SolidMaterial()
    {
        foreach(var (n,k) in new[]{(5,2),(6,2),(7,3),(8,2),(8,3)})
        {
            var d=new VCuttingDocument();var c=d.AddRegularStar(P(n,k))!;Check(CutContourEngine.Contains(c,new Point(150,150)),"star center fully cut");var mask=FlatMaterialMaskEngine.Build(d,new(1,new(0,300)));var mesh=BentSurfaceEngine.Build(d);Check(mesh.Triangles.Count>0,"star mesh built");
            for(var x=125.25;x<175;x+=3)for(var y=125.125;y<175;y+=3)
            {
                var p=new Point(x,y);if(c.Geometry.Min(edge=>SlitHitTesting.Distance(edge,p))<.02)continue;Check(mask.FillContains(new Point(x,300-y),1e-7,System.Windows.Media.ToleranceType.Absolute)!=CutContourEngine.Contains(c,p),$"Flat parity agrees with analytical cycles n={n} k={k} p={p} mask={mask.FillContains(new Point(x,300-y),1e-7,System.Windows.Media.ToleranceType.Absolute)} cut={CutContourEngine.Contains(c,p)}");
            }
            for(var i=0;i<mesh.Triangles.Count;i+=3)
            {
                var a=mesh.Points[mesh.Triangles[i]];var b=mesh.Points[mesh.Triangles[i+1]];var e=mesh.Points[mesh.Triangles[i+2]];
                // Unbent mesh projects back to the same document coordinates.
                Check(!CutContourEngine.Contains(c,new((a.X+b.X+e.X)/3,(a.Y+b.Y+e.Y)/3)),"3D does not fill star cut areas");
            }
            var circle=d.AddHole("Circle",150,150);d.UpdateCut(circle,150,150,100,100,7);Check(d.CanMergeInternalCuts(c)&&d.TryMergeInternalCuts(c)&&c.Shape=="Compound"&&d.Cuts.Count==1,"containing circle merges full crossing star safely");Check(CutContourEngine.Contains(c.Geometry,new(150,150)),"circle union fills central island");
        }
    }
    public static void ProfileIntegration()
    {
        ProfileInteraction();foreach(var (n,k) in new[]{(5,2),(6,2),(7,3),(8,3)})
        {
            var d=new VCuttingDocument();var history=new UndoRedoManager();history.Record(d);var c=d.AddRegularStar(P(n,k))!;
            Check(d.Cuts.Count==1&&d.InnerContours.Single().Segments.SequenceEqual(c.Geometry),"one logical object");Check(history.Undo(d)!.Cuts.Count==0,"create undo");Check(history.Redo(new VCuttingDocument())!.Cuts.Single().StarStep==k,"create redo step");
            var before=DocumentSnapshot.Capture(d);history.Record(d);var edited=P(n,k) with {Polygon=P(n,k).Polygon with {Radius=35,Rotation=-45}};
            Check(d.TryUpdateRegularStar(c,edited)&&c.Rotation==315,"property regeneration");Check(RegularStarGeometry.Parameters(history.Undo(d)!.Cuts.Single())==P(n,k),"property undo");
            var path=Path.Combine(Path.GetTempPath(),"star-"+Guid.NewGuid()+".dxf");try
            {
                VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path).Cuts.Single();Check(RegularStarGeometry.Parameters(c)==RegularStarGeometry.Parameters(loaded)&&c.Geometry.SequenceEqual(loaded.Geometry),"DXF metadata and geometry roundtrip");
                var parsed=DxfDocumentParser.Parse(path);Check(parsed.UnsupportedCounts.Count==0&&parsed.EntityCounts["LWPOLYLINE"]==1,"one closed profile polyline");
                var imported=GeneralDxfImportEngine.Import(path).Document;Check(imported.Cuts.Sum(c=>c.Geometry.Count)==2*n,"plain import retains only optimized profile segments");
            }finally{File.Delete(path);}
            d.UpdateCut(c,180,160,c.Width,c.Height,c.Sides);Eq(35,c.Radius);Eq(315,c.Rotation);Check(c.StarStep==k&&c.Sides==n,"move preserves shape");
            d.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);Eq(350,c.Radius);Eq(1800,c.CenterX);Check(RegularStarGeometry.Cycles(RegularStarGeometry.Parameters(c)).Sum(x=>x.Count)==n,"units retain topology");
            var restored=before.Restore();Check(restored.Cuts.Single().Shape=="RegularStar"&&RegularStarGeometry.Parameters(restored.Cuts.Single())==P(n,k),"snapshot parametric star");
            history.Record(restored);Check(restored.DeleteObject(restored.Cuts.Single())&&restored.InnerContours.Count==0,"delete whole object");Check(history.Undo(restored)!.Cuts.Single().StarStep==k,"delete undo");
        }
        foreach(var language in new[]{"en","ko"}){Localization.SetLanguage(language);Check(Localization.Kind("RegularStar")!="RegularStar"&&Localization.Text("star.instructions")!="star.instructions","localized star");}Localization.SetLanguage("en");
    }
    static void ProfileInteraction()
    {
        Exception? error=null;var thread=new Thread(()=>{try
        {
            var doc=new VCuttingDocument();var view=new FlatDesignerView{Document=doc,ActiveHoleShape="RegularStar",RegularPolygonSides=6,RegularStarStep=2};view.Measure(new Size(900,700));view.Arrange(new Rect(0,0,900,700));
            var flags=BindingFlags.Instance|BindingFlags.NonPublic;var click=typeof(FlatDesignerView).GetMethod("HandleRegularPolygonClick",flags)!;CutOperation? cut=null;view.RegularStarPlacementRequested+=(_,p)=>cut=doc.AddRegularStar(p);
            click.Invoke(view,[new Point(150,150)]);var state=(RegularPolygonDrawingState)typeof(FlatDesignerView).GetField("_regularDrawing",flags)!.GetValue(view)!;state.Move(new(180,150));
            using(var dc=new System.Windows.Media.DrawingVisual().RenderOpen())typeof(FlatDesignerView).GetMethod("OnRender",flags)!.Invoke(view,[dc]);
            view.RegularStarStep=3;click.Invoke(view,[new Point(180,150)]);Check(cut is null&&state.Center is not null,"invalid step retains preview");view.RegularStarStep=2;click.Invoke(view,[new Point(180,150)]);Check(cut?.Shape=="RegularStar"&&cut.Geometry.Count==12,"actual event creates hexagram");
            view.ActiveHoleShape=null;view.SelectedObject=cut;var transform=FlatViewportEngine.Calculate(900,700,300,300,true,1,new Vector());
            foreach(var edge in cut!.Geometry.Cast<LineSegment>()){object?[] args=[transform.ToScreen(new((edge.X1+edge.X2)/2,(edge.Y1+edge.Y2)/2)),null];Check(ReferenceEquals(typeof(FlatDesignerView).GetMethod("Hit",flags)!.Invoke(view,args),cut),"each profile edge selects whole object");}
            view.ActiveHoleShape="RegularStar";click.Invoke(view,[new Point(150,150)]);Check(view.HandleRegularPolygonKey(System.Windows.Input.Key.Escape)&&state.Center is null&&view.ActiveHoleShape is null,"star ESC cancel");
        }catch(Exception e){error=e;}});thread.SetApartmentState(ApartmentState.STA);thread.Start();thread.Join();if(error is not null)throw new InvalidOperationException("star UI event",error);
    }
    public static void ProfileBoundary()
    {
        for(var n=5;n<=24;n++)for(var k=2;k<=(n-1)/2;k++)
        {
            var p=P(n,k);var cut=new VCuttingDocument().AddRegularStar(p)!;
            Check(cut.Geometry.Count==2*n&&CutContourEngine.Loops(cut.Geometry).Count==1,"single closed profile with 2N edges");
            var raw=new List<GeometrySegment>();foreach(var cycle in RegularStarGeometry.Cycles(p))for(var i=0;i<cycle.Count;i++){var a=cycle[i];var b=cycle[(i+1)%cycle.Count];raw.Add(new LineSegment(a.X,a.Y,b.X,b.Y));}
            foreach(var line in cut.Geometry.Cast<LineSegment>())
            {
                var mid=new Point((line.X1+line.X2)/2,(line.Y1+line.Y2)/2);var tangent=new Vector(line.X2-line.X1,line.Y2-line.Y1);tangent.Normalize();var normal=new Vector(-tangent.Y,tangent.X)*.00001;
                Check(RegularStarGeometry.Contains(raw,mid+normal)&&!RegularStarGeometry.Contains(raw,mid-normal),"every path segment separates material from removed area, never interior waste");
            }
            for(var x=119.123;x<181;x+=4.21)for(var y=119.456;y<181;y+=4.31){var point=new Point(x,y);Check(RegularStarGeometry.Contains(raw,point)==CutContourEngine.Contains(cut,point),"optimized profile preserves full original solid star area");}
            var rawLength=raw.Cast<LineSegment>().Sum(l=>new Vector(l.X2-l.X1,l.Y2-l.Y1).Length);var length=cut.Geometry.Cast<LineSegment>().Sum(l=>new Vector(l.X2-l.X1,l.Y2-l.Y1).Length);Check(length<rawLength,"cutting length reduced");
        }
    }
}
