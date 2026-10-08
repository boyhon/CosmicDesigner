using System.Text.Json;
using System.Windows;

namespace VCutting;

public sealed record ClipboardGeometry(string Type,double A,double B,double C,double D,double Radius);
public sealed record ClipboardJoint(double Position,double Width,bool Enabled);
public sealed record ShapeClipboardData(int Version,MeasurementUnit Unit,string Type,string Shape,double CenterX,double CenterY,double Width,double Height,int Sides,double Radius,double Rotation,int StarStep,ClipboardGeometry[] Geometry,ClipboardJoint[] Joints);
public static class ShapeClipboard
{
    public const string Format="CosmicDesigner.Shape.v1";
    public const int MaxLength=2_000_000;
    static readonly HashSet<string> Shapes=["Circle","Ellipse","Semicircle","QuarterCircle","Triangle","Rectangle","Diamond","Parallelogram","Polygon","RegularPolygon","RegularStar","Compound"];
    public static bool TryCapture(VCuttingDocument d,DesignObject? selected,out string text)
    {
        text="";
        if(selected is GeometryObject child&&child.ParentContourId!="OUTER")selected=d.Cuts.FirstOrDefault(c=>c.Id==child.ParentContourId);
        if(selected is ContourObject inner&&inner.Kind==ContourKind.Inner)selected=d.Cuts.FirstOrDefault(c=>c.Id==inner.Id);
        ShapeClipboardData? value=null;
        if(selected is CutOperation cut&&d.Cuts.Contains(cut))value=new(1,d.Unit,"Cut",cut.Shape=="Rectangle"&&!RotatedRectangleGeometry.IsParametric(cut)?"Compound":cut.Shape,cut.CenterX,cut.CenterY,cut.Width,cut.Height,cut.Sides,cut.Radius,cut.Rotation,cut.StarStep,cut.Geometry.Select(Encode).ToArray(),d.MicroJoints.Where(j=>j.ParentContourId==cut.Id).Select(j=>new ClipboardJoint(j.Position,j.Width,j.Enabled)).ToArray());
        else if(selected is SlitOperation slit&&d.Slits.Contains(slit))value=new(1,d.Unit,"Slit",slit.Shape,0,0,0,0,0,0,0,0,slit.Geometry.Select(Encode).ToArray(),[]);
        else if(selected is GeometryObject outer&&outer.ParentContourId=="OUTER"&&outer.Geometry is LineSegment or ArcSegment&&d.OuterContour.Segments.Contains(outer.Geometry))value=new(1,d.Unit,"Slit",outer.Geometry is ArcSegment?"Arc":"Line",0,0,0,0,0,0,0,0,[Encode(outer.Geometry)],[]);
        if(value is {Type:"Cut"}&&value.Shape.StartsWith("Imported ",StringComparison.Ordinal))
        {
            var geometry=value.Geometry.Select(Decode).ToArray();
            if(CutContourEngine.Loops(geometry).Count==0)value=value with {Type="Slit",Shape=geometry.Length==1&&geometry[0] is ArcSegment?"Arc":geometry.Length==1?"Line":"Polyline",Joints=[]};
            else
            {
                var points=BoundsPoints(geometry).ToArray();var x=points.Min(p=>p.X);var y=points.Min(p=>p.Y);var w=points.Max(p=>p.X)-x;var h=points.Max(p=>p.Y)-y;
                value=value with {Shape=geometry.Length==1&&geometry[0] is CircleSegment?"Circle":"Compound",CenterX=x+w/2,CenterY=y+h/2,Width=w,Height=h,Radius=geometry[0] is CircleSegment circle?circle.Radius:value.Radius};
            }
        }
        if(value is null)return false;text=JsonSerializer.Serialize(value);return TryRead(text,out _);
    }
    public static bool TryRead(string? text,out ShapeClipboardData? data)
    {
        data=null;if(string.IsNullOrEmpty(text)||text.Length>MaxLength)return false;
        try
        {
            var p=JsonSerializer.Deserialize<ShapeClipboardData>(text,new JsonSerializerOptions{MaxDepth=16});
            if(p is null||p.Version!=1||!Enum.IsDefined(p.Unit)||p.Geometry is not {Length:>0 and <=10000}||p.Joints is null||p.Joints.Length>1000)return false;
            if(p.Type=="Cut")
            {
                if(!Shapes.Contains(p.Shape)||!Finite(p.CenterX,p.CenterY,p.Width,p.Height,p.Radius,p.Rotation)||p.Width<=0||p.Height<=0||p.Radius<=0)return false;
                if(p.Shape is "Polygon" or "RegularPolygon" or "RegularStar" && p.Sides is <3 or >512)return false;
                if(p.Shape=="RegularStar"&&(p.Sides<5||p.StarStep<2||p.StarStep>(p.Sides-1)/2))return false;
            }
            else if(p.Type!="Slit"||p.Shape is not ("Line" or "Arc" or "Polyline")||p.Joints.Length!=0)return false;
            foreach(var g in p.Geometry)
            {
                if(g is null||!Finite(g.A,g.B,g.C,g.D,g.Radius))return false;
                if(g.Type=="LINE"){if(new Vector(g.C-g.A,g.D-g.B).Length<1e-9)return false;}
                else if(g.Type=="ARC"){if(g.Radius<=0||Math.Abs(g.D-g.C)<1e-9||Math.Abs(g.D-g.C)>=360)return false;}
                else if(g.Type!="CIRCLE"||g.Radius<=0)return false;
            }
            if(p.Joints.Any(j=>j is null||!Finite(j.Position,j.Width)||j.Position<0||j.Width<=0))return false;
            var segments=p.Geometry.Select(Decode).ToArray();
            if(p.Type=="Cut"&&CutContourEngine.Loops(segments).Count==0)return false;
            // A declared parametric shape must match its actual profile; reject misleading metadata.
            if(p.Type=="Cut"&&p.Shape is "Rectangle" or "RegularPolygon" or "RegularStar")
            {
                var cut=Cut(p,1,0,0);var expected=Cut(p,1,0,0);
                if(p.Shape=="Rectangle")RotatedRectangleGeometry.Rebuild(expected);else if(p.Shape=="RegularPolygon")RegularPolygonGeometry.Rebuild(expected);else RegularStarGeometry.Rebuild(expected);
                if(cut.Geometry.Count!=expected.Geometry.Count||cut.Geometry.Zip(expected.Geometry).Any(pair=>!Near(pair.First,pair.Second)))return false;
            }
            data=p;return true;
        }
        catch(JsonException){return false;}catch(ArgumentException){return false;}catch(InvalidOperationException){return false;}
    }
    static bool Finite(params double[] values)=>values.All(x=>double.IsFinite(x)&&Math.Abs(x)<=1e12);
    static bool Near(GeometrySegment a,GeometrySegment b)=>a is LineSegment x&&b is LineSegment y&&new Vector(x.X1-y.X1,x.Y1-y.Y1).Length<1e-6&&new Vector(x.X2-y.X2,x.Y2-y.Y2).Length<1e-6;
    static ClipboardGeometry Encode(GeometrySegment g)=>g switch{LineSegment l=>new("LINE",l.X1,l.Y1,l.X2,l.Y2,0),ArcSegment a=>new("ARC",a.Cx,a.Cy,a.StartDegrees,a.EndDegrees,a.Radius),CircleSegment c=>new("CIRCLE",c.Cx,c.Cy,0,0,c.Radius),_=>throw new ArgumentException()};
    static GeometrySegment Decode(ClipboardGeometry g)=>g.Type switch{"LINE"=>new LineSegment(g.A,g.B,g.C,g.D),"ARC"=>new ArcSegment(g.A,g.B,g.Radius,g.C,g.D),_=>new CircleSegment(g.A,g.B,g.Radius)};
    static GeometrySegment Transform(GeometrySegment g,double scale,double x,double y)=>g switch{LineSegment l=>new LineSegment(l.X1*scale+x,l.Y1*scale+y,l.X2*scale+x,l.Y2*scale+y),ArcSegment a=>new ArcSegment(a.Cx*scale+x,a.Cy*scale+y,a.Radius*scale,a.StartDegrees,a.EndDegrees),CircleSegment c=>new CircleSegment(c.Cx*scale+x,c.Cy*scale+y,c.Radius*scale),_=>throw new ArgumentException()};
    static IEnumerable<Point> BoundsPoints(IReadOnlyList<GeometrySegment> geometry)=>geometry.SelectMany(g=>g is CircleSegment c?new[]{new Point(c.Cx-c.Radius,c.Cy-c.Radius),new Point(c.Cx+c.Radius,c.Cy+c.Radius)}:CutContourEngine.Extrema([g]));
    static CutOperation Cut(ShapeClipboardData p,double scale,double x,double y)
    {
        var c=new CutOperation{Kind=CutKind.Hole,Shape=p.Shape,ParentContourId="OUTER",CenterX=p.CenterX*scale+x,CenterY=p.CenterY*scale+y,Width=p.Width*scale,Height=p.Height*scale,Sides=p.Sides,Radius=p.Radius*scale,Rotation=p.Rotation,StarStep=p.StarStep};
        c.Geometry.AddRange(p.Geometry.Select(g=>Transform(Decode(g),scale,x,y)));return c;
    }
    public static bool TryPrepare(VCuttingDocument d,string? text,int step,out DesignObject? shape,out ClipboardJoint[] joints)
    {
        shape=null;joints=[];if(!TryRead(text,out var p)||p is null||step<1||step>100000)return false;
        var scale=p.Unit.Metres()/d.Unit.Metres();var original=p.Geometry.Select(g=>Transform(Decode(g),scale,0,0)).ToArray();
        var points=BoundsPoints(original).ToArray();if(points.Length==0)return false;
        var left=points.Min(v=>v.X);var right=points.Max(v=>v.X);var bottom=points.Min(v=>v.Y);var top=points.Max(v=>v.Y);
        if(right-left>d.Material.Width||top-bottom>d.Material.Height)return false;
        var delta=.01/d.Unit.Metres()*step;var x=Math.Clamp(delta,-left,d.Material.Width-right);var y=Math.Clamp(-delta,-bottom,d.Material.Height-top);
        if(p.Type=="Cut")shape=Cut(p,scale,x,y);
        else{var slit=new SlitOperation{Shape=p.Shape};slit.Geometry.AddRange(original.Select(g=>Transform(g,1,x,y)));shape=slit;}
        joints=p.Joints.Select(j=>j with {Position=j.Position*scale,Width=j.Width*scale}).ToArray();return true;
    }
    public static DesignObject Insert(VCuttingDocument d,DesignObject prepared,IReadOnlyList<ClipboardJoint> joints)
    {
        var prefix=prepared is CutOperation?"H":"SL";var number=1;var ids=d.Cuts.Select(c=>c.Id).Concat(d.Slits.Select(s=>s.Id)).ToHashSet();while(ids.Contains($"{prefix}{number:000}"))number++;
        prepared.Id=$"{prefix}{number:000}";prepared.Sequence=NextSequence(d);
        if(prepared is CutOperation cut){d.Cuts.Add(cut);var contour=new ContourObject{Id=cut.Id,Kind=ContourKind.Inner};contour.Segments.AddRange(cut.Geometry);d.InnerContours.Add(contour);}
        else d.Slits.Add((SlitOperation)prepared);
        foreach(var j in joints){var n=1;while(d.MicroJoints.Any(v=>v.Id==$"MJ{n:000}"))n++;d.MicroJoints.Add(new MicroJoint{Id=$"MJ{n:000}",Sequence=NextSequence(d),ParentContourId=prepared.Id,Position=j.Position,Width=j.Width,Enabled=j.Enabled});}
        d.Recalculate();return prepared;
    }
    static int NextSequence(VCuttingDocument d)=>d.Cuts.Cast<DesignObject>().Concat(d.Slits).Concat(d.Bends).Concat(d.MicroJoints).Select(o=>o.Sequence).DefaultIfEmpty(0).Max()+1;
}
