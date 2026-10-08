using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace VCutting;
public sealed record SvgImportResult(VCuttingDocument Document,int ClosedObjects,int OpenObjects,IReadOnlyList<string> Warnings);
public static class SvgImportEngine
{
    const double MmPerPx=25.4/96, Tolerance=.01;
    static readonly Regex Number=new(@"[+-]?(?:\d+\.?\d*|\.\d+)(?:[eE][+-]?\d+)?",RegexOptions.CultureInvariant|RegexOptions.NonBacktracking);
    static InvalidDataException Invalid(string reason)=>new(Localization.Format("svg.invalid",reason));
    public static SvgImportResult Import(string path,MeasurementUnit unit=MeasurementUnit.Millimeter,double thicknessMm=2)
    {
        if(new FileInfo(path).Length>8_000_000)throw Invalid(Localization.Text("svg.limit"));
        using var reader=XmlReader.Create(path,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null,MaxCharactersInDocument=8_000_000});
        XDocument xml;try{xml=XDocument.Load(reader);}catch(XmlException){throw Invalid(Localization.Text("svg.xml"));}
        var root=xml.Root;if(root is null||root.Name.LocalName!="svg"||root.Name.NamespaceName is not ("" or "http://www.w3.org/2000/svg"))throw Invalid(Localization.Text("svg.xml"));
        if(root.Descendants().Take(10001).Count()>10000||!Enum.IsDefined(unit)||!double.IsFinite(thicknessMm)||thicknessMm<=0)throw Invalid(Localization.Text("svg.limit"));
        var view=Numbers((string?)root.Attribute("viewBox")??"");if(view.Length!=0&&(view.Length!=4||view[2]<=0||view[3]<=0))throw Invalid("viewBox");
        var width=RootLength((string?)root.Attribute("width"),view.Length==4?view[2]:300);var height=RootLength((string?)root.Attribute("height"),view.Length==4?view[3]:150);
        if(width<=0||height<=0||width*MmPerPx>10000||height*MmPerPx>10000)throw Invalid(Localization.Text("svg.limit"));
        var viewport=ViewBox(view,width,height,(string?)root.Attribute("preserveAspectRatio"));viewport.Append(new Matrix(MmPerPx,0,0,-MmPerPx,0,height*MmPerPx));
        var document=new VCuttingDocument();document.ConfigureNew(width*MmPerPx,height*MmPerPx,thicknessMm,MeasurementUnit.Millimeter);
        var warnings=new HashSet<string>();var ids=new Dictionary<string,XElement>();foreach(var element in root.DescendantsAndSelf()){var id=(string?)element.Attribute("id");if(!string.IsNullOrEmpty(id)&&!ids.TryAdd(id,element))throw Invalid("id: "+id);}
        if(root.Descendants().Any(e=>e.Name.LocalName=="style"))warnings.Add(Localization.Format("svg.skipped","style"));
        var shapes=0;var segments=0;var active=new HashSet<XElement>();
        Walk(root,viewport,"nonzero","visible",0,true);
        if(document.Cuts.Count+document.Slits.Count==0)throw Invalid(Localization.Text("svg.empty")+" "+string.Join("; ",warnings));
        document.Recalculate();if(unit!=MeasurementUnit.Millimeter)document.ChangeUnit(unit,UnitChangeMode.PreservePhysicalSize);
        return new(document,document.Cuts.Count,document.Slits.Count,warnings.ToArray());

