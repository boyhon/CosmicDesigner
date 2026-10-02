using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace CosmicDesigner;

public static class SectionSelectionEngine
{
    public static double Clamp(double position,double extent)=>Math.Clamp(position,0,Math.Max(0,extent));
}

public static class BoundaryCutEngine
{
    const double Epsilon=1e-6;
    public static bool TrySubtractRectangle(IReadOnlyList<GeometrySegment> outer,Rect cut,out IReadOnlyList<GeometrySegment> contour)
    {
        contour=[];var lines=outer.OfType<LineSegment>().ToList();if(lines.Count!=outer.Count||lines.Count<4||lines.Any(x=>Math.Abs(x.X1-x.X2)>Epsilon&&Math.Abs(x.Y1-x.Y2)>Epsilon))return false;var polygon=lines.Select(x=>new Point(x.X1,x.Y1)).ToList();var xs=polygon.Select(x=>x.X).Append(cut.Left).Append(cut.Right).Distinct().Order().ToList();var ys=polygon.Select(x=>x.Y).Append(cut.Top).Append(cut.Bottom).Distinct().Order().ToList();var edges=new HashSet<(Point A,Point B)>();var removed=false;
        void Add(Point a,Point b){if(!edges.Remove((b,a)))edges.Add((a,b));}
        for(var xi=0;xi<xs.Count-1;xi++)for(var yi=0;yi<ys.Count-1;yi++){var x0=xs[xi];var x1=xs[xi+1];var y0=ys[yi];var y1=ys[yi+1];if(x1-x0<=Epsilon||y1-y0<=Epsilon)continue;var mid=new Point((x0+x1)/2,(y0+y1)/2);var inside=InsidePolygon(polygon,mid);var inCut=cut.Contains(mid);if(inside&&inCut){removed=true;continue;}if(!inside)continue;Add(new(x0,y0),new(x1,y0));Add(new(x1,y0),new(x1,y1));Add(new(x1,y1),new(x0,y1));Add(new(x0,y1),new(x0,y0));}
        if(!removed||edges.Count==0)return false;var remaining=new HashSet<(Point A,Point B)>(edges);var loops=new List<List<Point>>();while(remaining.Count>0){var first=remaining.First();var loop=new List<Point>{first.A};var edge=first;while(true){remaining.Remove(edge);loop.Add(edge.B);if((edge.B-first.A).Length<=Epsilon)break;var next=remaining.Where(x=>(x.A-edge.B).Length<=Epsilon).ToList();if(next.Count!=1)return false;edge=next[0];if(loop.Count>edges.Count+1)return false;}loop.RemoveAt(loop.Count-1);RemoveCollinear(loop);loops.Add(loop);}if(loops.Count!=1||loops[0].Count<4)return false;var points=loops[0];contour=Enumerable.Range(0,points.Count).Select(i=>(GeometrySegment)new LineSegment(points[i].X,points[i].Y,points[(i+1)%points.Count].X,points[(i+1)%points.Count].Y)).ToList();return true;
    }
    static bool InsidePolygon(IReadOnlyList<Point> polygon,Point p){var inside=false;for(int i=0,j=polygon.Count-1;i<polygon.Count;j=i++){var a=polygon[i];var b=polygon[j];if((a.Y>p.Y)!=(b.Y>p.Y)&&p.X<(b.X-a.X)*(p.Y-a.Y)/(b.Y-a.Y)+a.X)inside=!inside;}return inside;}
    static void RemoveCollinear(List<Point> points){for(var changed=true;changed&&points.Count>3;){changed=false;for(var i=0;i<points.Count;i++){var a=points[(i+points.Count-1)%points.Count];var b=points[i];var c=points[(i+1)%points.Count];if(Math.Abs(Vector.CrossProduct(b-a,c-b))<=Epsilon){points.RemoveAt(i);changed=true;break;}}}}
}

