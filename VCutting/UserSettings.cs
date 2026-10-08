using System.IO;
using System.Text.Json;

namespace VCutting;

public sealed class VCuttingSettings
{
    public string DisplayLanguage { get; set; }="en";
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
    public MeasurementUnit DefaultUnit { get; set; }=MeasurementUnit.Millimeter;
    public double NewWidth { get; set; }=3000;
    public double NewHeight { get; set; }=3000;
    public double NewThickness { get; set; }=2;
    public string DefaultOpenFolder { get; set; }="";
    public string DefaultSaveFolder { get; set; }="";
    public int RecentFileCount { get; set; }=10;
    public double ZoomStepPercent { get; set; }=15;
    public double SelectionTolerancePixels { get; set; }=7;
    public double HandleRadiusPixels { get; set; }=6;
    public bool Transparent3DDefault { get; set; }
    public int Transparent3DOpacity { get; set; }=105;
    public int Solid3DOpacity { get; set; }=175;
    public double Edge3DScale { get; set; }=1;
    public double HorizontalRulerIntervalMetres { get; set; }
    public double VerticalRulerIntervalMetres { get; set; }
    public double GridIntervalMetres { get; set; }
}

public static class UserSettingsStore
{
    // CR-069: keep the established user data location across the product rename.
    static readonly string DirectoryPath=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"CosmicDesigner");
    static readonly string FilePath=Path.Combine(DirectoryPath,"settings.json");
    public static VCuttingSettings Load()
    {
        try{return !File.Exists(FilePath)?new():Deserialize(File.ReadAllText(FilePath));}catch{return new();}
    }
    public static void Save(VCuttingSettings settings)
    {
        Directory.CreateDirectory(DirectoryPath);File.WriteAllText(FilePath,Serialize(settings));
    }
    public static void AddRecent(VCuttingSettings settings,string path)
    {
        UpdateRecentList(settings,path);Save(settings);
    }
    public static string Serialize(VCuttingSettings settings)=>JsonSerializer.Serialize(settings,new JsonSerializerOptions{WriteIndented=true});
    public static VCuttingSettings Deserialize(string json)
    {
        try
        {
            using var parsed=JsonDocument.Parse(json);
            var settings=JsonSerializer.Deserialize<VCuttingSettings>(json)??new();
            // Valid legacy settings without a unit retain their cm interpretation and defaults.
            if(parsed.RootElement.ValueKind!=JsonValueKind.Object)return new();
            if(!parsed.RootElement.TryGetProperty("DefaultUnit",out _)){settings.DefaultUnit=MeasurementUnit.Centimeter;if(!parsed.RootElement.TryGetProperty("NewWidth",out _))settings.NewWidth=300;if(!parsed.RootElement.TryGetProperty("NewHeight",out _))settings.NewHeight=300;if(!parsed.RootElement.TryGetProperty("NewThickness",out _))settings.NewThickness=.2;}
            if(!Enum.IsDefined(settings.DefaultUnit))settings.DefaultUnit=MeasurementUnit.Centimeter;
            if(!parsed.RootElement.TryGetProperty("NewWidth",out _))settings.NewWidth=3/settings.DefaultUnit.Metres();if(!parsed.RootElement.TryGetProperty("NewHeight",out _))settings.NewHeight=3/settings.DefaultUnit.Metres();if(!parsed.RootElement.TryGetProperty("NewThickness",out _))settings.NewThickness=.002/settings.DefaultUnit.Metres();
            if(string.IsNullOrWhiteSpace(settings.DisplayLanguage))settings.DisplayLanguage="en";
            settings.RecentFiles=(settings.RecentFiles??[]).Select(NormalizePath).Where(x=>x is not null).Cast<string>().Distinct(StringComparer.OrdinalIgnoreCase).Take(10).ToList();
            settings.WRotation=NormalizeRotation(settings.WRotation);settings.HRotation=NormalizeRotation(settings.HRotation);settings.RecentFileCount=Math.Clamp(settings.RecentFileCount,1,50);settings.ZoomStepPercent=Math.Clamp(settings.ZoomStepPercent,1,100);settings.SelectionTolerancePixels=Math.Clamp(settings.SelectionTolerancePixels,2,30);settings.HandleRadiusPixels=Math.Clamp(settings.HandleRadiusPixels,3,20);settings.Transparent3DOpacity=Math.Clamp(settings.Transparent3DOpacity,20,240);settings.Solid3DOpacity=Math.Clamp(settings.Solid3DOpacity,20,255);settings.Edge3DScale=Math.Clamp(settings.Edge3DScale,.25,4);settings.HorizontalRulerIntervalMetres=NormalizeInterval(settings.HorizontalRulerIntervalMetres);settings.VerticalRulerIntervalMetres=NormalizeInterval(settings.VerticalRulerIntervalMetres);settings.GridIntervalMetres=NormalizeInterval(settings.GridIntervalMetres);if(!double.IsFinite(settings.NewWidth)||settings.NewWidth<=0)settings.NewWidth=3/settings.DefaultUnit.Metres();if(!double.IsFinite(settings.NewHeight)||settings.NewHeight<=0)settings.NewHeight=3/settings.DefaultUnit.Metres();if(!double.IsFinite(settings.NewThickness)||settings.NewThickness<=0)settings.NewThickness=.002/settings.DefaultUnit.Metres();
            return settings;
        }
        catch{return new();}
    }
    public static void UpdateRecentList(VCuttingSettings settings,string path)
    {
        var full=NormalizePath(path)??throw new ArgumentException(Localization.Text("ui.0195"),nameof(path));
        settings.RecentFiles=(settings.RecentFiles??[]).Select(NormalizePath).Where(x=>x is not null&&!string.Equals(x,full,StringComparison.OrdinalIgnoreCase)).Cast<string>().ToList();
        settings.RecentFiles.Insert(0,full);var limit=Math.Clamp(settings.RecentFileCount,1,50);if(settings.RecentFiles.Count>limit)settings.RecentFiles.RemoveRange(limit,settings.RecentFiles.Count-limit);
    }
    static string? NormalizePath(string? path){if(string.IsNullOrWhiteSpace(path))return null;try{return Path.GetFullPath(path);}catch{return null;}}
    static int NormalizeRotation(int value)=>((value%4)+4)%4;
    static double NormalizeInterval(double value)=>double.IsFinite(value)&&value>0?value:0;
}
