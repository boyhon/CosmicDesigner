using System.IO;
using System.Text.Json;

namespace CosmicDesigner;

public sealed class CosmicDesignerSettings
{
    public List<string> RecentFiles { get; set; }=[];
    public bool ShowRuler { get; set; }=true;
    public bool ShowGrid { get; set; }=true;
    public bool ShowStatusBar { get; set; }=true;
    public bool WDimensions { get; set; }=true;
    public bool WBent { get; set; }
    public int WRotation { get; set; }
    public bool HDimensions { get; set; }=true;
    public bool HBent { get; set; }
    public int HRotation { get; set; }
}

public static class UserSettingsStore
{
    static readonly string DirectoryPath=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"CosmicDesigner");
    static readonly string FilePath=Path.Combine(DirectoryPath,"settings.json");
    public static CosmicDesignerSettings Load()
    {
        try{return !File.Exists(FilePath)?new():Deserialize(File.ReadAllText(FilePath));}catch{return new();}
    }
    public static void Save(CosmicDesignerSettings settings)
    {
        Directory.CreateDirectory(DirectoryPath);File.WriteAllText(FilePath,Serialize(settings));
    }
    public static void AddRecent(CosmicDesignerSettings settings,string path)
    {
        UpdateRecentList(settings,path);Save(settings);
    }
    public static string Serialize(CosmicDesignerSettings settings)=>JsonSerializer.Serialize(settings,new JsonSerializerOptions{WriteIndented=true});
    public static CosmicDesignerSettings Deserialize(string json)
    {
        try
        {
            var settings=JsonSerializer.Deserialize<CosmicDesignerSettings>(json)??new();
            settings.RecentFiles=(settings.RecentFiles??[]).Select(NormalizePath).Where(x=>x is not null).Cast<string>().Distinct(StringComparer.OrdinalIgnoreCase).Take(10).ToList();
            settings.WRotation=NormalizeRotation(settings.WRotation);settings.HRotation=NormalizeRotation(settings.HRotation);
            return settings;
        }
        catch{return new();}
    }
    public static void UpdateRecentList(CosmicDesignerSettings settings,string path)
    {
        var full=NormalizePath(path)??throw new ArgumentException("A valid file path is required.",nameof(path));
        settings.RecentFiles=(settings.RecentFiles??[]).Select(NormalizePath).Where(x=>x is not null&&!string.Equals(x,full,StringComparison.OrdinalIgnoreCase)).Cast<string>().ToList();
        settings.RecentFiles.Insert(0,full);if(settings.RecentFiles.Count>10)settings.RecentFiles.RemoveRange(10,settings.RecentFiles.Count-10);
    }
    static string? NormalizePath(string? path){if(string.IsNullOrWhiteSpace(path))return null;try{return Path.GetFullPath(path);}catch{return null;}}
    static int NormalizeRotation(int value)=>((value%4)+4)%4;
}
