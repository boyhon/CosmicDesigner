using System.Globalization;
using System.IO;
using System.Windows;
using VCutting;

static class SvgImportTests
{
    static void Check(bool ok,string why){if(!ok)throw new InvalidOperationException("SVG: "+why);}
    static void Eq(double a,double b,double tolerance=1e-4)=>Check(Math.Abs(a-b)<tolerance,$"{a}!={b}");
    static SvgImportResult Read(string body,string attributes="width='100mm' height='100mm' viewBox='0 0 100 100'",MeasurementUnit unit=MeasurementUnit.Millimeter)
    {
        var path=Path.Combine(Path.GetTempPath(),"svg-import-"+Guid.NewGuid()+".svg");try{File.WriteAllText(path,$"<svg xmlns='http://www.w3.org/2000/svg' {attributes}>{body}</svg>");return SvgImportEngine.Import(path,unit);}finally{File.Delete(path);}
    }
    public static void Coordinates()
    {
        var d=Read("<circle cx='20' cy='30' r='10'/>").Document;Eq(100,d.Material.Width);Eq(100,d.Material.Height);Eq(20,d.Cuts[0].CenterX);Eq(70,d.Cuts[0].CenterY);Check(d.Cuts[0].Geometry.Single() is CircleSegment,"native circle");
        foreach(var (unitName,dimension) in new[]{("mm",25.4),("cm",2.54),("in",1d),("pt",72d),("pc",6d),("px",96d),("",96d)})
        {var a=Read("<circle cx='48' cy='48' r='10'/>",$"width='{dimension.ToString(CultureInfo.InvariantCulture)}{unitName}' height='{dimension.ToString(CultureInfo.InvariantCulture)}{unitName}' viewBox='0 0 96 96'").Document;Eq(25.4,a.Material.Width);Eq(12.7,a.Cuts[0].CenterX);}
        var shifted=Read("<circle cx='0' cy='0' r='5'/>","width='40mm' height='20mm' viewBox='-20 -10 40 20'").Document;Eq(20,shifted.Cuts[0].CenterX);Eq(10,shifted.Cuts[0].CenterY);
        var meet=Read("<circle cx='50' cy='50' r='10'/>","width='100mm' height='50mm' viewBox='0 0 100 100'").Document;Eq(50,meet.Cuts[0].CenterX);Eq(25,meet.Cuts[0].CenterY);Eq(10,meet.Cuts[0].Width);
        var none=Read("<circle cx='50' cy='50' r='10'/>","width='100mm' height='50mm' viewBox='0 0 100 100' preserveAspectRatio='none'");Check(none.Document.Cuts[0].Shape=="Compound"&&none.Warnings.Count>0,"nonuniform circle flatten");Eq(20,none.Document.Cuts[0].Width,.02);Eq(10,none.Document.Cuts[0].Height,.02);
        var scaled=Read("<g transform='translate(20 10) scale(2)'><circle cx='10' cy='15' r='2'/></g>").Document;Eq(40,scaled.Cuts[0].CenterX);Eq(60,scaled.Cuts[0].CenterY);Eq(8,scaled.Cuts[0].Width);
        var nested=Read("<g transform='translate(10 20)'><g transform='rotate(90)'><line x1='5' y1='-10' x2='10' y2='-10'/></g></g>").Document;var l=(LineSegment)nested.Slits[0].Geometry[0];Eq(20,l.X1);Eq(75,l.Y1);Eq(70,l.Y2);
        var matrix=Read("<line x1='10' y1='10' x2='20' y2='10' transform='matrix(1 0 .5 1 10 5)'/>").Document;var ml=(LineSegment)matrix.Slits[0].Geometry[0];Eq(25,ml.X1);Eq(85,ml.Y1);Eq(35,ml.X2);
        var local=Read("<defs><rect id='shape' x='0' y='0' width='10' height='5'/></defs><use href='#shape' x='20' y='30' transform='scale(2)'/>").Document;Eq(50,local.Cuts[0].CenterX);Eq(35,local.Cuts[0].CenterY);Eq(20,local.Cuts[0].Width);
        foreach(var unit in new[]{MeasurementUnit.Centimeter,MeasurementUnit.Meter}){var physical=Read("<rect x='10' y='10' width='20' height='30'/>",unit:unit).Document;Eq(.1,physical.Material.Width*unit.Metres());Eq(.02,physical.Cuts[0].Width*unit.Metres());}
        var primitives=Read("<rect x='5' y='5' width='20' height='10'/><rect x='40' y='5' width='20' height='10' rx='2'/><ellipse cx='30' cy='40' rx='10' ry='5'/><polygon points='10,60 20,60 15,70'/><polyline points='40,60 50,60 50,70'/><line x1='10' y1='80' x2='20' y2='90'/>");Check(primitives.ClosedObjects==4&&primitives.OpenObjects==2,"basic shapes");Check(primitives.Document.Cuts.All(c=>CutContourEngine.Loops(c.Geometry).Count==1),"closed primitives");
    }
    public static void Paths()
    {
        var path=Read("<path d='M10 10h10v10H10z M40 40 l10 0 0 10 -10 0z M10 60 L20 70'/>");Check(path.ClosedObjects==1&&path.OpenObjects==1,"multiple closed and open subpaths");Check(CutContourEngine.Loops(path.Document.Cuts[0].Geometry).Count==2,"compound multi islands");
        var curves=Read("<path d='M10 20 C15 5 25 5 30 20 S45 35 50 20 Q55 5 60 20 T70 20 A10 5 30 0 1 90 20'/><path d='m10 60 c5 -10 15 -10 20 0 s15 10 20 0 q5 -10 10 0 t10 0 a10 5 0 0 1 20 0'/>");Check(curves.OpenObjects==2&&curves.Warnings.Count>0,"all curve commands");Check(curves.Document.Slits.All(s=>s.Geometry.Count>10),"adaptive curve flatten");
        Check(Read("<path d='M10 10A10 10 0 01 30 10'/>").OpenObjects==1,"compact SVG arc flags");
        var bezier=Read("<path d='M10 50 C10 10 90 10 90 50'/>").Document.Slits.Single().Geometry.Cast<LineSegment>().ToArray();
        for(var i=0;i<=100;i++){var t=i/100d;var u=1-t;var p=new Point(10*u*u*u+30*u*u*t+270*u*t*t+90*t*t*t,100-(50*u*u*u+30*u*u*t+30*u*t*t+50*t*t*t));Check(bezier.Min(l=>Distance(p,l))<.04,"physical curve approximation");}
        var rings="M10 10H90V90H10Z M30 30H70V70H30Z";
        var even=Read($"<path fill-rule='evenodd' d='{rings}'/>").Document.Cuts[0];var nonzero=Read($"<path d='{rings}'/>").Document.Cuts[0];Check(!CutContourEngine.Contains(even,new(50,50))&&CutContourEngine.Contains(nonzero,new(50,50)),"SVG fill winding and islands");Check(CutContourEngine.Loops(nonzero.Geometry).Count==1,"no redundant nonzero interior cuts");
        var star=Read("<path d='M50 10L75 85L10 40L90 40L25 85Z'/>").Document;Check(CutContourEngine.Contains(star.Cuts[0],new(50,45))&&star.Cuts[0].Geometry.Count==10,"self-crossing exterior profile only");
        var d=path.Document;Check(BentSurfaceEngine.Build(d).Triangles.Count>0,"3D mesh");var history=new UndoRedoManager();history.Record(d);Check(ShapeClipboard.TryCapture(d,d.Cuts[0],out var copied)&&ShapeClipboard.TryPrepare(d,copied,1,out var prepared,out var joints),"import clipboard");ShapeClipboard.TryPrepare(d,copied,1,out prepared,out joints);ShapeClipboard.Insert(d,prepared!,joints);Check(history.Undo(d)!.Cuts.Count==1,"imported edit undo");
        var saved=Path.Combine(Path.GetTempPath(),"svg-dxf-"+Guid.NewGuid()+".dxf");try{VCuttingDxfSerializer.Save(d,saved);var loaded=VCuttingDxfSerializer.Load(saved);Check(loaded.Cuts.Count==2&&loaded.Slits.Count==1&&loaded.Cuts[0].Geometry.SequenceEqual(d.Cuts[0].Geometry),"DXF roundtrip");}finally{File.Delete(saved);}
    }
    static double Distance(Point p,LineSegment l){var a=new Point(l.X1,l.Y1);var v=new Vector(l.X2-l.X1,l.Y2-l.Y1);var t=Math.Clamp(Vector.Multiply(p-a,v)/v.LengthSquared,0,1);return(p-(a+t*v)).Length;}
    public static void Validation()
    {
        var visible=Read("<g style='display:none'><circle cx='10' cy='10' r='5'/></g><circle opacity='0' cx='20' cy='20' r='5'/><circle cx='50' cy='50' r='5'/><text x='5' y='5'>a</text><image href='https://invalid.example/file.png'/><script>throw 'do not execute'</script><style>.a{display:none}</style><path style='transform:translate(10px)' d='M10 10H20V20Z'/><path clip-path='url(#clip)' d='M10 10H20V20Z'/><use href='https://invalid.example/x.svg#x'/>");Check(visible.ClosedObjects==1&&visible.Warnings.Count>=6,"hidden and unsupported no network");
        foreach(var body in new[]{"<path d='M0 0Lbad'/>","<path d='M1e999 0L2 2'/>","<circle cx='200' cy='10' r='5'/>","<rect width='-1' height='5'/>","<g transform='scale(0)'><circle r='3'/></g>","<g id='a'><use href='#a'/></g>","<path d='M1 1L2 2' transform='bad(1)'/>","<circle cx='5' cy='5' r='2' id='same'/><circle cx='10' cy='10' r='2' id='same'/>","<text>only unsupported</text>"})
        {var rejected=false;try{Read(body);}catch(InvalidDataException){rejected=true;}Check(rejected,"invalid rejected "+body);}
        foreach(var attributes in new[]{"width='100%' height='20mm'","width='10mm' height='10mm' viewBox='0 0 0 100'","width='10mm' height='10mm' viewBox='0 0 100 100' preserveAspectRatio='bad'"}){var rejected=false;try{Read("<circle cx='5' cy='5' r='2'/>",attributes);}catch(InvalidDataException){rejected=true;}Check(rejected,"bad viewport");}
        var file=Path.Combine(Path.GetTempPath(),"svg-xxe-"+Guid.NewGuid()+".svg");try{File.WriteAllText(file,"<!DOCTYPE svg [<!ENTITY xxe SYSTEM 'file:///nonexistent-private-file'>]><svg xmlns='http://www.w3.org/2000/svg'><text>&xxe;</text></svg>");var original=File.ReadAllText(file);var rejected=false;try{SvgImportEngine.Import(file);}catch(InvalidDataException){rejected=true;}Check(rejected&&File.ReadAllText(file)==original,"DTD rejected/source unchanged");}finally{File.Delete(file);}
        var culture=CultureInfo.CurrentCulture;try{CultureInfo.CurrentCulture=CultureInfo.GetCultureInfo("de-DE");var d=Read("<circle cx='10.5' cy='20.5' r='2.5'/>").Document;Eq(10.5,d.Cuts[0].CenterX);Eq(79.5,d.Cuts[0].CenterY);}finally{CultureInfo.CurrentCulture=culture;}
        var complexityRejected=false;try{Read(string.Concat(Enumerable.Repeat("<circle cx='50' cy='50' r='5'/>",501)));}catch(InvalidDataException){complexityRejected=true;}Check(complexityRejected,"shape count limit");
        var style=Read("<defs><style>.a{transform:translate(5px)}</style></defs><rect x='10' y='10' width='10' height='10'/>");Check(style.Warnings.Any(w=>w.Contains("style")),"stylesheet in definitions reported");
        var depthRejected=false;try{Read(string.Concat(Enumerable.Repeat("<g>",66))+"<circle cx='50' cy='50' r='5'/>"+string.Concat(Enumerable.Repeat("</g>",66)));}catch(InvalidDataException){depthRejected=true;}Check(depthRejected,"depth limit");
        foreach(var body in new[]{"<circle r='"+new string('9',200)+"'/>","<path d='M10 10L20 20' transform='"+new string('a',20001)+"'/>","<g transform='scale(1e8)'><g transform='scale(1e8)'><path d='M1 1Q2 4 3 1'/></g></g>"}){var rejected=false;try{Read(body);}catch(InvalidDataException){rejected=true;}Check(rejected,"bounded numeric/transform input");}
    }
}