public static class OuterContourEditEngine
{
    const double Epsilon=1e-6;
    public static IReadOnlyList<GeometrySegment> UpdateConnectedLine(IReadOnlyList<GeometrySegment> source,int index,LineSegment replacement)
    {
        if(index<0||index>=source.Count||source[index] is not LineSegment original)throw new ArgumentOutOfRangeException(nameof(index));
        var result=source.ToList();result[index]=replacement;
        for(var i=0;i<result.Count;i++){
            if(i==index||result[i] is not LineSegment neighbor)continue;
            var changed=false;var x1=neighbor.X1;var y1=neighbor.Y1;var x2=neighbor.X2;var y2=neighbor.Y2;
            if(Same(x1,y1,original.X1,original.Y1)){x1=replacement.X1;y1=replacement.Y1;changed=true;}
            else if(Same(x2,y2,original.X1,original.Y1)){x2=replacement.X1;y2=replacement.Y1;changed=true;}
            if(Same(x1,y1,original.X2,original.Y2)){x1=replacement.X2;y1=replacement.Y2;changed=true;}
            else if(Same(x2,y2,original.X2,original.Y2)){x2=replacement.X2;y2=replacement.Y2;changed=true;}
            if(changed)result[i]=new LineSegment(x1,y1,x2,y2);
        }
        return result;
    }
    static bool Same(double x1,double y1,double x2,double y2)=>Math.Abs(x1-x2)<=Epsilon&&Math.Abs(y1-y2)<=Epsilon;
}

public static class BendCalculationEngine
{
    public static void Recalculate(CosmicDesignerDocument document)
    {
        var d = document.Material.Thickness;
        foreach (var bend in document.Bends) { bend.BendAngle = 90; bend.VCutAngle = 90; bend.VCutDepth = d / 2; bend.ResidualThickness = d / 2; }
        document.Material.Width = FlatLength(document.WSegments, document.Bends.Where(b => b.Axis == SectionAxis.W), d);
        document.Material.Height = FlatLength(document.HSegments, document.Bends.Where(b => b.Axis == SectionAxis.H), d);
    }
    public static double FlatLength(IEnumerable<SectionSegment> segments, IEnumerable<BendObject> bends, double thickness)
    {
        var finished = segments.Sum(s => s.Length);
        var correction = bends.Count() * thickness / 2; // isolated policy point for a future material model
        return Math.Max(0.01, finished - correction);
    }
    public static double ExteriorSegmentLength(CosmicDesignerDocument document,SectionAxis axis,int segmentIndex)
    {
        var segments=(axis==SectionAxis.W?document.WSegments:document.HSegments).OrderBy(s=>s.Index).ToList();if(segmentIndex<0||segmentIndex>=segments.Count)throw new ArgumentOutOfRangeException(nameof(segmentIndex));return ExteriorSegmentLength(segments[segmentIndex].Length,document.Material.Thickness,segmentIndex,segments.Count);
    }
    public static double ExteriorSegmentLength(double centerLength,double thickness,int segmentIndex,int segmentCount)=>centerLength+ExteriorCorrection(thickness,segmentIndex,segmentCount);
    public static double CenterSegmentLengthFromExterior(double exteriorLength,double thickness,int segmentIndex,int segmentCount)=>Math.Max(.01,exteriorLength-ExteriorCorrection(thickness,segmentIndex,segmentCount));
    public static double ExteriorCorrection(double thickness,int segmentIndex,int segmentCount)
    {
        var adjacentBends=(segmentIndex>0?1:0)+(segmentIndex<segmentCount-1?1:0);return adjacentBends*thickness/2;
    }
}

