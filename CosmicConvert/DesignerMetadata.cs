using System.Text.Json;
namespace CosmicConvert;

internal static partial class DesignerMetadata
{
    internal const string Marker="COSMIC_DESIGNER_JSON:";
    public static string Serialize(ConvertedDrawing drawing)
    {
        var layout=ExportLayout.Create(drawing);var cuts=new List<CutDto>();var slits=new List<SlitDto>();int sequence=1;
        var items=drawing.Source.Outlines.Select((outline,index)=>(outline,index)).GroupBy(x=>x.outline.Source);
        foreach(var source in items)
        {
            var closed=new List<Entity>();
            foreach(var item in source)
            {
                var geometry=drawing.Groups[item.index].Select(e=>Translate(e,layout)).ToList();if(geometry.Count==0)continue;
                if(item.outline.Closed)closed.AddRange(geometry);
                else slits.Add(new($"S{slits.Count+1:000}",sequence++,geometry.Count==1?geometry[0].Kind=="ARC"?"Arc":"Line":"Polyline",geometry.Select(ToDto).ToList()));
            }
            if(closed.Count==0)continue;
            var points=closed.SelectMany(ExportLayout.Extrema).ToList();double x=points.Min(p=>p.X),y=points.Min(p=>p.Y),w=points.Max(p=>p.X)-x,h=points.Max(p=>p.Y)-y;
            bool circle=closed.Count==1&&closed[0].Kind=="CIRCLE";
            cuts.Add(new($"H{cuts.Count+1:000}",sequence++,circle?"Circle":"Compound",x+w/2,y+h/2,w,h,7,closed.Select(ToDto).ToList()));
        }
        var document=new DocumentDto(2,[new("W-S001",0,layout.Width)],[new("H-S001",0,layout.Height)],[],cuts,[],layout.Boundary.Select(ToDto).ToList(),"mm",new(true,false,0),new(true,false,0),slits);
        return JsonSerializer.Serialize(document);
    }
    static Entity Translate(Entity e,ExportLayout layout)=>e with{A=e.A+layout.Offset,B=e.B+layout.Offset,Center=e.Center+layout.Offset};
    static GeometryDto ToDto(Entity e)=>e.Kind switch
    {
        "LINE"=>new("LINE",e.A.X,e.A.Y,e.B.X,e.B.Y,0),
        "CIRCLE"=>new("CIRCLE",e.Center.X,e.Center.Y,0,0,e.Radius),
        "ARC"=>new("ARC",e.Center.X,e.Center.Y,e.Start*180/Math.PI,(e.Start+e.Sweep)*180/Math.PI,e.Radius),
        _=>throw new ConversionException(5,"Unsupported metadata entity.")
    };
    public static void Verify(string[] lines,ConvertedDrawing expected)
    {
        var found=lines.Where(l=>l.StartsWith(Marker,StringComparison.Ordinal)).ToList();
        if(found.Count!=1||found[0]!=Marker+Serialize(expected))throw new ConversionException(5,"Native object metadata missing or mismatched.");
    }
}
