using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace CosmicDesigner;

public enum FileShortcutAction{None,New,Open,Save}
public sealed class DocumentDirtyState
{
    public bool IsDirty { get; private set; }
    public event EventHandler? StateChanged;
    public void MarkChanged(){if(IsDirty)return;IsDirty=true;StateChanged?.Invoke(this,EventArgs.Empty);}
    public void MarkSaved(){if(!IsDirty)return;IsDirty=false;StateChanged?.Invoke(this,EventArgs.Empty);}
}
public static class FileShortcutEngine
{
    public static FileShortcutAction Resolve(System.Windows.Input.Key key,System.Windows.Input.ModifierKeys modifiers)
    {
        if(modifiers!=System.Windows.Input.ModifierKeys.Control)return FileShortcutAction.None;return key switch{System.Windows.Input.Key.N=>FileShortcutAction.New,System.Windows.Input.Key.O=>FileShortcutAction.Open,System.Windows.Input.Key.S=>FileShortcutAction.Save,_=>FileShortcutAction.None};
    }
}

public static class SectionSelectionEngine
{
    public static double Clamp(double position,double extent)=>Math.Clamp(position,0,Math.Max(0,extent));
}

public static class BoundaryCutEngine
{
    const double Epsilon=1e-6;
    public static bool TrySubtractRectangle(IReadOnlyList<GeometrySegment> outer,Rect cut,out IReadOnlyList<GeometrySegment> contour)
    {
        contour=[];var lines=outer.OfType<LineSegment>().ToList();
        if(outer.Any(segment=>segment is ArcSegment))return TrySubtractPolygon(outer,[new LineSegment(cut.Left,cut.Top,cut.Right,cut.Top),new LineSegment(cut.Right,cut.Top,cut.Right,cut.Bottom),new LineSegment(cut.Right,cut.Bottom,cut.Left,cut.Bottom),new LineSegment(cut.Left,cut.Bottom,cut.Left,cut.Top)],out contour);
        if(lines.Count!=outer.Count||lines.Count<3)return false;
        if(lines.Count<4||lines.Any(x=>Math.Abs(x.X1-x.X2)>Epsilon&&Math.Abs(x.Y1-x.Y2)>Epsilon))
        {
            IReadOnlyList<GeometrySegment> rectangle=[new LineSegment(cut.Left,cut.Top,cut.Right,cut.Top),new LineSegment(cut.Right,cut.Top,cut.Right,cut.Bottom),new LineSegment(cut.Right,cut.Bottom,cut.Left,cut.Bottom),new LineSegment(cut.Left,cut.Bottom,cut.Left,cut.Top)];
            return TrySubtractPolygon(outer,rectangle,out contour);
        }
        var polygon=lines.Select(x=>new Point(x.X1,x.Y1)).ToList();var xs=polygon.Select(x=>x.X).Append(cut.Left).Append(cut.Right).Distinct().Order().ToList();var ys=polygon.Select(x=>x.Y).Append(cut.Top).Append(cut.Bottom).Distinct().Order().ToList();var edges=new HashSet<(Point A,Point B)>();var removed=false;
        void Add(Point a,Point b){if(!edges.Remove((b,a)))edges.Add((a,b));}
        for(var xi=0;xi<xs.Count-1;xi++)for(var yi=0;yi<ys.Count-1;yi++){var x0=xs[xi];var x1=xs[xi+1];var y0=ys[yi];var y1=ys[yi+1];if(x1-x0<=Epsilon||y1-y0<=Epsilon)continue;var mid=new Point((x0+x1)/2,(y0+y1)/2);var inside=InsidePolygon(polygon,mid);var inCut=cut.Contains(mid);if(inside&&inCut){removed=true;continue;}if(!inside)continue;Add(new(x0,y0),new(x1,y0));Add(new(x1,y0),new(x1,y1));Add(new(x1,y1),new(x0,y1));Add(new(x0,y1),new(x0,y0));}
        if(!removed||edges.Count==0)return false;var remaining=new HashSet<(Point A,Point B)>(edges);var loops=new List<List<Point>>();while(remaining.Count>0){var first=remaining.First();var loop=new List<Point>{first.A};var edge=first;while(true){remaining.Remove(edge);loop.Add(edge.B);if((edge.B-first.A).Length<=Epsilon)break;var next=remaining.Where(x=>(x.A-edge.B).Length<=Epsilon).ToList();if(next.Count!=1)return false;edge=next[0];if(loop.Count>edges.Count+1)return false;}loop.RemoveAt(loop.Count-1);RemoveCollinear(loop);loops.Add(loop);}if(loops.Count!=1||loops[0].Count<4)return false;var points=loops[0];contour=Enumerable.Range(0,points.Count).Select(i=>(GeometrySegment)new LineSegment(points[i].X,points[i].Y,points[(i+1)%points.Count].X,points[(i+1)%points.Count].Y)).ToList();return true;
    }
    public static bool TrySubtractPolygon(IReadOnlyList<GeometrySegment> outer,IReadOnlyList<GeometrySegment> cut,out IReadOnlyList<GeometrySegment> contour)
    {
        if(outer.Any(segment=>segment is ArcSegment))return CurvedBoundaryCutEngine.TrySubtract(outer,cut,out contour);
        contour=[];var outerLines=outer.OfType<LineSegment>().ToList();var cutLines=cut.OfType<LineSegment>().ToList();if(outerLines.Count!=outer.Count||cutLines.Count!=cut.Count||outerLines.Count<3||cutLines.Count<3)return false;
        var outerPoints=outerLines.Select(line=>new Point(line.X1,line.Y1)).ToList();var cutPoints=cutLines.Select(line=>new Point(line.X1,line.Y1)).ToList();
        var difference=Geometry.Combine(PolygonGeometry(outerPoints),PolygonGeometry(cutPoints),GeometryCombineMode.Exclude,Transform.Identity).GetFlattenedPathGeometry(Epsilon,ToleranceType.Absolute);
        if(difference.Figures.Count!=1||!difference.Figures[0].IsClosed)return false;var points=FigurePoints(difference.Figures[0]);RemoveCollinear(points);if(points.Count<3)return false;
        var removedArea=Math.Abs(Area(outerPoints))-Math.Abs(Area(points));if(removedArea<=Epsilon)return false;
        contour=Enumerable.Range(0,points.Count).Select(i=>(GeometrySegment)new LineSegment(points[i].X,points[i].Y,points[(i+1)%points.Count].X,points[(i+1)%points.Count].Y)).ToList();return true;
    }
    static PathGeometry PolygonGeometry(IReadOnlyList<Point> points){var figure=new PathFigure{StartPoint=points[0],IsClosed=true,IsFilled=true};figure.Segments.Add(new PolyLineSegment(points.Skip(1),true));return new PathGeometry([figure]){FillRule=FillRule.Nonzero};}
    static List<Point> FigurePoints(PathFigure figure){var points=new List<Point>{figure.StartPoint};foreach(var segment in figure.Segments){if(segment is System.Windows.Media.LineSegment line)points.Add(line.Point);else if(segment is PolyLineSegment polyline)points.AddRange(polyline.Points);}if(points.Count>1&&(points[0]-points[^1]).Length<=Epsilon)points.RemoveAt(points.Count-1);return points;}
    static double Area(IReadOnlyList<Point> points){double area=0;for(var i=0;i<points.Count;i++){var next=(i+1)%points.Count;area+=points[i].X*points[next].Y-points[next].X*points[i].Y;}return area/2;}
    static bool InsidePolygon(IReadOnlyList<Point> polygon,Point p){var inside=false;for(int i=0,j=polygon.Count-1;i<polygon.Count;j=i++){var a=polygon[i];var b=polygon[j];if((a.Y>p.Y)!=(b.Y>p.Y)&&p.X<(b.X-a.X)*(p.Y-a.Y)/(b.Y-a.Y)+a.X)inside=!inside;}return inside;}
    static void RemoveCollinear(List<Point> points){for(var changed=true;changed&&points.Count>3;){changed=false;for(var i=0;i<points.Count;i++){var a=points[(i+points.Count-1)%points.Count];var b=points[i];var c=points[(i+1)%points.Count];if(Math.Abs(Vector.CrossProduct(b-a,c-b))<=Epsilon){points.RemoveAt(i);changed=true;break;}}}}
}

