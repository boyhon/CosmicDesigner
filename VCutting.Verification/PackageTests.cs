using System.IO;

static class PackageTests
{
    public static void IntegrationInstaller()
    {
        var root=new DirectoryInfo(AppContext.BaseDirectory);
        while(root is not null&&!File.Exists(Path.Combine(root.FullName,"VCutting.sln")))root=root.Parent;
        if(root is null)throw new Exception("Repository not found");
        var before=Directory.GetFiles(Path.Combine(root.FullName,"release/freezes"));
        var folder=Path.Combine(root.FullName,"artifacts","integration-validation-"+Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        try
        {
            (int Code,string Text) Invoke(string id,string output)
            {
                var info=new System.Diagnostics.ProcessStartInfo(Environment.GetEnvironmentVariable("VCUTTING_TEST_POWERSHELL")??"pwsh"){RedirectStandardOutput=true,RedirectStandardError=true,UseShellExecute=false,CreateNoWindow=true};
                foreach(var arg in new[]{"-NoProfile","-File",Path.Combine(root.FullName,"installer/Build-IntegrationInstaller.ps1"),"-BuildNumber",id,"-OutputRoot",output,"-ValidateOnly"})info.ArgumentList.Add(arg);
                using var process=System.Diagnostics.Process.Start(info)!;
                var stdout=process.StandardOutput.ReadToEnd();var stderr=process.StandardError.ReadToEnd();process.WaitForExit();
                return(process.ExitCode,stdout+stderr);
            }
            var valid=Invoke("fixture",Path.Combine(folder,"fresh"));
            if(valid.Code!=0||!valid.Text.Contains("Development")||!valid.Text.Contains("INTEGRATION_TEST"))throw new Exception("Valid development integration options rejected: "+valid.Text);
            if(Invoke("../invalid",Path.Combine(folder,"fresh")).Code==0)throw new Exception("Invalid ID accepted");
            if(Invoke("fixture",Path.Combine(root.FullName,"outside-artifacts")).Code==0)throw new Exception("Outside root accepted");
            if(Invoke("fixture",folder).Code==0)throw new Exception("Existing output accepted");
            if(Directory.Exists(Path.Combine(folder,"fresh")))throw new Exception("Validation wrote outputs");
            if(!before.SequenceEqual(Directory.GetFiles(Path.Combine(root.FullName,"release/freezes"))))throw new Exception("Integration allocated Freeze");
        }
        finally{Directory.Delete(folder,true);}
    }
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