public readonly record struct SectionPoint2(double X,double Y);
public readonly record struct SectionInterval(double Start,double End){public double Length=>Math.Max(0,End-Start);}
public static class ContourSectionEngine
{
    const double Epsilon=1e-7;
    public static SectionInterval MaterialInterval(IEnumerable<GeometrySegment> outer,SectionAxis axis,double position,double fallbackLength)
    {
        var crossings=new List<double>();foreach(var line in outer.OfType<LineSegment>()){if(axis==SectionAxis.H){if(Math.Abs(line.Y1-line.Y2)>Epsilon)continue;var min=Math.Min(line.X1,line.X2);var max=Math.Max(line.X1,line.X2);if(position>=min-Epsilon&&position<max-Epsilon)crossings.Add(line.Y1);}else{if(Math.Abs(line.X1-line.X2)>Epsilon)continue;var min=Math.Min(line.Y1,line.Y2);var max=Math.Max(line.Y1,line.Y2);if(position>=min-Epsilon&&position<max-Epsilon)crossings.Add(line.X1);}}
        crossings=crossings.Distinct().Order().ToList();var intervals=new List<SectionInterval>();for(var i=0;i+1<crossings.Count;i+=2)if(crossings[i+1]-crossings[i]>Epsilon)intervals.Add(new(crossings[i],crossings[i+1]));return intervals.Count==0?new(0,fallbackLength):intervals.OrderByDescending(x=>x.Length).First();
    }
}
public sealed record SectionGeometry(IReadOnlyList<SectionPoint2> Points,IReadOnlyList<BendObject> Bends,IReadOnlyList<double> SegmentLengths,double StartPosition,double EndPosition)
{
    public bool IsClipped(double fullLength)=>StartPosition>1e-6||EndPosition<fullLength-1e-6;
}
public static class SectionGeometryEngine
{
    public static SectionGeometry Build(CosmicDesignerDocument document,SectionAxis axis,bool bent,double sectionPosition=double.NaN)
    {
        var total=axis==SectionAxis.W?document.Material.Width:document.Material.Height;var selectorExtent=axis==SectionAxis.H?document.Material.Width:document.Material.Height;var selector=double.IsFinite(sectionPosition)?Math.Clamp(sectionPosition,0,selectorExtent):selectorExtent/2;var interval=ContourSectionEngine.MaterialInterval(document.OuterContour.Segments,axis,selector,total);var bends=document.Bends.Where(b=>b.Axis==axis&&b.Position>interval.Start+1e-7&&b.Position<interval.End-1e-7).OrderBy(b=>b.Position).ToList();var clipped=interval.Start>1e-6||interval.End<total-1e-6;List<double> segments;
        if(!clipped){segments=(axis==SectionAxis.W?document.WSegments:document.HSegments).OrderBy(s=>s.Index).Select(s=>s.Length).ToList();}
        else{segments=[];var last=interval.Start;foreach(var bend in bends){segments.Add(Math.Max(.01,bend.Position-last));last=bend.Position;}segments.Add(Math.Max(.01,interval.End-last));}
        var points=new List<SectionPoint2>{new(0,0)};double angle=0,x=0,y=0;
        for(var i=0;i<segments.Count;i++){
            x+=segments[i]*Math.Cos(angle);y+=segments[i]*Math.Sin(angle);points.Add(new(x,y));
            if(bent&&i<bends.Count)angle+=Turn(bends[i])*Math.PI/2;
        }
        return new(points,bends,segments,interval.Start,interval.End);
    }
    static int Turn(BendObject b)=>b.Direction is BendDirection.Up or BendDirection.Left?1:-1;
}

public static class MicroJointGeometryEngine
{
    public static IReadOnlyList<LineSegment> Split(LineSegment line, IEnumerable<MicroJoint> joints)
    {
        var dx=line.X2-line.X1; var dy=line.Y2-line.Y1; var length=Math.Sqrt(dx*dx+dy*dy); if(length<=1e-9) return [];
        var gaps=Normalize(joints,length); var output=new List<LineSegment>(); var cursor=0d;
        foreach(var (start,end) in gaps){ if(start>cursor) output.Add(PointLine(line,cursor/length,start/length)); cursor=Math.Max(cursor,end); }
        if(cursor<length) output.Add(PointLine(line,cursor/length,1)); return output;
    }
    public static IReadOnlyList<ArcSegment> Split(CircleSegment circle, IEnumerable<MicroJoint> joints)
    {
        var circumference=2*Math.PI*circle.Radius; var gaps=Normalize(joints,circumference); var output=new List<ArcSegment>(); var cursor=0d;
        foreach(var (start,end) in gaps){ if(start>cursor) output.Add(new(circle.Cx,circle.Cy,circle.Radius,cursor/circumference*360,start/circumference*360)); cursor=Math.Max(cursor,end); }
        if(cursor<circumference) output.Add(new(circle.Cx,circle.Cy,circle.Radius,cursor/circumference*360,360)); return output;
    }
    static List<(double Start,double End)> Normalize(IEnumerable<MicroJoint> joints,double length) => joints.Where(j=>j.Enabled&&j.Width>0).Select(j=>(Math.Max(0,j.Position-j.Width/2),Math.Min(length,j.Position+j.Width/2))).Where(g=>g.Item2>g.Item1).OrderBy(g=>g.Item1).ToList();
    static LineSegment PointLine(LineSegment l,double a,double b)=>new(l.X1+(l.X2-l.X1)*a,l.Y1+(l.Y2-l.Y1)*a,l.X1+(l.X2-l.X1)*b,l.Y1+(l.Y2-l.Y1)*b);
}

