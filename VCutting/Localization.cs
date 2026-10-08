using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace VCutting;

public sealed record LanguageOption(string Code,string Name)
{
    public override string ToString()=>Name;
}

/// <summary>Data-only external catalogs. Display culture never changes numeric or DXF culture.</summary>
public static class Localization
{
    public sealed record Token(string Key,object?[] Arguments);
    static readonly Dictionary<string,Token> Emitted=new(StringComparer.Ordinal);
    static Dictionary<string,string> _english=[],_active=[];
    static readonly List<string> Problems=[];
    static string _bundled="",_user="";
    public static string Language {get;private set;}="en";
    public static event Action? Changed;
    public static IReadOnlyList<string> Diagnostics=>Problems.ToArray();
    public static string UserDirectory=>Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"CosmicDesigner","Languages");
    public static IReadOnlyList<LanguageOption> Languages {get;private set;}=[new("en","English"),new("ko","한국어")];
    public static void Initialize(string language,string? bundled=null,string? user=null)
    {
        _bundled=bundled??Path.Combine(AppContext.BaseDirectory,"Languages");_user=user??UserDirectory;Problems.Clear();
        // Immutable embedded English is a recovery fallback, never a user-editable identifier table.
        using var stream=typeof(Localization).Assembly.GetManifestResourceStream("CosmicDesigner.Languages.en.json");
        _english=stream is null?[]:Read(stream,"embedded English",null).Strings;
        if(!File.Exists(Path.Combine(_bundled,"en.json")))Problems.Add("Bundled English missing; embedded recovery used");
        Merge(_english,Load(Path.Combine(_bundled,"en.json"),_english));
        var languages=new Dictionary<string,LanguageOption>(StringComparer.OrdinalIgnoreCase){["en"]=new("en","English"),["ko"]=new("ko","한국어")};
        foreach(var folder in new[]{_bundled,_user})
        {
            try { if(Directory.Exists(folder))foreach(var path in Directory.GetFiles(folder,"*.json"))
            {var code=Path.GetFileNameWithoutExtension(path);if(!ValidCode(code))continue;var pack=Load(path,_english);if(pack.Code==code&&!string.IsNullOrWhiteSpace(pack.Name))languages[code]=new(code,pack.Name);}}
            catch(Exception ex)when(ex is IOException or UnauthorizedAccessException){Problems.Add(ex.Message);}
        }
        Languages=languages.Values.OrderBy(x=>x.Code=="en"?0:x.Code=="ko"?1:2).ThenBy(x=>x.Code).ToArray();
        SetLanguage(language);
    }
    public static void SetLanguage(string language)
    {
        Language=ValidCode(language)&&Languages.Any(x=>x.Code==language)?language:"en";
        _active=new(_english,StringComparer.Ordinal);Merge(_active,Load(Path.Combine(_user,"en.json"),_english));
        if(Language!="en"){Merge(_active,Load(Path.Combine(_bundled,Language+".json"),_english));Merge(_active,Load(Path.Combine(_user,Language+".json"),_english));}
        Changed?.Invoke();
    }
    public static string Text(string key)=>Format(key,[]);
    public static string Kind(string value)=>_english.ContainsKey("kind."+value)?Text("kind."+value):value;
    sealed record KindArgument(string Value) {public override string ToString()=>Kind(Value);}
    sealed record ResourceArgument(string Key) {public override string ToString()=>Text(Key);}
    public static object DisplayText(string key)=>new ResourceArgument(key);
    public static object DisplayKind(string? value)=>new KindArgument(value??"");
    public static void Reload()=>Initialize(Language,_bundled,_user);
    public static string DxfFilter=>Text("file.dxf").Replace('|',' ').Replace("\0","")+" (*.dxf)|*.dxf";
    public static string About()=>"CosmicDesigner "+VCuttingRelease.VersionInfo.Display+"\n"+Text("about.development")+": "+VCuttingRelease.VersionInfo.Development+"\n"+Text("about.build")+": "+VCuttingRelease.VersionInfo.Build;
    public static string Format(string key,params object?[] arguments)
    {
        var text=Render(new(key,arguments));
        if(Emitted.Count>5000)Emitted.Clear();Emitted[text]=new(key,arguments.ToArray());return text;
    }
    public static string Render(Token token)
    {
        var template=_active.GetValueOrDefault(token.Key)??_english.GetValueOrDefault(token.Key)??token.Key;
        try{return token.Arguments.Length==0?template:string.Format(CultureInfo.CurrentCulture,template,token.Arguments);}
        catch(FormatException){Problems.Add("Format: "+token.Key);var fallback=_english.GetValueOrDefault(token.Key)??token.Key;try{return string.Format(CultureInfo.CurrentCulture,fallback,token.Arguments);}catch(FormatException){return fallback;}}
    }
    public static bool TryToken(string text,out Token token)
    {
        if(Emitted.TryGetValue(text,out token!))return true;
        var key=_english.FirstOrDefault(x=>x.Value==text).Key;
        if(key is not null){token=new(key,[]);return true;}token=null!;return false;
    }
    public static string DiagnosticText=>string.Join(Environment.NewLine,Problems.Distinct());
    static bool ValidCode(string? code)=>code is not null&&Regex.IsMatch(code,@"^[a-z]{2,3}(?:-[A-Za-z0-9]{2,8})*$");
    sealed record Pack(string Code,string Name,Dictionary<string,string> Strings);
    static Pack Load(string path,Dictionary<string,string>? expected)
    {
        if(!File.Exists(path))return new("","",[]);
        try{if(new FileInfo(path).Length>4*1024*1024)throw new InvalidDataException("Catalog exceeds 4 MB");using var file=File.OpenRead(path);return Read(file,path,expected);}
        catch(Exception ex)when(ex is IOException or InvalidDataException or UnauthorizedAccessException or JsonException or DecoderFallbackException or FormatException or InvalidOperationException or KeyNotFoundException){Problems.Add(Path.GetFileName(path)+": "+ex.Message);return new("","",[]);}
    }
    static Pack Read(Stream stream,string source,Dictionary<string,string>? expected)
    {
        using var reader=new StreamReader(stream,new UTF8Encoding(false,true),false,1024,leaveOpen:true);
        using var document=JsonDocument.Parse(reader.ReadToEnd().TrimStart('\uFEFF'));var root=document.RootElement;
        Unique(root);var code=root.GetProperty("code").GetString()??"";var name=root.GetProperty("name").GetString()??"";
        if(!ValidCode(code)||name.Length is 0 or >80)throw new InvalidDataException("Invalid language metadata");
        if(source!="embedded English"&&Path.GetFileNameWithoutExtension(source)!=code)throw new InvalidDataException("Language code differs from filename");
        var strings=root.GetProperty("strings");Unique(strings);var result=new Dictionary<string,string>(StringComparer.Ordinal);
        foreach(var item in strings.EnumerateObject())
        {
            if(item.Value.ValueKind!=JsonValueKind.String){Problems.Add(source+": nonstring "+item.Name);continue;}
            var value=item.Value.GetString()!;
            if(value.Contains('\0')||value.Length>16384){Problems.Add(source+": invalid text length/control character "+item.Name);continue;}
            try
            {
                _=System.Text.CompositeFormat.Parse(value);
                if(expected is not null&&(!expected.TryGetValue(item.Name,out var original)||!Slots(original).SequenceEqual(Slots(value)))){Problems.Add(source+": unknown key/placeholder mismatch "+item.Name);continue;}
                result[item.Name]=value;
            }
            catch(FormatException){Problems.Add(source+": invalid format "+item.Name);}
        }
        return new(code,name,result);
    }
    static string[] Slots(string value)=>Regex.Matches(value,@"(?<!\{)\{(\d+)(?:,[^}:]+)?(?::[^}]+)?\}(?!\})").Select(x=>x.Groups[1].Value).Distinct().Order().ToArray();
    static void Unique(JsonElement value)
    {
        if(value.ValueKind!=JsonValueKind.Object)throw new InvalidDataException("Expected object");var keys=new HashSet<string>(StringComparer.Ordinal);
        foreach(var p in value.EnumerateObject())if(!keys.Add(p.Name))throw new InvalidDataException("Duplicate key: "+p.Name);
    }
    static void Merge(Dictionary<string,string> target,Pack pack){foreach(var pair in pack.Strings)target[pair.Key]=pair.Value;}
}
