using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using CosmicConvert;
using VCutting;
using VCutting.Viewer;

internal static partial class Verification
{
    static void NativeContract()
    {
        var source=File.ReadAllText(Path.Combine(Repo,"VCutting","Engines.cs"));var snapshot=File.ReadAllText(Path.Combine(Repo,"CosmicConvert","DesignerMetadata.Contract.cs"));
        var declarations=System.Text.RegularExpressions.Regex.Matches(source,@"record (?:DocumentDto|SlitDto|ViewDto|SegmentDto|BendDto|CutDto|GeometryDto)\([^;]+;");
        Check(declarations.Count==7&&declarations.All(m=>snapshot.Contains(m.Value)),"exact existing serializer DTO snapshot");
        var drawing=Convert("<path fill='none' stroke='black' d='M1 1H20V20H1Z M3 3H5V5H3Z'/><circle cx='40' cy='40' r='3'/><path fill='none' stroke='black' d='M60 60A10 10 0 0 1 70 70'/>");
        var file=Path.Combine(Output,"contract.dxf");DxfWriter.Save(drawing,file);
        Check(VCuttingDxfSerializer.HasMetadata(file),"native dispatch");var d=VCuttingDxfSerializer.Load(file);
        Check(d.Unit==MeasurementUnit.Millimeter&&d.Cuts.Count==2&&d.Slits.Count==1,"units/element grouping/open slit");
        Check(d.Cuts[0].Shape=="Compound"&&CutContourEngine.Loops(d.Cuts[0].Geometry).Count==2,"subpaths");
        var again=Path.Combine(Output,"contract.native-resaved.dxf");VCuttingDxfSerializer.Save(d,again);
        var restored=VCuttingDxfSerializer.Load(again);Check(restored.Cuts.Count==2&&restored.Slits.Count==1,"actual native roundtrip");
        string Metadata(string p)=>File.ReadLines(p).Single(l=>l.StartsWith("COSMIC_DESIGNER_JSON:"))[21..];
        using var a=JsonDocument.Parse(Metadata(file));using var b=JsonDocument.Parse(Metadata(again));
        Check(a.RootElement.EnumerateObject().Select(p=>p.Name).SequenceEqual(b.RootElement.EnumerateObject().Select(p=>p.Name)),"native DTO field contract");
        Check(a.RootElement.GetProperty("Cuts")[0].EnumerateObject().Select(p=>p.Name).SequenceEqual(b.RootElement.GetProperty("Cuts")[0].EnumerateObject().Select(p=>p.Name)),"Cut DTO contract");
        var lines=File.ReadAllLines(file);var stripped=new List<string>();for(int i=0;i<lines.Length;i+=2){if(lines[i].Trim()=="999"&&lines[i+1].StartsWith("COSMIC_DESIGNER_JSON:"))continue;stripped.Add(lines[i]);stripped.Add(lines[i+1]);}
        var plain=Path.Combine(Output,"contract.plain.dxf");File.WriteAllLines(plain,stripped);
        Check(!VCuttingDxfSerializer.HasMetadata(plain)&&JsonSerializer.Serialize(DxfDocumentParser.Parse(file).Entities)==JsonSerializer.Serialize(DxfDocumentParser.Parse(plain).Entities),"metadata stripping leaves identical CAD geometry");
        try{DxfWriter.Verify(plain,drawing);throw new Exception("missing metadata accepted");}catch(ConversionException ex){Check(ex.Code==5,"no silent fallback");}
        var circle=d.Cuts[1];Check(Math.Abs(circle.Width-6)<1e-8&&Math.Abs(circle.Height-6)<1e-8,"circle bounds");
        Check(d.Cuts.Select(c=>c.Id).Concat(d.Slits.Select(s=>s.Id)).Distinct().Count()==3,"unique IDs");
        var input=FileSvg("<circle cx='20' cy='20' r='3'/>");var direct=Path.Combine(Output,"gui-engine-equivalence.dxf");DxfWriter.Save(Optimizer.Convert(SvgReader.Read(input)),direct);
        Check(ProcessRun(Path.ChangeExtension(input,null)!)==0&&File.ReadAllBytes(direct).SequenceEqual(File.ReadAllBytes(Path.ChangeExtension(input,".dxf"))),"quiet and GUI Save shared engine identical metadata/entities");
    }
    static void NativeApple()
    {
        var svg=@"C:\MyDisk\Projects\절곡도면설계프로그램\에코드롬\Sample - Apple.svg";var supplied=Path.ChangeExtension(svg,".dxf");
        string Hash(string p)=>System.Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(p)));var before=new[]{Hash(svg),Hash(supplied)};
        var timer=Stopwatch.StartNew();var drawing=Optimizer.Convert(SvgReader.Read(svg));var layout=ExportLayout.Create(drawing);var file=Path.Combine(Output,"Sample - Apple.native.dxf");DxfWriter.Save(drawing,file);timer.Stop();
        var d=VCuttingDxfSerializer.Load(file);var cut=d.Cuts.Single();Check(cut.Shape=="Compound"&&CutContourEngine.Loops(cut.Geometry).Count==3&&d.InnerContours.Count==1,"one Compound/three closed loops");
        Check(d.Unit==MeasurementUnit.Millimeter&&cut.Sequence==1&&cut.Id=="H001"&&Math.Abs(cut.Width-(layout.Width-20))<1e-8&&Math.Abs(cut.CenterX-layout.Width/2)<1e-8,"physical bounds/properties");
        var view=new FlatDesignerView{Document=d};view.Measure(new Size(900,700));view.Arrange(new Rect(0,0,900,700));
        var toScreen=typeof(FlatDesignerView).GetMethod("ToScreen",BindingFlags.Instance|BindingFlags.NonPublic,null,[typeof(double),typeof(double)],null)!;
        var hit=typeof(FlatDesignerView).GetMethod("Hit",BindingFlags.Instance|BindingFlags.NonPublic)!;
        foreach(var e in drawing.Entities.Take(12)){var p=e.A+layout.Offset;var screen=toScreen.Invoke(view,[p.X,p.Y]);Check(ReferenceEquals(hit.Invoke(view,[screen,null]),cut),"actual contour hit selects whole Compound");}
        var original=SvgImportEngine.Import(svg).Document.Cuts.Single();Check(original.Shape=="Compound"&&CutContourEngine.Loops(original.Geometry).Count==3,"actual SVG import reference");
        var mask=FlatMaterialMaskEngine.Build(d,new(1,new Point(0,layout.Height)));int tested=0;
        for(double x=.5;x<layout.Width;x+=1)for(double y=.5;y<layout.Height;y+=1){var p=new Point(x,y);if(Distance(p-layout.Offset,drawing.Entities)<.15)continue;bool removed=CutContourEngine.Contains(original,p-layout.Offset);Check(CutContourEngine.Contains(cut,p)==removed,"SVG/native hole parity");Check(mask.FillContains(new Point(x,layout.Height-y),.001,System.Windows.Media.ToleranceType.Absolute)!=removed,"actual 2D material mask");tested++;}
        var mesh=BentSurfaceEngine.Build(d);Check(mesh.Triangles.Count>0,"actual 3D material");int meshChecked=0;
        for(int i=0;i<mesh.Triangles.Count;i+=3){var vertices=mesh.Triangles.Skip(i).Take(3).Select(j=>mesh.Points[j]).ToArray();var p=new Point(vertices.Average(v=>v.X),vertices.Average(v=>v.Y));if(Distance(p-layout.Offset,drawing.Entities)<.15)continue;Check(!CutContourEngine.Contains(original,p-layout.Offset),"3D triangle not in SVG removal region");meshChecked++;}
        Check(meshChecked>100&&tested>100,"material comparison coverage");
        var undo=new UndoRedoManager();double cx=cut.CenterX;undo.Record(d);d.UpdateCut(cut,cx+1,cut.CenterY+1,cut.Width,cut.Height,cut.Sides);Check(Math.Abs(cut.CenterX-cx-1)<1e-8,"move");d=undo.Undo(d)!;Check(Math.Abs(d.Cuts.Single().CenterX-cx)<1e-8,"undo");d=undo.Redo(d)!;Check(Math.Abs(d.Cuts.Single().CenterX-cx-1)<1e-8,"redo");undo.Record(d);Check(d.DeleteObject(d.Cuts.Single())&&d.Cuts.Count==0,"delete");d=undo.Undo(d)!;
        var resave=Path.Combine(Output,"Sample - Apple.native-resaved.dxf");VCuttingDxfSerializer.Save(d,resave);Check(CutContourEngine.Loops(VCuttingDxfSerializer.Load(resave).Cuts.Single().Geometry).Count==3,"edit/save/reopen");
        Check(before.SequenceEqual(new[]{Hash(svg),Hash(supplied)}),"source hashes unchanged");
        var parsed=DxfDocumentParser.Parse(file);Check(parsed.Entities.Count==171&&drawing.ErrorBound<=.05,"optimized count/bound");
        File.WriteAllText(Path.Combine(Output,"native-report.json"),JsonSerializer.Serialize(new{file,Build=typeof(Optimizer).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().Single(a=>a.Key=="BuildIdentity").Value,Objects=1,Shape=cut.Shape,ClosedLoops=3,Counts=parsed.Entities.GroupBy(e=>e.GetType().Name).ToDictionary(g=>g.Key,g=>g.Count()),layout.Width,layout.Height,Margin=10,drawing.ErrorBound,Milliseconds=timer.Elapsed.TotalMilliseconds,Bytes=new FileInfo(file).Length,SourceHashes=before,MaterialSamples=tested,MeshTriangles=mesh.Triangles.Count/3,MeshChecked=meshChecked,Manual="PENDING"},new JsonSerializerOptions{WriteIndented=true}));
    }
}
