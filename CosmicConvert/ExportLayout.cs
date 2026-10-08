using System.Windows;
namespace CosmicConvert;

public sealed record ExportLayout(List<Entity> Shape,List<Entity> Boundary,Vector Offset,double Width,double Height)
{
    public const double Margin=10;
    public IEnumerable<Entity> Entities=>Boundary.Concat(Shape);
    public static ExportLayout Create(ConvertedDrawing drawing)
    {
        var source=drawing.Entities.ToList();if(source.Count==0)throw new ConversionException(5,"No geometry to frame.");
        var points=source.SelectMany(Extrema).ToList();double minX=points.Min(p=>p.X),minY=points.Min(p=>p.Y),maxX=points.Max(p=>p.X),maxY=points.Max(p=>p.Y);
        var offset=new Vector(Margin-minX,Margin-minY);double width=maxX-minX+2*Margin,height=maxY-minY+2*Margin;
        var shape=source.Select(e=>e with{A=e.A+offset,B=e.B+offset,Center=e.Center+offset}).ToList();
        Point a=new(0,0),b=new(width,0),c=new(width,height),d=new(0,height);
        return new(shape,[Entity.Line(a,b),Entity.Line(b,c),Entity.Line(c,d),Entity.Line(d,a)],offset,width,height);
    }
    public static IEnumerable<Point> Extrema(Entity e)
    {
        if(e.Kind=="LINE"){yield return e.A;yield return e.B;yield break;}
        yield return e.At(0);yield return e.At(1);
        for(int i=0;i<4;i++)
        {
            double angle=i*Math.PI/2;double delta=(angle-e.Start)%(2*Math.PI);if(e.Sweep>0&&delta<0)delta+=2*Math.PI;if(e.Sweep<0&&delta>0)delta-=2*Math.PI;
            if(e.Kind=="CIRCLE"||delta/e.Sweep is >=0 and <=1)yield return e.Center+new Vector(e.Radius*Math.Cos(angle),e.Radius*Math.Sin(angle));
        }
    }
}
