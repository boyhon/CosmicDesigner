using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
namespace CosmicConvert;

public static partial class SvgReader
{
    const double MmPerPx=25.4/96;
    static readonly Regex Number=new(@"[+-]?(?:\d+\.?\d*|\.\d+)(?:[eE][+-]?\d+)?",RegexOptions.CultureInvariant|RegexOptions.NonBacktracking);
    static ConversionException Invalid(string reason)=>new(4,"Invalid/unsupported SVG: "+reason);
    public static SvgDrawing Read(string path)
    {
        XDocument xml;
        try{if(new FileInfo(path).Length>8000000)throw Invalid("file limit");using var reader=XmlReader.Create(path,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null,MaxCharactersInDocument=8000000});xml=XDocument.Load(reader,LoadOptions.SetLineInfo);}
        catch(XmlException ex){throw Invalid(ex.Message);}catch(Exception ex)when(ex is IOException or UnauthorizedAccessException){throw new ConversionException(3,ex.Message);}
        var root=xml.Root;if(root is null||root.Name.LocalName!="svg")throw Invalid("root");
        if(root.Descendants().Any(e=>e.Name.LocalName=="style"))throw Invalid("stylesheet (including defs)");
        var view=Numbers((string?)root.Attribute("viewBox")??"");if(view.Length!=0&&(view.Length!=4||view[2]<=0||view[3]<=0))throw Invalid("viewBox");
        var width=RootLength((string?)root.Attribute("width"),view.Length==4?view[2]:300);var height=RootLength((string?)root.Attribute("height"),view.Length==4?view[3]:150);if(width<=0||height<=0||width>1e6||height>1e6)throw Invalid("viewport");
        var viewport=ViewBox(view,width,height,(string?)root.Attribute("preserveAspectRatio"));viewport.Append(new Matrix(MmPerPx,0,0,-MmPerPx,0,height*MmPerPx));
        var outlines=new List<Outline>();var ids=new Dictionary<string,XElement>();foreach(var e in root.DescendantsAndSelf()){if(ids.Count>10000)throw Invalid("element limit");var id=(string?)e.Attribute("id");if(id is not null&&!ids.TryAdd(id,e))throw Invalid("duplicate id");}
        var active=new HashSet<XElement>();int shapes=0,curves=0,visits=0;
        Walk(root,viewport,"black","none","nonzero","visible",0,"root");
        if(outlines.Count==0)throw Invalid("no visible geometry");
        foreach(var c in outlines.SelectMany(o=>o.Curves))foreach(var p in c.Bezier??[c.Center,c.Center+c.U,c.Center+c.V])if(!double.IsFinite(p.X)||!double.IsFinite(p.Y)||Math.Abs(p.X)>1e7||Math.Abs(p.Y)>1e7)throw Invalid("coordinate limit");
        if(xml.Nodes().OfType<XProcessingInstruction>().Any(p=>p.Target=="xml-stylesheet"))throw Invalid("external stylesheet");return new(width*MmPerPx,height*MmPerPx,outlines);
        void Walk(XElement e,Matrix parent,string inheritedFill,string inheritedStroke,string inheritedRule,string inheritedVisibility,int depth,string source)
        {
            if(depth>64||++visits>10000||!active.Add(e))throw Invalid("reference/depth limit");
            try
            {
                var name=e.Name.LocalName;var location=$"{name} line {((IXmlLineInfo)e).LineNumber}";
                string Property(string key,string fallback="")
                {
                    var v=(string?)e.Attribute(key)??fallback;foreach(var rule in ((string?)e.Attribute("style")??"").Split(';')){var p=rule.Split(':',2);if(p.Length==2&&p[0].Trim()==key)v=p[1].Trim();}return v;
                }
                if(e.Name.NamespaceName is not ("" or "http://www.w3.org/2000/svg"))throw Invalid(location+" namespace");
                var opacity=Property("opacity");if(Property("display")=="none"||opacity.Length>0&&double.TryParse(opacity.TrimEnd('%'),NumberStyles.Float,CultureInfo.InvariantCulture,out var alpha)&&alpha==0)return;
                var visibility=Property("visibility",inheritedVisibility);var fill=Property("fill",inheritedFill);var stroke=Property("stroke",inheritedStroke);var rule=Property("fill-rule",inheritedRule);if(rule is not ("nonzero" or "evenodd"))throw Invalid(location+" fill-rule");
                if(new[]{"clip-path","mask","filter"}.Any(k=>Property(k) is not ("" or "none")))throw Invalid(location+" clipping/mask/filter");
                foreach(var entry in Property("style").Split(';'))if(entry.Split(':',2)[0].Trim() is "transform" or "d" or "x" or "y" or "width" or "height" or "cx" or "cy" or "r" or "rx" or "ry")throw Invalid(location+" CSS geometry");
                var matrix=Transform((string?)e.Attribute("transform"));matrix.Append(parent);
                if(name is "defs" or "metadata" or "title" or "desc" or "symbol")return;
                if(name is "svg" or "g" or "a")
                {if(name=="svg"&&!ReferenceEquals(root,e))throw Invalid(location+" nested viewport");foreach(var child in e.Elements())Walk(child,matrix,fill,stroke,rule,visibility,depth+1,source);return;}
                if(name=="use")
                {
                    var href=(string?)e.Attribute("href")??(string?)e.Attribute(XName.Get("href","http://www.w3.org/1999/xlink"));if(href is null||!href.StartsWith('#')||!ids.TryGetValue(href[1..],out var target))throw Invalid(location+" external/missing use");
                    if(target.Name.LocalName is "svg" or "symbol")throw Invalid(location+" use viewport");var offset=Matrix.Identity;offset.Translate(Value(e,"x"),Value(e,"y"));offset.Append(matrix);Walk(target,offset,fill,stroke,rule,visibility,depth+1,source+"/use"+visits);return;
                }
                if(visibility is "hidden" or "collapse")return;
                // Exporters may retain an invisible accessibility label next to outlined text.
                // A text subtree can override paint properties, so only a proven invisible leaf is skipped.
                bool ZeroPaint(string key)=>double.TryParse(Property(key).TrimEnd('%'),NumberStyles.Float,CultureInfo.InvariantCulture,out var paintAlpha)&&paintAlpha==0;
                if(name=="text"&&!e.HasElements&&(fill=="none"||ZeroPaint("fill-opacity"))&&(stroke=="none"||ZeroPaint("stroke-opacity")))return;
                if(name is not ("path" or "line" or "polyline" or "polygon" or "rect" or "circle" or "ellipse"))throw Invalid(location);
                if(fill=="none"&&stroke=="none")return;if(++shapes>2000)throw Invalid("shape limit");source+="/"+shapes;
                if(name is "circle" or "ellipse")
                {
                    var rx=Value(e,name=="circle"?"r":"rx");var ry=name=="circle"?rx:Value(e,"ry");if(rx<0||ry<0)throw Invalid(location);if(rx==0||ry==0)return;
                    var c=new Curve(null,matrix.Transform(new Point(Value(e,"cx"),Value(e,"cy"))),matrix.Transform(new Vector(rx,0)),matrix.Transform(new Vector(0,ry)),0,2*Math.PI);outlines.Add(new([c],true,fill,rule,stroke,source));return;
                }
                PathGeometry geometry;try{geometry=Shape(e);}catch(Exception ex)when(ex is FormatException or ArgumentException){throw Invalid(location+": "+ex.Message);}
                foreach(var f in geometry.Figures)
                {
                    var items=new List<Curve>();var start=f.StartPoint;var prev=start;
                    void Line(Point p){if(prev!=p)items.Add(new([matrix.Transform(prev),matrix.Transform(p)],default,default,default,0,0));prev=p;}
                    void Bezier(Point a,Point b,Point p){items.Add(new([matrix.Transform(prev),matrix.Transform(a),matrix.Transform(b),matrix.Transform(p)],default,default,default,0,0));prev=p;}
                    void Quadratic(Point a,Point p){Bezier(prev+(a-prev)*(2d/3),p+(a-p)*(2d/3),p);}
                    foreach(var s in f.Segments)switch(s)
                    {
                        case LineSegment l:Line(l.Point);break;
                        case PolyLineSegment l:foreach(var p in l.Points)Line(p);break;
                        case BezierSegment b:Bezier(b.Point1,b.Point2,b.Point3);break;
                        case PolyBezierSegment b:for(int i=0;i<b.Points.Count;i+=3)Bezier(b.Points[i],b.Points[i+1],b.Points[i+2]);break;
                        case QuadraticBezierSegment q:Quadratic(q.Point1,q.Point2);break;
                        case PolyQuadraticBezierSegment q:for(int i=0;i<q.Points.Count;i+=2)Quadratic(q.Points[i],q.Points[i+1]);break;
                        case ArcSegment a:var arc=EllipseArc(prev,a,matrix);if(arc is not null)items.Add(arc);else Line(a.Point);prev=a.Point;break;
                        default:throw Invalid(location+" segment");
                    }
                    if(f.IsClosed)Line(start);if((curves+=items.Count)>10000)throw Invalid("curve limit");if(items.Count>0)outlines.Add(new(items,f.IsClosed,fill,rule,stroke,source));
                }
            }
            finally{active.Remove(e);}
        }
    }
    static Curve? EllipseArc(Point from,ArcSegment arc,Matrix matrix)
    {
        if(from==arc.Point)return null;double rx=Math.Abs(arc.Size.Width),ry=Math.Abs(arc.Size.Height);if(rx==0||ry==0)return null;
        double phi=arc.RotationAngle*Math.PI/180,co=Math.Cos(phi),si=Math.Sin(phi);var half=(from-arc.Point)/2;double x=co*half.X+si*half.Y,y=-si*half.X+co*half.Y;
        double lambda=x*x/(rx*rx)+y*y/(ry*ry);if(lambda>1){rx*=Math.Sqrt(lambda);ry*=Math.Sqrt(lambda);}
        bool sweep=arc.SweepDirection==SweepDirection.Clockwise;double factor=Math.Sqrt(Math.Max(0,(rx*rx*ry*ry-rx*rx*y*y-ry*ry*x*x)/(rx*rx*y*y+ry*ry*x*x)));if(arc.IsLargeArc==sweep)factor=-factor;
        double cx=factor*rx*y/ry,cy=-factor*ry*x/rx;var center=new Point(co*cx-si*cy+(from.X+arc.Point.X)/2,si*cx+co*cy+(from.Y+arc.Point.Y)/2);
        var a=new Vector((x-cx)/rx,(y-cy)/ry);var b=new Vector((-x-cx)/rx,(-y-cy)/ry);double start=Math.Atan2(a.Y,a.X),delta=Math.Atan2(Vector.CrossProduct(a,b),Vector.Multiply(a,b));if(sweep&&delta<0)delta+=2*Math.PI;if(!sweep&&delta>0)delta-=2*Math.PI;
        return new(null,matrix.Transform(center),matrix.Transform(new Vector(rx*co,rx*si)),matrix.Transform(new Vector(-ry*si,ry*co)),start,delta);
    }
    static PathGeometry Shape(XElement e)
    {
        var name=e.Name.LocalName;
        if(name=="path"){var data=(string?)e.Attribute("d")??"";if(data.Length==0)return new();if(data.Length>200000||!Regex.IsMatch(data,@"^[MmLlHhVvCcSsQqTtAaZzEe0-9+.,\s-]+$"))throw Invalid("path");Numbers(Regex.Replace(data,@"[MmLlHhVvCcSsQqTtAaZz]"," "));return PathGeometry.CreateFromGeometry(Geometry.Parse(data));}
        if(name=="rect")
        {
            double w=Value(e,"width"),h=Value(e,"height"),rx=Value(e,"rx",Value(e,"ry")),ry=Value(e,"ry",rx),x=Value(e,"x"),y=Value(e,"y");if(w<0||h<0||rx<0||ry<0)throw Invalid("rect");if(w==0||h==0)return new();rx=Math.Min(rx,w/2);ry=Math.Min(ry,h/2);
            if(rx==0||ry==0)rx=ry=0;var rectangleFigure=new PathFigure{StartPoint=new(x+rx,y),IsClosed=true};
            void Line(double a,double b)=>rectangleFigure.Segments.Add(new LineSegment(new(a,b),true));
            void Arc(double a,double b){if(rx==0||ry==0)Line(a,b);else rectangleFigure.Segments.Add(new ArcSegment(new(a,b),new Size(rx,ry),0,false,SweepDirection.Clockwise,true));}
            Line(x+w-rx,y);Arc(x+w,y+ry);Line(x+w,y+h-ry);Arc(x+w-rx,y+h);Line(x+rx,y+h);Arc(x,y+h-ry);Line(x,y+ry);Arc(x+rx,y);return new([rectangleFigure]);
        }
        Point[] points;if(name=="line")points=[new(Value(e,"x1"),Value(e,"y1")),new(Value(e,"x2"),Value(e,"y2"))];else{var n=Numbers((string?)e.Attribute("points")??"");if(n.Length%2!=0)throw Invalid("points");points=n.Chunk(2).Select(p=>new Point(p[0],p[1])).ToArray();}
        if(points.Length<2)return new();var f=new PathFigure{StartPoint=points[0],IsClosed=name=="polygon"};f.Segments.Add(new PolyLineSegment(points.Skip(1),true));return new([f]);
    }
}
