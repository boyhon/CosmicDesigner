using System.IO;
using System.Security.Cryptography;
using System.Xml.Linq;
using CosmicConvert;
using VCutting;
internal static partial class Verification
{
    static void InvisibleText()
    {
        const string shape="<circle cx='20' cy='20' r='5'/>";
        foreach(var label in new[]{"<text fill-opacity='0'>양영권회장</text>","<text style='fill-opacity:0%'>label</text>","<text fill='none'>label</text>","<text fill-opacity='0' stroke='black' stroke-opacity='0'>label</text>"})
            Check(Read(shape+label).Outlines.Count==1,"invisible leaf label excluded");
        Reject(shape+"<text fill-opacity='0' stroke='black'>visible stroke</text>");Reject(shape+"<text fill-opacity='0'><tspan fill-opacity='1'>visible child</tspan></text>");Reject(shape+"<text fill-opacity='0' style='fill-opacity:1'>visible CSS override</text>");Reject(shape+"<text>visible</text>");
        var source=@"C:\MyDisk\Projects\절곡도면설계프로그램\에코드롬\Sample - 양영권회장.svg";var hash=SHA256.HashData(File.ReadAllBytes(source));
        var xml=XDocument.Load(source);foreach(var text in xml.Descendants().Where(e=>e.Name.LocalName=="text").ToArray())text.Remove();var pathOnly=Path.Combine(Fixtures,"cr088-path-only.svg");xml.Save(pathOnly);
        var actual=Optimizer.Convert(SvgReader.Read(source));var reference=Optimizer.Convert(SvgReader.Read(pathOnly));
        Check(actual.Entities.SequenceEqual(reference.Entities)&&actual.ErrorBound==reference.ErrorBound,"sample matches path-only exactly");
        var folder=Path.Combine(Repo,"artifacts","cr088-comparison");Directory.CreateDirectory(folder);var target=Path.Combine(folder,"Sample - 양영권회장.native.dxf");DxfWriter.Save(actual,target);DxfWriter.Verify(target,actual);
        Check(VCuttingDxfSerializer.HasMetadata(target)&&VCuttingDxfSerializer.Load(target).Cuts.Count>0,"actual native load");
        Check(hash.SequenceEqual(SHA256.HashData(File.ReadAllBytes(source))),"original SVG preserved");
        File.WriteAllText(Path.Combine(folder,"metrics.txt"),$"SourceSHA256={System.Convert.ToHexString(hash)}\nEntities={actual.Entities.Count()+4}\nErrorBoundMm={actual.ErrorBound:R}\nOutput={target}\n");
    }
}
