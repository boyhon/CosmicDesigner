using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
namespace CosmicConvert;
public static partial class SvgReader
{
    static double Value(XElement e,string name,double fallback=0)=>e.Attribute(name) is { } a?RootLength(a.Value,fallback):fallback;
    static double RootLength(string? value,double fallback)
    {
        if(value is null)return fallback;if(value.Length>128)throw Invalid("resource limit");var match=Regex.Match(value.Trim(),@"^([+-]?(?:\d+\.?\d*|\.\d+)(?:[eE][+-]?\d+)?)(px|mm|cm|in|pt|pc)?$",RegexOptions.CultureInvariant|RegexOptions.NonBacktracking);if(!match.Success)throw Invalid(value);
        var number=double.Parse(match.Groups[1].Value,CultureInfo.InvariantCulture);var factor=match.Groups[2].Value switch{"mm"=>96/25.4,"cm"=>96/2.54,"in"=>96,"pt"=>96/72d,"pc"=>16,_=>1};if(!double.IsFinite(number)||Math.Abs(number)>1e8)throw Invalid(value);return number*factor;
    }
    static double[] Numbers(string text)
    {
        if(text.Length>200000)throw Invalid("resource limit");var matches=Number.Matches(text);if(matches.Count>10000||!string.IsNullOrWhiteSpace(Number.Replace(text,"").Replace(",","")))throw Invalid(text.Length>64?text[..64]:text);
        var values=matches.Select(m=>double.Parse(m.Value,CultureInfo.InvariantCulture)).ToArray();if(values.Any(v=>!double.IsFinite(v)||Math.Abs(v)>1e8))throw Invalid("resource limit");return values;
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
        if(string.IsNullOrWhiteSpace(text))return Matrix.Identity;if(text.Length>20000)throw Invalid("resource limit");const RegexOptions options=RegexOptions.CultureInvariant|RegexOptions.NonBacktracking;var matches=Regex.Matches(text,@"([A-Za-z]+)\s*\(([^)]*)\)",options);if(!string.IsNullOrWhiteSpace(Regex.Replace(text,@"([A-Za-z]+)\s*\(([^)]*)\)","",options).Replace(",","")))throw Invalid("transform");var result=Matrix.Identity;
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
