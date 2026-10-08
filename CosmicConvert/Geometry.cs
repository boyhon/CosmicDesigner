using System.Windows;
namespace CosmicConvert;

public sealed class ConversionException(int code,string message) : Exception(message) { public int Code { get; }=code; }
public sealed record Entity(string Kind,Point A,Point B,Point Center,double Radius=0,double Start=0,double Sweep=0,double Bound=0)
{
    public static Entity Line(Point a,Point b,double bound=0)=>new("LINE",a,b,default,Bound:bound);
    public static Entity Arc(Point a,Point b,Point c,double r,double start,double sweep,double bound=0)=>new("ARC",a,b,c,r,start,sweep,bound);
    public Point At(double t)=>Kind=="LINE"?A+(B-A)*t:Center+new Vector(Radius*Math.Cos(Start+Sweep*t),Radius*Math.Sin(Start+Sweep*t));
}
public sealed record Curve(Point[]? Bezier,Point Center,Vector U,Vector V,double Start,double Sweep)
{
    public Point At(double t)
    {
        if(Bezier is null)return Center+U*Math.Cos(Start+Sweep*t)+V*Math.Sin(Start+Sweep*t);
        var p=Bezier.ToArray();for(int k=p.Length-1;k>0;k--)for(int i=0;i<k;i++)p[i]=p[i]+(p[i+1]-p[i])*t;return p[0];
    }
    public (Curve,Curve) Split()
    {
        if(Bezier is null)return (this with{Sweep=Sweep/2},this with{Start=Start+Sweep/2,Sweep=Sweep/2});
        var p=Bezier.ToArray();var a=new Point[p.Length];var b=new Point[p.Length];a[0]=p[0];b[^1]=p[^1];
        for(int k=p.Length-1;k>0;k--){for(int i=0;i<k;i++)p[i]=p[i]+(p[i+1]-p[i])*.5;a[p.Length-k]=p[0];b[k-1]=p[k-1];}
        return(this with{Bezier=a},this with{Bezier=b});
    }
    // Parameter-wise bound: Bezier convex hull relative to linear interpolation,
    // or interpolation error <= max|f''|/8 for a transformed elliptic arc.
    public double ChordBound(Point a,Point b)
    {
        if(Bezier is null)return Math.Max((At(0)-a).Length,(At(1)-b).Length)+(U.Length+V.Length)*Sweep*Sweep/8;
        double bound=0;for(int i=0;i<Bezier.Length;i++)bound=Math.Max(bound,(Bezier[i]-(a+(b-a)*(i/(double)(Bezier.Length-1)))).Length);return bound;
    }
}
public sealed record Outline(List<Curve> Curves,bool Closed,string Fill,string FillRule,string Stroke,string Source);
public sealed record SvgDrawing(double Width,double Height,List<Outline> Outlines);
public sealed record ConvertedDrawing(SvgDrawing Source,List<List<Entity>> Groups,double ErrorBound)
{
    public IEnumerable<Entity> Entities=>Groups.SelectMany(g=>g).DistinctBy(Key);
    static string Key(Entity e)
    {
        string P(Point p)=>FormattableString.Invariant($"{p.X:G17},{p.Y:G17}");
        if(e.Kind=="CIRCLE")return FormattableString.Invariant($"C:{P(e.Center)}:{e.Radius:G17}");
        var a=P(e.A);var b=P(e.B);if(string.CompareOrdinal(a,b)>0)(a,b)=(b,a);
        if(e.Kind=="LINE")return "L:"+a+":"+b;
        // Midpoint distinguishes major/minor arcs sharing endpoints. Reversed paths deduplicate.
        return FormattableString.Invariant($"A:{P(e.Center)}:{e.Radius:G17}:{a}:{b}:{P(e.At(.5))}");
    }
}

