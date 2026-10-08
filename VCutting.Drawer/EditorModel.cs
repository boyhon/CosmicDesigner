using System.Globalization;
using System.IO;
using System.Text;
using VCutting.Viewer;

namespace VCutting.Drawer;

public static class GeometryMath
{
    public const double Epsilon = .001;
    public static bool Near(double a, double b, double e = Epsilon) => Math.Abs(a-b) <= e;
    public static bool Near(DxfPoint a, DxfPoint b, double e = Epsilon) => Near(a.X,b.X,e)&&Near(a.Y,b.Y,e);
    public static double DegreesToRadians(double v)=>v*Math.PI/180;
    public static double RadiansToDegrees(double v)=>v*180/Math.PI;
    public static double GradiansToRadians(double v)=>v*Math.PI/200;
    public static double RadiansToGradians(double v)=>v*200/Math.PI;
    public static double Cross(DxfPoint a,DxfPoint b,DxfPoint c)=>(b.X-a.X)*(c.Y-a.Y)-(b.Y-a.Y)*(c.X-a.X);
}

public abstract record EditorEntity(int Id, string Layer)
{
    public abstract IEnumerable<DxfPoint> Points { get; }
    public abstract GeometryEntity ToGeometry();
}
public sealed record EditorLine(int EntityId,string EntityLayer,DxfPoint Start,DxfPoint End):EditorEntity(EntityId,EntityLayer)
{
    public double Dx=>End.X-Start.X; public double Dy=>End.Y-Start.Y; public double Length=>Math.Sqrt(Dx*Dx+Dy*Dy);
    public double AngleRadians=>Math.Atan2(Dy,Dx); public double AngleDegrees=>GeometryMath.RadiansToDegrees(AngleRadians);
    public double? Gradient=>Math.Abs(Dx)<=GeometryMath.Epsilon?null:Dy/Dx;
    public bool Horizontal=>Math.Abs(Dy)<=GeometryMath.Epsilon; public bool Vertical=>Math.Abs(Dx)<=GeometryMath.Epsilon;
    public override IEnumerable<DxfPoint> Points { get { yield return Start; yield return End; } }
    public override GeometryEntity ToGeometry()=>new GeometryLine{Id=Id,EntityType="LINE",LayerName=Layer,OriginalEntity=[],Start=Start,End=End};
}
public sealed record EditorCircle(int EntityId,string EntityLayer,DxfPoint Center,double Radius):EditorEntity(EntityId,EntityLayer)
{
    public override IEnumerable<DxfPoint> Points { get { yield return Center; } }
    public override GeometryEntity ToGeometry()=>new GeometryCircle{Id=Id,EntityType="CIRCLE",LayerName=Layer,OriginalEntity=[],Center=Center,Radius=Radius};
}
public enum ArcDirection { Clockwise, Counterclockwise }
public sealed record EditorArc(int EntityId,string EntityLayer,DxfPoint Center,double Radius,double StartAngle,double EndAngle,ArcDirection Direction=ArcDirection.Clockwise):EditorEntity(EntityId,EntityLayer)
{
    public double SweepAngle => GeometryArc.NormalizeAngle(EndAngle-StartAngle);
    public DxfPoint StartPoint=>At(StartAngle); public DxfPoint EndPoint=>At(EndAngle);
    public EditorArc WithStartAngle(double angle)=>this with{StartAngle=GeometryArc.NormalizeAngle(angle)};
    public EditorArc WithEndAngle(double angle)=>this with{EndAngle=GeometryArc.NormalizeAngle(angle)};
    public DxfPoint At(double degrees){var r=GeometryMath.DegreesToRadians(degrees);return new(Center.X+Radius*Math.Cos(r),Center.Y+Radius*Math.Sin(r));}
    public override IEnumerable<DxfPoint> Points { get { yield return Center;yield return StartPoint;yield return EndPoint; } }
    public override GeometryEntity ToGeometry()=>new GeometryArc{Id=Id,EntityType="ARC",LayerName=Layer,OriginalEntity=[],Center=Center,Radius=Radius,StartAngle=GeometryArc.NormalizeAngle(StartAngle),EndAngle=GeometryArc.NormalizeAngle(EndAngle)};
}

public sealed class DrawingDocument
{
    public List<EditorEntity> Entities { get; }=[]; public int NextId=>Entities.Count==0?1:Entities.Max(x=>x.Id)+1;
    public DrawingDocument Clone(){var d=new DrawingDocument();d.Entities.AddRange(Entities);return d;}
    public DxfDocument ToDxfDocument(string path="") { var d=new DxfDocument{FilePath=path}; foreach(var e in Entities){var g=e.ToGeometry();d.Entities.Add(g);d.Layers.Add(g.LayerName);g.ExpandBounds(d.Bounds);} return d; }
    public static DrawingDocument Load(string path){var source=DxfDocumentParser.Parse(path);var d=new DrawingDocument();foreach(var e in source.Entities)d.Entities.Add(e switch{GeometryLine l=>new EditorLine(l.Id,l.LayerName,l.Start,l.End),GeometryCircle c=>new EditorCircle(c.Id,c.LayerName,c.Center,c.Radius),GeometryArc a=>new EditorArc(a.Id,a.LayerName,a.Center,a.Radius,a.StartAngle,a.EndAngle,ArcDirection.Counterclockwise),_=>throw new NotSupportedException()});return d;}
}

