using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
namespace CosmicConvert;

public static class DxfWriter
{
    static readonly CultureInfo Invariant=CultureInfo.InvariantCulture;
    public static void Save(ConvertedDrawing drawing,string path,CancellationToken token=default,IProgress<string>? progress=null)
    {
        string? temp=null;
        try
        {
            path=Path.GetFullPath(path);temp=Path.Combine(Path.GetDirectoryName(path)!,"."+Path.GetFileName(path)+"."+Guid.NewGuid()+".tmp");
            progress?.Report("Writing DXF");using(var writer=new StreamWriter(new FileStream(temp,FileMode.CreateNew,FileAccess.Write,FileShare.None),new UTF8Encoding(false)))
            {
                void Pair(int code,object value){writer.WriteLine(code);writer.WriteLine(value is double d?d.ToString("G17",Invariant):value);}
                Pair(0,"SECTION");Pair(2,"HEADER");Pair(9,"$ACADVER");Pair(1,"AC1015");Pair(9,"$INSUNITS");Pair(70,4);Pair(9,"$MEASUREMENT");Pair(70,1);Pair(0,"ENDSEC");Pair(0,"SECTION");Pair(2,"ENTITIES");
                foreach(var e in ExportLayout.Create(drawing).Entities)
                {
                    token.ThrowIfCancellationRequested();Pair(0,e.Kind);Pair(8,"L");
                    if(e.Kind=="LINE"){Pair(10,e.A.X);Pair(20,e.A.Y);Pair(30,0d);Pair(11,e.B.X);Pair(21,e.B.Y);Pair(31,0d);}
                    else{Pair(10,e.Center.X);Pair(20,e.Center.Y);Pair(30,0d);Pair(40,e.Radius);if(e.Kind=="ARC"){double start=e.Sweep>0?e.Start:e.Start+e.Sweep;Pair(50,Degrees(start));Pair(51,Degrees(start+Math.Abs(e.Sweep)));}}
                }
                Pair(0,"ENDSEC");Pair(999,DesignerMetadata.Marker+DesignerMetadata.Serialize(drawing));Pair(0,"EOF");
            }
            progress?.Report("Re-reading and validating DXF");Verify(temp,drawing);token.ThrowIfCancellationRequested();
            // Same directory: replacement happens only after complete write and read-back validation.
            if(File.Exists(path))File.Replace(temp,path,null);else File.Move(temp,path);temp=null;
        }
        catch(ConversionException){throw;}catch(OperationCanceledException){throw;}
        catch(Exception ex)when(ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException){throw new ConversionException(6,ex.Message);}
        finally{if(temp is not null&&File.Exists(temp))File.Delete(temp);}
    }
    static double Degrees(double a)=>(a*180/Math.PI%360+360)%360;
    public static void Verify(string path,ConvertedDrawing expected)
    {
        var lines=File.ReadAllLines(path);if(lines.Length%2!=0||lines.Length<4||lines[^1]!="EOF")throw new ConversionException(5,"Incomplete DXF.");
        var parsed=new List<Dictionary<int,string>>();Dictionary<int,string>? current=null;bool units=false;
        for(int i=0;i<lines.Length;i+=2)
        {
            int code=int.Parse(lines[i],Invariant);var value=lines[i+1];
            if(code==9&&value=="$INSUNITS")units=i+3<lines.Length&&lines[i+3]=="4";
            if(code==0){current=null;if(value is "LINE" or "ARC" or "CIRCLE"){current=new(){{0,value}};parsed.Add(current);}}
            else if(current is not null)current[code]=value;
        }
        var entities=ExportLayout.Create(expected).Entities.ToList();if(!units||parsed.Count!=entities.Count)throw new ConversionException(5,"DXF units/entity count mismatch.");
        for(int i=0;i<entities.Count;i++)
        {
            var e=entities[i];var p=parsed[i];double N(int code)=>double.Parse(p[code],Invariant);bool Near(double a,double b)=>double.IsFinite(b)&&Math.Abs(a-b)<=1e-9;
            if(p[0]!=e.Kind||p[8]!="L")throw new ConversionException(5,"DXF type/layer mismatch.");
            var a=e.Kind=="LINE"?e.A:e.Center;if(!Near(a.X,N(10))||!Near(a.Y,N(20)))throw new ConversionException(5,"DXF coordinates mismatch.");
            if(e.Kind=="LINE"){if(!Near(e.B.X,N(11))||!Near(e.B.Y,N(21)))throw new ConversionException(5,"DXF endpoint mismatch.");}
            else{if(!Near(e.Radius,N(40)))throw new ConversionException(5,"DXF radius mismatch.");if(e.Kind=="ARC"){var start=e.Sweep>0?e.Start:e.Start+e.Sweep;if(!Near(Degrees(start),N(50))||!Near(Degrees(start+Math.Abs(e.Sweep)),N(51)))throw new ConversionException(5,"DXF angle mismatch.");}}
        }
        if(expected.ErrorBound>Optimizer.Tolerance)throw new ConversionException(5,"Tolerance exceeded.");
        DesignerMetadata.Verify(lines,expected);
    }
}