        void Warn(string element)=>warnings.Add(Localization.Format("svg.skipped",element));
        void Walk(XElement e,Matrix parent,string inheritedFill,string inheritedVisibility,int depth,bool isRoot=false)
        {
            if(depth>64||!active.Add(e))throw Invalid(Localization.Text("svg.limit"));
            try
            {
                var name=e.Name.LocalName;if(e.Name.NamespaceName is not ("" or "http://www.w3.org/2000/svg")){Warn(name);return;}
                string Property(string key,string fallback="")
                {
                    var value=(string?)e.Attribute(key)??fallback;
                    foreach(var rule in ((string?)e.Attribute("style")??"").Split(';')){var pair=rule.Split(':',2);if(pair.Length==2&&pair[0].Trim()==key)value=pair[1].Trim();}return value;
                }
                if(Property("display")=="none")return;
                var opacity=Property("opacity");if(opacity.Length>0&&double.TryParse(opacity.TrimEnd('%'),NumberStyles.Float,CultureInfo.InvariantCulture,out var alpha)&&alpha==0)return;
                var inlineStyle=(string?)e.Attribute("style")??"";
                if(inlineStyle.Split(';').Select(r=>r.Split(':',2)[0].Trim()).Any(key=>key is "transform" or "d" or "x" or "y" or "width" or "height" or "cx" or "cy" or "r" or "rx" or "ry")){Warn(name+" (CSS geometry)");return;}
                var visibility=Property("visibility",inheritedVisibility);var fill=Property("fill-rule",inheritedFill);
                if(fill is not ("nonzero" or "evenodd"))throw Invalid("fill-rule");
                if(new[]{"clip-path","mask","filter"}.Any(key=>Property(key) is not ("" or "none"))){Warn(name+" (clip/mask/filter)");return;}
                var local=Transform((string?)e.Attribute("transform"));local.Append(parent);
                if(name=="svg"&&!isRoot){Warn("nested svg");return;}
                if(name is "svg" or "g" or "a"){foreach(var child in e.Elements())Walk(child,local,fill,visibility,depth+1);return;}
                if(name=="use")
                {
                    var href=(string?)e.Attribute("href")??(string?)e.Attribute(XName.Get("href","http://www.w3.org/1999/xlink"));
                    if(href is null||!href.StartsWith('#')||!ids.TryGetValue(href[1..],out var target)){Warn("use");return;}
                    if(target.Name.LocalName is "symbol" or "svg"){Warn("use viewport");return;}
                    var offset=Matrix.Identity;offset.Translate(Value(e,"x"),Value(e,"y"));offset.Append(local);Walk(target,offset,fill,visibility,depth+1);return;
                }
                if(name is "defs" or "symbol" or "metadata" or "title" or "desc")return;
                if(name is not ("path" or "line" or "polyline" or "polygon" or "rect" or "circle" or "ellipse")){Warn(name);return;}
                if(visibility is "hidden" or "collapse")return;
                if(++shapes>500)throw Invalid(Localization.Text("svg.limit"));
                Geometry? geometry;
                try{geometry=Shape(e);}catch(Exception ex) when(ex is FormatException or ArgumentException){throw Invalid(name);}
                if(geometry is null)return;
                var matrix=local;
                if(name=="circle"&&Similarity(matrix,out var scale))
                {
                    var center=matrix.Transform(new Point(Value(e,"cx"),Value(e,"cy")));var radius=Value(e,"r")*scale;CheckBounds([new(center.X-radius,center.Y-radius),new(center.X+radius,center.Y+radius)]);
                    var cut=new CutOperation{Id=$"H{document.Cuts.Count+1:000}",Sequence=document.NextSequence(),Kind=CutKind.Hole,Shape="Circle",CenterX=center.X,CenterY=center.Y,Width=2*radius,Height=2*radius,Radius=radius};cut.Geometry.Add(new CircleSegment(center.X,center.Y,radius));AddCut(cut);return;
                }
                var pathGeometry=PathGeometry.CreateFromGeometry(geometry);pathGeometry.FillRule=fill=="evenodd"?FillRule.EvenOdd:FillRule.Nonzero;
                var magnitude=Math.Sqrt(matrix.M11*matrix.M11+matrix.M12*matrix.M12+matrix.M21*matrix.M21+matrix.M22*matrix.M22);if(!double.IsFinite(magnitude)||magnitude<=0||magnitude>1e6)throw Invalid(Localization.Text("svg.limit"));var tolerance=Tolerance/magnitude;
                var closed=new PathGeometry{FillRule=pathGeometry.FillRule};var open=new PathGeometry();
                foreach(var f in pathGeometry.Figures)(f.IsClosed?closed:open).Figures.Add(f.Clone());
                if(name is "circle" or "ellipse"||pathGeometry.Figures.Any(f=>f.Segments.Any(s=>s is not (System.Windows.Media.LineSegment or PolyLineSegment))))warnings.Add(Localization.Text("svg.approx"));
                // Convert SVG winding to explicit boundaries compatible with Compound even-odd loops.
                if(closed.Figures.Count>0)
                {
                    var outline=closed.GetOutlinedPathGeometry(tolerance,ToleranceType.Absolute).GetFlattenedPathGeometry(tolerance,ToleranceType.Absolute);
                    var lines=Lines(outline,true,matrix);if(lines.Count>0){var points=lines.Cast<LineSegment>().SelectMany(l=>new[]{new Point(l.X1,l.Y1),new Point(l.X2,l.Y2)}).ToArray();CheckBounds(points);var x=points.Min(p=>p.X);var y=points.Min(p=>p.Y);var w=points.Max(p=>p.X)-x;var h=points.Max(p=>p.Y)-y;
                        var cut=new CutOperation{Id=$"H{document.Cuts.Count+1:000}",Sequence=document.NextSequence(),Kind=CutKind.Hole,Shape="Compound",CenterX=x+w/2,CenterY=y+h/2,Width=w,Height=h};cut.Geometry.AddRange(lines);if(CutContourEngine.Loops(lines).Count==0)throw Invalid(name);AddCut(cut);}
                }
                var flat=open.GetFlattenedPathGeometry(tolerance,ToleranceType.Absolute);
                foreach(var figure in flat.Figures)
                {
                    var lines=Lines(new PathGeometry([figure]),false,matrix);if(lines.Count==0)continue;CheckBounds(lines.Cast<LineSegment>().SelectMany(l=>new[]{new Point(l.X1,l.Y1),new Point(l.X2,l.Y2)}));
                    var slit=document.AddSlit(lines.Count==1?"Line":"Polyline",lines);if(slit is null)throw Invalid(name);
                }
            }
            finally{active.Remove(e);}
        }
        void AddCut(CutOperation cut){document.Cuts.Add(cut);var inner=new ContourObject{Id=cut.Id,Kind=ContourKind.Inner};inner.Segments.AddRange(cut.Geometry);document.InnerContours.Add(inner);}
        void CheckBounds(IEnumerable<Point> points){if(points.Any(p=>!double.IsFinite(p.X)||!double.IsFinite(p.Y)||p.X< -1e-5||p.Y< -1e-5||p.X>document.Material.Width+1e-5||p.Y>document.Material.Height+1e-5))throw Invalid(Localization.Text("svg.bounds"));}
        List<GeometrySegment> Lines(PathGeometry g,bool close,Matrix matrix)
        {
            var result=new List<GeometrySegment>();foreach(var f in g.Figures){var prev=f.StartPoint;foreach(var s in f.Segments){IEnumerable<Point> points=s switch{PolyLineSegment p=>p.Points,System.Windows.Media.LineSegment l=>[l.Point],_=>throw Invalid("curve")};foreach(var p in points){Add(prev,p);prev=p;}}if(close)Add(prev,f.StartPoint);}
            return result;
            void Add(Point a,Point b){a=matrix.Transform(a);b=matrix.Transform(b);if((a-b).Length<1e-8)return;if(++segments>2000)throw Invalid(Localization.Text("svg.limit"));result.Add(new LineSegment(a.X,a.Y,b.X,b.Y));}
        }
    }
    static Geometry? Shape(XElement e)
    {
        var name=e.Name.LocalName;
        if(name=="path"){var data=(string?)e.Attribute("d")??"";if(data.Length==0)return null;if(data.Length>200000||!Regex.IsMatch(data,@"^[MmLlHhVvCcSsQqTtAaZzEe0-9+.,\s-]+$",RegexOptions.CultureInvariant))throw Invalid("path");Numbers(Regex.Replace(data,@"[MmLlHhVvCcSsQqTtAaZz]"," "));return Geometry.Parse(data);}
        if(name is "circle" or "ellipse"){var rx=Value(e,name=="circle"?"r":"rx");var ry=name=="circle"?rx:Value(e,"ry");if(rx<0||ry<0)throw Invalid(name);return rx==0||ry==0?null:new EllipseGeometry(new Point(Value(e,"cx"),Value(e,"cy")),rx,ry);}
        if(name=="rect"){var w=Value(e,"width");var h=Value(e,"height");if(w<0||h<0)throw Invalid(name);if(w==0||h==0)return null;var rx=Value(e,"rx",Value(e,"ry"));var ry=Value(e,"ry",rx);if(rx<0||ry<0)throw Invalid(name);return new System.Windows.Media.RectangleGeometry(new Rect(Value(e,"x"),Value(e,"y"),w,h),Math.Min(rx,w/2),Math.Min(ry,h/2));}
        Point[] points;
        if(name=="line")points=[new(Value(e,"x1"),Value(e,"y1")),new(Value(e,"x2"),Value(e,"y2"))];
        else{var values=Numbers((string?)e.Attribute("points")??"");if(values.Length%2!=0)throw Invalid(name);points=values.Chunk(2).Select(p=>new Point(p[0],p[1])).ToArray();}
        if(points.Length<2)return null;var figure=new PathFigure{StartPoint=points[0],IsClosed=name=="polygon"};figure.Segments.Add(new PolyLineSegment(points.Skip(1),true));return new PathGeometry([figure]);
    }
    static double Value(XElement e,string name,double fallback=0)=>e.Attribute(name) is { } a?RootLength(a.Value,fallback):fallback;
    static double RootLength(string? value,double fallback)
    {
        if(value is null)return fallback;if(value.Length>128)throw Invalid(Localization.Text("svg.limit"));var match=Regex.Match(value.Trim(),@"^([+-]?(?:\d+\.?\d*|\.\d+)(?:[eE][+-]?\d+)?)(px|mm|cm|in|pt|pc)?$",RegexOptions.CultureInvariant|RegexOptions.NonBacktracking);if(!match.Success)throw Invalid(value);
        var number=double.Parse(match.Groups[1].Value,CultureInfo.InvariantCulture);var factor=match.Groups[2].Value switch{"mm"=>96/25.4,"cm"=>96/2.54,"in"=>96,"pt"=>96/72d,"pc"=>16,_=>1};if(!double.IsFinite(number)||Math.Abs(number)>1e8)throw Invalid(value);return number*factor;
    }
    static double[] Numbers(string text)
    {
        if(text.Length>200000)throw Invalid(Localization.Text("svg.limit"));var matches=Number.Matches(text);if(matches.Count>10000||!string.IsNullOrWhiteSpace(Number.Replace(text,"").Replace(",","")))throw Invalid(text.Length>64?text[..64]:text);
        var values=matches.Select(m=>double.Parse(m.Value,CultureInfo.InvariantCulture)).ToArray();if(values.Any(v=>!double.IsFinite(v)||Math.Abs(v)>1e8))throw Invalid(Localization.Text("svg.limit"));return values;
    }
    static Matrix ViewBox(double[] v,double width,double height,string? aspect)
    {
        if(v.Length==0)return Matrix.Identity;var sx=width/v[2];var sy=height/v[3];var dx=-v[0]*sx;var dy=-v[1]*sy;var parts=(aspect??"xMidYMid meet").Split(' ',StringSplitOptions.RemoveEmptyEntries);if(parts.Length==0)parts=["xMidYMid","meet"];
        if(parts[0]!="none")
        {
            if(!Regex.IsMatch(parts[0],"^x(Min|Mid|Max)Y(Min|Mid|Max)$")||parts.Length>2||parts.Length==2&&parts[1] is not ("meet" or "slice"))throw Invalid("preserveAspectRatio");
            var s=parts.Length==2&&parts[1]=="slice"?Math.Max(sx,sy):Math.Min(sx,sy);sx=sy=s;var ax=parts[0].StartsWith("xMin")?0:parts[0].StartsWith("xMid")?.5:1;var ay=parts[0].EndsWith("YMin")?0:parts[0].EndsWith("YMid")?.5:1;dx=-v[0]*s+(width-v[2]*s)*ax;dy=-v[1]*s+(height-v[3]*s)*ay;
        }
        else if(parts.Length>1)throw Invalid("preserveAspectRatio");
        return new Matrix(sx,0,0,sy,dx,dy);
    }
    static Matrix Transform(string? text)
    {
        if(string.IsNullOrWhiteSpace(text))return Matrix.Identity;if(text.Length>20000)throw Invalid(Localization.Text("svg.limit"));const RegexOptions options=RegexOptions.CultureInvariant|RegexOptions.NonBacktracking;var matches=Regex.Matches(text,@"([A-Za-z]+)\s*\(([^)]*)\)",options);if(!string.IsNullOrWhiteSpace(Regex.Replace(text,@"([A-Za-z]+)\s*\(([^)]*)\)","",options).Replace(",","")))throw Invalid("transform");var result=Matrix.Identity;
        foreach(Match match in matches){var p=Numbers(match.Groups[2].Value);var m=Matrix.Identity;switch(match.Groups[1].Value)
            {
                case "matrix" when p.Length==6:m=new(p[0],p[1],p[2],p[3],p[4],p[5]);break;
                case "translate" when p.Length is 1 or 2:m.Translate(p[0],p.Length==2?p[1]:0);break;
                case "scale" when p.Length is 1 or 2:m.Scale(p[0],p.Length==2?p[1]:p[0]);break;
                case "rotate" when p.Length is 1 or 3:if(p.Length==3)m.RotateAt(p[0],p[1],p[2]);else m.Rotate(p[0]);break;
                case "skewX" when p.Length==1:m=new(1,0,Math.Tan(p[0]*Math.PI/180),1,0,0);break;
                case "skewY" when p.Length==1:m=new(1,Math.Tan(p[0]*Math.PI/180),0,1,0,0);break;
                default:throw Invalid("transform");
            }result.Prepend(m);
        }if(!result.HasInverse)throw Invalid("transform");return result;
    }
    static bool Similarity(Matrix m,out double scale){var a=new Vector(m.M11,m.M12);var b=new Vector(m.M21,m.M22);scale=a.Length;return scale>0&&double.IsFinite(scale)&&Math.Abs(a.Length-b.Length)<scale*1e-8&&Math.Abs(Vector.Multiply(a,b))<a.Length*b.Length*1e-8;}
}