public static class HoleGeometryEngine
{
    public static void Rebuild(CutOperation cut)
    {
        cut.Geometry.Clear();var shape=cut.Shape;var cx=cut.CenterX;var cy=cut.CenterY;var w=cut.Width;var h=cut.Height;
        if(shape=="Circle"){cut.Geometry.Add(new CircleSegment(cx,cy,w/2));return;}
        IReadOnlyList<(double X,double Y)> points=shape switch{
            "Arc"=>ArcBand(cx,cy,w/2,h/2,-140,100,20),
            "Sector"=>Sector(cx,cy,w/2,h/2,-35,70,20),
            "Ellipse"=>Regular(cx,cy,w/2,h/2,48,-90),
            "Triangle"=>Regular(cx,cy,w/2,h/2,3,-90),
            "Rectangle"=>[ (cx-w/2,cy-h/2),(cx+w/2,cy-h/2),(cx+w/2,cy+h/2),(cx-w/2,cy+h/2) ],
            "Diamond"=>[ (cx,cy-h/2),(cx+w/2,cy),(cx,cy+h/2),(cx-w/2,cy) ],
            "Parallelogram"=>[ (cx-w*.35,cy-h/2),(cx+w/2,cy-h/2),(cx+w*.35,cy+h/2),(cx-w/2,cy+h/2) ],
            "Pentagon"=>Regular(cx,cy,w/2,h/2,5,-90),"Hexagon"=>Regular(cx,cy,w/2,h/2,6,0),_=>Regular(cx,cy,w/2,h/2,Math.Max(3,cut.Sides),-90)};
        for(var i=0;i<points.Count;i++){var a=points[i];var b=points[(i+1)%points.Count];cut.Geometry.Add(new LineSegment(a.X,a.Y,b.X,b.Y));}
    }
    static List<(double X,double Y)> Regular(double cx,double cy,double rx,double ry,int count,double start){var p=new List<(double,double)>();for(var i=0;i<count;i++){var a=(start+i*360d/count)*Math.PI/180;p.Add((cx+rx*Math.Cos(a),cy+ry*Math.Sin(a)));}return p;}
    static List<(double X,double Y)> Sector(double cx,double cy,double rx,double ry,double start,double sweep,int steps){var p=new List<(double,double)>{(cx,cy)};for(var i=0;i<=steps;i++){var a=(start+sweep*i/steps)*Math.PI/180;p.Add((cx+rx*Math.Cos(a),cy+ry*Math.Sin(a)));}return p;}
    static List<(double X,double Y)> ArcBand(double cx,double cy,double rx,double ry,double start,double sweep,int steps){var p=new List<(double,double)>();for(var i=0;i<=steps;i++){var a=(start+sweep*i/steps)*Math.PI/180;p.Add((cx+rx*Math.Cos(a),cy+ry*Math.Sin(a)));}for(var i=steps;i>=0;i--){var a=(start+sweep*i/steps)*Math.PI/180;p.Add((cx+rx*.65*Math.Cos(a),cy+ry*.65*Math.Sin(a)));}return p;}
}

public static class DesignValidationEngine
{
    public static IReadOnlyList<string> Validate(CosmicDesignerDocument d)
    {
        var issues=new List<string>(); if(d.Material.Thickness<=0) issues.Add("Thickness must be greater than zero.");
        if(d.OuterContour.Segments.Count==0) issues.Add("Exactly one outer contour is required.");
        issues.AddRange(d.Bends.Where(b=>b.VCutDepth>=d.Material.Thickness).Select(b=>$"{b.Id}: VCutDepth must be less than thickness."));
        issues.AddRange(d.MicroJoints.Where(j=>j.Width<=0).Select(j=>$"{j.Id}: Micro joint width must be greater than zero.")); return issues;
    }
}