public static class Optimizer
{
    public const double Tolerance=.05;
    public static ConvertedDrawing Convert(SvgDrawing drawing,CancellationToken token=default,IProgress<string>? progress=null)
    {
        progress?.Report("Optimizing curves");var groups=new List<List<Entity>>();int count=0;
        foreach(var outline in drawing.Outlines)
        {
            var result=new List<Entity>();foreach(var curve in outline.Curves)Fit(curve,0,result);groups.Add(result);
        }
        if(count==0)throw new ConversionException(4,"SVG contains no cutting paths.");
        progress?.Report("Validating contours");FillBoundaries(drawing,groups,token);
        for(int i=0;i<groups.Count;i++)
        {
            var g=groups[i];for(int j=1;j<g.Count;j++)if((g[j-1].At(1)-g[j].At(0)).Length>1e-7)throw new ConversionException(5,"Disconnected output path.");
            if(drawing.Outlines[i].Closed&&g.Count>0&&(g[^1].At(1)-g[0].At(0)).Length>1e-7)throw new ConversionException(5,"Output contour did not close.");
        }
        if(groups.All(g=>g.Count==0))throw new ConversionException(4,"SVG contains no cutting boundary.");
        return new(drawing,groups,groups.SelectMany(g=>g).Select(e=>e.Bound).DefaultIfEmpty().Max()+1e-9);
        void Fit(Curve c,int depth,List<Entity> result)
        {
            token.ThrowIfCancellationRequested();if(depth>24||count>100000)throw new ConversionException(5,"Geometry resource limit reached.");
            if(c.Bezier is{} coincident&&coincident.All(p=>p==coincident[0]))return;
            var a=c.At(0);var b=c.At(1);if(c.Bezier is {Length:2}){if(a!=b)Add(Entity.Line(a,b));return;}
            // Exact circle/arc under a similarity transform, including reflected axes.
            if(c.Bezier is null&&Math.Abs(c.U.Length-c.V.Length)<=1e-12*Math.Max(1,c.U.Length)&&Math.Abs(Vector.Multiply(c.U,c.V))<=1e-12*Math.Max(1,c.U.Length*c.V.Length))
            {
                var start=Math.Atan2(a.Y-c.Center.Y,a.X-c.Center.X);var sweep=c.Sweep*Math.Sign(Vector.CrossProduct(c.U,c.V));
                if(Math.Abs(Math.Abs(sweep)-2*Math.PI)<1e-10)Add(new("CIRCLE",a,a,c.Center,c.U.Length,start,sweep));
                else Add(Entity.Arc(a,b,c.Center,c.U.Length,start,sweep));return;
            }
            double lineBound=c.ChordBound(a,b);
            if(lineBound<=Tolerance-1e-8&&a!=b){Add(Entity.Line(a,b,lineBound));return;}
            if(TryArc(a,c.At(.5),b,out var candidate)&&Certify(c,candidate,0,out var bound)){Add(candidate with{Bound=bound});return;}
            var (left,right)=c.Split();Fit(left,depth+1,result);Fit(right,depth+1,result);
            void Add(Entity e)
            {
                if(!double.IsFinite(e.Radius)||new[]{e.A.X,e.A.Y,e.B.X,e.B.Y,e.Center.X,e.Center.Y}.Any(v=>!double.IsFinite(v)||Math.Abs(v)>1e7))throw new ConversionException(5,"Non-finite or excessive coordinates.");
                if(e.Kind=="LINE"&&e.Bound==0&&result.LastOrDefault() is{Kind:"LINE",Bound:0} previous&&previous.B==e.A&&Vector.CrossProduct(previous.B-previous.A,e.B-e.A)==0&&Vector.Multiply(previous.B-previous.A,e.B-e.A)>0){result[^1]=Entity.Line(previous.A,e.B);return;}
                result.Add(e);count++;
            }
        }
    }
    static bool TryArc(Point a,Point m,Point b,out Entity e)
    {
        e=Entity.Line(a,b);var u=m-a;var v=b-a;double d=2*Vector.CrossProduct(u,v);if(Math.Abs(d)<1e-12)return false;
        var center=a+new Vector((u.LengthSquared*v.Y-v.LengthSquared*u.Y)/d,(v.LengthSquared*u.X-u.LengthSquared*v.X)/d);var r=(a-center).Length;if(!double.IsFinite(r)||r<1e-8||r>1e7)return false;
        double start=Math.Atan2(a.Y-center.Y,a.X-center.X),mid=Math.Atan2(m.Y-center.Y,m.X-center.X),end=Math.Atan2(b.Y-center.Y,b.X-center.X);
        double sweep=Positive(end-start);if(Positive(mid-start)>sweep)sweep-=2*Math.PI;if(Math.Abs(sweep)<1e-10||Math.Abs(sweep)>Math.PI*1.9)return false;
        e=Entity.Arc(a,b,center,r,start,sweep);return true;
    }
    static double Positive(double angle)=>(angle%(2*Math.PI)+2*Math.PI)%(2*Math.PI);
    static bool Certify(Curve c,Entity arc,int depth,out double bound)
    {
        // Both curves use the same t: triangle inequality through the arc endpoint chord.
        bound=c.ChordBound(arc.At(0),arc.At(1))+arc.Radius*arc.Sweep*arc.Sweep/8;
        if(bound<=Tolerance-1e-8)return true;if(depth>=10)return false;
        var(l,r)=c.Split();var mid=arc.At(.5);
        var left=arc with{B=mid,Sweep=arc.Sweep/2};var right=arc with{A=mid,Start=arc.Start+arc.Sweep/2,Sweep=arc.Sweep/2};
        if(!Certify(l,left,depth+1,out var lb)||!Certify(r,right,depth+1,out var rb))return false;bound=Math.Max(lb,rb);return true;
    }
    static void FillBoundaries(SvgDrawing drawing,List<List<Entity>> groups,CancellationToken token)
    {
        // Nonintersecting nested fill rings: retain only transitions of the winding fill.
        // Complex intersecting fill is rejected rather than claiming certified Boolean output.
        var bySource=drawing.Outlines.Select((o,i)=>(o,i)).Where(x=>x.o.Fill!="none"&&x.o.Closed).GroupBy(x=>x.o.Source);
        foreach(var family in bySource)
        {
            var rings=family.Select(x=>(x.o,x.i,Points:Polygon(groups[x.i]))).ToList();
            // Sweep bounding boxes first; disjoint lettering must not exhaust the exact-pair budget.
            var edges=rings.SelectMany(r=>r.Points.Select((p,a)=>(Ring:r.i,Index:a,Count:r.Points.Count,A:p,B:r.Points[(a+1)%r.Points.Count])))
                .OrderBy(e=>Math.Min(e.A.X,e.B.X)).ToList();long comparisons=0,scanned=0;
            for(int a=0;a<edges.Count;a++)for(int b=a+1;b<edges.Count;b++)
            {
                token.ThrowIfCancellationRequested();var x=edges[a];var y=edges[b];
                if(Math.Min(y.A.X,y.B.X)>Math.Max(x.A.X,x.B.X))break;
                if(++scanned>20000000)throw new ConversionException(5,"Fill topology resource limit.");
                if(x.Ring==y.Ring&&(Math.Abs(x.Index-y.Index)==1||Math.Abs(x.Index-y.Index)==x.Count-1))continue;
                if(Math.Max(x.A.Y,x.B.Y)<Math.Min(y.A.Y,y.B.Y)||Math.Max(y.A.Y,y.B.Y)<Math.Min(x.A.Y,x.B.Y))continue;
                if(++comparisons>4000000)throw new ConversionException(5,"Fill topology resource limit.");
                if(Intersects(x.A,x.B,y.A,y.B))throw new ConversionException(4,"Intersecting/touching fill boundaries are not supported. Use nonintersecting contours or stroke paths.");
            }
            foreach(var ring in rings)
            {
                int winding=0,depth=0;foreach(var other in rings.Where(x=>x.i!=ring.i))if(Inside(other.Points,ring.Points[0])){depth++;winding+=Math.Sign(Area(other.Points));}
                bool outside=ring.o.FillRule=="evenodd"?depth%2==1:winding!=0;
                bool inside=ring.o.FillRule=="evenodd"?!outside:winding+Math.Sign(Area(ring.Points))!=0;
                if(outside==inside&&ring.o.Stroke=="none")groups[ring.i].Clear();
            }
        }
    }
    internal static List<Point> Polygon(List<Entity> entities)
    {
        var p=new List<Point>();foreach(var e in entities){int n=e.Kind=="LINE"?1:Math.Max(2,(int)Math.Ceiling(Math.Abs(e.Sweep)/.04));for(int i=0;i<n;i++)p.Add(e.At(i/(double)n));}return p;
    }
    static double Area(List<Point> p)=>p.Select((a,i)=>Vector.CrossProduct((Vector)a,(Vector)p[(i+1)%p.Count])).Sum()/2;
    static bool Inside(List<Point> p,Point q){bool inside=false;for(int i=0,j=p.Count-1;i<p.Count;j=i++)if((p[i].Y>q.Y)!=(p[j].Y>q.Y)&&q.X<(p[j].X-p[i].X)*(q.Y-p[i].Y)/(p[j].Y-p[i].Y)+p[i].X)inside=!inside;return inside;}
    static bool Intersects(Point a,Point b,Point c,Point d)
    {
        if(Math.Max(a.X,b.X)<Math.Min(c.X,d.X)||Math.Max(c.X,d.X)<Math.Min(a.X,b.X)||Math.Max(a.Y,b.Y)<Math.Min(c.Y,d.Y)||Math.Max(c.Y,d.Y)<Math.Min(a.Y,b.Y))return false;
        double x=Vector.CrossProduct(b-a,c-a),y=Vector.CrossProduct(b-a,d-a),u=Vector.CrossProduct(d-c,a-c),v=Vector.CrossProduct(d-c,b-c);return x*y<=0&&u*v<=0;
    }
}