public sealed class HistoryManager
{
    readonly Stack<DrawingDocument> undo=[]; readonly Stack<DrawingDocument> redo=[];
    public void Checkpoint(DrawingDocument d){undo.Push(d.Clone());redo.Clear();}
    public DrawingDocument? Undo(DrawingDocument current){if(undo.Count==0)return null;redo.Push(current.Clone());return undo.Pop();}
    public DrawingDocument? Redo(DrawingDocument current){if(redo.Count==0)return null;undo.Push(current.Clone());return redo.Pop();}
}

public enum ValidationSeverity { Warning, Error }
public sealed record ValidationIssue(int EntityId, string EntityType, string Layer, ValidationSeverity Severity, string Code, string Message)
{
    public string DisplayText => $"#{EntityId}  {EntityType}  {Layer.Trim('-')}  {Message}";
}

public static class DrawingValidation
{
    public static IReadOnlyList<ValidationIssue> Validate(DrawingDocument d)
    {
      var issues=d.Entities.SelectMany(e=>e switch {
        EditorLine l when !Finite(l.Start.X,l.Start.Y,l.End.X,l.End.Y)=>[Issue(e,"INVALID_COORDINATE","유효하지 않은 좌표")],
        EditorLine l when l.Length<=GeometryMath.Epsilon=>[Issue(e,"ZERO_LENGTH_LINE","길이가 0인 선")],
        EditorLine l when IsBend(l.Layer)&&!l.Horizontal&&!l.Vertical=>[Issue(e,"INVALID_BEND_DIRECTION","V/V1 선은 수평 또는 수직이어야 합니다.")],
        EditorCircle c when c.Radius<=0||!Finite(c.Center.X,c.Center.Y,c.Radius)=>[Issue(e,"INVALID_CIRCLE","Radius가 0 이하이거나 좌표가 유효하지 않음")],
        EditorArc a when a.Radius<=0||!Finite(a.Center.X,a.Center.Y,a.Radius,a.StartAngle,a.EndAngle)=>[Issue(e,"INVALID_ARC","Radius가 0 이하이거나 원호 정의가 유효하지 않음")],
        _=>Array.Empty<ValidationIssue>()}).ToList();
      foreach(var bend in d.Entities.OfType<EditorLine>().Where(x=>GeometryRules.IsBendLayer(x.Layer)))
        if(GeometryRules.HasForbiddenCutBendOverlap(bend,d.Entities))issues.Add(Issue(bend,"CUT_BEND_OVERLAP","L-Layer 절단선과 중첩됨"));
      return issues;
    }
    public static IReadOnlyList<string> Errors(DrawingDocument d)=>Validate(d).Where(x=>x.Severity==ValidationSeverity.Error).Select(x=>x.DisplayText).ToList();
    static ValidationIssue Issue(EditorEntity e,string code,string message)=>new(e.Id,e switch{EditorLine=>"LINE",EditorArc=>"ARC",EditorCircle=>"CIRCLE",_=>"ENTITY"},e.Layer,ValidationSeverity.Error,code,message);
    static bool IsBend(string s)=>s.Trim('-',' ').Equals("V",StringComparison.OrdinalIgnoreCase)||s.Trim('-',' ').Equals("V1",StringComparison.OrdinalIgnoreCase);
    static bool Finite(params double[] v)=>v.All(double.IsFinite);
}

public static class GeometryRules
{
    public static bool IsCutLayer(string layer)=>layer.Trim('-',' ').Equals("L",StringComparison.OrdinalIgnoreCase);
    public static bool IsBendLayer(string layer)=>layer.Trim('-',' ').Equals("V",StringComparison.OrdinalIgnoreCase)||layer.Trim('-',' ').Equals("V1",StringComparison.OrdinalIgnoreCase);
    public static bool HasForbiddenCutBendOverlap(EditorLine candidate,IEnumerable<EditorEntity> entities)
    {
        if(!IsCutLayer(candidate.Layer)&&!IsBendLayer(candidate.Layer))return false;
        return entities.OfType<EditorLine>().Where(x=>x.Id!=candidate.Id&&((IsCutLayer(candidate.Layer)&&IsBendLayer(x.Layer))||(IsBendLayer(candidate.Layer)&&IsCutLayer(x.Layer)))).Any(x=>CollinearOverlapLength(candidate,x)>GeometryMath.Epsilon);
    }
    public static double CollinearOverlapLength(EditorLine a,EditorLine b)
    {
        if(a.Length<=GeometryMath.Epsilon||b.Length<=GeometryMath.Epsilon)return 0;
        var tolerance=GeometryMath.Epsilon*Math.Max(1,a.Length);
        if(Math.Abs(GeometryMath.Cross(a.Start,a.End,b.Start))>tolerance||Math.Abs(GeometryMath.Cross(a.Start,a.End,b.End))>tolerance)return 0;
        var ux=a.Dx/a.Length;var uy=a.Dy/a.Length;double Project(DxfPoint p)=>(p.X-a.Start.X)*ux+(p.Y-a.Start.Y)*uy;
        var b0=Project(b.Start);var b1=Project(b.End);return Math.Max(0,Math.Min(a.Length,Math.Max(b0,b1))-Math.Max(0,Math.Min(b0,b1)));
    }
}