public static class OuterContourFilletEngine
{
    const double Epsilon=1e-6;
    public static bool TryApply(IReadOnlyList<GeometrySegment> source,int vertexIndex,double radius,out IReadOnlyList<GeometrySegment> contour)
    {
        contour=source;if(radius<=Epsilon||source.Count<3||vertexIndex<0||vertexIndex>=source.Count)return false;
        var previousIndex=(vertexIndex+source.Count-1)%source.Count;if(source[vertexIndex] is not LineSegment current||source[previousIndex] is not LineSegment previous)return false;var a=new Point(previous.X1,previous.Y1);var b=new Point(current.X1,current.Y1);var previousEnd=new Point(previous.X2,previous.Y2);var c=new Point(current.X2,current.Y2);if((previousEnd-b).Length>Epsilon)return false;
        var incoming=b-a;var outgoing=c-b;var incomingLength=incoming.Length;var outgoingLength=outgoing.Length;if(incomingLength<=Epsilon||outgoingLength<=Epsilon)return false;incoming.Normalize();outgoing.Normalize();var interior=Math.Acos(Math.Clamp(Vector.Multiply(-incoming,outgoing),-1,1));if(interior<=1e-4||interior>=Math.PI-1e-4)return false;var tangent=radius/Math.Tan(interior/2);if(!double.IsFinite(tangent)||incomingLength<=tangent+Epsilon||outgoingLength<=tangent+Epsilon)return false;
        var points=source.Select(StartPoint).ToList();var area=SignedArea(points);var turn=Vector.CrossProduct(incoming,outgoing);if(Math.Sign(turn)!=Math.Sign(area))return false;
        var start=b-incoming*tangent;var end=b+outgoing*tangent;var normal=new Vector(-incoming.Y,incoming.X);var center=start+normal*radius;var startAngle=Math.Atan2(start.Y-center.Y,start.X-center.X)*180/Math.PI;var endAngle=Math.Atan2(end.Y-center.Y,end.X-center.X)*180/Math.PI;if(endAngle<=startAngle)endAngle+=360;if(endAngle-startAngle>180+Epsilon||(center-(end+new Vector(-outgoing.Y,outgoing.X)*radius)).Length>1e-4)return false;
        var result=source.ToList();result[previousIndex]=new LineSegment(previous.X1,previous.Y1,start.X,start.Y);result[vertexIndex]=new LineSegment(end.X,end.Y,current.X2,current.Y2);result.Insert(vertexIndex,new ArcSegment(center.X,center.Y,radius,startAngle,endAngle));contour=result;return true;
    }
    public static bool TryResize(IReadOnlyList<GeometrySegment> source,int arcIndex,double radius,out IReadOnlyList<GeometrySegment> contour)
    {
        contour=source;if(radius<=Epsilon||source.Count<4||arcIndex<0||arcIndex>=source.Count||source[arcIndex] is not ArcSegment)return false;var previousIndex=(arcIndex+source.Count-1)%source.Count;var nextIndex=(arcIndex+1)%source.Count;if(source[previousIndex] is not LineSegment previous||source[nextIndex] is not LineSegment next||!TryIntersection(previous,next,out var corner))return false;
        var restored=source.ToList();restored[previousIndex]=new LineSegment(previous.X1,previous.Y1,corner.X,corner.Y);restored[nextIndex]=new LineSegment(corner.X,corner.Y,next.X2,next.Y2);restored.RemoveAt(arcIndex);var vertexIndex=arcIndex<restored.Count?arcIndex:0;return TryApply(restored,vertexIndex,radius,out contour);
    }
    static bool TryIntersection(LineSegment first,LineSegment second,out Point intersection){intersection=new();var p=new Point(first.X1,first.Y1);var r=new Vector(first.X2-first.X1,first.Y2-first.Y1);var q=new Point(second.X1,second.Y1);var s=new Vector(second.X2-second.X1,second.Y2-second.Y1);var cross=Vector.CrossProduct(r,s);if(Math.Abs(cross)<=Epsilon)return false;var t=Vector.CrossProduct(q-p,s)/cross;intersection=p+r*t;return double.IsFinite(intersection.X)&&double.IsFinite(intersection.Y);}
    static Point StartPoint(GeometrySegment segment)=>segment switch{LineSegment line=>new(line.X1,line.Y1),ArcSegment arc=>new(arc.Cx+arc.Radius*Math.Cos(arc.StartDegrees*Math.PI/180),arc.Cy+arc.Radius*Math.Sin(arc.StartDegrees*Math.PI/180)),_=>new(double.NaN,double.NaN)};
    static double SignedArea(IReadOnlyList<Point> points){double area=0;for(var i=0;i<points.Count;i++){var next=(i+1)%points.Count;area+=points[i].X*points[next].Y-points[next].X*points[i].Y;}return area/2;}
}

