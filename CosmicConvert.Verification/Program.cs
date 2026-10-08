using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using CosmicConvert;
using System.Windows;
using VCutting.Viewer;

internal static partial class Verification
{
    sealed class FixtureUnavailable(string reason):Exception(reason);
    static string Repo=Directory.GetCurrentDirectory();
    static string Fixtures=Path.Combine(Repo,"artifacts","cr085-fixtures");
    static string Output=Path.Combine(Repo,"artifacts","cr087-comparison");
    [STAThread] static int Main(string[] args)
    {
        Directory.CreateDirectory(Fixtures);Directory.CreateDirectory(Output);
        var tests=new (string Id,Action Run)[]{("TC-085-001",Reader),("TC-085-002",Curves),("TC-085-003",Quiet),("TC-085-004",Apple),("TC-086-001",Margin),("TC-087-001",NativeContract),("TC-087-002",NativeApple),("TC-088-001",InvisibleText)};
        if(args.SequenceEqual(new[]{"--list"})){foreach(var t in tests)Console.WriteLine(t.Id);return 0;}
        if(args.Length!=0&&(args.Length!=2||args[0]!="--tests")){Console.Error.WriteLine("Use --list or --tests TC-085-001,...");return 2;}
        var selected=args.Length==2&&args[0]=="--tests"?args[1].Split(',').ToHashSet():tests.Select(t=>t.Id).ToHashSet();int failures=0,notRun=0;
        if(selected.Any(id=>!tests.Any(t=>t.Id==id))){Console.Error.WriteLine("Unknown test ID");return 2;}
        foreach(var t in tests.Where(t=>selected.Contains(t.Id)))try{t.Run();Console.WriteLine(t.Id+" PASS");}catch(FixtureUnavailable ex){notRun++;Console.WriteLine(t.Id+" NOT_RUN "+ex.Message);}catch(Exception ex){failures++;Console.WriteLine(t.Id+" FAIL "+ex);}
        Console.WriteLine($"AUTO: {selected.Count-failures-notRun}/{selected.Count} PASS; NOT_RUN {notRun}; human verification PENDING_MANUAL");return failures==0&&notRun==0?0:1;
    }
    static void Check(bool ok,string reason){if(!ok)throw new Exception(reason);}
    static string FileSvg(string body,string root="width='100mm' height='100mm' viewBox='0 0 100 100'")
    {
        var path=Path.Combine(Fixtures,Guid.NewGuid()+".svg");File.WriteAllText(path,$"<svg xmlns='http://www.w3.org/2000/svg' {root}>{body}</svg>");return path;
    }
    static SvgDrawing Read(string body,string root="width='100mm' height='100mm' viewBox='0 0 100 100'")=>SvgReader.Read(FileSvg(body,root));
    static ConvertedDrawing Convert(string body)=>Optimizer.Convert(Read(body));
    static void Reject(string body,int code=4){try{Convert(body);throw new Exception("Expected rejection");}catch(ConversionException ex){Check(ex.Code==code,"Error code");}}
    static void Reader()
    {
        var line=Read("<path fill='none' stroke='black' d='M10 20L30 40'/>");var c=line.Outlines[0].Curves[0];Check((c.At(0)-new Point(10,80)).Length<1e-10,"mm/viewBox/Y");
        foreach(var size in new[]{"25.4mm","2.54cm","1in","72pt","6pc","96px","96"})Check(Math.Abs(Read("<line x2='50' stroke='black'/>",$"width='{size}' height='{size}'").Width-25.4)<1e-10,"absolute units "+size);
        Check(Math.Abs(Read("<line x2='10' stroke='black'/>","viewBox='0 0 96 96'").Width-25.4)<1e-10,"viewBox-only px");
        var use=Read("<defs><path id='x' fill='none' stroke='black' d='M0 0L1 0'/></defs><g transform='translate(10 20)'><use href='#x' x='2'/></g>");Check((use.Outlines[0].Curves[0].At(0)-new Point(12,80)).Length<1e-8,"use nesting");
        var skew=Read("<g transform='skewX(45) skewY(0)'><line y1='10' y2='10' x2='10' stroke='black'/></g>");Check(Math.Abs(skew.Outlines[0].Curves[0].At(0).X-10)<1e-8,"skew");
        var path=Read("<path stroke='black' fill='none' d='m1 1h4v3l2 1c1 2 2 3 4 2s2 -2 3 1q1 2 3 1t3 2a4 3 30 0 1 5 5z'/>");Check(path.Outlines[0].Closed&&path.Outlines[0].Curves.Count>8,"relative/all commands");
        Check(Convert("<rect x='1' y='1' width='20' height='10' rx='2' ry='3'/>").Entities.Count()>4,"rounded rect");
        Check(Convert("<rect x='1' y='1' width='20' height='10' rx='2'/>").Entities.Count(e=>e.Kind=="ARC")==4,"exact rounded circular corners");
        var reflected=Convert("<g transform='translate(50 0) scale(-1 1)'><path fill='none' stroke='black' d='M10 10A10 10 0 1 0 20 20'/></g>");Check(reflected.Entities.Single().Kind=="ARC"&&Math.Abs(reflected.Entities.Single().Sweep)>Math.PI,"reflected major circular arc");
        Reject("<text>abc</text>");Reject("<image href='https://example.com/x'/>");Reject("<script>alert(1)</script>");Reject("<style>path{display:none}</style><path d='M0 0L1 1'/>");Reject("<use href='#missing'/>");Reject("<defs><g id='x'><use href='#x'/></g></defs><use href='#x'/>");Reject("<path clip-path='url(#x)' d='M0 0L1 1'/>");Reject("<path style='d: x' d='M0 0L1 1'/>");Reject("<path d='M0 0C1 2'/>");
        Reject("<defs><style>path{display:none}</style></defs><path d='M0 0L1 1'/>");
        var hidden=Read("<g display='none'><text>x</text></g><circle cx='10' cy='10' r='2'/><circle visibility='hidden' r='1'/>");Check(hidden.Outlines.Count==1,"visibility");
        var malicious=Path.Combine(Fixtures,"dtd.svg");File.WriteAllText(malicious,"<!DOCTYPE svg [<!ENTITY x SYSTEM 'file:///secret'>]><svg>&x;</svg>");try{SvgReader.Read(malicious);throw new Exception("DTD accepted");}catch(ConversionException ex){Check(ex.Code==4,"DTD code");}
        var aspect=Read("<line x2='100' stroke='black'/>","width='200mm' height='100mm' viewBox='0 0 100 100'");Check(Math.Abs(aspect.Outlines[0].Curves[0].At(0).X-50)<1e-8,"meet");
        var none=Read("<line x2='100' stroke='black'/>","width='200mm' height='100mm' viewBox='0 0 100 100' preserveAspectRatio='none'");Check(Math.Abs(none.Outlines[0].Curves[0].At(1).X-200)<1e-8,"none aspect");
    }
    static void Curves()
    {
        var circle=Convert("<circle cx='10' cy='20' r='5'/>");Check(circle.Entities.Single().Kind=="CIRCLE","exact circle");
        var ellipse=Convert("<g transform='scale(2 1)'><circle cx='10' cy='20' r='5'/></g>");Check(ellipse.Entities.All(e=>e.Kind!="CIRCLE"),"nonuniform circle");
        var native=Convert("<path fill='none' stroke='black' d='M10 10A10 10 0 0 1 20 20'/>");Check(native.Entities.Single().Kind=="ARC","exact SVG circular arc");
        var cubic=Convert("<path fill='none' stroke='black' d='M10 10C10 30 30 30 30 10'/>");Check(cubic.Entities.Any(e=>e.Kind=="ARC")&&cubic.ErrorBound<=.05,"certified cubic arcs");
        var quadratic=Convert("<path fill='none' stroke='black' d='M10 10Q20 30 30 10'/>");Check(quadratic.ErrorBound<=.05,"quadratic");
        var tiny=Convert("<path fill='none' stroke='black' d='M1 1L1.001 1L1.001 1.001L1 1.001Z'/>");Check(tiny.Entities.Count()==4,"small corners");
        Check(Convert("<path fill='none' stroke='black' d='M1 1L2 1L3 1'/>").Entities.Count()==1,"exact collinear merge");
        Check(Convert("<path fill='none' stroke='black' d='M1 1L2 2'/><path fill='none' stroke='black' d='M2 2L1 1'/>").Entities.Count()==1,"exact duplicate cutting path");
        var open=Convert("<polyline fill='none' stroke='black' points='1,1 3,2 4,5'/>");Check(!open.Source.Outlines[0].Closed&&open.Entities.Count()==2,"open");
        var rings="M10 10L90 10L90 90L10 90Z M20 20L80 20L80 80L20 80Z";
        Check(Convert($"<path d='{rings}'/>").Groups.Count(g=>g.Count>0)==1,"nonzero nested");Check(Convert($"<path fill-rule='evenodd' d='{rings}'/>").Groups.Count(g=>g.Count>0)==2,"evenodd hole");
        Reject("<path d='M10 10L90 90L10 90L90 10Z'/>");
        foreach(var drawing in new[]{ellipse,cubic,quadratic,native})
        {
            var file=Path.Combine(Fixtures,Guid.NewGuid()+".dxf");DxfWriter.Save(drawing,file);DxfWriter.Verify(file,drawing);
            var parsed=DxfDocumentParser.Parse(file);Check(parsed.UnsupportedCounts.Count==0&&parsed.Entities.Count==drawing.Entities.Count()+4,"existing parser");
            foreach(var curve in drawing.Source.Outlines.SelectMany(o=>o.Curves))for(int i=0;i<=300;i++)Check(Distance(curve.At(i/300d),drawing.Entities)<=.0500001,"independent sampled diagnostic");
        }
        foreach(double sweep in new[]{20d,-20d})
        {
            double start=(sweep>0?350:10)*Math.PI/180,span=sweep*Math.PI/180;var center=new Point(30,30);Point At(double angle)=>center+new Vector(10*Math.Cos(angle),10*Math.Sin(angle));
            var arc=Entity.Arc(At(start),At(start+span),center,10,start,span);var fixture=new ConvertedDrawing(native.Source,[[arc]],1e-9);var file=Path.Combine(Fixtures,Guid.NewGuid()+".dxf");DxfWriter.Save(fixture,file);DxfWriter.Verify(file,fixture);var parsed=DxfDocumentParser.Parse(file);Check(parsed.Entities.OfType<GeometryArc>().Single() is GeometryArc a&&Math.Abs(a.StartAngle-350)<1e-8&&Math.Abs(a.EndAngle-10)<1e-8,"zero-crossing signed ARC encoding");
        }
        using var cancelled=new CancellationTokenSource();cancelled.Cancel();try{Optimizer.Convert(cubic.Source,cancelled.Token);throw new Exception("cancel ignored");}catch(OperationCanceledException){}
    }
    static double Distance(Point p,IEnumerable<Entity> entities)=>entities.Min(e=>
    {
        if(e.Kind=="LINE"){var v=e.B-e.A;double t=v.LengthSquared==0?0:Math.Clamp(Vector.Multiply(p-e.A,v)/v.LengthSquared,0,1);return(p-(e.A+v*t)).Length;}
        var angle=Math.Atan2(p.Y-e.Center.Y,p.X-e.Center.X);double delta=(angle-e.Start)%(2*Math.PI);if(e.Sweep>0&&delta<0)delta+=2*Math.PI;if(e.Sweep<0&&delta>0)delta-=2*Math.PI;
        return e.Kind=="CIRCLE"||delta/e.Sweep is >=0 and <=1?Math.Abs((p-e.Center).Length-e.Radius):Math.Min((p-e.A).Length,(p-e.B).Length);
    });
    static int ProcessRun(params string[] args)
    {
        var exe=Path.Combine(AppContext.BaseDirectory,"CosmicConvert.exe");var info=new ProcessStartInfo(exe){UseShellExecute=false,WorkingDirectory=Fixtures,RedirectStandardError=true,CreateNoWindow=true};foreach(var a in args)info.ArgumentList.Add(a);
        using var p=Process.Start(info)!;if(!p.WaitForExit(30000)){p.Kill();throw new Exception("quiet timeout");}Check(p.MainWindowHandle==IntPtr.Zero,"quiet main window");return p.ExitCode;
    }
    static void Quiet()
    {
        Check(ProcessRun("one","two")==2,"extra args");Check(ProcessRun(" ")==2,"empty arg");Check(ProcessRun("missing")==3,"missing input");
        var name="한글 공백.v1";File.WriteAllText(Path.Combine(Fixtures,name+".svg"),"<svg width='100' height='100'><circle cx='20' cy='20' r='10'/></svg>");Check(ProcessRun(name)==0,"relative unicode path");var dxf=Path.Combine(Fixtures,name+".dxf");var before=File.ReadAllBytes(dxf);Check(ProcessRun(Path.Combine(Fixtures,name))==0,"absolute overwrite");
        File.WriteAllText(Path.Combine(Fixtures,name+".svg"),"<svg><text>x</text></svg>");Check(ProcessRun(name)==4&&before.SequenceEqual(File.ReadAllBytes(dxf)),"failed input preserves output");
        var valid="<svg><circle cx='20' cy='20' r='10'/></svg>";File.WriteAllText(Path.Combine(Fixtures,"locked.svg"),valid);var locked=Path.Combine(Fixtures,"locked.dxf");File.WriteAllText(locked,"original");using(var file=new FileStream(locked,FileMode.Open,FileAccess.ReadWrite,FileShare.None))Check(ProcessRun("locked")==6,"locked save error");Check(File.ReadAllText(locked)=="original","locked output preserved");
        var good=Optimizer.Convert(Read("<circle cx='10' cy='10' r='2'/>","width='100' height='100'"));try{DxfWriter.Save(good with{ErrorBound=1},locked);throw new Exception("bad bound saved");}catch(ConversionException ex){Check(ex.Code==5&&File.ReadAllText(locked)=="original","validation preservation/code5");}
        using(var lockedInput=new FileStream(Path.Combine(Fixtures,"locked.svg"),FileMode.Open,FileAccess.ReadWrite,FileShare.None))Check(ProcessRun("locked")==3,"input read lock");
        using var cancel=new CancellationTokenSource();cancel.Cancel();try{DxfWriter.Save(good,locked,cancel.Token);throw new Exception("cancel saved");}catch(OperationCanceledException){Check(File.ReadAllText(locked)=="original","cancelled save preserves file");}
        Check(!Directory.EnumerateFiles(Fixtures,"*.tmp").Any(),"temporary cleanup");
    }
    static void Apple()
    {
        var svg=@"C:\MyDisk\Projects\절곡도면설계프로그램\에코드롬\Sample - Apple.svg";var old=Path.ChangeExtension(svg,".dxf");string Hash(string p)=>ConvertHex(SHA256.HashData(File.ReadAllBytes(p)));var hashes=new[]{Hash(svg),Hash(old)};
        if(hashes[1]!="24A39E1116EFD6A651C984395F45BECF8E28F30E6D36D2D353F5D5A121EF6373")throw new FixtureUnavailable("Original 850-LINE Apple DXF no longer available at supplied path. Historical baseline expectation retained; current hash "+hashes[1]);
        var original=SvgReader.Read(svg);Check(original.Outlines.Count==3,"Apple 3 paths");Check(original.Outlines.SelectMany(o=>o.Curves).Count(c=>c.Bezier is{Length:4})==45,"Apple 45 cubics");
        var clock=Stopwatch.StartNew();var result=Optimizer.Convert(original);var output=Path.Combine(Output,"Sample - Apple.optimized.dxf");DxfWriter.Save(result,output);clock.Stop();
        var parsed=DxfDocumentParser.Parse(output);var baseline=DxfDocumentParser.Parse(old);Check(baseline.Entities.Count==850,"baseline count");Check(parsed.Entities.Count==result.Entities.Count()+4&&parsed.UnsupportedCounts.Count==0,"parser supports output");Check(result.Entities.Count()<846&&result.Entities.Any(e=>e.Kind=="ARC"),"optimization/mixed entities");Check(result.Groups.Count==3,"paths retained");
        var layout=ExportLayout.Create(result);var shapeBounds=new DxfBounds();foreach(var e in parsed.Entities.Skip(4))e.ExpandBounds(shapeBounds);var baselineShape=new DxfBounds();foreach(var entity in baseline.Entities.Skip(4))entity.ExpandBounds(baselineShape);
        Check(Math.Abs(shapeBounds.MinX-layout.Offset.X-baselineShape.MinX)<.06&&Math.Abs(shapeBounds.MaxX-layout.Offset.X-baselineShape.MaxX)<.06&&Math.Abs(shapeBounds.MinY-layout.Offset.Y-baselineShape.MinY)<.06&&Math.Abs(shapeBounds.MaxY-layout.Offset.Y-baselineShape.MaxY)<.06,"physical bounds vs baseline shape");
        foreach(var group in result.Groups)for(int i=0;i<group.Count;i++)Check((group[i].At(1)-group[(i+1)%group.Count].At(0)).Length<1e-7,"Apple closure");
        double sampled=0;foreach(var c in original.Outlines.SelectMany(o=>o.Curves))for(int i=0;i<=300;i++)sampled=Math.Max(sampled,Distance(c.At(i/300d),result.Entities));Check(sampled<=result.ErrorBound+1e-8,"sample diagnostic vs conservative bound");
        var imported=VCutting.GeneralDxfImportEngine.Import(output);var grouping=imported.Document.Cuts.GroupBy(c=>c.Shape).ToDictionary(g=>g.Key,g=>g.Count());
        Check(hashes[0]==Hash(svg)&&hashes[1]==Hash(old),"original hashes unchanged");
        var metrics=new{originalSvgBytes=new FileInfo(svg).Length,baselineDxfBytes=new FileInfo(old).Length,baselineLines=850,baselineShapeLines=846,svgPaths=3,svgCubics=45,newDxfBytes=new FileInfo(output).Length,marginMm=ExportLayout.Margin,frameWidth=layout.Width,frameHeight=layout.Height,newEntities=layout.Entities.GroupBy(e=>e.Kind).ToDictionary(g=>g.Key,g=>g.Count()),conversionAndSaveMs=clock.Elapsed.TotalMilliseconds,conservativeCurveBoundMm=result.ErrorBound,sampledDiagnosticMm=sampled,bounds=new{parsed.Bounds.MinX,parsed.Bounds.MinY,parsed.Bounds.MaxX,parsed.Bounds.MaxY,parsed.Bounds.Width,parsed.Bounds.Height},baselineShapeBounds=new{baselineShape.MinX,baselineShape.MinY,baselineShape.MaxX,baselineShape.MaxY,baselineShape.Width,baselineShape.Height},cosmicDesignerImportedCuts=grouping,originalSha256=hashes,output,uiPerformance="NOT_RUN",human="PENDING_MANUAL"};
        var json=JsonSerializer.Serialize(metrics,new JsonSerializerOptions{WriteIndented=true});File.WriteAllText(Path.Combine(Output,"metrics.json"),json);Console.WriteLine(json);
    }
    static void Margin()
    {
        foreach(var body in new[]{"<circle cx='10' cy='10' r='5'/>","<path fill='none' stroke='black' d='M-10 -20L20 -20'/>","<path fill='none' stroke='black' d='M10 10C10 30 30 30 30 10'/>","<path fill='none' stroke='black' d='M10 10A10 10 0 1 0 20 20'/>"})
        {
            var result=Convert(body);var layout=ExportLayout.Create(result);var p=layout.Shape.SelectMany(ExportLayout.Extrema).ToList();
            Check(Math.Abs(p.Min(x=>x.X)-10)<1e-8&&Math.Abs(p.Min(x=>x.Y)-10)<1e-8&&Math.Abs(layout.Width-p.Max(x=>x.X)-10)<1e-8&&Math.Abs(layout.Height-p.Max(x=>x.Y)-10)<1e-8,"four margins exactly 10mm");
            Check(layout.Boundary.Count==4&&layout.Boundary.All(e=>e.Kind=="LINE")&&layout.Width>=20&&layout.Height>=20,"closed valid rectangle including degenerate shape bounds");
            for(int i=0;i<4;i++)Check(layout.Boundary[i].B==layout.Boundary[(i+1)%4].A,"boundary closure");
            var original=result.Entities.ToList();for(int i=0;i<original.Count;i++){Check((layout.Shape[i].A-original[i].A-layout.Offset).Length<1e-8,"rigid translation");Check(layout.Shape[i].Radius==original[i].Radius&&layout.Shape[i].Sweep==original[i].Sweep&&layout.Shape[i].Bound==original[i].Bound,"curve sizes/error retained");}
            var file=Path.Combine(Fixtures,Guid.NewGuid()+".dxf");DxfWriter.Save(result,file);DxfWriter.Verify(file,result);var parsed=DxfDocumentParser.Parse(file);Check(Math.Abs(parsed.Bounds.MinX)<1e-8&&Math.Abs(parsed.Bounds.MinY)<1e-8&&Math.Abs(parsed.Bounds.Width-layout.Width)<1e-8&&Math.Abs(parsed.Bounds.Height-layout.Height)<1e-8,"read-back frame");
        }
        var circle=Convert("<circle cx='10' cy='10' r='5'/>");var framed=ExportLayout.Create(circle);Check(Math.Abs(framed.Width-30)<1e-8&&Math.Abs(framed.Height-30)<1e-8,"circle frame 30mm");
        var name="margin.quiet";File.WriteAllText(Path.Combine(Fixtures,name+".svg"),"<svg width='100mm' height='100mm' viewBox='0 0 100 100'><circle cx='10' cy='10' r='5'/></svg>");Check(ProcessRun(name)==0,"quiet conversion");var document=DxfDocumentParser.Parse(Path.Combine(Fixtures,name+".dxf"));Check(document.Entities.Count==5&&Math.Abs(document.Bounds.Width-30)<1e-8,"actual quiet frame");
        var svg=@"C:\MyDisk\Projects\절곡도면설계프로그램\에코드롬\Sample - Apple.svg";var old=Path.ChangeExtension(svg,".dxf");var hashes=new[]{ConvertHex(SHA256.HashData(File.ReadAllBytes(svg))),ConvertHex(SHA256.HashData(File.ReadAllBytes(old)))};
        var apple=Optimizer.Convert(SvgReader.Read(svg));var layoutApple=ExportLayout.Create(apple);Check(apple.Groups.Count==3&&apple.Entities.Count()==167,"Apple optimization retained");
        var output=Path.Combine(Output,"Sample - Apple.margin10.dxf");DxfWriter.Save(apple,output);DxfWriter.Verify(output,apple);var readback=DxfDocumentParser.Parse(output);var shape=new DxfBounds();foreach(var e in readback.Entities.Skip(4))e.ExpandBounds(shape);
        Check(readback.Entities.Count==171&&Math.Abs(shape.MinX-10)<1e-8&&Math.Abs(shape.MinY-10)<1e-8&&Math.Abs(readback.Bounds.MaxX-shape.MaxX-10)<1e-8&&Math.Abs(readback.Bounds.MaxY-shape.MaxY-10)<1e-8,"Apple four margins/readback");
        var imported=VCutting.GeneralDxfImportEngine.Import(output);Check(imported.OuterEntities==4,"Designer recognizes external rectangle");
        Check(hashes[0]==ConvertHex(SHA256.HashData(File.ReadAllBytes(svg)))&&hashes[1]==ConvertHex(SHA256.HashData(File.ReadAllBytes(old))),"current input files preserved");
        var metrics=new{marginMm=10,frameWidth=layoutApple.Width,frameHeight=layoutApple.Height,newEntities=layoutApple.Entities.GroupBy(e=>e.Kind).ToDictionary(g=>g.Key,g=>g.Count()),shapeEntities=167,originalSha256=hashes,output,errorBoundMm=apple.ErrorBound,outerEntities=imported.OuterEntities};var json=JsonSerializer.Serialize(metrics,new JsonSerializerOptions{WriteIndented=true});File.WriteAllText(Path.Combine(Output,"margin-metrics.json"),json);Console.WriteLine(json);
    }
    static string ConvertHex(byte[] bytes)=>System.Convert.ToHexString(bytes);
}