public static class DxfWriter
{
    public static void Save(DrawingDocument d,string path)
    {
        var errors=DrawingValidation.Validate(d).Where(x=>x.Severity==ValidationSeverity.Error).ToList();if(errors.Count>0)throw new DrawingValidationException(errors);
        var b=new StringBuilder(); void G(int c,object v)=>b.AppendLine(c.ToString(CultureInfo.InvariantCulture)).AppendLine(Convert.ToString(v,CultureInfo.InvariantCulture));
        G(0,"SECTION");G(2,"HEADER");G(9,"$ACADVER");G(1,"AC1009");G(9,"$INSUNITS");G(70,4);G(0,"ENDSEC");
        G(0,"SECTION");G(2,"TABLES");G(0,"TABLE");G(2,"LAYER");G(70,3);foreach(var l in new[]{"-L-","-V-","-V1-"}){G(0,"LAYER");G(2,l);G(70,0);G(62,l=="-L-"?7:l=="-V-"?1:5);G(6,"CONTINUOUS");}G(0,"ENDTAB");G(0,"ENDSEC");G(0,"SECTION");G(2,"ENTITIES");
        foreach(var e in d.Entities){G(0,e switch{EditorLine=>"LINE",EditorArc=>"ARC",_=>"CIRCLE"});G(8,e.Layer);switch(e){case EditorLine l:G(10,l.Start.X);G(20,l.Start.Y);G(30,0);G(11,l.End.X);G(21,l.End.Y);G(31,0);break;case EditorCircle c:G(10,c.Center.X);G(20,c.Center.Y);G(30,0);G(40,c.Radius);break;case EditorArc a:G(10,a.Center.X);G(20,a.Center.Y);G(30,0);G(40,a.Radius);G(50,GeometryArc.NormalizeAngle(a.StartAngle));G(51,GeometryArc.NormalizeAngle(a.EndAngle));break;}}
        G(0,"ENDSEC");G(0,"EOF");File.WriteAllText(path,b.ToString(),Encoding.ASCII);
    }
}

public sealed class DrawingValidationException(IReadOnlyList<ValidationIssue> issues):InvalidOperationException(string.Join(Environment.NewLine,issues.Select(x=>x.DisplayText)))
{ public IReadOnlyList<ValidationIssue> Issues { get; }=issues; }

public static class ContourAnalyzer
{
    public static IReadOnlyList<IReadOnlyList<EditorEntity>> Closed(DrawingDocument d)
    {
        var remaining=d.Entities.Where(x=>x.Layer.Equals("-L-",StringComparison.OrdinalIgnoreCase)&&x is EditorLine or EditorArc).ToList();var result=new List<IReadOnlyList<EditorEntity>>();
        while(remaining.Count>0){var chain=new List<EditorEntity>{remaining[0]};remaining.RemoveAt(0);var start=Ends(chain[0]).A;var end=Ends(chain[0]).B;while(!GeometryMath.Near(start,end)){var next=remaining.FirstOrDefault(x=>{var p=Ends(x);return GeometryMath.Near(p.A,end)||GeometryMath.Near(p.B,end);});if(next is null)break;remaining.Remove(next);var q=Ends(next);if(GeometryMath.Near(q.B,end))next=Reverse(next);chain.Add(next);end=Ends(next).B;}if(GeometryMath.Near(start,end))result.Add(chain);}
        foreach(var c in d.Entities.OfType<EditorCircle>().Where(x=>x.Layer.Equals("-L-",StringComparison.OrdinalIgnoreCase)))result.Add([c]);return result;
    }
    static (DxfPoint A,DxfPoint B) Ends(EditorEntity e)=>e switch{EditorLine l=>(l.Start,l.End),EditorArc a=>(a.StartPoint,a.EndPoint),_=>(e.Points.First(),e.Points.First())};
    static EditorEntity Reverse(EditorEntity e)=>e switch{EditorLine l=>l with{Start=l.End,End=l.Start},EditorArc a=>a with{StartAngle=a.EndAngle,EndAngle=a.StartAngle},_=>e};
}