public static class CosmicDxfSerializer
{
    const string MetadataMarker="COSMIC_DESIGNER_JSON:";
    public static bool HasMetadata(string path)=>File.ReadLines(path).Any(x=>x.Contains(MetadataMarker,StringComparison.Ordinal));
    public static void Save(CosmicDesignerDocument d,string path)
    {
        var issues=DesignValidationEngine.Validate(d); if(issues.Count>0) throw new InvalidOperationException(string.Join(Environment.NewLine,issues));
        var sb=new StringBuilder(); Pair(sb,0,"SECTION");Pair(sb,2,"HEADER");Pair(sb,9,"$ACADVER");Pair(sb,1,"AC1015");Pair(sb,9,"$MEASUREMENT");Pair(sb,70,"1");Pair(sb,9,"$INSUNITS");Pair(sb,70,"5");Pair(sb,0,"ENDSEC");
        Pair(sb,0,"SECTION");Pair(sb,2,"ENTITIES");
        foreach(var item in Ordered(d)) WriteEntity(sb,item);
        Pair(sb,0,"ENDSEC");Pair(sb,999,MetadataMarker+JsonSerializer.Serialize(ToDto(d)));Pair(sb,0,"EOF"); File.WriteAllText(path,sb.ToString(),Encoding.ASCII);
    }
    public static CosmicDesignerDocument Load(string path)
    {
        var text=File.ReadAllText(path); var line=text.Split(new[] {'\r','\n'},StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x=>x.StartsWith(MetadataMarker,StringComparison.Ordinal));
        if(line is null) throw new InvalidDataException("This DXF has no CosmicDesigner metadata."); var dto=JsonSerializer.Deserialize<DocumentDto>(line[MetadataMarker.Length..]) ?? throw new InvalidDataException("Invalid CosmicDesigner metadata.");
        var scale=string.Equals(dto.Unit,"cm",StringComparison.OrdinalIgnoreCase)?1d:.1d;var d=new CosmicDesignerDocument(); d.WSegments.Clear();d.HSegments.Clear();d.Bends.Clear();d.Cuts.Clear();d.InnerContours.Clear();d.MicroJoints.Clear();
        foreach(var x in dto.WSegments)d.WSegments.Add(new SectionSegment{Id=x.Id,Axis=SectionAxis.W,Index=x.Index,Length=x.Length*scale}); foreach(var x in dto.HSegments)d.HSegments.Add(new SectionSegment{Id=x.Id,Axis=SectionAxis.H,Index=x.Index,Length=x.Length*scale});
        foreach(var x in dto.Bends)d.Bends.Add(new(){Id=x.Id,Sequence=x.Sequence,Axis=x.Axis,Direction=x.Direction,Position=x.Position*scale});foreach(var x in dto.Cuts??[]){var cut=new CutOperation{Id=x.Id,Sequence=x.Sequence,Kind=CutKind.Hole,Shape=x.Shape,CenterX=x.CenterX*scale,CenterY=x.CenterY*scale,Width=x.Width*scale,Height=x.Height*scale,Sides=x.Sides};if(x.Geometry is {Count:>0})cut.Geometry.AddRange(x.Geometry.Select(g=>Scale(FromDto(g),scale)));else HoleGeometryEngine.Rebuild(cut);d.Cuts.Add(cut);var contour=new ContourObject{Id=cut.Id,Kind=ContourKind.Inner};contour.Segments.AddRange(cut.Geometry);d.InnerContours.Add(contour);} foreach(var x in dto.MicroJoints)d.MicroJoints.Add(new(){Id=x.Id,Sequence=x.Sequence,ParentContourId=x.ParentContourId,Position=x.Position*scale,Width=x.Width*scale,Enabled=x.Enabled}); d.ChangeThickness(dto.Thickness*scale);if(dto.Outer is {Count:>0})d.PreserveOuterGeometry(dto.Outer.Select(g=>Scale(FromDto(g),scale)));ApplyView(d.WView,dto.WView);ApplyView(d.HView,dto.HView);return d;
    }
    static IEnumerable<object> Ordered(CosmicDesignerDocument d) { yield return d.OuterContour; foreach(var x in d.Bends.Cast<DesignObject>().Concat(d.Cuts).OrderBy(x=>x.Sequence))yield return x; }
    static void WriteEntity(StringBuilder sb,object item)
    {
        if(item is ContourObject c)foreach(var s in c.Segments)WriteSegment(sb,s,"L");
        if(item is BendObject b){GeometrySegment s=b.Axis==SectionAxis.W?new LineSegment(b.Position,0,b.Position,10000):new LineSegment(0,b.Position,10000,b.Position);WriteSegment(sb,s,b.Layer);}
        if(item is CutOperation cut)foreach(var s in cut.Geometry)WriteSegment(sb,s,"L");
    }
    static void WriteSegment(StringBuilder sb,GeometrySegment segment,string layer)
    {
        var n=CultureInfo.InvariantCulture;if(segment is LineSegment l){Pair(sb,0,"LINE");Pair(sb,8,layer);Pair(sb,10,l.X1.ToString(n));Pair(sb,20,l.Y1.ToString(n));Pair(sb,11,l.X2.ToString(n));Pair(sb,21,l.Y2.ToString(n));}
        else if(segment is CircleSegment c){Pair(sb,0,"CIRCLE");Pair(sb,8,layer);Pair(sb,10,c.Cx.ToString(n));Pair(sb,20,c.Cy.ToString(n));Pair(sb,40,c.Radius.ToString(n));}
        else if(segment is ArcSegment a){Pair(sb,0,"ARC");Pair(sb,8,layer);Pair(sb,10,a.Cx.ToString(n));Pair(sb,20,a.Cy.ToString(n));Pair(sb,40,a.Radius.ToString(n));Pair(sb,50,a.StartDegrees.ToString(n));Pair(sb,51,a.EndDegrees.ToString(n));}
    }
    static void Pair(StringBuilder sb,int code,string value)=>sb.Append(code).Append("\r\n").Append(value).Append("\r\n");
    static DocumentDto ToDto(CosmicDesignerDocument d)=>new(d.Material.Thickness,d.WSegments.Select(x=>new SegmentDto(x.Id,x.Index,x.Length)).ToList(),d.HSegments.Select(x=>new SegmentDto(x.Id,x.Index,x.Length)).ToList(),d.Bends.Select(x=>new BendDto(x.Id,x.Sequence,x.Axis,x.Direction,x.Position)).ToList(),d.Cuts.Select(x=>new CutDto(x.Id,x.Sequence,x.Shape,x.CenterX,x.CenterY,x.Width,x.Height,x.Sides,x.Geometry.Select(ToDto).ToList())).ToList(),d.MicroJoints.ToList(),d.OuterContour.Segments.Select(ToDto).ToList(),"cm",ToDto(d.WView),ToDto(d.HView));
    static GeometryDto ToDto(GeometrySegment x)=>x switch{LineSegment l=>new("LINE",l.X1,l.Y1,l.X2,l.Y2,0),CircleSegment c=>new("CIRCLE",c.Cx,c.Cy,0,0,c.Radius),ArcSegment a=>new("ARC",a.Cx,a.Cy,a.StartDegrees,a.EndDegrees,a.Radius),_=>throw new NotSupportedException()};
    static GeometrySegment FromDto(GeometryDto x)=>x.Type switch{"LINE"=>new LineSegment(x.A,x.B,x.C,x.D),"CIRCLE"=>new CircleSegment(x.A,x.B,x.Radius),"ARC"=>new ArcSegment(x.A,x.B,x.Radius,x.C,x.D),_=>throw new InvalidDataException($"Unsupported metadata geometry: {x.Type}")};
    static GeometrySegment Scale(GeometrySegment x,double s)=>x switch{LineSegment l=>new LineSegment(l.X1*s,l.Y1*s,l.X2*s,l.Y2*s),CircleSegment c=>new CircleSegment(c.Cx*s,c.Cy*s,c.Radius*s),ArcSegment a=>new ArcSegment(a.Cx*s,a.Cy*s,a.Radius*s,a.StartDegrees,a.EndDegrees),_=>x};
    static ViewDto ToDto(SectionViewState x)=>new(x.Dimensions,x.BentMode,x.RotationQuarterTurns);static void ApplyView(SectionViewState target,ViewDto? source){if(source is null)return;target.Dimensions=source.Dimensions;target.BentMode=source.BentMode;target.RotationQuarterTurns=((source.RotationQuarterTurns%4)+4)%4;}
    record DocumentDto(double Thickness,List<SegmentDto> WSegments,List<SegmentDto> HSegments,List<BendDto> Bends,List<CutDto>? Cuts,List<MicroJoint> MicroJoints,List<GeometryDto>? Outer=null,string? Unit=null,ViewDto? WView=null,ViewDto? HView=null);
    record ViewDto(bool Dimensions,bool BentMode,int RotationQuarterTurns);
    record SegmentDto(string Id,int Index,double Length); record BendDto(string Id,int Sequence,SectionAxis Axis,BendDirection Direction,double Position);record CutDto(string Id,int Sequence,string Shape,double CenterX,double CenterY,double Width,double Height,int Sides,List<GeometryDto>? Geometry=null);record GeometryDto(string Type,double A,double B,double C,double D,double Radius);
}