public static class OuterContourEditEngine
{
    public static IReadOnlyList<GeometrySegment> CloseGaps(IReadOnlyList<GeometrySegment> source)
    {
        if(source.Count<2||source.Any(s=>s is not (LineSegment or ArcSegment)))return source;
        Point Endpoint(GeometrySegment s,bool end)=>s switch
        {
            LineSegment l=>end?new(l.X2,l.Y2):new(l.X1,l.Y1),
            ArcSegment a=>new(a.Cx+a.Radius*Math.Cos((end?a.EndDegrees:a.StartDegrees)*Math.PI/180),a.Cy+a.Radius*Math.Sin((end?a.EndDegrees:a.StartDegrees)*Math.PI/180)),
            _=>throw new ArgumentException()
        };
        var result=new List<GeometrySegment>();
        for(var i=0;i<source.Count;i++)
        {
            result.Add(source[i]);var end=Endpoint(source[i],true);var start=Endpoint(source[(i+1)%source.Count],false);
            if((start-end).Length>Epsilon)result.Add(new LineSegment(end.X,end.Y,start.X,start.Y));
        }
        return result;
    }
    public static bool TryDeleteLine(IReadOnlyList<GeometrySegment> source,int index,out IReadOnlyList<GeometrySegment> updated)
    {
        updated=source;
        if(source.Count<=3||index<0||index>=source.Count)return false;
        if(source[index] is ArcSegment||source[index] is LineSegment&&source[(index+1)%source.Count] is ArcSegment)
        {
            var remaining=source.ToList();remaining.RemoveAt(index);updated=CloseGaps(remaining);return true;
        }
        if(source[index] is not LineSegment removed)return false;
        var nextIndex=(index+1)%source.Count;
        if(source[nextIndex] is not LineSegment next||Math.Abs(removed.X2-next.X1)>1e-6||Math.Abs(removed.Y2-next.Y1)>1e-6)return false;
        var replacement=new LineSegment(removed.X1,removed.Y1,next.X2,next.Y2);
        if(Math.Abs(replacement.X2-replacement.X1)+Math.Abs(replacement.Y2-replacement.Y1)<1e-6)return false;
        var result=source.ToList();result[nextIndex]=replacement;result.RemoveAt(index);
        if(result.All(segment=>segment is LineSegment))
        {
            var lines=result.Cast<LineSegment>().ToList();
            if(Math.Abs(lines.Sum(l=>l.X1*l.Y2-l.X2*l.Y1))<1e-6)return false;
            // Reject self-intersections rather than create an ambiguous material region.
            for(var i=0;i<lines.Count;i++)for(var j=i+1;j<lines.Count;j++)
            {
                if(j==i+1||(i==0&&j==lines.Count-1))continue;
                var a=new Point(lines[i].X1,lines[i].Y1);var b=new Point(lines[i].X2,lines[i].Y2);
                var c=new Point(lines[j].X1,lines[j].Y1);var d=new Point(lines[j].X2,lines[j].Y2);
                var ab=b-a;var cd=d-c;var cross=Vector.CrossProduct(ab,cd);
                if(Math.Abs(cross)<1e-9)continue;
                var t=Vector.CrossProduct(c-a,cd)/cross;var u=Vector.CrossProduct(c-a,ab)/cross;
                if(t>=-1e-9&&t<=1+1e-9&&u>=-1e-9&&u<=1+1e-9)return false;
            }
        }
        updated=result;return true;
    }
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
        return CloseGaps(result);
    }
    static bool Same(double x1,double y1,double x2,double y2)=>Math.Abs(x1-x2)<=Epsilon&&Math.Abs(y1-y2)<=Epsilon;
    public static bool TryMoveSectionBoundary(IReadOnlyList<GeometrySegment> source,SectionAxis axis,double selector,double boundary,double delta,out IReadOnlyList<GeometrySegment> updated)
    {
        updated=source;for(var i=0;i<source.Count;i++){if(source[i] is not LineSegment line)continue;if(axis==SectionAxis.H&&Math.Abs(line.Y1-boundary)<=Epsilon&&Math.Abs(line.Y2-boundary)<=Epsilon&&selector>=Math.Min(line.X1,line.X2)-Epsilon&&selector<=Math.Max(line.X1,line.X2)+Epsilon){updated=UpdateConnectedLine(source,i,new(line.X1,line.Y1+delta,line.X2,line.Y2+delta));return true;}if(axis==SectionAxis.W&&Math.Abs(line.X1-boundary)<=Epsilon&&Math.Abs(line.X2-boundary)<=Epsilon&&selector>=Math.Min(line.Y1,line.Y2)-Epsilon&&selector<=Math.Max(line.Y1,line.Y2)+Epsilon){updated=UpdateConnectedLine(source,i,new(line.X1+delta,line.Y1,line.X2+delta,line.Y2));return true;}}return false;
    }
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
        // SectionSegment.Length is already the developed length shown in non-Bent mode.
        // Thickness compensation belongs only to Bent exterior dimensions; subtracting it
        // here a second time makes Flat W/H shorter by bendCount * thickness / 2.
        return Math.Max(0.01, segments.Sum(s => s.Length));
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
        var intervals=MaterialIntervals(outer,axis,position);
        return intervals.Count==0?new(0,fallbackLength):intervals.OrderByDescending(x=>x.Length).First();
    }
    public static IReadOnlyList<SectionInterval> MaterialIntervals(IEnumerable<GeometrySegment> outer,SectionAxis axis,double position)
    {
        var crossings=new List<double>();
        foreach(var segment in outer)
        {
            if(segment is LineSegment line)
            {
                var a=axis==SectionAxis.H?line.X1:line.Y1;var b=axis==SectionAxis.H?line.X2:line.Y2;
                if(Math.Abs(a-b)<=Epsilon||position<Math.Min(a,b)-Epsilon||position>=Math.Max(a,b)-Epsilon)continue;
                var t=(position-a)/(b-a);crossings.Add(axis==SectionAxis.H?line.Y1+(line.Y2-line.Y1)*t:line.X1+(line.X2-line.X1)*t);
            }
            else if(segment is ArcSegment arc)
            {
                var ratio=(position-(axis==SectionAxis.H?arc.Cx:arc.Cy))/arc.Radius;
                if(Math.Abs(ratio)>=1-Epsilon)continue; // Tangency does not cross the material boundary.
                var baseAngle=(axis==SectionAxis.H?Math.Acos(ratio):Math.Asin(ratio))*180/Math.PI;
                var sweep=arc.EndDegrees-arc.StartDegrees;if(Math.Abs(sweep)<=Epsilon)continue;
                foreach(var root in axis==SectionAxis.H?new[]{baseAngle,-baseAngle}:new[]{baseAngle,180-baseAngle})
                {
                    var turn=Math.Round((arc.StartDegrees-root)/360);
                    foreach(var angle in new[]{root+turn*360,root+(turn+1)*360,root+(turn-1)*360})
                    {
                        var t=(angle-arc.StartDegrees)/sweep;if(t< -Epsilon||t>1+Epsilon)continue;
                        var radians=angle*Math.PI/180;var derivative=(axis==SectionAxis.H?-Math.Sin(radians):Math.Cos(radians))*sweep;
                        if(t<=Epsilon&&derivative<=Epsilon||t>=1-Epsilon&&derivative>=-Epsilon)continue;
                        crossings.Add(axis==SectionAxis.H?arc.Cy+arc.Radius*Math.Sin(radians):arc.Cx+arc.Radius*Math.Cos(radians));break;
                    }
                }
            }
        }
        crossings=crossings.Distinct().Order().ToList();var intervals=new List<SectionInterval>();for(var i=0;i+1<crossings.Count;i+=2)if(crossings[i+1]-crossings[i]>Epsilon)intervals.Add(new(crossings[i],crossings[i+1]));return intervals;
    }
}
public sealed record SectionGeometry(IReadOnlyList<SectionPoint2> Points,IReadOnlyList<BendObject> Bends,IReadOnlyList<double> SegmentLengths,double StartPosition,double EndPosition)
{
    public bool IsClipped(double fullLength)=>StartPosition>1e-6||EndPosition<fullLength-1e-6;
}
public static class SectionGeometryEngine
{
    public static IReadOnlyList<SectionGeometry> BuildAll(CosmicDesignerDocument document,SectionAxis axis,bool bent,double sectionPosition=double.NaN)
    {
        var extent=axis==SectionAxis.H?document.Material.Width:document.Material.Height;
        var selector=double.IsFinite(sectionPosition)?Math.Clamp(sectionPosition,0,extent):extent/2;
        var intervals=ContourSectionEngine.MaterialIntervals(document.OuterContour.Segments,axis,selector);
        return intervals.Select(interval=>BuildInterval(document,axis,bent,interval)).ToList();
    }
    public static SectionGeometry Build(CosmicDesignerDocument document,SectionAxis axis,bool bent,double sectionPosition=double.NaN)
    {
        var total=axis==SectionAxis.W?document.Material.Width:document.Material.Height;var selectorExtent=axis==SectionAxis.H?document.Material.Width:document.Material.Height;var selector=double.IsFinite(sectionPosition)?Math.Clamp(sectionPosition,0,selectorExtent):selectorExtent/2;var interval=ContourSectionEngine.MaterialInterval(document.OuterContour.Segments,axis,selector,total);
        return BuildInterval(document,axis,bent,interval);
    }
    static SectionGeometry BuildInterval(CosmicDesignerDocument document,SectionAxis axis,bool bent,SectionInterval interval)
    {
        var total=axis==SectionAxis.W?document.Material.Width:document.Material.Height;var bends=document.Bends.Where(b=>b.Axis==axis&&b.Position>interval.Start+1e-7&&b.Position<interval.End-1e-7).OrderBy(b=>b.Position).ToList();var clipped=interval.Start>1e-6||interval.End<total-1e-6;List<double> segments;
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

public readonly record struct SurfacePoint3(double X,double Y,double Z);
public readonly record struct SurfaceBendEdge(int A,int B,string Layer);
public sealed record SurfaceMeshData(IReadOnlyList<SurfacePoint3> Points,IReadOnlyList<int> Triangles,IReadOnlyList<(int A,int B)> Edges,IReadOnlyList<SurfaceBendEdge> BendEdges);
public static class BentSurfaceEngine
{
    const double Epsilon=1e-7;
    public static SurfaceMeshData Build(CosmicDesignerDocument document)
    {
        var polygon=ContourPolygon(document.OuterContour.Segments);if(polygon.Count<3)return new([],[],[],[]);var cutPolygons=document.Cuts.Select(CutPolygon).Where(x=>x.Count>=3).ToList();var cutPoints=cutPolygons.SelectMany(x=>x).ToList();var wx=document.Bends.Where(b=>b.Axis==SectionAxis.W).Select(b=>b.Position).ToList();var hy=document.Bends.Where(b=>b.Axis==SectionAxis.H).Select(b=>b.Position).ToList();var xs=polygon.Select(p=>p.X).Concat(cutPoints.Select(p=>p.X)).Concat(wx).Distinct().Order().ToList();var ys=polygon.Select(p=>p.Y).Concat(cutPoints.Select(p=>p.Y)).Concat(hy).Distinct().Order().ToList();var wg=SectionGeometryEngine.Build(document,SectionAxis.W,true);var hg=SectionGeometryEngine.Build(document,SectionAxis.H,true);var points=new List<SurfacePoint3>();var triangles=new List<int>();var indices=new Dictionary<(double X,double Y),int>();var boundaryEdges=new HashSet<(int A,int B)>();
        var fold=CreateFoldMap(document,polygon,cutPolygons)??LegacyMap;
        SurfacePoint3 LegacyMap(Point p){var wp=MapAxis(document,SectionAxis.W,wg,p.X);var hp=MapAxis(document,SectionAxis.H,hg,p.Y);return new(wp.X,hp.X,wp.Y+hp.Y);}
        int Vertex(double x,double y){if(indices.TryGetValue((x,y),out var existing))return existing;var index=points.Count;points.Add(fold(new(x,y)));indices[(x,y)]=index;return index;}
        var contourEdges=new[]{polygon}.Concat(cutPolygons).SelectMany(contour=>contour.Select((a,i)=>(A:a,B:contour[(i+1)%contour.Count]))).ToList();
        for(var xi=0;xi<xs.Count-1;xi++)for(var yi=0;yi<ys.Count-1;yi++)
        {
            var x0=xs[xi];var x1=xs[xi+1];var y0=ys[yi];var y1=ys[yi+1];
            if(x1-x0<=Epsilon||y1-y0<=Epsilon)continue;
            var pieces=new List<List<Point>>{new(){new(x0,y0),new(x1,y0),new(x1,y1),new(x0,y1)}};
            // Split before classification so no retained triangle crosses a contour.
            foreach(var (a,b) in contourEdges)
            {
                if(Math.Max(a.X,b.X)<x0-Epsilon||Math.Min(a.X,b.X)>x1+Epsilon||Math.Max(a.Y,b.Y)<y0-Epsilon||Math.Min(a.Y,b.Y)>y1+Epsilon)continue;
                var split=new List<List<Point>>();
                foreach(var piece in pieces)
                {
                    var distances=piece.Select(p=>SignedDistance(a,b,p)).ToList();
                    if(distances.Any(d=>d>Epsilon)&&distances.Any(d=>d< -Epsilon))
                    {
                        split.Add(ClipHalfPlane(piece,distances,true));split.Add(ClipHalfPlane(piece,distances,false));
                    }
                    else split.Add(piece);
                }
                pieces=split;
            }
            foreach(var piece in pieces)
            {
                if(piece.Count<3)continue;
                var center=new Point(piece.Average(p=>p.X),piece.Average(p=>p.Y));
                if(!Inside(polygon,center)||cutPolygons.Any(cut=>Inside(cut,center)))continue;
                for(var i=1;i<piece.Count-1;i++)
                {
                    if(Math.Abs(Vector.CrossProduct(piece[i]-piece[0],piece[i+1]-piece[0]))<=Epsilon*Epsilon)continue;
                    triangles.AddRange([Vertex(piece[0].X,piece[0].Y),Vertex(piece[i].X,piece[i].Y),Vertex(piece[i+1].X,piece[i+1].Y)]);
                }
            }
        }
        void Boundary(Point start,Point end){var ts=new List<double>{0,1};var dx=end.X-start.X;var dy=end.Y-start.Y;if(Math.Abs(dx)>Epsilon)ts.AddRange(wx.Select(x=>(x-start.X)/dx).Where(t=>t>Epsilon&&t<1-Epsilon));if(Math.Abs(dy)>Epsilon)ts.AddRange(hy.Select(y=>(y-start.Y)/dy).Where(t=>t>Epsilon&&t<1-Epsilon));var ordered=ts.Distinct().Order().ToList();for(var i=0;i<ordered.Count-1;i++){var p0=new Point(start.X+dx*ordered[i],start.Y+dy*ordered[i]);var p1=new Point(start.X+dx*ordered[i+1],start.Y+dy*ordered[i+1]);var a=Vertex(p0.X,p0.Y);var b=Vertex(p1.X,p1.Y);boundaryEdges.Add(a<b?(a,b):(b,a));}}
        void Contour(IReadOnlyList<Point> contour){for(var i=0;i<contour.Count;i++)Boundary(contour[i],contour[(i+1)%contour.Count]);}
        Contour(polygon);foreach(var cut in cutPolygons)Contour(cut);foreach(var slit in document.Slits)foreach(var segment in slit.Geometry){var samples=SlitDrawingEngine.Sample(segment).ToList();for(var i=1;i<samples.Count;i++)Boundary(samples[i-1],samples[i]);}var bendEdges=new List<SurfaceBendEdge>();bool MaterialAt(Point point)=>Inside(polygon,point)&&!cutPolygons.Any(cut=>Inside(cut,point));foreach(var bend in document.Bends){if(bend.Axis==SectionAxis.W){for(var i=0;i<ys.Count-1;i++){var y0=ys[i];var y1=ys[i+1];if(MaterialAt(new(bend.Position,(y0+y1)/2)))bendEdges.Add(new(Vertex(bend.Position,y0),Vertex(bend.Position,y1),bend.Layer));}}else{for(var i=0;i<xs.Count-1;i++){var x0=xs[i];var x1=xs[i+1];if(MaterialAt(new((x0+x1)/2,bend.Position)))bendEdges.Add(new(Vertex(x0,bend.Position),Vertex(x1,bend.Position),bend.Layer));}}}return new(points,triangles,boundaryEdges.ToList(),bendEdges);
    }
    static double SignedDistance(Point a,Point b,Point p){var edge=b-a;return edge.Length<=Epsilon?0:Vector.CrossProduct(edge,p-a)/edge.Length;}
    static List<Point> ClipHalfPlane(IReadOnlyList<Point> polygon,IReadOnlyList<double> distances,bool positive)
    {
        var result=new List<Point>();
        for(var i=0;i<polygon.Count;i++)
        {
            var j=(i+1)%polygon.Count;var da=distances[i];var db=distances[j];
            var insideA=positive?da>=0:da<=0;var insideB=positive?db>=0:db<=0;
            if(insideA)result.Add(polygon[i]);
            if(insideA!=insideB){var t=da/(da-db);result.Add(polygon[i]+(polygon[j]-polygon[i])*t);}
        }
        return result;
    }
    // Fold already bent material about the current, transformed hinge. Section
    // profiles are local measurements and must not be added as independent Z offsets.
    static Func<Point,SurfacePoint3>? CreateFoldMap(CosmicDesignerDocument document,List<Point> outer,List<List<Point>> cuts)
    {
        Func<Point,System.Windows.Media.Media3D.Point3D> map=p=>new(p.X,p.Y,0);
        double HingeCoverage(BendObject bend)
        {
            var vertical=bend.Axis==SectionAxis.W;
            var stations=outer.Concat(cuts.SelectMany(c=>c)).Select(p=>vertical?p.Y:p.X).Distinct().Order().ToList();
            double length=0;
            for(var i=0;i+1<stations.Count;i++)
            {
                var t=(stations[i]+stations[i+1])/2;var p=vertical?new Point(bend.Position,t):new Point(t,bend.Position);
                if(Inside(outer,p)&&!cuts.Any(c=>Inside(c,p)))length+=stations[i+1]-stations[i];
            }
            return length/Math.Max(Epsilon,vertical?document.Material.Height:document.Material.Width);
        }
        // Fold the broad wing hinges before the relieved connecting bridge.
        // This also handles the same design rotated through ninety degrees.
        var axisOrder=document.Bends.GroupBy(b=>b.Axis).OrderByDescending(g=>g.Average(HingeCoverage)).ThenBy(g=>g.Key);
        foreach(var bend in axisOrder.SelectMany(g=>g.OrderBy(b=>b.Position)))
        {
            var vertical=bend.Axis==SectionAxis.W;
            Point Flat(double t)=>vertical?new(bend.Position,t):new(t,bend.Position);
            var stations=outer.Concat(cuts.SelectMany(c=>c)).Select(p=>vertical?p.Y:p.X)
                .Concat(document.Bends.Where(b=>b.Axis!=bend.Axis).Select(b=>b.Position)).Distinct().Order().ToList();
            var intervals=new List<(double Start,double End)>();
            for(var i=0;i+1<stations.Count;i++)
            {
                var p=Flat((stations[i]+stations[i+1])/2);
                if(Inside(outer,p)&&!cuts.Any(c=>Inside(c,p)))intervals.Add((stations[i],stations[i+1]));
            }
            if(intervals.Count==0)continue;
            var hinge=intervals.OrderByDescending(s=>s.End-s.Start).First();
            var previous=map;var origin=previous(Flat(hinge.Start));var axis=previous(Flat(hinge.End))-origin;
            if(axis.Length<=Epsilon)continue;axis.Normalize();
            // A continuous crossed crease is not a single rigid hinge after the
            // earlier fold. Preserve the existing preview for that unsupported
            // configuration rather than stretching faces about an arbitrary axis.
            if(intervals.Any(s=>new[]{s.Start,s.End}.Any(t=>System.Windows.Media.Media3D.Vector3D.CrossProduct(axis,previous(Flat(t))-origin).Length>1e-6)))return null;
            var turn=bend.Direction is BendDirection.Up or BendDirection.Left?1d:-1d;
            var angle=turn*(vertical?-1:1)*bend.BendAngle*Math.PI/180;
            var cos=Math.Cos(angle);var sin=Math.Sin(angle);
            map=p=>
            {
                var q=previous(p);if((vertical?p.X:p.Y)<=bend.Position+Epsilon)return q;
                var v=q-origin;
                return origin+v*cos+System.Windows.Media.Media3D.Vector3D.CrossProduct(axis,v)*sin
                    +axis*(System.Windows.Media.Media3D.Vector3D.DotProduct(axis,v)*(1-cos));
            };
        }
        return p=>{var q=map(p);return new(q.X,q.Y,q.Z);};
    }
    static SectionPoint2 MapAxis(CosmicDesignerDocument document,SectionAxis axis,SectionGeometry geometry,double station){var total=axis==SectionAxis.W?document.Material.Width:document.Material.Height;var knots=new List<double>{0};knots.AddRange(document.Bends.Where(b=>b.Axis==axis).OrderBy(b=>b.Position).Select(b=>b.Position));knots.Add(total);for(var i=0;i<knots.Count-1&&i+1<geometry.Points.Count;i++){if(station>knots[i+1]+Epsilon)continue;var span=Math.Max(Epsilon,knots[i+1]-knots[i]);var t=Math.Clamp((station-knots[i])/span,0,1);var a=geometry.Points[i];var b=geometry.Points[i+1];return new(a.X+(b.X-a.X)*t,a.Y+(b.Y-a.Y)*t);}return geometry.Points[^1];}
    static List<Point> CutPolygon(CutOperation cut){if(cut.Geometry.Count==1&&cut.Geometry[0] is CircleSegment circle)return Enumerable.Range(0,24).Select(i=>{var angle=i*Math.PI*2/24;return new Point(circle.Cx+circle.Radius*Math.Cos(angle),circle.Cy+circle.Radius*Math.Sin(angle));}).ToList();return ContourPolygon(cut.Geometry);}
    static List<Point> ContourPolygon(IEnumerable<GeometrySegment> segments)
    {
        var points=new List<Point>();
        foreach(var segment in segments)
        {
            if(segment is LineSegment line)points.Add(new(line.X1,line.Y1));
            else if(segment is ArcSegment arc)
            {
                var sweep=arc.EndDegrees-arc.StartDegrees;
                var steps=Math.Max(1,(int)Math.Ceiling(Math.Abs(sweep)/5-1e-9));
                for(var i=0;i<steps;i++)
                {
                    var angle=(arc.StartDegrees+sweep*i/steps)*Math.PI/180;
                    points.Add(new(arc.Cx+arc.Radius*Math.Cos(angle),arc.Cy+arc.Radius*Math.Sin(angle)));
                }
            }
        }
        return points.Where(point=>double.IsFinite(point.X)&&double.IsFinite(point.Y)).ToList();
    }
    static bool Inside(IReadOnlyList<Point> polygon,Point p){var inside=false;for(int i=0,j=polygon.Count-1;i<polygon.Count;j=i++){var a=polygon[i];var b=polygon[j];if((a.Y>p.Y)!=(b.Y>p.Y)&&p.X<(b.X-a.X)*(p.Y-a.Y)/(b.Y-a.Y)+a.X)inside=!inside;}return inside;}
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
        var semicircleAngle=cut.Geometry.OfType<ArcSegment>().FirstOrDefault()?.StartDegrees??0;
        cut.Geometry.Clear();var shape=cut.Shape;var cx=cut.CenterX;var cy=cut.CenterY;var w=cut.Width;var h=cut.Height;
        if(shape=="Circle"){cut.Geometry.Add(new CircleSegment(cx,cy,w/2));return;}
        if(shape=="QuarterCircle")
        {
            var radius=w;var sx=semicircleAngle is 0 or 270?1:-1;var sy=semicircleAngle is 0 or 90?1:-1;
            var ax=cx-sx*radius/2;var ay=cy-sy*radius/2;var a=semicircleAngle*Math.PI/180;var b=a+Math.PI/2;
            cut.Geometry.Add(new ArcSegment(ax,ay,radius,semicircleAngle,semicircleAngle+90));
            cut.Geometry.Add(new LineSegment(ax+radius*Math.Cos(b),ay+radius*Math.Sin(b),ax,ay));
            cut.Geometry.Add(new LineSegment(ax,ay,ax+radius*Math.Cos(a),ay+radius*Math.Sin(a)));return;
        }
        if(shape=="Semicircle")
        {
            var vertical=semicircleAngle is 0 or 180;var radius=vertical?w/2:h/2;
            var ax=cx+(semicircleAngle==270?-radius/2:semicircleAngle==90?radius/2:0);
            var ay=cy+(semicircleAngle==0?-radius/2:semicircleAngle==180?radius/2:0);
            var a=semicircleAngle*Math.PI/180;var b=a+Math.PI;
            cut.Geometry.Add(new ArcSegment(ax,ay,radius,semicircleAngle,semicircleAngle+180));
            cut.Geometry.Add(new LineSegment(ax+radius*Math.Cos(b),ay+radius*Math.Sin(b),ax+radius*Math.Cos(a),ay+radius*Math.Sin(a)));
            return;
        }
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
        var sb=new StringBuilder(); Pair(sb,0,"SECTION");Pair(sb,2,"HEADER");Pair(sb,9,"$ACADVER");Pair(sb,1,"AC1015");Pair(sb,9,"$MEASUREMENT");Pair(sb,70,"1");Pair(sb,9,"$INSUNITS");Pair(sb,70,d.Unit.DxfCode().ToString(CultureInfo.InvariantCulture));Pair(sb,0,"ENDSEC");
        Pair(sb,0,"SECTION");Pair(sb,2,"ENTITIES");
        foreach(var item in Ordered(d)) WriteEntity(sb,item);
        Pair(sb,0,"ENDSEC");Pair(sb,999,MetadataMarker+JsonSerializer.Serialize(ToDto(d)));Pair(sb,0,"EOF"); File.WriteAllText(path,sb.ToString(),Encoding.ASCII);
    }
    public static CosmicDesignerDocument Load(string path)
    {
        var text=File.ReadAllText(path); var line=text.Split(new[] {'\r','\n'},StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(x=>x.StartsWith(MetadataMarker,StringComparison.Ordinal));
        if(line is null) throw new InvalidDataException("This DXF has no CosmicDesigner metadata."); var dto=JsonSerializer.Deserialize<DocumentDto>(line[MetadataMarker.Length..]) ?? throw new InvalidDataException("Invalid CosmicDesigner metadata.");
        var recognized=MeasurementUnits.TryParse(dto.Unit,out var unit);var scale=recognized?1d:.1d;if(!recognized)unit=MeasurementUnit.Centimeter;var d=new CosmicDesignerDocument();d.SetUnit(unit);d.WSegments.Clear();d.HSegments.Clear();d.Bends.Clear();d.Cuts.Clear();d.InnerContours.Clear();d.MicroJoints.Clear();
        foreach(var x in dto.WSegments)d.WSegments.Add(new SectionSegment{Id=x.Id,Axis=SectionAxis.W,Index=x.Index,Length=x.Length*scale}); foreach(var x in dto.HSegments)d.HSegments.Add(new SectionSegment{Id=x.Id,Axis=SectionAxis.H,Index=x.Index,Length=x.Length*scale});
        foreach(var x in dto.Bends)d.Bends.Add(new(){Id=x.Id,Sequence=x.Sequence,Axis=x.Axis,Direction=x.Direction,Position=x.Position*scale});foreach(var x in dto.Cuts??[]){var cut=new CutOperation{Id=x.Id,Sequence=x.Sequence,Kind=CutKind.Hole,Shape=x.Shape,CenterX=x.CenterX*scale,CenterY=x.CenterY*scale,Width=x.Width*scale,Height=x.Height*scale,Sides=x.Sides};if(x.Geometry is {Count:>0})cut.Geometry.AddRange(x.Geometry.Select(g=>Scale(FromDto(g),scale)));else HoleGeometryEngine.Rebuild(cut);d.Cuts.Add(cut);var contour=new ContourObject{Id=cut.Id,Kind=ContourKind.Inner};contour.Segments.AddRange(cut.Geometry);d.InnerContours.Add(contour);} foreach(var x in dto.MicroJoints)d.MicroJoints.Add(new(){Id=x.Id,Sequence=x.Sequence,ParentContourId=x.ParentContourId,Position=x.Position*scale,Width=x.Width*scale,Enabled=x.Enabled}); d.ChangeThickness(dto.Thickness*scale);if(dto.Outer is {Count:>0})d.PreserveOuterGeometry(dto.Outer.Select(g=>Scale(FromDto(g),scale)));ApplyView(d.WView,dto.WView);ApplyView(d.HView,dto.HView);foreach(var x in dto.Slits??[]){var slit=new SlitOperation{Id=x.Id,Sequence=x.Sequence,Shape=x.Shape};slit.Geometry.AddRange(x.Geometry.Select(g=>Scale(FromDto(g),scale)));d.Slits.Add(slit);}return d;
    }
    static IEnumerable<object> Ordered(CosmicDesignerDocument d) { yield return d.OuterContour; foreach(var x in d.Bends.Cast<DesignObject>().Concat(d.Cuts).Concat(d.Slits).OrderBy(x=>x.Sequence))yield return x; }
    static void WriteEntity(StringBuilder sb,object item)
    {
        if(item is ContourObject c)foreach(var s in c.Segments)WriteSegment(sb,s,"L");
        if(item is BendObject b){GeometrySegment s=b.Axis==SectionAxis.W?new LineSegment(b.Position,0,b.Position,10000):new LineSegment(0,b.Position,10000,b.Position);WriteSegment(sb,s,b.Layer);}
        if(item is SlitOperation slit)foreach(var s in slit.Geometry)WriteSegment(sb,s,"L");
        if(item is CutOperation cut)foreach(var s in cut.Geometry)WriteSegment(sb,s,"L");
    }
    static void WriteSegment(StringBuilder sb,GeometrySegment segment,string layer)
    {
        var n=CultureInfo.InvariantCulture;if(segment is LineSegment l){Pair(sb,0,"LINE");Pair(sb,8,layer);Pair(sb,10,l.X1.ToString(n));Pair(sb,20,l.Y1.ToString(n));Pair(sb,11,l.X2.ToString(n));Pair(sb,21,l.Y2.ToString(n));}
        else if(segment is CircleSegment c){Pair(sb,0,"CIRCLE");Pair(sb,8,layer);Pair(sb,10,c.Cx.ToString(n));Pair(sb,20,c.Cy.ToString(n));Pair(sb,40,c.Radius.ToString(n));}
        else if(segment is ArcSegment a){Pair(sb,0,"ARC");Pair(sb,8,layer);Pair(sb,10,a.Cx.ToString(n));Pair(sb,20,a.Cy.ToString(n));Pair(sb,40,a.Radius.ToString(n));Pair(sb,50,(a.EndDegrees>=a.StartDegrees?a.StartDegrees:a.EndDegrees).ToString(n));Pair(sb,51,(a.EndDegrees>=a.StartDegrees?a.EndDegrees:a.StartDegrees).ToString(n));}
    }
    static void Pair(StringBuilder sb,int code,string value)=>sb.Append(code).Append("\r\n").Append(value).Append("\r\n");
    static DocumentDto ToDto(CosmicDesignerDocument d)=>new(d.Material.Thickness,d.WSegments.Select(x=>new SegmentDto(x.Id,x.Index,x.Length)).ToList(),d.HSegments.Select(x=>new SegmentDto(x.Id,x.Index,x.Length)).ToList(),d.Bends.Select(x=>new BendDto(x.Id,x.Sequence,x.Axis,x.Direction,x.Position)).ToList(),d.Cuts.Select(x=>new CutDto(x.Id,x.Sequence,x.Shape,x.CenterX,x.CenterY,x.Width,x.Height,x.Sides,x.Geometry.Select(ToDto).ToList())).ToList(),d.MicroJoints.ToList(),d.OuterContour.Segments.Select(ToDto).ToList(),d.Unit.Symbol(),ToDto(d.WView),ToDto(d.HView),d.Slits.Select(x=>new SlitDto(x.Id,x.Sequence,x.Shape,x.Geometry.Select(ToDto).ToList())).ToList());
    static GeometryDto ToDto(GeometrySegment x)=>x switch{LineSegment l=>new("LINE",l.X1,l.Y1,l.X2,l.Y2,0),CircleSegment c=>new("CIRCLE",c.Cx,c.Cy,0,0,c.Radius),ArcSegment a=>new("ARC",a.Cx,a.Cy,a.StartDegrees,a.EndDegrees,a.Radius),_=>throw new NotSupportedException()};
    static GeometrySegment FromDto(GeometryDto x)=>x.Type switch{"LINE"=>new LineSegment(x.A,x.B,x.C,x.D),"CIRCLE"=>new CircleSegment(x.A,x.B,x.Radius),"ARC"=>new ArcSegment(x.A,x.B,x.Radius,x.C,x.D),_=>throw new InvalidDataException($"Unsupported metadata geometry: {x.Type}")};
    static GeometrySegment Scale(GeometrySegment x,double s)=>x switch{LineSegment l=>new LineSegment(l.X1*s,l.Y1*s,l.X2*s,l.Y2*s),CircleSegment c=>new CircleSegment(c.Cx*s,c.Cy*s,c.Radius*s),ArcSegment a=>new ArcSegment(a.Cx*s,a.Cy*s,a.Radius*s,a.StartDegrees,a.EndDegrees),_=>x};
    static ViewDto ToDto(SectionViewState x)=>new(x.Dimensions,x.BentMode,x.RotationQuarterTurns);static void ApplyView(SectionViewState target,ViewDto? source){if(source is null)return;target.Dimensions=source.Dimensions;target.BentMode=source.BentMode;target.RotationQuarterTurns=((source.RotationQuarterTurns%4)+4)%4;}
    record DocumentDto(double Thickness,List<SegmentDto> WSegments,List<SegmentDto> HSegments,List<BendDto> Bends,List<CutDto>? Cuts,List<MicroJoint> MicroJoints,List<GeometryDto>? Outer=null,string? Unit=null,ViewDto? WView=null,ViewDto? HView=null,List<SlitDto>? Slits=null);
    record SlitDto(string Id,int Sequence,string Shape,List<GeometryDto> Geometry);
    record ViewDto(bool Dimensions,bool BentMode,int RotationQuarterTurns);
    record SegmentDto(string Id,int Index,double Length); record BendDto(string Id,int Sequence,SectionAxis Axis,BendDirection Direction,double Position);record CutDto(string Id,int Sequence,string Shape,double CenterX,double CenterY,double Width,double Height,int Sides,List<GeometryDto>? Geometry=null);record GeometryDto(string Type,double A,double B,double C,double D,double Radius);
}
