using System.IO;

static class PackageTests
{
    public static void Run()
    {
        var root=new DirectoryInfo(AppContext.BaseDirectory);
        while(root is not null&&!File.Exists(Path.Combine(root.FullName,"VCutting.sln")))root=root.Parent;
        if(root is null)throw new Exception("Repository not found");
        var script=File.ReadAllText(Path.Combine(root.FullName,"installer","Build-Installer.ps1"));
        var definition=File.ReadAllText(Path.Combine(root.FullName,"installer","VCutting.iss"));
        if(!script.Contains("CosmicConvert\\CosmicConvert.csproj")||!script.Contains("\"CosmicConvert.exe\""))throw new Exception("Converter publish/stage contract missing");
        if(!definition.Contains("Name: \"{group}\\CosmicConvert\"; Filename: \"{app}\\CosmicConvert.exe\""))throw new Exception("Converter shortcut missing");
        if(!definition.Contains("Filename: \"{app}\\CosmicDesigner.exe\""))throw new Exception("Designer identity changed");
    }
}
