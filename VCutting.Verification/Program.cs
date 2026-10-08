using System.Reflection;
using VCutting;
using System.IO;
using System.Text;

Localization.Initialize("en");
var regressionTests=new (string Id,Action Run)[]{
    ("TC-093-001", (Action)PackageTests.IntegrationInstaller),
    ("TC-092-001", (Action)ReleaseVersionPolicy),
    ("TC-091-001", (Action)ReleaseVersionPolicy),
    ("TC-090-001", (Action)PackageTests.Run),
    ("TC-089-001", (Action)SectionFitTests.Run),
    ("TC-084-001", (Action)SvgImportTests.Coordinates),
    ("TC-084-002", (Action)SvgImportTests.Paths),
    ("TC-084-003", (Action)SvgImportTests.Validation),
    ("TC-083-001", (Action)ClipboardTests.Run),
    ("TC-081-001", (Action)AngleEditingTests.Arcs),
    ("TC-082-001", (Action)AngleEditingTests.Rectangles),
    ("TC-079-001", (Action)RegularStarTests.Geometry),
    ("TC-079-007", (Action)RegularStarTests.ProfileIntegration),
    ("TC-079-008", (Action)RegularStarTests.ProfileBoundary),
    ("TC-079-005", (Action)RegularStarTests.SolidMaterial),
    ("TC-078-001", (Action)RegularPolygonTests.Triangle),
    ("TC-078-002", (Action)RegularPolygonTests.Square),
    ("TC-078-003", (Action)RegularPolygonTests.EqualEdges),
    ("TC-078-004", (Action)RegularPolygonTests.RadiusDistance),
    ("TC-078-005", (Action)RegularPolygonTests.ChangeSides),
    ("TC-078-006", (Action)RegularPolygonTests.ChangeRadius),
    ("TC-078-007", (Action)RegularPolygonTests.ChangeRotation),
    ("TC-078-008", (Action)RegularPolygonTests.ChangeCenter),
    ("TC-078-009", (Action)RegularPolygonTests.Export),
    ("TC-078-010", (Action)RegularPolygonTests.Invalid),
    ("TC-078-011", (Action)RegularPolygonTests.Integration),

    ("TC-077-001", (Action)CutUnionTests.Run),
    ("TC-075-001", (Action)EllipsePropertyTests.Run),
    ("TC-076-001", (Action)EllipsePropertyTests.Identity),
    ("TC-074-001", (Action)EllipseTests.Run),
    ("TC-072-001", (Action)LocalizationTests.Languages),
    ("TC-073-001", (Action)LocalizationTests.Millimeters),
    ("TC-071-001", (Action)InstallationScreenshots),
    ("TC-070-001", (Action)ReleaseVersionPolicy),
    ("TC-069-001", (Action)RenameCompatibility),
    ("TC-068-001", (Action)InstallerBranding),
    ("TC-066-001", (Action)InstallerBranding),
    ("TC-065-001", (Action)InstallerRelease),
    ("TC-064-001", (Action)UserDocumentation),
    ("TC-063-001", (Action)CutCenterGuides),
    ("TC-062-001", (Action)DisconnectedSections),
    ("TC-061-001", (Action)RelievedWingFolding),
    ("TC-054-003", (Action)SlitClickSelection),
    ("TC-060-001", (Action)FlatBendTools),
    ("TC-059-001", (Action)SectionDimensionPlacement),
    ("TC-058-001", (Action)SectionTrueThickness),
    ("TC-056-001", (Action)SectionRectangleResize),
    ("TC-054-001", (Action)SlitOperations),
    ("TC-053-001", (Action)FlatLengthFromSectionSegments),
    ("TC-052-001", (Action)QuarterCircleOperations),
    ("TC-051-001", (Action)SemicircleProperties),
    ("TC-050-001", (Action)SemicircleBoundaryMerge),
    ("TC-049-001", (Action)SemicircleCreation),
    ("TC-048-001", (Action)CircleRadius),
    ("TC-047-001", (Action)TriangleLowerLeft),
    ("TC-047-003", (Action)TriangleAnchoredSize),
    ("TC-002-001", (Action)RecentFiles),
    ("TC-002-002", (Action)SettingsPersistence),
    ("TC-003-001", (Action)SettingsPersistence),
    ("TC-004-001", (Action)ViewportCoordinates),
    ("TC-005-001", (Action)HoleOperations),
    ("TC-005-002", (Action)SelectionHighlight),
    ("TC-005-003", (Action)DeleteObjects),
    ("TC-005-004", (Action)UndoRedo),
    ("TC-006-001", (Action)ExteriorDimensions),
    ("TC-007-001", (Action)RoundTrip),
    ("TC-007-002", (Action)SettingsPersistence),
    ("TC-008-001", (Action)SectionViewportCoordinates),
    ("TC-009-001", (Action)SectionViewportCoordinates),
    ("TC-010-001", (Action)BentSurface),
    ("TC-011-001", (Action)ExteriorDimensions),
    ("TC-012-001", (Action)SectionSelection),
    ("TC-013-001", (Action)SectionViewportCoordinates),
    ("TC-014-001", (Action)SectionViewportCoordinates),
    ("TC-015-001", (Action)BoundaryCuts),
    ("TC-016-001", (Action)ContourSections),
    ("TC-017-001", (Action)OuterContourEditing),
    ("TC-018-001", (Action)ClippedSectionEditing),
    ("TC-019-001", (Action)BentSurface),
    ("TC-020-001", (Action)FileShortcuts),
    ("TC-021-001", (Action)DirtyState),
    ("TC-022-001", (Action)SettingsPersistence),
    ("TC-023-001", (Action)DocumentUnits),
    ("TC-024-001", (Action)Defaults),
    ("TC-025-001", (Action)ViewportCoordinates),
    ("TC-026-001", (Action)FlatMaterialMask),
    ("TC-027-001", (Action)CutSurface),
    ("TC-028-001", (Action)CutSurface),
    ("TC-029-001", (Action)BentSurface),
    ("TC-030-001", (Action)TriangleEditing),
    ("TC-031-001", (Action)HoleDragCreation),
    ("TC-032-001", (Action)TriangleBoundaryCut),
    ("TC-033-001", (Action)OuterContourFillet),
    ("TC-034-001", (Action)OuterContourFillet),
    ("TC-035-001", (Action)OuterContourFillet),
    ("TC-036-001", (Action)ArbitraryAndHoleFillet),
    ("TC-038-001", (Action)QuadrilateralDragCreation),
    ("TC-039-001", (Action)ExactCutSurface),
    ("TC-040-001", (Action)ParallelogramBoundaryCut),
    ("TC-041-001", (Action)TriangleDragDirection),
    ("TC-042-001", (Action)OuterLineDeletion),
    ("TC-043-001", (Action)RectangleAfterDiagonal),
    ("TC-044-001", (Action)RoundedFilletSurface),
    ("TC-045-001", (Action)CurvedBoundaryMerge),
    ("TC-046-001", (Action)OuterGapClosure),
    ("BASE-Thickness", (Action)Thickness),
    ("BASE-Bends", (Action)Bends),
    ("BASE-SegmentsAndBentGeometry", (Action)SegmentsAndBentGeometry),
    ("BASE-EditableAxisDistances", (Action)EditableAxisDistances),
    ("BASE-MicroJoints", (Action)MicroJoints),
    ("BASE-GeneralDxfImport", (Action)GeneralDxfImport),
};
if(args.Contains("--list")){foreach(var test in regressionTests)Console.WriteLine(test.Id);return;}
var filterIndex=Array.IndexOf(args,"--tests");
if(args.Length>0&&(filterIndex!=0||args.Length!=2))throw new ArgumentException("Use --list or --tests TC-046-001,TC-045-001");
var requested=filterIndex>=0?args[filterIndex+1].Split(',',StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries):[];
if(filterIndex>=0&&requested.Length==0)throw new ArgumentException("Empty test selection");
foreach(var id in requested)if(!regressionTests.Any(test=>test.Id==id))throw new ArgumentException("Unknown test ID: "+id);
var selected=filterIndex>=0?regressionTests.Where(test=>requested.Contains(test.Id)).ToList():regressionTests.ToList();
var failures=new List<string>();
foreach(var group in selected.GroupBy(test=>test.Run))
{
    try{group.Key();foreach(var test in group)Console.WriteLine("PASS "+test.Id);}
    catch(Exception error){foreach(var test in group){failures.Add(test.Id);Console.WriteLine("FAIL "+test.Id+": "+error.Message);}}
}
Console.WriteLine($"Regression: {selected.Count-failures.Count}/{selected.Count} PASS; manual cases are NOT RUN");
if(failures.Count>0)Environment.ExitCode=1;
static void InstallationScreenshots()
{
    var root=new DirectoryInfo(AppContext.BaseDirectory);
    while(root is not null&&!File.Exists(Path.Combine(root.FullName,"VCutting.sln")))root=root.Parent;
    if(root is null)Fail("repository required");
    var help=Path.Combine(root!.FullName,"help-content","help");
    var records=System.Text.Json.JsonDocument.Parse(File.ReadAllText(Path.Combine(help,"images","installation","provenance.json"))).RootElement.GetProperty("images");
    foreach(var page in new[]{"getting-started.html","installation.html"})
    {
        var text=File.ReadAllText(Path.Combine(help,page));
        var figures=System.Text.RegularExpressions.Regex.Matches(text,@"<figure class=""installation-figure"" data-installation-step=""([0-9]+)"">(.*?)</figure>",System.Text.RegularExpressions.RegexOptions.Singleline);
        if(figures.Count!=6)Fail("six installation figures required: "+page);
        for(int i=0;i<6;i++)
        {
            var item=records[i];var file=item.GetProperty("file").GetString()!;var caption=item.GetProperty("caption").GetString()!;
            var figure=figures[i];
            if(figure.Groups[1].Value!=(i+1).ToString()||!figure.Value.Contains("<figcaption>"+caption+"</figcaption>")||!figure.Value.Contains("images/installation/"+file)||!System.Text.RegularExpressions.Regex.IsMatch(figure.Value,@"alt=""[^""]+"""))Fail("installation order/caption/alt: "+page);
            var bytes=File.ReadAllBytes(Path.Combine(help,"images","installation",file));
            if(!bytes.Take(8).SequenceEqual(new byte[]{137,80,78,71,13,10,26,10})||Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes)).ToLowerInvariant()!=item.GetProperty("sha256").GetString())Fail("original screenshot integrity: "+file);
            if(!bytes.SequenceEqual(File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory,"help","images","installation",file))))Fail("shipped screenshot integrity: "+file);
        }
    }
}
static void RenameCompatibility()
{
    var root=new DirectoryInfo(AppContext.BaseDirectory);
    while(root is not null&&!File.Exists(Path.Combine(root.FullName,"VCutting.sln")))root=root.Parent;
    if(root is null)Fail("VCutting solution missing");
    var repo=root!.FullName;
    foreach(var name in new[]{"VCutting","VCutting.Viewer","VCutting.Explorer","VCutting.Simulator","VCutting.Drawer"})
    {
        var project=Path.Combine(repo,name,name+".csproj");
        if(!File.Exists(project)||!File.ReadAllText(project).Contains("<AssemblyName>"+(name=="VCutting"?"CosmicDesigner":name)+"</AssemblyName>"))Fail("project/assembly name: "+name);
        var window=File.ReadAllText(Path.Combine(repo,name,"MainWindow.xaml"));
        if(!window.Contains("x:Class=\""+name+".MainWindow\""))Fail("XAML namespace: "+name);
    }
    if(!File.ReadAllText(Path.Combine(repo,"VCutting","UserSettings.cs")).Contains("\"CosmicDesigner\""))Fail("legacy designer settings location");
    if(!File.ReadAllText(Path.Combine(repo,"VCutting.Explorer","MainWindow.xaml.cs")).Contains("\"DXFExplorer\", \"settings.json\""))Fail("legacy explorer settings location");
    var path=Path.Combine(repo,"artifacts","cr069-legacy-roundtrip.dxf");
    try
    {
        var d=new VCuttingDocument();d.AddHole("Circle",100,100);
        VCuttingDxfSerializer.Save(d,path);
        if(!File.ReadAllText(path).Contains("COSMIC_DESIGNER_JSON:"))Fail("legacy metadata marker");
        if(!VCuttingDxfSerializer.HasMetadata(path))Fail("legacy metadata detection");
        var loaded=VCuttingDxfSerializer.Load(path);Eq(d.Material.Width,loaded.Material.Width);
        if(loaded.Cuts.Count!=1)Fail("legacy metadata cut restore");
    }
    finally{if(File.Exists(path))File.Delete(path);}
    InstallerBranding();
    var setup=File.ReadAllText(Path.Combine(repo,"installer","VCutting.iss"));
    foreach(var contract in new[]{"UsePreviousAppDir=yes","UsePreviousGroup=yes","SetupIconFile=compiler:SetupClassicIcon.ico"})if(!setup.Contains(contract))Fail("installer compatibility: "+contract);
}
static void InstallerBranding()
{
    var root = new DirectoryInfo(AppContext.BaseDirectory);
    while (root is not null && !File.Exists(Path.Combine(root.FullName,"Directory.Build.props"))) root = root.Parent;
    if(root is null) throw new InvalidOperationException("Repository output required");
    var setup=File.ReadAllText(Path.Combine(root.FullName,"installer","VCutting.iss"));
    foreach(var required in new[]{"#define AppName \"VCutting\"", "DefaultDirName={autopf}\\VCutting", "DefaultGroupName=VCutting", "바탕 화면에 VCutting 바로가기 만들기", "Name: \"{autodesktop}\\VCutting\"; Filename: \"{app}\\CosmicDesigner.exe\"", "Filename: \"{app}\\CosmicDesigner.exe\"; Description: \"VCutting 실행\"", "AppId={{72D5CE59-6E07-47D0-88C6-ADE37D3368A1}"})
        if(!setup.Contains(required)) Fail("installer branding/upgrade contract: "+required);
}
static void ReleaseVersionPolicy()
{
    var root=new DirectoryInfo(AppContext.BaseDirectory);
    while(root is not null&&!File.Exists(Path.Combine(root.FullName,"release","versioning.py")))root=root.Parent;
    if(root is null)Fail("release authority missing");
    var python=Environment.GetEnvironmentVariable("VCUTTING_RELEASE_PYTHON")??"python";
    var start=new System.Diagnostics.ProcessStartInfo(python){UseShellExecute=false,RedirectStandardOutput=true,RedirectStandardError=true};
    start.ArgumentList.Add(Path.Combine(root!.FullName,"tests","regression","versioning_tests.py"));
    using var process=System.Diagnostics.Process.Start(start)!;
    var stdout=process.StandardOutput.ReadToEnd();var stderr=process.StandardError.ReadToEnd();process.WaitForExit();
    if(process.ExitCode!=0)Fail("release fixture tests: "+stdout+stderr);
    var version=System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes<System.Reflection.AssemblyMetadataAttribute>().Single(x=>x.Key=="CustomerVersion").Value;
    if(string.IsNullOrWhiteSpace(version)||version.Contains("1.20.29"))Fail("customer/development identity separation");
    var about=File.ReadAllText(Path.Combine(root.FullName,"VCutting","MainWindow.xaml.cs"));
    var localizedAbout=File.ReadAllText(Path.Combine(root.FullName,"VCutting","Localization.cs"));
    if(!about.Contains("Localization.About()")||!new[]{"VersionInfo.Display","VersionInfo.Development","VersionInfo.Build"}.All(localizedAbout.Contains))Fail("localized About must use shared version metadata");
    Console.WriteLine("PASS isolated Freeze/rebuild/mismatch/promotion/version mapping checks");
}
static void InstallerRelease()
{
    var root=new DirectoryInfo(AppContext.BaseDirectory);
    while(root is not null&&!File.Exists(Path.Combine(root.FullName,"release","versioning.py")))root=root.Parent;
    if(root is null)Fail("release authority missing");
    var source=File.ReadAllText(Path.Combine(root!.FullName,"installer","VCutting.iss"));
    if(!source.Contains("#ifndef ReleaseVersionFile")||!source.Contains("#error Approved Freeze required"))Fail("installer Freeze gate missing");
    var build=File.ReadAllText(Path.Combine(root.FullName,"installer","Build-Installer.ps1"));
    if(!build.Contains("[string]$FreezeRecord")||!build.Contains("validate --freeze")||!build.Contains("prepare --freeze")||!build.Contains("record --freeze"))Fail("installer must consume immutable Freeze");
    var path=Environment.GetEnvironmentVariable("COSMIC_INSTALLER_PATH");
    if(!string.IsNullOrEmpty(path))
    {
        var infoPath=Environment.GetEnvironmentVariable("VCUTTING_INSTALLER_BUILD_INFO")??throw new InvalidOperationException("Selected installer needs its build.json evidence");
        var info=System.Text.Json.JsonDocument.Parse(File.ReadAllText(infoPath)).RootElement;
        if(Path.GetFileName(path)!="VCuttingSetup.exe")Fail("installer filename");
        var version=System.Diagnostics.FileVersionInfo.GetVersionInfo(path);
        var number=$"{version.FileMajorPart}.{version.FileMinorPart}.{version.FileBuildPart}.{version.FilePrivatePart}";
        if(number!=info.GetProperty("numericVersion").GetString()||version.FileVersion?.Trim()!=info.GetProperty("version").GetString()||version.ProductVersion?.Trim()!=info.GetProperty("version").GetString())Fail("installer/frozen build metadata mismatch");
    }
    else Console.WriteLine("PASS installer Freeze gate; no approved candidate selected, actual new installer NOT_RUN");
}
static void UserDocumentation()
{
    var root = new DirectoryInfo(AppContext.BaseDirectory);
    while (root is not null && !File.Exists(Path.Combine(root.FullName,"Directory.Build.props"))) root = root.Parent;
    if(root is null) throw new InvalidOperationException("Run documentation checks from a repository build output");
    var source = Path.Combine(root.FullName,"help-content");
    var deployed = AppContext.BaseDirectory;
    var sections = System.Text.Json.JsonDocument.Parse(File.ReadAllText(Path.Combine(source,"help","manual-sections.json"))).RootElement.EnumerateArray().ToList();
    if(sections.Count<20) Fail("manual coverage missing");
    var paths=sections.Select(s=>s.GetProperty("path").GetString()!).ToList();
    var utf8=new UTF8Encoding(false,true);
    foreach(var file in Directory.EnumerateFiles(source,"*",SearchOption.AllDirectories).Where(f=>new[]{".html",".css",".js",".png",".svg"}.Contains(Path.GetExtension(f).ToLowerInvariant())))
    {
        var relative=Path.GetRelativePath(source,file);var published=Path.Combine(deployed,relative);
        if(!File.Exists(published))Fail("help output missing: "+relative);
        if(Path.GetFileName(file)=="version.js"){if(!File.ReadAllText(published).Contains("window.VCuttingVersion"))Fail("generated Help version missing");}
        else if(!File.ReadAllBytes(file).SequenceEqual(File.ReadAllBytes(published)))Fail("help output stale: "+relative);
        if(Path.GetExtension(file)!=".html")continue;
        var text=utf8.GetString(File.ReadAllBytes(file));
        if(!text.Contains("charset=\"utf-8\"",StringComparison.OrdinalIgnoreCase))Fail("help UTF8 declaration: "+relative);
        foreach(System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(text,"(?:href|src)\\s*=\\s*[\"']([^\"']+)[\"']",System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {
            var link=System.Net.WebUtility.HtmlDecode(match.Groups[1].Value);
            if(link.Contains(':')||link.StartsWith("//"))Fail("external/absolute help dependency: "+link);
            var split=link.Split('#',2);var target=Path.GetFullPath(Path.Combine(Path.GetDirectoryName(published)!,Uri.UnescapeDataString(split[0])));
            if(split[0].Length==0)target=published;
            if(!target.StartsWith(Path.GetFullPath(deployed).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)||!File.Exists(target))Fail("broken help reference: "+relative+" -> "+link);
            if(split.Length>1&&split[1].Length>0)
            {
                var targetText=utf8.GetString(File.ReadAllBytes(target));var anchor=Uri.UnescapeDataString(split[1]);
                if(!System.Text.RegularExpressions.Regex.IsMatch(targetText,"(?:id|name)\\s*=\\s*[\"']"+System.Text.RegularExpressions.Regex.Escape(anchor)+"[\"']"))Fail("broken help anchor: "+link);
            }
        }
    }
    foreach(var path in paths)
    {
        var text=File.ReadAllText(Path.Combine(deployed,"help",path));
        foreach(var marker in new[]{"lang=\"ko\"","viewport","<h1>","aria-label=\"도움말 목차\"","관련 도움말","이전:","다음:","Home"})if(!text.Contains(marker))Fail("manual page structure "+path+": "+marker);
        foreach(var other in paths)
        {
            var relative=Path.GetRelativePath(Path.GetDirectoryName(Path.Combine(deployed,"help",path))!,Path.Combine(deployed,"help",other)).Replace('\\','/');
            if(!text.Contains("href=\""+relative+"\""))Fail("manual navigation missing "+path+" -> "+other);
        }
    }
    foreach(var page in new[]{"index.html","getting-started.html","reference/shortcuts.html","troubleshooting/common-errors.html"})
    {
        var custom=Path.Combine(root.FullName,"artifacts","installer-location-with-spaces","app");
        if(UserHelp.ResolvePath(page,custom)!=Path.GetFullPath(Path.Combine(custom,"help",page)))Fail("help executable-relative resolution");
        if(!File.Exists(UserHelp.ResolvePath(page,deployed)))Fail("menu destination not shipped");
    }
    try{UserHelp.ResolvePath("../outside.html",deployed);Fail("unsafe help route accepted");}catch(ArgumentException){}
    var xaml=File.ReadAllText(Path.Combine(root.FullName,"VCutting","MainWindow.xaml"));
    foreach(var page in new[]{"index.html","getting-started.html","reference/shortcuts.html","troubleshooting/common-errors.html"})if(!xaml.Contains("Tag=\""+page+"\" Click=\"Help_Click\""))Fail("help menu route missing");
    var build=File.ReadAllText(Path.Combine(root.FullName,"installer","Build-Installer.ps1"));
    if(!build.Contains("VCutting\\VCutting.csproj")||!build.Contains("CosmicDesigner.exe"))Fail("installer cosmic omitted");
    if(!File.ReadAllText(Path.Combine(root.FullName,"installer","VCutting.iss")).Contains("ignoreversion recursesubdirs createallsubdirs"))Fail("installer recursive help missing");
    if(Directory.EnumerateFiles(Path.Combine(deployed,"help"),"*.md",SearchOption.AllDirectories).Any()||Directory.EnumerateFiles(Path.Combine(deployed,"help"),"*.json",SearchOption.AllDirectories).Any())Fail("internal help metadata shipped");
}
static void CutCenterGuides()
{
    var d=new VCuttingDocument();var cut=d.AddHole("Circle",150,150);d.UpdateCut(cut,150,150,30,30,7);
    var guides=CutAlignmentEngine.Find(d,cut,1);if(guides.Count!=2)Fail("two axis midpoint");foreach(var guide in guides){Eq(150,guide.Position);Eq(0,guide.First);Eq(300,guide.Second);}
    d.AddBend(SectionAxis.W,100,BendDirection.Up);cut.CenterX=50;guides=CutAlignmentEngine.Find(d,cut,1);Eq(50,guides.Single(g=>g.Vertical).Position);
    d.AddBend(SectionAxis.W,200,BendDirection.Down);cut.CenterX=150;guides=CutAlignmentEngine.Find(d,cut,1);var between=guides.Single(g=>g.Vertical);Eq(100,between.First);Eq(200,between.Second);
    foreach(var scale in new[]{.1,1d,20d}){cut.CenterX=150+1/scale;if(!CutAlignmentEngine.Find(d,cut,scale).Any(g=>g.Vertical))Fail("screen tolerance inside");cut.CenterX=150+2/scale;if(CutAlignmentEngine.Find(d,cut,scale).Any(g=>g.Vertical))Fail("screen tolerance outside");}
    if(CutAlignmentEngine.Find(d,cut,double.NaN).Count!=0||CutAlignmentEngine.Find(d,cut,0).Count!=0)Fail("invalid guide scale");
    var semi=new CutOperation{Shape="Semicircle",CenterX=50,CenterY=60};semi.Geometry.Add(new ArcSegment(50,50,20,0,180));Eq(50,CutAlignmentEngine.Center(semi).Y);
    Exception? error=null;var thread=new System.Threading.Thread(()=>{try{
        var doc=new VCuttingDocument();var c=doc.AddHole("Circle",150,150);doc.UpdateCut(c,150,150,30,30,7);
        var view=new FlatDesignerView{Document=doc,SelectedObject=c,ShowGrid=false,ShowRuler=false};view.SetSectionPositions(280,280);view.Measure(new(500,500));view.Arrange(new(0,0,500,500));
        var flags=System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance;var mode=typeof(FlatDesignerView).GetField("_dragMode",flags)!;var moving=typeof(FlatDesignerView).GetField("_movingCut",flags)!;
        mode.SetValue(view,Enum.Parse(mode.FieldType,"Move"));moving.SetValue(view,c);view.Refresh();view.UpdateLayout();
        var bitmap=new System.Windows.Media.Imaging.RenderTargetBitmap(500,500,96,96,System.Windows.Media.PixelFormats.Pbgra32);bitmap.Render(view);
        int Reds(){var bytes=new byte[500*500*4];bitmap.CopyPixels(bytes,2000,0);var count=0;for(var y=50;y<180;y++)for(var x=248;x<253;x++){var i=(y*500+x)*4;if(bytes[i+2]>bytes[i+1]+30&&bytes[i+2]>bytes[i]+30)count++;}return count;}
        if(Reds()<20)Fail("move guide not rendered: "+Reds());
        var directory=Environment.GetEnvironmentVariable("COSMIC_ALIGNMENT_PREVIEW_DIR");if(!string.IsNullOrEmpty(directory)){Directory.CreateDirectory(directory);var encoder=new System.Windows.Media.Imaging.PngBitmapEncoder();encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmap));using var file=File.Create(Path.Combine(directory,"cut-center-guides.png"));encoder.Save(file);}
        typeof(FlatDesignerView).GetMethod("EndDrag",flags)!.Invoke(view,null);view.UpdateLayout();bitmap.Clear();bitmap.Render(view);if(Reds()!=0)Fail("guide remains after drag");Eq(150,c.CenterX);Eq(150,c.CenterY);
        mode.SetValue(view,Enum.Parse(mode.FieldType,"NW"));moving.SetValue(view,c);view.Refresh();view.UpdateLayout();bitmap.Clear();bitmap.Render(view);if(Reds()!=0)Fail("resize guide");
    }catch(Exception ex){error=ex;}});thread.SetApartmentState(System.Threading.ApartmentState.STA);thread.Start();thread.Join();if(error is not null)throw error;
}
static void DisconnectedSections()
{
    var d=new VCuttingDocument();
    var outline=new System.Windows.Point[]{new(0,0),new(300,0),new(300,190),new(170,190),new(170,210),new(300,210),new(300,300),new(0,300),new(0,210),new(130,210),new(130,190),new(0,190)};
    d.OuterContour.Segments.Clear();d.OuterContour.Segments.AddRange(outline.Select((p,i)=>{var q=outline[(i+1)%outline.Length];return (GeometrySegment)new LineSegment(p.X,p.Y,q.X,q.Y);}));
    var parts=SectionGeometryEngine.BuildAll(d,SectionAxis.H,false,233.333);
    if(parts.Count!=2)Fail("H disconnected material");Eq(0,parts[0].StartPosition);Eq(190,parts[0].EndPosition);Eq(210,parts[1].StartPosition);Eq(300,parts[1].EndPosition);
    if(SectionGeometryEngine.BuildAll(d,SectionAxis.H,false,150).Count!=1)Fail("bridge continuous section");
    var w=SectionGeometryEngine.BuildAll(d,SectionAxis.W,false,200);if(w.Count!=1)Fail("W bridge interval");Eq(130,w[0].StartPosition);Eq(170,w[0].EndPosition);
    if(ContourSectionEngine.MaterialIntervals(d.OuterContour.Segments,SectionAxis.H,400).Count!=0)Fail("empty scan should not fallback");
    var arc=new GeometrySegment[]{new LineSegment(0,0,100,0),new LineSegment(100,0,100,100),new LineSegment(100,100,0,100),new LineSegment(0,100,0,60),new ArcSegment(0,50,10,90,-90),new LineSegment(0,40,0,0)};
    var curved=ContourSectionEngine.MaterialIntervals(arc,SectionAxis.H,5);if(curved.Count!=2)Fail("ARC split intervals");Eq(50-Math.Sqrt(75),curved[0].End);Eq(50+Math.Sqrt(75),curved[1].Start);
    Exception? error=null;var thread=new System.Threading.Thread(()=>{try{
        foreach(var axis in new[]{SectionAxis.H,SectionAxis.W})
        {
            var doc=DocumentSnapshot.Capture(d).Restore();if(axis==SectionAxis.W){doc.OuterContour.Segments.Clear();doc.OuterContour.Segments.AddRange(outline.Select((p,i)=>{var q=outline[(i+1)%outline.Length];return (GeometrySegment)new LineSegment(p.Y,p.X,q.Y,q.X);}));}
            var view=new SectionDesignerView{Document=doc,Axis=axis,SectionPosition=233.333};view.Measure(new(500,500));view.Arrange(new(0,0,500,500));view.Refresh();view.UpdateLayout();
            if(view.Children.OfType<System.Windows.Controls.TextBox>().Count()!=2)Fail("all section dimension editors");
            System.Windows.Point At(double station)=>axis==SectionAxis.H?new(250,445-station/300*390):new(55+station/300*390,250);
            if(view.TryGetFlatSectionStation(At(200),out _))Fail("gap bend hit");
            if(!view.TryGetFlatSectionStation(At(250),out var station))Fail("second interval hit");Eq(250,station);
            var bitmap=new System.Windows.Media.Imaging.RenderTargetBitmap(500,500,96,96,System.Windows.Media.PixelFormats.Pbgra32);bitmap.Render(view);var pixels=new byte[500*500*4];bitmap.CopyPixels(pixels,500*4,0);
            var gap=At(200);var offset=((int)Math.Round(gap.Y)*500+(int)Math.Round(gap.X))*4;if(pixels[offset]!=255||pixels[offset+1]!=255||pixels[offset+2]!=255)Fail("rendered gap filled");
            var body=At(250);offset=((int)Math.Round(body.Y)*500+(int)Math.Round(body.X))*4;if(pixels[offset]==255&&pixels[offset+1]==255&&pixels[offset+2]==255)Fail("second interval not rendered");
            var previewDirectory=Environment.GetEnvironmentVariable("COSMIC_SECTION_PREVIEW_DIR");
            if(axis==SectionAxis.H&&!string.IsNullOrEmpty(previewDirectory)){Directory.CreateDirectory(previewDirectory);var encoder=new System.Windows.Media.Imaging.PngBitmapEncoder();encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmap));using var file=File.Create(Path.Combine(previewDirectory,"H-section-gap-preview.png"));encoder.Save(file);VCuttingDxfSerializer.Save(doc,Path.Combine(previewDirectory,"section-gap-example.dxf"));}
            view.BentMode=true;view.Refresh();bitmap.Render(view);if(view.Children.OfType<System.Windows.Controls.TextBox>().Count()!=2)Fail("bent disjoint dimensions");
        }
    }catch(Exception ex){error=ex;}});thread.SetApartmentState(System.Threading.ApartmentState.STA);thread.Start();thread.Join();if(error is not null)throw error;
    var bentDoc=DocumentSnapshot.Capture(d).Restore();bentDoc.AddBend(SectionAxis.H,100,BendDirection.Left);bentDoc.AddBend(SectionAxis.H,200,BendDirection.Left);bentDoc.AddBend(SectionAxis.H,250,BendDirection.Right);
    // Restore the custom outline after recalculation to keep this fixture explicit.
    bentDoc.OuterContour.Segments.Clear();bentDoc.OuterContour.Segments.AddRange(d.OuterContour.Segments);
    var bent=SectionGeometryEngine.BuildAll(bentDoc,SectionAxis.H,true,233.333);if(bent.Count!=2||bent[0].Bends.Count!=1||bent[1].Bends.Count!=1||bent.Any(g=>g.Bends.Any(b=>b.Position==200)))Fail("bends across void");
    var editable=DocumentSnapshot.Capture(d).Restore();var before=SectionGeometryEngine.BuildAll(editable,SectionAxis.H,false,233.333);var history=new UndoRedoManager();history.Record(editable);
    if(!editable.UpdateClippedSectionSegment(SectionAxis.H,before[0],0,180,233.333))Fail("disjoint local dimension edit");
    var after=SectionGeometryEngine.BuildAll(editable,SectionAxis.H,false,233.333);if(after.Count!=2)Fail("edit joined disjoint sections");Eq(180,after[0].EndPosition);Eq(210,after[1].StartPosition);Eq(90,after[1].SegmentLengths.Sum());
    var undo=history.Undo(editable)??throw new Exception();Eq(190,SectionGeometryEngine.BuildAll(undo,SectionAxis.H,false,233.333)[0].EndPosition);
}
static void RelievedWingFolding()
{
    var d=new VCuttingDocument();
    d.AddBend(SectionAxis.W,130,BendDirection.Down);d.AddBend(SectionAxis.W,170,BendDirection.Down);
    d.AddBend(SectionAxis.H,200,BendDirection.Left);
    var outline=new System.Windows.Point[]{new(0,0),new(300,0),new(300,195),new(170,195),new(170,200),new(170,205),new(300,205),new(300,300),new(0,300),new(0,205),new(130,205),new(130,200),new(130,195),new(0,195)};
    d.OuterContour.Segments.Clear();d.OuterContour.Segments.AddRange(outline.Select((p,i)=>{var q=outline[(i+1)%outline.Length];return (GeometrySegment)new LineSegment(p.X,p.Y,q.X,q.Y);}));
    var flat=DocumentSnapshot.Capture(d).Restore();flat.Bends.Clear();var original=BentSurfaceEngine.Build(flat);var folded=BentSurfaceEngine.Build(d);
    if(original.Points.Count!=folded.Points.Count||!original.Triangles.SequenceEqual(folded.Triangles))Fail("fold topology");
    double Distance(SurfacePoint3 a,SurfacePoint3 b)=>Math.Sqrt(Math.Pow(a.X-b.X,2)+Math.Pow(a.Y-b.Y,2)+Math.Pow(a.Z-b.Z,2));
    for(var i=0;i<original.Triangles.Count;i+=3)for(var j=0;j<3;j++)
    {
        var a=original.Triangles[i+j];var b=original.Triangles[i+(j+1)%3];
        Eq(Distance(original.Points[a],original.Points[b]),Distance(folded.Points[a],folded.Points[b]));
    }
    SurfacePoint3 At(double x,double y){var index=original.Points.ToList().FindIndex(p=>Math.Abs(p.X-x)<1e-6&&Math.Abs(p.Y-y)<1e-6);if(index<0)Fail("wing fixture vertex");return folded.Points[index];}
    var lower=At(0,0);var upper=At(0,300);
    Eq(0,lower.Z);Eq(0,upper.Z);Eq(230,upper.X);Eq(330,upper.Y);Eq(-40,At(300,300).Z);
    var bridgeA=At(130,200);var bridgeB=At(170,200);Eq(40,Distance(bridgeA,bridgeB));
    if(folded.BendEdges.Any(e=>e.Layer=="V"&&Math.Abs(Distance(folded.Points[e.A],folded.Points[e.B])-40)>1e-6))Fail("red hinge clipped to bridge");
    var rotated=new VCuttingDocument();rotated.AddBend(SectionAxis.H,130,BendDirection.Right);rotated.AddBend(SectionAxis.H,170,BendDirection.Right);rotated.AddBend(SectionAxis.W,200,BendDirection.Up);
    rotated.OuterContour.Segments.Clear();rotated.OuterContour.Segments.AddRange(outline.Select((p,i)=>{var q=outline[(i+1)%outline.Length];return (GeometrySegment)new LineSegment(p.Y,p.X,q.Y,q.X);}));
    var rotatedFlat=DocumentSnapshot.Capture(rotated).Restore();rotatedFlat.Bends.Clear();var rf=BentSurfaceEngine.Build(rotatedFlat);var rb=BentSurfaceEngine.Build(rotated);
    for(var i=0;i<rf.Triangles.Count;i+=3)for(var j=0;j<3;j++){var a=rf.Triangles[i+j];var b=rf.Triangles[i+(j+1)%3];Eq(Distance(rf.Points[a],rf.Points[b]),Distance(rb.Points[a],rb.Points[b]));}
    foreach(var axis in new[]{SectionAxis.W,SectionAxis.H})foreach(var direction in axis==SectionAxis.W?new[]{BendDirection.Up,BendDirection.Down}:new[]{BendDirection.Left,BendDirection.Right})
    {
        var plain=new VCuttingDocument();plain.AddBend(axis,100,direction);var mesh=BentSurfaceEngine.Build(plain);
        Eq(200,mesh.Points.Max(p=>Math.Abs(p.Z)));if(mesh.Points.Any(p=>direction is BendDirection.Up or BendDirection.Left?p.Z< -1e-6:p.Z>1e-6))Fail("single bend direction");
    }
}
static void FlatBendTools()
{
    foreach(var layer in new[]{"V","V1"})
    foreach(var horizontal in new[]{true,false})
    {
        var start=new System.Windows.Point(100,150);var end=horizontal?new System.Windows.Point(180,151):new System.Windows.Point(101,80);
        if(!FlatBendDrawingEngine.TryCalculate(layer,start,end,300,300,out var p))Fail("bend drag");
        if(p.Axis!=(horizontal?SectionAxis.H:SectionAxis.W))Fail("bend axis");
        var expected=horizontal?(layer=="V"?BendDirection.Left:BendDirection.Right):(layer=="V"?BendDirection.Up:BendDirection.Down);
        if(p.Direction!=expected)Fail("bend direction");Eq(horizontal?150:100,p.Position);
        var d=new VCuttingDocument();var history=new UndoRedoManager();history.Record(d);
        var bend=d.AddBend(p.Axis,p.Position,p.Direction);
        if(bend.Layer!=layer||SectionGeometryEngine.Build(d,p.Axis,false).Bends.Single().Direction!=expected)Fail("wedge layer");
        var moved=FlatBendDrawingEngine.MovePosition(p.Axis,p.Position,start,new(130,190),300,300);
        Eq(horizontal?190:130,moved);history.Record(d);d.UpdateBend(bend,moved,bend.Direction,bend.Sequence);
        var restored=history.Undo(d)??throw new Exception("bend move undo");Eq(p.Position,restored.Bends.Single().Position);
        restored=history.Redo(restored)??throw new Exception("bend move redo");Eq(moved,restored.Bends.Single().Position);
        Eq(299.99,FlatBendDrawingEngine.MovePosition(p.Axis,p.Position,start,new(1000,1000),300,300));
        Eq(.01,FlatBendDrawingEngine.MovePosition(p.Axis,p.Position,start,new(-1000,-1000),300,300));
        var path=Path.Combine(Path.GetTempPath(),$"flat-bend-{Guid.NewGuid():N}.dxf");
        try{VCuttingDxfSerializer.Save(restored,path);var loaded=VCuttingDxfSerializer.Load(path);Eq(moved,loaded.Bends.Single().Position);if(loaded.Bends.Single().Direction!=expected||loaded.Bends.Single().Layer!=layer)Fail("bend DXF");}
        finally{if(File.Exists(path))File.Delete(path);}
    }
    foreach(var end in new System.Windows.Point[]{new(50,150),new(100,200),new(100,150)})
        if(FlatBendDrawingEngine.TryCalculate("V",new(100,150),end,300,300,out _))Fail("unsupported drag");
}
static void SectionDimensionPlacement()
{
    foreach(var vertical in new[]{false,true})
    {
        System.Windows.Point P(double x,double y)=>vertical?new(y,x):new(x,y);
        var edges=new (System.Windows.Point A,System.Windows.Point B)[]{(P(0,0),P(10,0)),(P(0,0),P(20,0)),(P(40,0),P(50,0))};
        var result=SectionDimensionEngine.Arrange(edges,new System.Windows.Rect(0,0,60,60));
        for(var i=0;i<result.Count;i++){Eq((result[i].A.X+result[i].B.X)/2,result[i].Label.X);Eq((result[i].A.Y+result[i].B.Y)/2,result[i].Label.Y);}
        Eq(30,System.Windows.Vector.Multiply(result[1].A-result[0].A,result[0].Normal));
        Eq(0,System.Windows.Vector.Multiply(result[2].A-result[0].A,result[0].Normal));
        var reversed=SectionDimensionEngine.Arrange(edges.Reverse().ToList(),new System.Windows.Rect(0,0,60,60));
        Eq(result[1].A.X,reversed[1].A.X);Eq(result[1].A.Y,reversed[1].A.Y);
    }
}
static void SectionTrueThickness()
{
    foreach(var axis in new[]{SectionAxis.H,SectionAxis.W})
    foreach(var thickness in new[]{.2,2d})
    foreach(var scale in new[]{.5,1.5,3d})
    {
        var d=new VCuttingDocument();d.ConfigureNew(122,122,thickness,MeasurementUnit.Centimeter);
        var direction=axis==SectionAxis.H?BendDirection.Left:BendDirection.Up;
        foreach(var station in new[]{19d,37d,85d,103d})d.AddBend(axis,station,direction);
        var g=SectionGeometryEngine.Build(d,axis,true);
        var center=g.Points.Select(p=>new System.Windows.Point(p.X*scale,-p.Y*scale)).ToList();
        var edges=SectionProfileEngine.Edges(center,SectionProfileEngine.HalfThickness(thickness,scale));
        Eq(thickness*scale,(edges.Plus[0]-edges.Minus[0]).Length);
        for(var i=0;i<center.Count-1;i++)
        {
            var delta=center[i+1]-center[i];delta.Normalize();var normal=new System.Windows.Vector(-delta.Y,delta.X);
            Eq(thickness*scale,Math.Abs(System.Windows.Vector.Multiply(edges.Plus[i]-edges.Minus[i],normal)));
            Eq(thickness*scale,Math.Abs(System.Windows.Vector.Multiply(edges.Plus[i+1]-edges.Minus[i+1],normal)));
        }
        // 3D uses the same folded center path; compare H/W mapped vertices at every station.
        var mesh=BentSurfaceEngine.Build(d);
        foreach(var p in g.Points)
            if(!mesh.Points.Any(v=>Math.Abs((axis==SectionAxis.H?v.Y:v.X)-p.X)<1e-6&&Math.Abs(v.Z-p.Y)<1e-6))Fail("Section/3D center mismatch");
        // Rotation must preserve the true thickness and the narrow opening.
        var rotated=center.Select(p=>new System.Windows.Point(-p.Y,p.X)).ToList();
        var re=SectionProfileEngine.Edges(rotated,SectionProfileEngine.HalfThickness(thickness,scale));
        Eq(thickness*scale,(re.Plus[0]-re.Minus[0]).Length);
        Eq(10*scale,(center[^1]-center[0]).Length);
    }
}
static void SectionRectangleResize()
{
    var path=Path.Combine(Path.GetTempPath(),$"section-resize-{Guid.NewGuid():N}.dxf");
    try
    {
        VCuttingDxfSerializer.Save(new VCuttingDocument(),path);
        var d=VCuttingDxfSerializer.Load(path);
        var history=new UndoRedoManager();history.Record(d);
        d.UpdateSegment(d.HSegments.Single(),100);
        Check(d,300,100);
        var restored=history.Undo(d)??throw new Exception("resize undo");Check(restored,300,300);
        restored=history.Redo(restored)??throw new Exception("resize redo");Check(restored,300,100);
        restored.UpdateSegment(restored.WSegments.Single(),150);Check(restored,150,100);
        VCuttingDxfSerializer.Save(restored,path);restored=VCuttingDxfSerializer.Load(path);
        restored.UpdateSegment(restored.HSegments.Single(),200);Check(restored,150,200);
        var custom=new VCuttingDocument();
        custom.UpdateOuterContourLine(custom.OuterContour.Segments.ToList(),2,new LineSegment(300,250,0,250));
        var contour=custom.OuterContour.Segments.ToList();
        custom.UpdateSegment(custom.HSegments.Single(),400);
        if(!custom.OuterContour.Segments.SequenceEqual(contour))Fail("custom contour resized");
    }
    finally{if(File.Exists(path))File.Delete(path);}
    static void Check(VCuttingDocument d,double w,double h)
    {
        Eq(w,d.Material.Width);Eq(h,d.Material.Height);
        var lines=d.OuterContour.Segments.OfType<LineSegment>().ToList();
        Eq(w,lines.SelectMany(l=>new[]{l.X1,l.X2}).Max());Eq(h,lines.SelectMany(l=>new[]{l.Y1,l.Y2}).Max());
        var transform=FlatViewportEngine.Calculate(800,800,w,h,false,1,new());
        var mask=FlatMaterialMaskEngine.Build(d,transform);
        if(!mask.FillContains(transform.ToScreen(new(w/2,h/2)))||mask.FillContains(transform.ToScreen(new(w/2,h+10))))Fail("resized Flat mask");
    }
}
static void SlitClickSelection()
{
    SlitOperation Slit(string shape,params GeometrySegment[] geometry){var s=new SlitOperation{Shape=shape};s.Geometry.AddRange(geometry);return s;}
    var line=Slit("Line",new LineSegment(10,10,90,10));
    var arc=Slit("Arc",new ArcSegment(100,100,70,350,550));
    var reverse=Slit("Arc",new ArcSegment(100,100,70,550,350));
    var poly=Slit("Polyline",new LineSegment(20,20,20,80),new LineSegment(20,80,80,80));
    foreach(var scale in new[]{.2,1d,20d})
    {
        var t=new FlatViewportTransform(scale,new(500,700));
        if(SlitHitTesting.Find([line],t,t.ToScreen(new(50,10))+new System.Windows.Vector(0,6),7)!=line)Fail("line pixel tolerance");
        if(SlitHitTesting.Find([line],t,t.ToScreen(new(50,10))+new System.Windows.Vector(0,8),7)!=null)Fail("line outside tolerance");
        foreach(var s in new[]{arc,reverse})
        {
            var angle=37.5*Math.PI/180;var p=new System.Windows.Point(100+70*Math.Cos(angle),100+70*Math.Sin(angle));
            if(SlitHitTesting.Find([s],t,t.ToScreen(p),.001)!=s)Fail("exact arc hit zoom/pan/sweep");
        }
        if(SlitHitTesting.Find([poly],t,t.ToScreen(new(50,80)),1)!=poly)Fail("poly second segment");
    }
    Eq(70,SlitHitTesting.Distance(arc.Geometry[0],new(100,100)));
    var shortArc=new ArcSegment(100,100,70,350,370);
    if(SlitHitTesting.Distance(shortArc,new(30,100))<100)Fail("arc outside angular span");
    var duplicate=Slit("Line",line.Geometry.ToArray());
    var identity=new FlatViewportTransform(1,new(0,0));
    if(SlitHitTesting.Find([line,duplicate],identity,identity.ToScreen(new(50,10)),7)!=duplicate)Fail("overlap topmost");
    Exception? error=null;
    var thread=new System.Threading.Thread(()=>{try{
        var document=new VCuttingDocument();document.AddSlit("Line",line.Geometry);document.AddSlit("Arc",arc.Geometry);document.AddSlit("Polyline",poly.Geometry);
        document.AddBend(SectionAxis.H,10,BendDirection.Left);
        var view=new FlatDesignerView{Document=document,ShowRuler=false};view.Measure(new(800,800));view.Arrange(new(0,0,800,800));
        var t=FlatViewportEngine.Calculate(800,800,300,300,false,1,new());DesignObject? notification=null;view.ObjectSelected+=(_,o)=>notification=o;
        foreach(var pair in new[]{(0,new System.Windows.Point(50,10)),(1,new System.Windows.Point(170,100)),(2,new System.Windows.Point(50,80))})
        {if(!view.TrySelectSlitAt(t.ToScreen(pair.Item2))||!ReferenceEquals(view.SelectedObject,document.Slits[pair.Item1])||!ReferenceEquals(notification,view.SelectedObject))Fail("view click selection/event");}
        var selected=view.SelectedObject;
        if(view.TrySelectSlitAt(t.ToScreen(new(250,250)))||view.SelectedObject!=selected)Fail("miss selection");
        view.BeginBendDrawing("V");if(view.TrySelectSlitAt(t.ToScreen(new(50,10))))Fail("bend drawing consumed by slit");view.CancelBendDrawing();
        view.ActiveHoleShape="Circle";if(view.TrySelectSlitAt(t.ToScreen(new(50,10))))Fail("hole consumed by slit");view.ActiveHoleShape=null;
        view.FilletMode=true;if(view.TrySelectSlitAt(t.ToScreen(new(50,10))))Fail("fillet consumed by slit");view.FilletMode=false;
        view.BeginSlitDrawing("Line");if(view.TrySelectSlitAt(t.ToScreen(new(50,10))))Fail("slit drawing consumed by selection");view.CancelSlitDrawing();
        view.ShowRuler=true;if(view.TrySelectSlitAt(new(20,100)))Fail("ruler selection");
    }catch(Exception ex){error=ex;}});
    thread.SetApartmentState(System.Threading.ApartmentState.STA);thread.Start();thread.Join();if(error is not null)throw error;
}
static void SlitOperations()
{
    var d=new VCuttingDocument();var outer=d.OuterContour.Segments.ToList();var baseMesh=BentSurfaceEngine.Build(d);
    var transform=FlatViewportEngine.Calculate(800,800,300,300,false,1,new());
    var history=new UndoRedoManager();history.Record(d);
    if(!SlitDrawingEngine.TryCreate("Line",[new(10,10),new(60,50)],out var line))Fail("slit line");
    var slit=d.AddSlit("Line",line)??throw new Exception("slit add");
    var undone=history.Undo(d)??throw new Exception("slit undo");Eq(0,undone.Slits.Count);
    d=history.Redo(undone)??throw new Exception("slit redo");if(!d.Slits[0].Geometry.SequenceEqual(line))Fail("slit redo geometry");
    if(!SlitDrawingEngine.TryCreate("Polyline",[new(20,20),new(20,60),new(70,60)],out var poly))Fail("slit polyline");
    Eq(2,poly.Count);d.AddSlit("Polyline",poly);
    foreach(var points in new System.Windows.Point[][]{
        [new(150,100),new(100,150),new(50,100)],
        [new(50,100),new(100,150),new(150,100)],
        [new(150,100),new(50,100),new(100,50)]})
    {
        if(!SlitDrawingEngine.TryCreate("Arc",points,out var arcGeometry))Fail("slit arc");
        var a=(ArcSegment)arcGeometry[0];Eq(100,a.Cx);Eq(100,a.Cy);Eq(50,a.Radius);
        var samples=SlitDrawingEngine.Sample(a).ToList();Eq(points[0].X,samples[0].X);Eq(points[0].Y,samples[0].Y);Eq(points[2].X,samples[^1].X);Eq(points[2].Y,samples[^1].Y);
        if(!samples.Any(p=>(p-points[1]).Length<1e-6))Fail("arc through middle");
        if(d.AddSlit("Arc",arcGeometry) is null)Fail("arc commit");
    }
    foreach(var points in new System.Windows.Point[][]{[new(0,0),new(1,1),new(2,2)],[new(1,1),new(1,1),new(2,2)],[new(double.NaN,0),new(1,1),new(2,2)]})
        if(SlitDrawingEngine.TryCreate("Arc",points,out _))Fail("degenerate arc accepted");
    if(SlitDrawingEngine.TryCreate("Line",[new(1,1),new(1,1)],out _))Fail("zero slit accepted");
    if(d.AddSlit("Line",[new LineSegment(-1,0,10,10)]) is not null)Fail("outside slit");
    if(SlitDrawingEngine.Fits([new ArcSegment(10,10,20,0,180)],300,300))Fail("arc extrema outside");
    if(!d.OuterContour.Segments.SequenceEqual(outer)||d.Cuts.Count!=0||d.InnerContours.Count!=0)Fail("slit changed contours");
    foreach(var p in new System.Windows.Point[]{new(25,25),new(100,100),new(100,150)})
        if(!FlatMaterialMaskEngine.Build(d,transform).FillContains(transform.ToScreen(p)))Fail("slit removed material");
    var mesh=BentSurfaceEngine.Build(d);if(!mesh.Triangles.SequenceEqual(baseMesh.Triangles)||mesh.Edges.Count<=baseMesh.Edges.Count)Fail("slit 3D face/edge");
    var expected=d.Slits.SelectMany(s=>s.Geometry).ToList();
    var path=Path.Combine(Path.GetTempPath(),$"slit-{Guid.NewGuid():N}.dxf");
    try{
        VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path);
        if(!loaded.Slits.SelectMany(s=>s.Geometry).SequenceEqual(expected)||loaded.Slits.Count!=d.Slits.Count)Fail("slit DXF metadata");
        var parsed=VCutting.Viewer.DxfParser.Parse(path);
        if(parsed.TotalEntities!=4+expected.Count)Fail("slit DXF entities");
        var originalArc=loaded.Slits.First(s=>s.Shape=="Arc").Geometry[0] as ArcSegment;
        loaded.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);
        Eq(100,((LineSegment)loaded.Slits[0].Geometry[0]).X1);Eq(originalArc!.Radius*10,((ArcSegment)loaded.Slits.First(s=>s.Shape=="Arc").Geometry[0]).Radius);
        loaded.ChangeUnit(MeasurementUnit.Centimeter,UnitChangeMode.PreservePhysicalSize);
        if(!loaded.Slits.SelectMany(s=>s.Geometry).SequenceEqual(expected))Fail("slit unit roundtrip");
        history.Record(loaded);var deleted=loaded.Slits[0];if(!loaded.DeleteObject(deleted))Fail("slit delete");
        loaded=history.Undo(loaded)??throw new Exception("slit delete undo");Eq(d.Slits.Count,loaded.Slits.Count);
        loaded=history.Redo(loaded)??throw new Exception("slit delete redo");Eq(d.Slits.Count-1,loaded.Slits.Count);
        var added=loaded.AddSlit("Line",line)!;if(loaded.Slits.Select(s=>s.Id).Distinct().Count()!=loaded.Slits.Count)Fail("slit reused ID");
        VCuttingDxfSerializer.Save(new VCuttingDocument(),path);
        var text=File.ReadAllText(path);var marker="COSMIC_DESIGNER_JSON:";
        var metadata=text.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries).Single(x=>x.StartsWith(marker));
        var json=System.Text.Json.Nodes.JsonNode.Parse(metadata[marker.Length..])!.AsObject();json.Remove("Slits");
        File.WriteAllText(path,text.Replace(metadata,marker+json.ToJsonString()));
        Eq(0,VCuttingDxfSerializer.Load(path).Slits.Count);
    }finally{if(File.Exists(path))File.Delete(path);}
    d.AddBend(SectionAxis.W,100,BendDirection.Up);if(BentSurfaceEngine.Build(d).Edges.Count==0)Fail("folded slit");
}
static void QuarterCircleOperations()
{
    CutOperation Create(VCuttingDocument d,double x,double y,int sx,int sy)
    {
        if(!HoleDragEngine.TryCalculate("QuarterCircle",new(x,y),new(x+sx*20,y+sy*30),300,300,out var g))throw new Exception("quarter drag");
        var preview=HoleDragEngine.CreatePreview("QuarterCircle",g);var cut=d.AddHole("QuarterCircle",g.CenterX,g.CenterY);
        cut.Geometry.Clear();cut.Geometry.Add(preview.Geometry[0]);d.UpdateCut(cut,g.CenterX,g.CenterY,g.Width,g.Height,7);
        if(!cut.Geometry.SequenceEqual(preview.Geometry))Fail("quarter preview mismatch");return cut;
    }
    System.Windows.Point At(GeometrySegment s,bool end)=>s switch{LineSegment l=>end?new(l.X2,l.Y2):new(l.X1,l.Y1),ArcSegment a=>new(a.Cx+a.Radius*Math.Cos((end?a.EndDegrees:a.StartDegrees)*Math.PI/180),a.Cy+a.Radius*Math.Sin((end?a.EndDegrees:a.StartDegrees)*Math.PI/180)),_=>throw new Exception()};
    void Closed(IReadOnlyList<GeometrySegment> segments){for(var i=0;i<segments.Count;i++)if((At(segments[i],true)-At(segments[(i+1)%segments.Count],false)).Length>1e-6)Fail("quarter closure");}
    void Persist(VCuttingDocument d){var path=Path.Combine(Path.GetTempPath(),$"quarter-{Guid.NewGuid():N}.dxf");try{VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path);if(!loaded.OuterContour.Segments.SequenceEqual(d.OuterContour.Segments)||!loaded.Cuts.SelectMany(c=>c.Geometry).SequenceEqual(d.Cuts.SelectMany(c=>c.Geometry)))Fail("quarter DXF");}finally{if(File.Exists(path))File.Delete(path);}}
    foreach(var direction in new[]{(1,1,0),(-1,1,90),(-1,-1,180),(1,-1,270)})
    {
        var (sx,sy,angle)=direction;var d=new VCuttingDocument();var c=Create(d,100,100,sx,sy);var arc=c.QuarterCircleArc!;
        Eq(100,arc.Cx);Eq(100,arc.Cy);Eq(20,arc.Radius);Eq(angle,arc.StartDegrees);Eq(90,arc.EndDegrees-arc.StartDegrees);Closed(c.Geometry);
        if(d.CanMergeBoundaryCut(c))Fail("interior quarter merge");
        var transform=FlatViewportEngine.Calculate(800,800,300,300,false,1,new());var inside=new System.Windows.Point(100+sx*5,100+sy*5);
        if(FlatMaterialMaskEngine.Build(d,transform).FillContains(transform.ToScreen(inside)))Fail("quarter mask");
        if(BentSurfaceEngine.Build(d).Points.Count==0)Fail("quarter mesh");Persist(d);
        var history=new UndoRedoManager();history.Record(d);if(!d.TryUpdateQuarterCircle(c,100,100,30))Fail("quarter radius");Eq(100,c.QuarterCircleArc!.Cx);Eq(100,c.QuarterCircleArc.Cy);Eq(30,c.QuarterCircleArc.Radius);Eq(angle,c.QuarterCircleArc.StartDegrees);
        var before=c.Geometry.ToList();foreach(var r in new[]{0d,-1d,.09,double.NaN,double.PositiveInfinity,400d})if(d.TryUpdateQuarterCircle(c,100,100,r)||!before.SequenceEqual(c.Geometry))Fail("quarter invalid radius");
        if(d.TryUpdateQuarterCircle(c,-1,100,20)||d.TryUpdateQuarterCircle(c,100,double.NaN,20))Fail("quarter invalid center");
        var restored=history.Undo(d)!;Eq(20,restored.Cuts.Single().QuarterCircleArc!.Radius);restored=history.Redo(restored)!;Eq(30,restored.Cuts.Single().QuarterCircleArc!.Radius);
        d.UpdateCut(c,c.CenterX+10,c.CenterY+10,c.Width,c.Height,7);Eq(110,c.QuarterCircleArc!.Cx);Eq(110,c.QuarterCircleArc.Cy);Eq(angle,c.QuarterCircleArc.StartDegrees);
        d.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);Eq(300,c.QuarterCircleArc.Radius);Persist(d);
        foreach(var boundary in new[]{(sx>0?0d:300d,100d),(100d,sy>0?0d:300d),(sx>0?0d:300d,sy>0?0d:300d)})
        {
            var edge=new VCuttingDocument();var cut=Create(edge,boundary.Item1,boundary.Item2,sx,sy);var undo=new UndoRedoManager();undo.Record(edge);
            if(!edge.CanMergeBoundaryCut(cut)||!edge.TryMergeBoundaryCut(cut))Fail("quarter either radius merge");
            if(edge.Cuts.Count!=0||edge.InnerContours.Count!=0)Fail("quarter merge orphan");Closed(edge.OuterContour.Segments);
            var merged=edge.OuterContour.Segments.OfType<ArcSegment>().Single();Eq(20,merged.Radius);Eq(90,Math.Abs(merged.EndDegrees-merged.StartDegrees));
            if(FlatMaterialMaskEngine.Build(edge,transform).FillContains(transform.ToScreen(new(boundary.Item1+sx*5,boundary.Item2+sy*5))))Fail("quarter merged mask");
            if(BentSurfaceEngine.Build(edge).Points.Count==0)Fail("quarter merged mesh");Persist(edge);
            var previous=undo.Undo(edge)!;if(previous.Cuts.Count!=1||!previous.CanMergeBoundaryCut(previous.Cuts.Single()))Fail("quarter merge undo");previous=undo.Redo(previous)!;if(!previous.OuterContour.Segments.SequenceEqual(edge.OuterContour.Segments))Fail("quarter merge redo");
        }
    }
    if(HoleDragEngine.TryCalculate("QuarterCircle",new(100,100),new(100,120),300,300,out _)||HoleDragEngine.TryCalculate("QuarterCircle",new(100,100),new(100.01,120),300,300,out _))Fail("quarter degenerate drag");
    if(!HoleDragEngine.TryCalculate("QuarterCircle",new(295,295),new(400,400),300,300,out var clipped))Fail("quarter clamp");Eq(5,clipped.Width);Eq(5,clipped.Height);
    var tangent=new VCuttingDocument();var touching=Create(tangent,20,100,-1,1);if(tangent.CanMergeBoundaryCut(touching))Fail("quarter arc point contact");
}
static void SemicircleProperties()
{
    foreach(var input in new[]{(100d,0d,0d),(100d,300d,180d),(0d,100d,270d),(300d,150d,90d)})
    {
        var d=new VCuttingDocument();var cut=d.AddHole("Semicircle",100,100);cut.Geometry.Clear();cut.Geometry.Add(new ArcSegment(100,100,20,input.Item3,input.Item3+180));
        if(!d.TryUpdateSemicircle(cut,input.Item1,input.Item2,41.582))Fail("semicircle center property");
        var arc=cut.SemicircleArc!;Eq(input.Item1,arc.Cx);Eq(input.Item2,arc.Cy);Eq(41.582,arc.Radius);
        if(input.Item3==90)Eq(279.209,cut.CenterX);
        if(!d.CanMergeBoundaryCut(cut))Fail("semicircle property merge candidate");
        var history=new UndoRedoManager();history.Record(d);
        if(!d.TryUpdateSemicircle(cut,arc.Cx,arc.Cy,30))Fail("semicircle radius edit");
        arc=cut.SemicircleArc!;Eq(input.Item1,arc.Cx);Eq(input.Item2,arc.Cy);Eq(30,arc.Radius);Eq(input.Item3,arc.StartDegrees);
        if(!d.CanMergeBoundaryCut(cut)||!d.InnerContours.Single().Segments.SequenceEqual(cut.Geometry))Fail("semicircle diameter/inner retained");
        var geometry=cut.Geometry.ToList();
        foreach(var r in new[]{0d,-1d,.09,double.NaN,double.PositiveInfinity,400d})if(d.TryUpdateSemicircle(cut,arc.Cx,arc.Cy,r)||!cut.Geometry.SequenceEqual(geometry))Fail("invalid semicircle radius rejected");
        foreach(var center in new[]{(-1d,arc.Cy),(arc.Cx,301d),(double.NaN,arc.Cy),(arc.Cx,double.PositiveInfinity)})if(d.TryUpdateSemicircle(cut,center.Item1,center.Item2,30)||!cut.Geometry.SequenceEqual(geometry))Fail("invalid semicircle center rejected");
        var restored=history.Undo(d)!;Eq(41.582,restored.Cuts.Single().SemicircleArc!.Radius);restored=history.Redo(restored)!;Eq(30,restored.Cuts.Single().SemicircleArc!.Radius);
        if(!d.TryUpdateSemicircle(cut,150,150,30))Fail("semicircle center move");Eq(150,cut.SemicircleArc!.Cx);Eq(150,cut.SemicircleArc.Cy);Eq(input.Item3,cut.SemicircleArc.StartDegrees);
        d.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);Eq(300,cut.SemicircleArc.Radius);Eq(1500,cut.SemicircleArc.Cx);
        var path=Path.Combine(Path.GetTempPath(),$"semicircle-properties-{Guid.NewGuid():N}.dxf");
        try{VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path);var c=loaded.Cuts.Single();Eq(1500,c.SemicircleArc!.Cx);Eq(300,c.SemicircleArc.Radius);if(!loaded.TryUpdateSemicircle(c,1500,1500,400))Fail("semicircle loaded radius edit");Eq(400,c.SemicircleArc!.Radius);}
        finally{if(File.Exists(path))File.Delete(path);}
    }
}
static void SemicircleBoundaryMerge()
{
    CutOperation Cut(VCuttingDocument d,double x,double y,double start,double radius=20)
    {
        var vertical=start is 0 or 180;
        var geometry=new HoleDragGeometry(x+(start==270?radius/2:start==90?-radius/2:0),y+(start==0?radius/2:start==180?-radius/2:0),vertical?radius*2:radius,vertical?radius:radius*2,null,start);
        var preview=HoleDragEngine.CreatePreview("Semicircle",geometry);
        var cut=d.AddHole("Semicircle",geometry.CenterX,geometry.CenterY);cut.Geometry.Clear();cut.Geometry.AddRange(preview.Geometry);
        cut.CenterX=geometry.CenterX;cut.CenterY=geometry.CenterY;cut.Width=geometry.Width;cut.Height=geometry.Height;
        d.UpdateCut(cut,cut.CenterX,cut.CenterY,cut.Width,cut.Height,cut.Sides);return cut;
    }
    System.Windows.Point At(GeometrySegment s,bool end)=>s switch{LineSegment l=>end?new(l.X2,l.Y2):new(l.X1,l.Y1),ArcSegment a=>new(a.Cx+a.Radius*Math.Cos((end?a.EndDegrees:a.StartDegrees)*Math.PI/180),a.Cy+a.Radius*Math.Sin((end?a.EndDegrees:a.StartDegrees)*Math.PI/180)),_=>throw new Exception()};
    void Closed(VCuttingDocument d){for(var i=0;i<d.OuterContour.Segments.Count;i++)if((At(d.OuterContour.Segments[i],true)-At(d.OuterContour.Segments[(i+1)%d.OuterContour.Segments.Count],false)).Length>1e-6)Fail("semicircle merge closure");}
    double Area(SurfaceMeshData mesh){var area=0d;for(var i=0;i<mesh.Triangles.Count;i+=3){var a=mesh.Points[mesh.Triangles[i]];var b=mesh.Points[mesh.Triangles[i+1]];var c=mesh.Points[mesh.Triangles[i+2]];area+=Math.Abs((b.X-a.X)*(c.Y-a.Y)-(b.Y-a.Y)*(c.X-a.X))/2;}return area;}
    foreach(var input in new[]{(100d,0d,0d),(100d,300d,180d),(0d,100d,270d),(300d,100d,90d)})
    {
        var d=new VCuttingDocument();var cut=Cut(d,input.Item1,input.Item2,input.Item3);var original=(ArcSegment)cut.Geometry[0];var history=new UndoRedoManager();history.Record(d);
        if(!d.CanMergeBoundaryCut(cut)||!d.TryMergeBoundaryCut(cut))Fail("semicircle edge candidate/merge");
        if(d.Cuts.Count!=0||d.InnerContours.Count!=0)Fail("semicircle orphan cut");Closed(d);
        var arc=d.OuterContour.Segments.OfType<ArcSegment>().Single();Eq(original.Cx,arc.Cx);Eq(original.Cy,arc.Cy);Eq(20,arc.Radius);Eq(-180,arc.EndDegrees-arc.StartDegrees);
        var angle=(original.StartDegrees+90)*Math.PI/180;var center=new System.Windows.Point(original.Cx+10*Math.Cos(angle),original.Cy+10*Math.Sin(angle));
        var transform=FlatViewportEngine.Calculate(800,800,300,300,false,1,new());var mask=FlatMaterialMaskEngine.Build(d,transform);
        if(mask.FillContains(transform.ToScreen(center))||!mask.FillContains(transform.ToScreen(new(150,150))))Fail("merged semicircle mask");
        Eq(90000-36*400*Math.Sin(Math.PI/36)/2,Area(BentSurfaceEngine.Build(d)));
        var sectionAxis=input.Item3 is 0 or 180?SectionAxis.H:SectionAxis.W;
        var section=ContourSectionEngine.MaterialInterval(d.OuterContour.Segments,sectionAxis,100,300);
        Eq(input.Item3 is 0 or 270?20:0,section.Start);Eq(input.Item3 is 180 or 90?280:300,section.End);
        var restored=history.Undo(d)!;if(restored.Cuts.Count!=1||!restored.CanMergeBoundaryCut(restored.Cuts.Single()))Fail("semicircle merge undo");restored=history.Redo(restored)!;if(!restored.OuterContour.Segments.SequenceEqual(d.OuterContour.Segments))Fail("semicircle merge redo");
        var path=Path.Combine(Path.GetTempPath(),$"merged-semicircle-{Guid.NewGuid():N}.dxf");
        try{VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path);if(!loaded.OuterContour.Segments.SequenceEqual(d.OuterContour.Segments))Fail("semicircle merged DXF");Closed(loaded);
            var pairs=File.ReadAllLines(path);var index=Array.FindIndex(pairs,v=>v=="ARC");var start=double.NaN;var end=double.NaN;
            for(var i=index+1;i+1<pairs.Length&&pairs[i]!="0";i+=2){if(pairs[i]=="50")start=double.Parse(pairs[i+1],System.Globalization.CultureInfo.InvariantCulture);if(pairs[i]=="51")end=double.Parse(pairs[i+1],System.Globalization.CultureInfo.InvariantCulture);}
            Eq(original.StartDegrees,start);Eq(original.EndDegrees,end);
        }finally{if(File.Exists(path))File.Delete(path);}
    }
    var interior=new VCuttingDocument();var inner=Cut(interior,100,100,0);if(interior.CanMergeBoundaryCut(inner))Fail("internal semicircle candidate");
    var outside=new VCuttingDocument();var outward=Cut(outside,100,0,180);outward.CenterY=-10;HoleGeometryEngine.Rebuild(outward);if(outside.CanMergeBoundaryCut(outward))Fail("outward semicircle candidate");
    var tangent=new VCuttingDocument();var touching=Cut(tangent,100,20,180);if(tangent.CanMergeBoundaryCut(touching))Fail("arc tangent only candidate");
    var repeated=new VCuttingDocument();foreach(var input in new[]{(100d,0d,0d),(100d,300d,180d),(0d,100d,270d),(300d,100d,90d)}){var cut=Cut(repeated,input.Item1,input.Item2,input.Item3);if(!repeated.TryMergeBoundaryCut(cut))Fail("sequential semicircle merge");Closed(repeated);}Eq(4,repeated.OuterContour.Segments.OfType<ArcSegment>().Count());
    var rounded=new VCuttingDocument();if(!rounded.TryFilletOuterCorner(0,10))Fail("semicircle fillet fixture");var fillet=rounded.OuterContour.Segments.OfType<ArcSegment>().Single();if(!rounded.TryMergeBoundaryCut(Cut(rounded,100,0,0))||!rounded.OuterContour.Segments.Contains(fillet))Fail("semicircle fillet preservation");Closed(rounded);
    var split=new VCuttingDocument();var first=(LineSegment)split.OuterContour.Segments[0];split.OuterContour.Segments.RemoveAt(0);split.OuterContour.Segments.InsertRange(0,[new LineSegment(first.X1,first.Y1,150,0),new LineSegment(150,0,first.X2,first.Y2)]);if(!split.TryMergeBoundaryCut(Cut(split,150,0,0)))Fail("semicircle multiple collinear edges");Closed(split);
    var partial=new VCuttingDocument();var notch=partial.AddHole("Rectangle",10,10);partial.UpdateCut(notch,10,10,20,20,7);if(!partial.TryMergeBoundaryCut(notch))Fail("partial semicircle fixture");var half=Cut(partial,20,20,270);if(!partial.TryMergeBoundaryCut(half))Fail("partial diameter overlap");Closed(partial);
}
static void SemicircleCreation()
{
    foreach(var input in new[]{(0d,20d,0d),(0d,-20d,180d),(20d,0d,270d),(-20d,0d,90d),(5d,20d,0d),(20d,5d,270d)})
    {
        if(!HoleDragEngine.TryCalculate("Semicircle",new(100,100),new(100+input.Item1,100+input.Item2),300,300,out var g))Fail("semicircle axial drag");
        Eq(input.Item3,g.SemicircleStartDegrees);
        var preview=HoleDragEngine.CreatePreview("Semicircle",g);var arc=(ArcSegment)preview.Geometry[0];var line=(LineSegment)preview.Geometry[1];
        Eq(100,arc.Cx);Eq(100,arc.Cy);Eq(20,arc.Radius);Eq(180,arc.EndDegrees-arc.StartDegrees);
        Eq(arc.Cx+arc.Radius*Math.Cos(arc.EndDegrees*Math.PI/180),line.X1);Eq(arc.Cy+arc.Radius*Math.Sin(arc.EndDegrees*Math.PI/180),line.Y1);
        Eq(arc.Cx+arc.Radius*Math.Cos(arc.StartDegrees*Math.PI/180),line.X2);Eq(arc.Cy+arc.Radius*Math.Sin(arc.StartDegrees*Math.PI/180),line.Y2);
        var midpoint=(arc.StartDegrees+90)*Math.PI/180;var inside=new System.Windows.Point(arc.Cx+10*Math.Cos(midpoint),arc.Cy+10*Math.Sin(midpoint));
        var d=new VCuttingDocument();var history=new UndoRedoManager();history.Record(d);
        var cut=d.AddHole("Semicircle",g.CenterX,g.CenterY);cut.Geometry.Clear();cut.Geometry.Add(arc);d.UpdateCut(cut,g.CenterX,g.CenterY,g.Width,g.Height,cut.Sides);
        if(!cut.Geometry.SequenceEqual(preview.Geometry)||!d.InnerContours.Single().Segments.SequenceEqual(cut.Geometry))Fail("semicircle preview/commit");
        var transform=FlatViewportEngine.Calculate(800,800,300,300,false,1,new());var mask=FlatMaterialMaskEngine.Build(d,transform);
        if(mask.FillContains(transform.ToScreen(inside))||!mask.FillContains(transform.ToScreen(new(200,200))))Fail("semicircle material mask");
        var mesh=BentSurfaceEngine.Build(d);if(mesh.Points.Count==0)Fail("semicircle 3D surface");
        var restored=history.Undo(d)!;if(restored.Cuts.Count!=0)Fail("semicircle undo");restored=history.Redo(restored)!;if(!restored.Cuts.Single().Geometry.SequenceEqual(cut.Geometry))Fail("semicircle redo");
        var path=Path.Combine(Path.GetTempPath(),$"semicircle-{Guid.NewGuid():N}.dxf");
        try{VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path);var c=loaded.Cuts.Single();if(!c.Geometry.SequenceEqual(cut.Geometry))Fail("semicircle DXF");loaded.UpdateCut(c,c.CenterX+10,c.CenterY+10,c.Width,c.Height,c.Sides);var moved=c.Geometry.OfType<ArcSegment>().Single();Eq(110,moved.Cx);Eq(110,moved.Cy);Eq(input.Item3,moved.StartDegrees);loaded.UpdateCut(c,c.CenterX,c.CenterY,c.Width*1.5,c.Height,c.Sides);Eq(30,c.Geometry.OfType<ArcSegment>().Single().Radius);Eq(input.Item3,c.Geometry.OfType<ArcSegment>().Single().StartDegrees);}
        finally{if(File.Exists(path))File.Delete(path);}
    }
    if(HoleDragEngine.TryCalculate("Semicircle",new(100,100),new(100,100.01),300,300,out _))Fail("semicircle tiny drag");
    if(!HoleDragEngine.TryCalculate("Semicircle",new(5,100),new(5,200),300,300,out var clipped))Fail("semicircle boundary");Eq(10,clipped.Width);Eq(5,clipped.Height);
    if(HoleDragEngine.TryCalculate("Semicircle",new(0,100),new(0,200),300,300,out _))Fail("semicircle no available radius");
}
static void TriangleAnchoredSize()
{
    foreach(var vertices in new System.Windows.Point[][]{[new(20,30),new(70,45),new(35,90)],[new(20,90),new(70,90),new(45,30)]})
    {
        var d=new VCuttingDocument();var c=d.AddHole("Triangle",50,60);d.UpdateTriangleVertices(c,vertices);
        var history=new UndoRedoManager();history.Record(d);
        if(!d.TryResizeTriangleLowerLeft(c,100,c.Height))Fail("anchored width resize");
        Eq(20,c.LowerLeftX);Eq(30,c.LowerLeftY);Eq(100,c.Width);Eq(60,c.Height);
        if(!d.TryResizeTriangleLowerLeft(c,c.Width,30))Fail("anchored height resize");
        Eq(20,c.LowerLeftX);Eq(30,c.LowerLeftY);Eq(100,c.Width);Eq(30,c.Height);
        for(var i=0;i<3;i++){var l=(LineSegment)c.Geometry[i];Eq(20+(vertices[i].X-20)*2,l.X1);Eq(30+(vertices[i].Y-30)*.5,l.Y1);}
        if(!d.InnerContours.Single().Segments.SequenceEqual(c.Geometry))Fail("anchored resize inner contour");
        var restored=history.Undo(d)!;Eq(50,restored.Cuts.Single().Width);Eq(20,restored.Cuts.Single().LowerLeftX);
        restored=history.Redo(restored)!;Eq(100,restored.Cuts.Single().Width);Eq(30,restored.Cuts.Single().LowerLeftY);
        var geometry=c.Geometry.ToList();
        foreach(var size in new[]{(400d,30d),(100d,400d),(-1d,30d),(100d,0d),(double.NaN,30d),(100d,double.PositiveInfinity)})
        {if(d.TryResizeTriangleLowerLeft(c,size.Item1,size.Item2)||!geometry.SequenceEqual(c.Geometry))Fail("invalid size must not mutate");Eq(20,c.LowerLeftX);Eq(30,c.LowerLeftY);Eq(100,c.Width);Eq(30,c.Height);}
        var path=Path.Combine(Path.GetTempPath(),$"anchored-size-{Guid.NewGuid():N}.dxf");
        try{VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path).Cuts.Single();Eq(20,loaded.LowerLeftX);Eq(30,loaded.LowerLeftY);if(!loaded.Geometry.SequenceEqual(c.Geometry))Fail("anchored size roundtrip");}finally{if(File.Exists(path))File.Delete(path);}
        d.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);
        if(!d.TryResizeTriangleLowerLeft(c,1500,c.Height))Fail("anchored mm resize");Eq(200,c.LowerLeftX);Eq(300,c.LowerLeftY);Eq(300,c.Height);
    }
    var f=new VCuttingDocument();var t=f.AddHole("Triangle",100,100);f.UpdateTriangleVertices(t,[new(50,50),new(100,50),new(50,100)]);
    if(!f.TryFilletContourCorner(t.Id,0,5,out _))Fail("anchored fillet setup");
    var before=t.Geometry.ToList();if(f.TryResizeTriangleLowerLeft(t,t.Width*2,t.Height)||!before.SequenceEqual(t.Geometry))Fail("unsupported ARC scaling must preserve contour");
}
static void TriangleLowerLeft()
{
    var d=new VCuttingDocument();var c=d.AddHole("Triangle",50,60);
    d.UpdateTriangleVertices(c,[new(20,30),new(70,45),new(35,90)]);
    Eq(20,c.LowerLeftX);Eq(30,c.LowerLeftY);
    var original=c.Geometry.Cast<LineSegment>().ToList();var history=new UndoRedoManager();history.Record(d);
    d.MoveTriangleLowerLeft(c,100,c.LowerLeftY);d.MoveTriangleLowerLeft(c,c.LowerLeftX,110);
    Eq(100,c.LowerLeftX);Eq(110,c.LowerLeftY);Eq(50,c.Width);Eq(60,c.Height);
    for(var i=0;i<3;i++){var line=(LineSegment)c.Geometry[i];Eq(original[i].X1+80,line.X1);Eq(original[i].Y1+80,line.Y1);Eq(original[i].X2+80,line.X2);Eq(original[i].Y2+80,line.Y2);}
    if(!d.InnerContours.Single().Segments.SequenceEqual(c.Geometry))Fail("lower-left inner sync");
    var restored=history.Undo(d)!;Eq(20,restored.Cuts.Single().LowerLeftX);Eq(30,restored.Cuts.Single().LowerLeftY);
    restored=history.Redo(restored)!;Eq(100,restored.Cuts.Single().LowerLeftX);Eq(110,restored.Cuts.Single().LowerLeftY);
    var path=Path.Combine(Path.GetTempPath(),$"lower-left-{Guid.NewGuid():N}.dxf");
    try{VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path);if(!loaded.Cuts.Single().Geometry.SequenceEqual(c.Geometry))Fail("lower-left persistence");Eq(100,loaded.Cuts.Single().LowerLeftX);Eq(110,loaded.Cuts.Single().LowerLeftY);}finally{if(File.Exists(path))File.Delete(path);}
    d.MoveTriangleLowerLeft(c,-50,1000);Eq(0,c.LowerLeftX);Eq(d.Material.Height-c.Height,c.LowerLeftY);
    var saved=c.Geometry.ToList();d.MoveTriangleLowerLeft(c,double.NaN,0);if(!saved.SequenceEqual(c.Geometry))Fail("invalid lower-left input");
    d.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);Eq(500,c.Width);
    d.MoveTriangleLowerLeft(c,200,300);Eq(200,c.LowerLeftX);Eq(300,c.LowerLeftY);
    var f=new VCuttingDocument();var t=f.AddHole("Triangle",100,100);
    f.UpdateTriangleVertices(t,[new(50,50),new(100,50),new(50,100)]);
    if(!f.TryFilletContourCorner(t.Id,0,5,out _))Fail("lower-left fillet setup");
    var arc=t.Geometry.OfType<ArcSegment>().Single();var x=t.LowerLeftX;var y=t.LowerLeftY;
    f.MoveTriangleLowerLeft(t,x+10,y+20);var moved=t.Geometry.OfType<ArcSegment>().Single();Eq(arc.Cx+10,moved.Cx);Eq(arc.Cy+20,moved.Cy);Eq(arc.Radius,moved.Radius);Eq(arc.StartDegrees,moved.StartDegrees);Eq(arc.EndDegrees,moved.EndDegrees);
}
static void CircleRadius()
{
    var d=new VCuttingDocument();var c=d.AddHole("Circle",80,90);var history=new UndoRedoManager();history.Record(d);var old=c.Width;
    if(!d.TryResizeCircleRadius(c,23.59))Fail("circle radius edit");
    Eq(47.18,c.Width);Eq(c.Width,c.Height);Eq(80,c.CenterX);Eq(90,c.CenterY);
    var circle=c.Geometry.OfType<CircleSegment>().Single();Eq(23.59,circle.Radius);
    if(!c.Geometry.SequenceEqual(d.InnerContours.Single().Segments))Fail("circle contour sync");
    foreach(var invalid in new[]{0d,-1,.049,81,double.NaN,double.PositiveInfinity})
    {if(d.TryResizeCircleRadius(c,invalid)||!c.Geometry.SequenceEqual(new GeometrySegment[]{circle}))Fail("invalid circle radius changed geometry");Eq(47.18,c.Width);Eq(47.18,c.Height);Eq(80,c.CenterX);Eq(90,c.CenterY);}
    var undo=history.Undo(d)!;Eq(old,undo.Cuts.Single().Width);var redo=history.Redo(undo)!;Eq(47.18,redo.Cuts.Single().Width);
    var path=Path.Combine(Path.GetTempPath(),$"circle-radius-{Guid.NewGuid():N}.dxf");
    try{VCuttingDxfSerializer.Save(d,path);Eq(23.59,VCuttingDxfSerializer.Load(path).Cuts.Single().Geometry.OfType<CircleSegment>().Single().Radius);}finally{if(File.Exists(path))File.Delete(path);}
    d.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);Eq(235.9,c.Width/2);
    if(!d.TryResizeCircleRadius(c,100))Fail("circle mm edit");Eq(200,c.Width);Eq(800,c.CenterX);Eq(900,c.CenterY);
    d.ChangeUnit(MeasurementUnit.Meter,UnitChangeMode.PreservePhysicalSize);Eq(.1,c.Width/2);
    if(!d.TryResizeCircleRadius(c,.2))Fail("circle meter edit");Eq(.4,c.Width);Eq(.8,c.CenterX);
    if(d.TryResizeCircleRadius(d.AddHole("Triangle",1,1),.1))Fail("non-circle radius edit");
}
static void Defaults(){var d=new VCuttingDocument();Eq(300,d.Material.Width);Eq(300,d.Material.Height);Eq(.2,d.Material.Thickness);if(WindowTitleFormatter.Format(null)!="Untitled - VCutting"||WindowTitleFormatter.Format(@"C:\Designs\Sample.dxf")!="Sample.dxf - VCutting"||WindowTitleFormatter.Format(@"C:\Designs\Sample.dxf",true)!="*Sample.dxf - VCutting")Fail("window title format");}
static void FileShortcuts(){if(FileShortcutEngine.Resolve(System.Windows.Input.Key.N,System.Windows.Input.ModifierKeys.Control)!=FileShortcutAction.New||FileShortcutEngine.Resolve(System.Windows.Input.Key.O,System.Windows.Input.ModifierKeys.Control)!=FileShortcutAction.Open||FileShortcutEngine.Resolve(System.Windows.Input.Key.S,System.Windows.Input.ModifierKeys.Control)!=FileShortcutAction.Save||FileShortcutEngine.Resolve(System.Windows.Input.Key.S,System.Windows.Input.ModifierKeys.Control|System.Windows.Input.ModifierKeys.Shift)!=FileShortcutAction.None)Fail("file shortcuts");}
static void DirtyState(){var state=new DocumentDirtyState();if(state.IsDirty)Fail("new dirty state");state.MarkChanged();if(!state.IsDirty)Fail("dirty change");state.MarkSaved();if(state.IsDirty)Fail("dirty save");}
static void DocumentUnits(){var d=new VCuttingDocument();var bend=d.AddBend(SectionAxis.W,50,BendDirection.Up);var cut=d.AddHole("Rectangle",30,40);d.MicroJoints.Add(new(){Id="MJ001",Position=2,Width=.1});var width=d.Material.Width;d.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);if(d.Unit!=MeasurementUnit.Millimeter)Fail("unit conversion target");Eq(width*10,d.Material.Width);Eq(500,bend.Position);Eq(300,cut.CenterX);Eq(20,d.MicroJoints[0].Position);var numeric=d.Material.Width;d.ChangeUnit(MeasurementUnit.Meter,UnitChangeMode.PreserveNumbers);Eq(numeric,d.Material.Width);if(d.Unit!=MeasurementUnit.Meter)Fail("unit reinterpretation");var path=Path.Combine(Path.GetTempPath(),$"unit-{Guid.NewGuid():N}.dxf");try{VCuttingDxfSerializer.Save(d,path);var text=File.ReadAllText(path);if(!text.Contains("\r\n$INSUNITS\r\n70\r\n6\r\n"))Fail("DXF unit header");var loaded=VCuttingDxfSerializer.Load(path);if(loaded.Unit!=MeasurementUnit.Meter)Fail("unit roundtrip");Eq(d.Material.Width,loaded.Material.Width);}finally{if(File.Exists(path))File.Delete(path);}}
static void Thickness(){var d=new VCuttingDocument();var b=d.AddBend(SectionAxis.W,10,BendDirection.Up);Eq(.1,b.VCutDepth);Eq(.1,b.ResidualThickness);d.ChangeThickness(.3);Eq(.15,b.VCutDepth);Eq(.15,b.ResidualThickness);}
static void Bends(){var d=new VCuttingDocument();d.AddBend(SectionAxis.W,50,BendDirection.Up);d.AddBend(SectionAxis.W,70,BendDirection.Down);d.AddBend(SectionAxis.H,20,BendDirection.Left);if(d.Bends.Count!=3||d.Bends.Count(x=>x.Layer=="V")!=2||d.Bends.Count(x=>x.Layer=="V1")!=1)Fail("bend synchronization");Eq(300,d.Material.Width);Eq(300,d.Material.Height);}
static void FlatLengthFromSectionSegments(){var d=new VCuttingDocument();d.ConfigureNew(2396,2396,2,MeasurementUnit.Centimeter);d.AddBend(SectionAxis.H,199,BendDirection.Left);d.AddBend(SectionAxis.H,2197,BendDirection.Right);d.UpdateSegment(d.HSegments[0],199);d.UpdateSegment(d.HSegments[1],1998);d.UpdateSegment(d.HSegments[2],199);Eq(2396,d.HSegments.Sum(s=>s.Length));Eq(2396,d.Material.Height);d.AddBend(SectionAxis.W,199,BendDirection.Up);d.AddBend(SectionAxis.W,2197,BendDirection.Down);d.UpdateSegment(d.WSegments[0],199);d.UpdateSegment(d.WSegments[1],1998);d.UpdateSegment(d.WSegments[2],199);Eq(2396,d.WSegments.Sum(s=>s.Length));Eq(2396,d.Material.Width);d.ChangeThickness(4);Eq(2396,d.Material.Width);Eq(2396,d.Material.Height);Eq(201,BendCalculationEngine.ExteriorSegmentLength(d,SectionAxis.H,0));Eq(2002,BendCalculationEngine.ExteriorSegmentLength(d,SectionAxis.H,1));Eq(201,BendCalculationEngine.ExteriorSegmentLength(d,SectionAxis.H,2));}
static void SegmentsAndBentGeometry(){var d=new VCuttingDocument();var first=d.AddBend(SectionAxis.W,50,BendDirection.Up);d.AddBend(SectionAxis.W,70,BendDirection.Down);if(d.WSegments.Count!=3)Fail("segment split");Eq(50,d.WSegments[0].Length);Eq(20,d.WSegments[1].Length);var g=SectionGeometryEngine.Build(d,SectionAxis.W,true);if(g.Points.Count!=4)Fail("bent nodes");Eq(50,g.Points[1].X);Eq(0,g.Points[1].Y);Eq(50,g.Points[2].X);Eq(20,g.Points[2].Y);d.UpdateBend(first,40,BendDirection.Down,10);if(first.Layer!="V1"||first.Sequence!=10)Fail("editable bend");d.UpdateSegment(d.WSegments[0],35);Eq(35,d.Bends.OrderBy(x=>x.Position).First().Position);}
static void EditableAxisDistances(){var d=new VCuttingDocument();d.AddBend(SectionAxis.H,40,BendDirection.Left);d.AddBend(SectionAxis.H,90,BendDirection.Right);Eq(40,d.HSegments[0].Length);Eq(50,d.HSegments[1].Length);d.UpdateSegment(d.HSegments[0],32.5);var ordered=d.Bends.Where(b=>b.Axis==SectionAxis.H).OrderBy(b=>b.Position).ToList();Eq(32.5,ordered[0].Position);Eq(82.5,ordered[1].Position);d.UpdateSegment(d.HSegments[1],60);Eq(92.5,ordered[1].Position);}
static void ExteriorDimensions(){var d=new VCuttingDocument();d.AddBend(SectionAxis.H,40,BendDirection.Left);d.AddBend(SectionAxis.H,90,BendDirection.Right);Eq(40.1,BendCalculationEngine.ExteriorSegmentLength(d,SectionAxis.H,0));Eq(50.2,BendCalculationEngine.ExteriorSegmentLength(d,SectionAxis.H,1));Eq(d.HSegments[2].Length+.1,BendCalculationEngine.ExteriorSegmentLength(d,SectionAxis.H,2));Eq(40,BendCalculationEngine.CenterSegmentLengthFromExterior(40.1,.2,0,3));Eq(50,BendCalculationEngine.CenterSegmentLengthFromExterior(50.2,.2,1,3));d.ChangeThickness(.3);Eq(40.15,BendCalculationEngine.ExteriorSegmentLength(d,SectionAxis.H,0));Eq(50.3,BendCalculationEngine.ExteriorSegmentLength(d,SectionAxis.H,1));}
static void HoleOperations(){var shapes=new[]{"Circle","Arc","Sector","Ellipse","Triangle","Rectangle","Diamond","Parallelogram","Pentagon","Hexagon","Polygon"};var d=new VCuttingDocument();foreach(var shape in shapes){var cut=d.AddHole(shape,10+d.Cuts.Count,20);if(cut.Geometry.Count==0||d.InnerContours.All(c=>c.Id!=cut.Id))Fail($"{shape} hole");if(shape!="Circle"&&cut.Geometry.OfType<LineSegment>().Any(l=>l.X1==l.X2&&l.Y1==l.Y2))Fail($"{shape} zero segment");}if(d.Cuts.Count!=11)Fail("hole count");var polygon=d.Cuts.Last();d.UpdateCut(polygon,30,40,8,6,9);if(polygon.Geometry.Count!=9||polygon.CenterX!=30||polygon.CenterY!=40)Fail("editable polygon");}
static void TriangleDragDirection()
{
    foreach(var up in new[]{false,true})foreach(var left in new[]{false,true})
    {
        var start=new System.Windows.Point(150,150);var end=new System.Windows.Point(left?90:210,up?230:70);
        if(!HoleDragEngine.TryCalculate("Triangle",start,end,300,300,out var bounds)||bounds.TriangleVertices is not {Count:3})Fail("triangle direction bounds");
        var vertices=bounds.TriangleVertices!;Eq(150,vertices[0].Y);Eq(150,vertices[1].Y);Eq(end.Y,vertices[2].Y);Eq((start.X+end.X)/2,vertices[2].X);
        Eq(60,bounds.Width);Eq(80,bounds.Height);
        var transform=new FlatViewportTransform(1,new(0,300));
        var apex=transform.ToScreen(vertices[2]);var baseline=transform.ToScreen(vertices[0]);
        if(up?apex.Y>=baseline.Y:apex.Y<=baseline.Y)Fail("triangle screen orientation");
        var preview=HoleDragEngine.CreatePreview("Triangle",bounds);
        var d=new VCuttingDocument();var history=new UndoRedoManager();history.Record(d);
        var cut=d.AddHole("Triangle",bounds.CenterX,bounds.CenterY);d.UpdateTriangleVertices(cut,vertices);
        if(!preview.Geometry.SequenceEqual(cut.Geometry)||!cut.Geometry.SequenceEqual(d.InnerContours.Single().Segments))Fail("triangle preview/create mismatch");
        var restored=history.Undo(d)??throw new InvalidOperationException();if(restored.Cuts.Count!=0)Fail("triangle creation undo");
        var redo=history.Redo(restored)??throw new InvalidOperationException();if(!redo.Cuts.Single().Geometry.SequenceEqual(preview.Geometry))Fail("triangle direction redo");
        var path=Path.Combine(Path.GetTempPath(),"triangle-direction-"+Guid.NewGuid().ToString("N")+".dxf");
        try{VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path);if(!loaded.Cuts.Single().Geometry.SequenceEqual(preview.Geometry))Fail("triangle direction persistence");}
        finally{if(File.Exists(path))File.Delete(path);}
        d.UpdateTriangleVertices(cut,vertices.Select(p=>new System.Windows.Point(p.X+10,p.Y+10)).ToList());
        var moved=cut.Geometry.Cast<LineSegment>().ToList();if(up?moved[2].Y1<=moved[0].Y1:moved[2].Y1>=moved[0].Y1)Fail("triangle direction move");
    }
    if(!HoleDragEngine.TryCalculate("Triangle",new(150,150),new(-20,500),300,300,out var clamped))Fail("triangle direction clamp");
    Eq(0,clamped.TriangleVertices![0].X);Eq(300,clamped.TriangleVertices[2].Y);
    if(HoleDragEngine.TryCalculate("Triangle",new(10,10),new(10,10),300,300,out _)||HoleDragEngine.TryCalculate("Triangle",new(10,10),new(20,10.05),300,300,out _))Fail("triangle click/tiny drag");
}
static void QuadrilateralDragCreation()
{
    foreach(var shape in new[]{"Diamond","Parallelogram"})
    {
        if(!HoleDragEngine.Supports(shape))Fail(shape+" drag support");
        foreach(var reverse in new[]{false,true})
        {
            var start=reverse?new System.Windows.Point(80,90):new System.Windows.Point(20,30);
            var end=reverse?new System.Windows.Point(20,30):new System.Windows.Point(80,90);
            if(!HoleDragEngine.TryCalculate(shape,start,end,300,300,out var bounds))Fail(shape+" drag bounds");
            Eq(50,bounds.CenterX);Eq(60,bounds.CenterY);Eq(60,bounds.Width);Eq(60,bounds.Height);
            var document=new VCuttingDocument();var history=new UndoRedoManager(100);history.Record(document);
            var cut=document.AddHole(shape,bounds.CenterX,bounds.CenterY);
            document.UpdateCut(cut,bounds.CenterX,bounds.CenterY,bounds.Width,bounds.Height,cut.Sides);
            var lines=cut.Geometry.OfType<LineSegment>().ToList();
            if(lines.Count!=4||document.Cuts.Count!=1)Fail(shape+" drag contour");
            Eq(20,lines.Min(l=>l.X1));Eq(80,lines.Max(l=>l.X1));Eq(30,lines.Min(l=>l.Y1));Eq(90,lines.Max(l=>l.Y1));
            for(var i=0;i<4;i++){Eq(lines[i].X2,lines[(i+1)%4].X1);Eq(lines[i].Y2,lines[(i+1)%4].Y1);}
            if(history.Undo(document)?.Cuts.Count!=0)Fail(shape+" creation undo");
        }
        if(HoleDragEngine.TryCalculate(shape,new(20,30),new(20,30),300,300,out _)||
           HoleDragEngine.TryCalculate(shape,new(20,30),new(20.05,60),300,300,out _))Fail(shape+" click/tiny drag");
        if(!HoleDragEngine.TryCalculate(shape,new(280,290),new(400,500),300,300,out var clamped))Fail(shape+" boundary drag");
        Eq(290,clamped.CenterX);Eq(295,clamped.CenterY);Eq(20,clamped.Width);Eq(10,clamped.Height);
    }
}
static void HoleDragCreation(){if(!HoleDragEngine.Supports("Circle")||!HoleDragEngine.Supports("Triangle")||!HoleDragEngine.Supports("Rectangle")||!HoleDragEngine.Supports("Ellipse"))Fail("hole drag support set");if(!HoleDragEngine.TryCalculate("Rectangle",new(10,20),new(40,70),300,300,out var rectangle))Fail("rectangle drag");Eq(25,rectangle.CenterX);Eq(45,rectangle.CenterY);Eq(30,rectangle.Width);Eq(50,rectangle.Height);if(!HoleDragEngine.TryCalculate("Triangle",new(80,90),new(20,30),300,300,out var triangle))Fail("reverse triangle drag");Eq(50,triangle.CenterX);Eq(60,triangle.CenterY);Eq(60,triangle.Width);Eq(60,triangle.Height);if(!HoleDragEngine.TryCalculate("Circle",new(10,10),new(50,30),300,300,out var circle))Fail("circle drag");Eq(20,circle.Width);Eq(20,circle.Height);Eq(20,circle.CenterX);Eq(20,circle.CenterY);if(HoleDragEngine.TryCalculate("Rectangle",new(10,10),new(10.05,40),300,300,out _))Fail("tiny hole drag");if(!HoleDragEngine.TryCalculate("Rectangle",new(-10,-20),new(400,500),300,300,out var clamped))Fail("clamped hole drag");Eq(300,clamped.Width);Eq(300,clamped.Height);}
static void TriangleEditing(){var d=new VCuttingDocument();var cut=d.AddHole("Triangle",50,50);var vertices=new[]{new System.Windows.Point(30,30),new System.Windows.Point(70,30),new System.Windows.Point(30,80)};d.UpdateTriangleVertices(cut,vertices);if(cut.Geometry.Count!=3||cut.Geometry.Any(g=>g is not LineSegment)||d.InnerContours.Single(c=>c.Id==cut.Id).Segments.Count!=3)Fail("triangle line contour");Eq(50,cut.CenterX);Eq(55,cut.CenterY);Eq(40,cut.Width);Eq(50,cut.Height);var lines=cut.Geometry.Cast<LineSegment>().ToList();Eq(30,lines[0].X1);Eq(70,lines[0].X2);Eq(30,lines[1].Y1);Eq(80,lines[1].Y2);Eq(30,lines[2].X2);Eq(30,lines[2].Y2);var path=Path.Combine(Path.GetTempPath(),$"triangle-{Guid.NewGuid():N}.dxf");try{VCuttingDxfSerializer.Save(d,path);var restored=VCuttingDxfSerializer.Load(path);var restoredLines=restored.Cuts.Single().Geometry.Cast<LineSegment>().ToList();if(restoredLines.Count!=3)Fail("triangle line roundtrip");Eq(70,restoredLines[0].X2);Eq(80,restoredLines[1].Y2);}finally{if(File.Exists(path))File.Delete(path);}}
static void BoundaryCuts(){var d=new VCuttingDocument();var cut=d.AddHole("Rectangle",10,290);d.UpdateCut(cut,10,290,20,20,7);if(!d.CanMergeBoundaryCut(cut)||d.OuterContour.Segments.Count!=4||d.Cuts.Count!=1)Fail("boundary candidate must remain editable");var history=new UndoRedoManager();history.Record(d);if(!d.TryMergeBoundaryCut(cut)||d.Cuts.Count!=0||d.InnerContours.Count!=0||d.OuterContour.Segments.Count!=6)Fail("corner boundary rectangle merge");var undo=history.Undo(d)??throw new InvalidOperationException();if(undo.Cuts.Count!=1||undo.OuterContour.Segments.Count!=4)Fail("boundary merge undo");foreach(var p in new[]{(290d,290d),(290d,10d),(10d,10d)}){var next=d.AddHole("Rectangle",p.Item1,p.Item2);d.UpdateCut(next,p.Item1,p.Item2,20,20,7);if(!d.CanMergeBoundaryCut(next)||!d.TryMergeBoundaryCut(next))Fail("sequential corner boundary merge");}if(d.OuterContour.Segments.Count!=12)Fail("four corner notches");var path=Path.Combine(Path.GetTempPath(),$"boundary-{Guid.NewGuid():N}.dxf");try{VCuttingDxfSerializer.Save(d,path);var restored=VCuttingDxfSerializer.Load(path);if(restored.OuterContour.Segments.Count!=12||restored.Cuts.Count!=0||restored.InnerContours.Count!=0)Fail("boundary notch metadata roundtrip");}finally{if(File.Exists(path))File.Delete(path);}var plain=new VCuttingDocument();var edge=new System.Windows.Rect(120,280,40,20);if(!BoundaryCutEngine.TrySubtractRectangle(plain.OuterContour.Segments,edge,out var contour)||contour.Count!=8)Fail("edge boundary rectangle merge");if(BoundaryCutEngine.TrySubtractRectangle(plain.OuterContour.Segments,new(120,120,40,20),out _))Fail("interior rectangle must remain cut");}
static void ParallelogramBoundaryCut()
{
    foreach(var (x,y) in new[]{(150d,290d),(150d,10d),(30d,290d),(270d,10d)})
    {
        var d=new VCuttingDocument();var cut=d.AddHole("Parallelogram",x,y);d.UpdateCut(cut,x,y,60,20,7);
        if(!d.CanMergeBoundaryCut(cut)||d.Cuts.Count!=1||d.OuterContour.Segments.Count!=4)Fail("parallelogram candidate must remain editable");
        d.UpdateCut(cut,150,150,60,20,7);if(d.CanMergeBoundaryCut(cut))Fail("moved interior parallelogram candidate");
        d.UpdateCut(cut,x,y,60,20,7);
        var history=new UndoRedoManager();history.Record(d);
        if(!d.TryMergeBoundaryCut(cut)||d.Cuts.Count!=0||d.InnerContours.Count!=0)Fail("parallelogram merge");
        var lines=d.OuterContour.Segments.Cast<LineSegment>().ToList();
        if(!lines.Any(l=>Math.Abs(l.X1-l.X2)>1e-6&&Math.Abs(l.Y1-l.Y2)>1e-6))Fail("parallelogram slants lost");
        for(var i=0;i<lines.Count;i++){Eq(lines[i].X2,lines[(i+1)%lines.Count].X1);Eq(lines[i].Y2,lines[(i+1)%lines.Count].Y1);}
        var undo=history.Undo(d)??throw new InvalidOperationException();if(undo.Cuts.Count!=1||undo.InnerContours.Count!=1||undo.OuterContour.Segments.Count!=4)Fail("parallelogram merge undo");
        var redo=history.Redo(undo)??throw new InvalidOperationException();if(redo.Cuts.Count!=0||redo.OuterContour.Segments.Count!=lines.Count)Fail("parallelogram merge redo");
        if(BentSurfaceEngine.Build(d).Triangles.Count==0)Fail("merged parallelogram 3D");
        var interval=ContourSectionEngine.MaterialInterval(d.OuterContour.Segments,SectionAxis.H,x,300);
        if(y>150?interval.End>=300:interval.Start<=0)Fail("merged parallelogram local section");
        var path=Path.Combine(Path.GetTempPath(),"parallelogram-merge-"+Guid.NewGuid().ToString("N")+".dxf");
        try{VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path);if(loaded.Cuts.Count!=0||loaded.InnerContours.Count!=0||loaded.OuterContour.Segments.Count!=lines.Count)Fail("merged parallelogram DXF roundtrip");}
        finally{if(File.Exists(path))File.Delete(path);}
    }
    var split=new VCuttingDocument();var strip=split.AddHole("Parallelogram",150,150);
    strip.Width=500;strip.Height=20;HoleGeometryEngine.Rebuild(strip);
    if(split.CanMergeBoundaryCut(strip)||split.TryMergeBoundaryCut(strip))Fail("splitting parallelogram must not merge");
}
static void RectangleAfterDiagonal()
{
    foreach(var fixture in new[]{"Delete","Triangle","Parallelogram","ThreeLines"})
    {
        var d=new VCuttingDocument();
        if(fixture=="Delete")
        {
            var notch=d.AddHole("Rectangle",90,15);d.UpdateCut(notch,90,15,60,30,7);if(!d.TryMergeBoundaryCut(notch))Fail("rectangle recovery notch fixture");
            var line=d.OuterContour.Segments.OfType<LineSegment>().Single(l=>Math.Abs(l.Y1-30)<1e-6&&Math.Abs(l.Y2-30)<1e-6);
            if(!d.DeleteObject(new GeometryObject{Geometry=line}))Fail("rectangle recovery deletion fixture");
        }
        else if(fixture=="ThreeLines"){if(!d.DeleteObject(new GeometryObject{Geometry=d.OuterContour.Segments[0]}))Fail("triangle outer fixture");}
        else
        {
            var cut=d.AddHole(fixture,30,290);
            if(fixture=="Triangle")d.UpdateTriangleVertices(cut,[new(0,250),new(0,300),new(40,300)]);else d.UpdateCut(cut,30,290,60,20,7);
            if(!d.TryMergeBoundaryCut(cut))Fail("rectangle recovery polygon fixture");
        }
        var original=d.OuterContour.Segments.ToList();if(!original.OfType<LineSegment>().Any(l=>Math.Abs(l.X1-l.X2)>1e-6&&Math.Abs(l.Y1-l.Y2)>1e-6))Fail("diagonal outer required");
        var rectangle=d.AddHole("Rectangle",290,290);d.UpdateCut(rectangle,290,290,20,20,7);
        if(!d.CanMergeBoundaryCut(rectangle)||d.Cuts.Count!=1)Fail("rectangle recovery candidate");
        var history=new UndoRedoManager();history.Record(d);
        if(!d.TryMergeBoundaryCut(rectangle)||d.Cuts.Count!=0||d.InnerContours.Count!=0)Fail("rectangle recovery merge");
        var lines=d.OuterContour.Segments.Cast<LineSegment>().ToList();
        for(var i=0;i<lines.Count;i++){Eq(lines[i].X2,lines[(i+1)%lines.Count].X1);Eq(lines[i].Y2,lines[(i+1)%lines.Count].Y1);}
        if(!lines.Any(l=>Math.Abs(l.X1-l.X2)>1e-6&&Math.Abs(l.Y1-l.Y2)>1e-6))Fail("rectangle recovery lost diagonal");
        var undo=history.Undo(d)??throw new InvalidOperationException();if(!undo.OuterContour.Segments.SequenceEqual(original)||undo.Cuts.Count!=1)Fail("rectangle recovery undo");
        var redo=history.Redo(undo)??throw new InvalidOperationException();if(!redo.OuterContour.Segments.SequenceEqual(d.OuterContour.Segments)||redo.Cuts.Count!=0)Fail("rectangle recovery redo");
        var interior=d.AddHole("Rectangle",150,200);d.UpdateCut(interior,150,200,20,20,7);
        if(d.CanMergeBoundaryCut(interior))Fail("diagonal outer interior rectangle");
        d.DeleteObject(interior);
        var path=Path.Combine(Path.GetTempPath(),"rectangle-recovery-"+Guid.NewGuid().ToString("N")+".dxf");
        try{VCuttingDxfSerializer.Save(d,path);if(!VCuttingDxfSerializer.Load(path).OuterContour.Segments.SequenceEqual(d.OuterContour.Segments))Fail("rectangle recovery DXF");}
        finally{if(File.Exists(path))File.Delete(path);}
        if(BoundaryCutEngine.TrySubtractRectangle(d.OuterContour.Segments,new System.Windows.Rect(-10,190,320,20),out _))Fail("diagonal rectangle splitting rejection");
    }
}
static void TriangleBoundaryCut(){var d=new VCuttingDocument();var cut=d.AddHole("Triangle",20,275);d.UpdateTriangleVertices(cut,[new(0,250),new(0,300),new(40,300)]);if(!d.CanMergeBoundaryCut(cut))Fail("triangle boundary candidate");if(!d.TryMergeBoundaryCut(cut)||d.Cuts.Count!=0||d.InnerContours.Count!=0||d.OuterContour.Segments.Count!=5)Fail("triangle boundary merge");var diagonal=d.OuterContour.Segments.OfType<LineSegment>().SingleOrDefault(line=>Math.Abs(line.X1-line.X2)>1e-6&&Math.Abs(line.Y1-line.Y2)>1e-6);if(diagonal is null)Fail("triangle diagonal contour");var interior=new VCuttingDocument();var inner=interior.AddHole("Triangle",100,100);interior.UpdateTriangleVertices(inner,[new(80,80),new(120,80),new(100,130)]);if(interior.CanMergeBoundaryCut(inner))Fail("interior triangle candidate");}
static void OuterContourFillet(){var d=new VCuttingDocument();int Corner(double x,double y)=>d.OuterContour.Segments.Select((segment,index)=>(segment,index)).First(item=>item.segment is LineSegment line&&Math.Abs(line.X1-x)<1e-6&&Math.Abs(line.Y1-y)<1e-6).index;if(!d.TryFilletOuterCorner(Corner(0,300),10))Fail("top-left fillet");if(!d.TryFilletOuterCorner(Corner(300,300),10))Fail("top-right fillet after arc");if(!d.TryFilletOuterCorner(Corner(300,0),10))Fail("bottom-right fillet after arcs");if(!d.TryFilletOuterCorner(Corner(0,0),10))Fail("bottom-left fillet after arcs");if(d.OuterContour.Segments.Count!=8||d.OuterContour.Segments.Count(segment=>segment is ArcSegment)!=4)Fail("four corner fillet contour");var arcs=d.OuterContour.Segments.OfType<ArcSegment>().ToList();if(arcs.Any(arc=>Math.Abs(arc.Radius-10)>1e-6)||!arcs.Any(arc=>Math.Abs(arc.Cx-290)<1e-6&&Math.Abs(arc.Cy-10)<1e-6))Fail("outer fillet geometry");var resizedSource=arcs.Single(arc=>Math.Abs(arc.Cx-290)<1e-6&&Math.Abs(arc.Cy-10)<1e-6);if(!d.TryResizeOuterFillet(resizedSource,20,out var resized)||resized is null)Fail("fillet radius edit");Eq(20,resized!.Radius);Eq(280,resized.Cx);Eq(20,resized.Cy);if(d.TryResizeOuterFillet(resized,500,out _))Fail("oversized fillet radius edit");var mesh=BentSurfaceEngine.Build(d);if(mesh.Points.Count==0||mesh.Edges.Count<4)Fail("fillet 3D chord");var path=Path.Combine(Path.GetTempPath(),$"fillet-{Guid.NewGuid():N}.dxf");try{VCuttingDxfSerializer.Save(d,path);var text=File.ReadAllText(path);if(!text.Contains("\r\nARC\r\n"))Fail("fillet DXF arc");var restored=VCuttingDxfSerializer.Load(path);if(restored.OuterContour.Segments.Count(segment=>segment is ArcSegment)!=4||!restored.OuterContour.Segments.OfType<ArcSegment>().Any(arc=>Math.Abs(arc.Radius-20)<1e-6))Fail("fillet roundtrip");}finally{if(File.Exists(path))File.Delete(path);}}
static void RoundedFilletSurface()
{
    double Area(SurfaceMeshData mesh){var area=0d;for(var i=0;i<mesh.Triangles.Count;i+=3){var a=mesh.Points[mesh.Triangles[i]];var b=mesh.Points[mesh.Triangles[i+1]];var c=mesh.Points[mesh.Triangles[i+2]];area+=Math.Abs((b.X-a.X)*(c.Y-a.Y)-(b.Y-a.Y)*(c.X-a.X))/2;}return area;}
    var d=new VCuttingDocument();
    foreach(var (x,y) in new[]{(0d,0d),(300d,0d),(300d,300d),(0d,300d)})
    {
        var index=d.OuterContour.Segments.FindIndex(s=>s is LineSegment l&&Math.Abs(l.X1-x)<1e-6&&Math.Abs(l.Y1-y)<1e-6);
        if(!d.TryFilletOuterCorner(index,50))Fail("rounded surface fillet fixture");
    }
    var mesh=BentSurfaceEngine.Build(d);if(mesh.Edges.Count!=76)Fail("rounded four-corner edge count");
    Eq(90000-4*2500+4*18*2500*Math.Sin(Math.PI/36)/2,Area(mesh));
    foreach(var arc in d.OuterContour.Segments.OfType<ArcSegment>())
    {
        var angle=(arc.StartDegrees+arc.EndDegrees)*Math.PI/360;var x=arc.Cx+arc.Radius*Math.Cos(angle);var y=arc.Cy+arc.Radius*Math.Sin(angle);
        if(!mesh.Points.Any(p=>Math.Abs(p.X-x)<1e-6&&Math.Abs(p.Y-y)<1e-6))Fail("rounded arc midpoint absent");
    }
    var hole=d.AddHole("Rectangle",150,150);d.UpdateCut(hole,150,150,60,60,7);
    if(!d.TryFilletContourCorner(hole.Id,0,10,out var holeArc))Fail("rounded hole fixture");
    var withHole=BentSurfaceEngine.Build(d);if(withHole.Edges.Count!=98)Fail("rounded hole edges");
    Eq(Area(mesh)-(3600-100+18*100*Math.Sin(Math.PI/36)/2),Area(withHole));
    if(!d.TryResizeFillet(hole.Id,holeArc!,20,out _))Fail("rounded hole radius update");
    var resized=BentSurfaceEngine.Build(d);Eq(Area(mesh)-(3600-400+18*400*Math.Sin(Math.PI/36)/2),Area(resized));
    d.AddBend(SectionAxis.W,40,BendDirection.Up);d.AddBend(SectionAxis.H,40,BendDirection.Right);
    var bent=BentSurfaceEngine.Build(d);if(bent.Edges.Count<resized.Edges.Count||bent.Points.All(p=>Math.Abs(p.Z)<1e-6))Fail("rounded folded surface");
}
static void CurvedBoundaryMerge()
{
    VCuttingDocument Rounded()
    {
        var d=new VCuttingDocument();
        foreach(var (x,y) in new[]{(0d,0d),(300d,0d),(300d,300d),(0d,300d)})
        {
            var index=d.OuterContour.Segments.FindIndex(s=>s is LineSegment l&&Math.Abs(l.X1-x)<1e-6&&Math.Abs(l.Y1-y)<1e-6);
            if(!d.TryFilletOuterCorner(index,30))Fail("curved merge fillet fixture");
        }
        return d;
    }
    System.Windows.Point Start(GeometrySegment s)=>s is LineSegment l?new(l.X1,l.Y1):s is ArcSegment a?new(a.Cx+a.Radius*Math.Cos(a.StartDegrees*Math.PI/180),a.Cy+a.Radius*Math.Sin(a.StartDegrees*Math.PI/180)):throw new Exception();
    System.Windows.Point End(GeometrySegment s)=>s is LineSegment l?new(l.X2,l.Y2):s is ArcSegment a?new(a.Cx+a.Radius*Math.Cos(a.EndDegrees*Math.PI/180),a.Cy+a.Radius*Math.Sin(a.EndDegrees*Math.PI/180)):throw new Exception();
    void Closed(VCuttingDocument d){for(var i=0;i<d.OuterContour.Segments.Count;i++)if((End(d.OuterContour.Segments[i])-Start(d.OuterContour.Segments[(i+1)%d.OuterContour.Segments.Count])).Length>1e-6)Fail("curved merge closure");}
    var doc=Rounded();var arcs=doc.OuterContour.Segments.OfType<ArcSegment>().ToList();
    foreach(var (shape,x) in new[]{("Rectangle",80d),("Triangle",150d),("Parallelogram",220d)})
    {
        var before=doc.OuterContour.Segments.ToList();var cut=doc.AddHole(shape,x,290);
        if(shape=="Triangle")doc.UpdateTriangleVertices(cut,[new(x-20,300),new(x+20,300),new(x,270)]);else doc.UpdateCut(cut,x,290,40,20,7);
        if(!doc.CanMergeBoundaryCut(cut)||doc.Cuts.Count!=1)Fail(shape+" curved candidate");
        var history=new UndoRedoManager();history.Record(doc);
        if(!doc.TryMergeBoundaryCut(cut)||doc.Cuts.Count!=0||doc.InnerContours.Count!=0)Fail(shape+" curved merge");
        Closed(doc);if(!arcs.All(a=>doc.OuterContour.Segments.Contains(a)))Fail("untouched fillets lost");
        var undo=history.Undo(doc)??throw new Exception();if(!undo.OuterContour.Segments.SequenceEqual(before)||undo.Cuts.Count!=1)Fail("curved merge undo");
        var redo=history.Redo(undo)??throw new Exception();if(!redo.OuterContour.Segments.SequenceEqual(doc.OuterContour.Segments)||redo.Cuts.Count!=0)Fail("curved merge redo");
    }
    var inner=doc.AddHole("Rectangle",150,150);doc.UpdateCut(inner,150,150,20,20,7);if(doc.CanMergeBoundaryCut(inner))Fail("curved internal hole rejection");doc.DeleteObject(inner);
    if(BoundaryCutEngine.TrySubtractRectangle(doc.OuterContour.Segments,new System.Windows.Rect(-10,140,320,20),out _))Fail("curved split material rejection");
    var path=Path.Combine(Path.GetTempPath(),"curved-merge-"+Guid.NewGuid().ToString("N")+".dxf");
    try{VCuttingDxfSerializer.Save(doc,path);var loaded=VCuttingDxfSerializer.Load(path);if(!loaded.OuterContour.Segments.SequenceEqual(doc.OuterContour.Segments))Fail("curved merge ARC DXF");}
    finally{if(File.Exists(path))File.Delete(path);}
    if(BentSurfaceEngine.Build(doc).Triangles.Count==0)Fail("curved merge 3D");
    var crossing=Rounded();var originalArcs=crossing.OuterContour.Segments.OfType<ArcSegment>().ToList();var corner=crossing.AddHole("Rectangle",10,290);crossing.UpdateCut(corner,10,290,20,20,7);
    if(!crossing.CanMergeBoundaryCut(corner)||!crossing.TryMergeBoundaryCut(corner))Fail("ARC crossing rectangle merge");
    Closed(crossing);
    var trimmed=crossing.OuterContour.Segments.OfType<ArcSegment>().ToList();
    if(trimmed.Count!=5||trimmed.Any(a=>!originalArcs.Any(o=>o.Cx==a.Cx&&o.Cy==a.Cy&&o.Radius==a.Radius)))Fail("crossing ARC analytic preservation");
    if(!trimmed.Any(a=>originalArcs.All(o=>o!=a)))Fail("crossing ARC not trimmed");
    var transform=new FlatViewportTransform(1,new(0,300));var mask=FlatMaterialMaskEngine.Build(crossing,transform);
    if(mask.FillContains(transform.ToScreen(new(15,295)))||!mask.FillContains(transform.ToScreen(new(25,280))))Fail("curved merged material mask");
}
static void ArbitraryAndHoleFillet(){var d=new VCuttingDocument();var triangle=d.AddHole("Triangle",100,100);d.UpdateTriangleVertices(triangle,[new(70,70),new(140,70),new(90,140)]);if(!d.TryFilletContourCorner(triangle.Id,1,5,out var acuteArc)||acuteArc is null||triangle.Geometry.Count!=4)Fail("arbitrary angle hole fillet");Eq(5,acuteArc!.Radius);if(d.InnerContours.Single(contour=>contour.Id==triangle.Id).Segments.Count!=4)Fail("hole fillet inner synchronization");if(!d.TryResizeFillet(triangle.Id,acuteArc,8,out var resized)||resized is null)Fail("hole fillet radius edit");Eq(8,resized!.Radius);var rectangle=d.AddHole("Rectangle",200,200);d.UpdateCut(rectangle,200,200,40,30,7);if(!d.TryFilletContourCorner(rectangle.Id,0,4,out var rectangleArc)||rectangleArc is null)Fail("rectangle hole fillet");if(rectangle.Geometry.Count!=5||d.InnerContours.Single(contour=>contour.Id==rectangle.Id).Segments.Count!=5)Fail("rectangle hole fillet contour");var path=Path.Combine(Path.GetTempPath(),$"hole-fillet-{Guid.NewGuid():N}.dxf");try{VCuttingDxfSerializer.Save(d,path);var restored=VCuttingDxfSerializer.Load(path);if(restored.Cuts.Sum(cut=>cut.Geometry.Count(segment=>segment is ArcSegment))!=2)Fail("hole fillet roundtrip");}finally{if(File.Exists(path))File.Delete(path);}}
static void OuterContourEditing(){var source=new GeometrySegment[]{new LineSegment(0,0,10,0),new LineSegment(10,0,10,10),new LineSegment(10,10,0,10),new LineSegment(0,10,0,0)};var moved=OuterContourEditEngine.UpdateConnectedLine(source,1,new LineSegment(8,0,8,10)).Cast<LineSegment>().ToList();Eq(8,moved[0].X2);Eq(8,moved[1].X1);Eq(8,moved[1].X2);Eq(8,moved[2].X1);var resized=OuterContourEditEngine.UpdateConnectedLine(source,1,new LineSegment(10,0,10,7)).Cast<LineSegment>().ToList();Eq(7,resized[1].Y2);Eq(7,resized[2].Y1);Eq(0,resized[0].Y2);var d=new VCuttingDocument();var snapshot=d.OuterContour.Segments.ToList();d.UpdateOuterContourLine(snapshot,1,new LineSegment(280,0,280,300));if(d.OuterContour.Segments.Count!=4)Fail("outer edit segment count");Eq(280,((LineSegment)d.OuterContour.Segments[0]).X2);Eq(280,((LineSegment)d.OuterContour.Segments[2]).X1);}
static void OuterLineDeletion()
{
    var d=new VCuttingDocument();var notch=d.AddHole("Rectangle",90,15);d.UpdateCut(notch,90,15,60,30,7);
    if(!d.TryMergeBoundaryCut(notch))Fail("line deletion notch fixture");
    var original=d.OuterContour.Segments.ToList();
    var index=original.FindIndex(s=>s is LineSegment l&&Math.Abs(l.Y1-30)<1e-6&&Math.Abs(l.Y2-30)<1e-6);
    if(index<0)Fail("notch top line fixture");
    var removed=(LineSegment)original[index];var next=(LineSegment)original[(index+1)%original.Count];
    var history=new UndoRedoManager();history.Record(d);
    if(!d.DeleteObject(new GeometryObject{Geometry=removed}))Fail("outer line delete");
    var lines=d.OuterContour.Segments.Cast<LineSegment>().ToList();
    if(lines.Count!=original.Count-1||!lines.Contains(new LineSegment(removed.X1,removed.Y1,next.X2,next.Y2)))Fail("line deletion reconnect");
    for(var i=0;i<lines.Count;i++){Eq(lines[i].X2,lines[(i+1)%lines.Count].X1);Eq(lines[i].Y2,lines[(i+1)%lines.Count].Y1);}
    var transform=new FlatViewportTransform(1,new(0,300));var mask=FlatMaterialMaskEngine.Build(d,transform);
    // Sample each side of the new diagonal: one is material, the other is the notch.
    var a=new System.Windows.Point(removed.X1,removed.Y1);var b=new System.Windows.Point(next.X2,next.Y2);
    var mid=new System.Windows.Point((a.X+b.X)/2,(a.Y+b.Y)/2);var direction=b-a;var normal=new System.Windows.Vector(-direction.Y,direction.X);normal.Normalize();
    if(mask.FillContains(transform.ToScreen(mid+normal*2))==mask.FillContains(transform.ToScreen(mid-normal*2)))Fail("new diagonal material boundary");
    var mesh=BentSurfaceEngine.Build(d);
    if(!mesh.Edges.Any(e=>{var p=mesh.Points[e.A];var q=mesh.Points[e.B];return Math.Abs(p.X-a.X)<1e-6&&Math.Abs(p.Y-a.Y)<1e-6&&Math.Abs(q.X-b.X)<1e-6&&Math.Abs(q.Y-b.Y)<1e-6||Math.Abs(q.X-a.X)<1e-6&&Math.Abs(q.Y-a.Y)<1e-6&&Math.Abs(p.X-b.X)<1e-6&&Math.Abs(p.Y-b.Y)<1e-6;}))Fail("deleted line 3D boundary");
    var undo=history.Undo(d)??throw new InvalidOperationException();if(!undo.OuterContour.Segments.SequenceEqual(original))Fail("line deletion undo");
    var redo=history.Redo(undo)??throw new InvalidOperationException();if(!redo.OuterContour.Segments.SequenceEqual(d.OuterContour.Segments))Fail("line deletion redo");
    var path=Path.Combine(Path.GetTempPath(),"line-delete-"+Guid.NewGuid().ToString("N")+".dxf");
    try{VCuttingDxfSerializer.Save(d,path);if(!VCuttingDxfSerializer.Load(path).OuterContour.Segments.SequenceEqual(d.OuterContour.Segments))Fail("line deletion DXF");}
    finally{if(File.Exists(path))File.Delete(path);}
    foreach(var edgeIndex in new[]{0,3})
    {
        var plain=new VCuttingDocument();if(!plain.DeleteObject(new GeometryObject{Geometry=plain.OuterContour.Segments[edgeIndex]}))Fail("first/last line deletion");
        var triangle=plain.OuterContour.Segments.Cast<LineSegment>().ToList();
        for(var i=0;i<3;i++){Eq(triangle[i].X2,triangle[(i+1)%3].X1);Eq(triangle[i].Y2,triangle[(i+1)%3].Y1);}
        var snapshot=plain.OuterContour.Segments.ToList();if(plain.DeleteObject(new GeometryObject{Geometry=snapshot[0]})||!plain.OuterContour.Segments.SequenceEqual(snapshot))Fail("minimum contour deletion");
    }
}
static void OuterGapClosure()
{
    VCuttingDocument Rounded()
    {
        var d=new VCuttingDocument();
        foreach(var (x,y) in new[]{(0d,0d),(300d,0d),(300d,300d),(0d,300d)})
        {
            var index=d.OuterContour.Segments.FindIndex(s=>s is LineSegment l&&Math.Abs(l.X1-x)<1e-6&&Math.Abs(l.Y1-y)<1e-6);
            if(!d.TryFilletOuterCorner(index,20))Fail("gap closure fixture");
        }
        return d;
    }
    System.Windows.Point Point(GeometrySegment s,bool end)=>s is LineSegment l?(end?new(l.X2,l.Y2):new(l.X1,l.Y1)):s is ArcSegment a?new(a.Cx+a.Radius*Math.Cos((end?a.EndDegrees:a.StartDegrees)*Math.PI/180),a.Cy+a.Radius*Math.Sin((end?a.EndDegrees:a.StartDegrees)*Math.PI/180)):throw new Exception();
    void Closed(VCuttingDocument d){var segments=d.OuterContour.Segments;for(var i=0;i<segments.Count;i++)if((Point(segments[i],true)-Point(segments[(i+1)%segments.Count],false)).Length>1e-6)Fail("outer contour remains open");}
    foreach(var endpoints in new[]{false,true})
    {
        var d=Rounded();var original=d.OuterContour.Segments.ToList();var arcs=original.OfType<ArcSegment>().ToList();
        var index=original.FindIndex(s=>s is LineSegment l&&Math.Abs(l.X1-300)<1e-6&&Math.Abs(l.X2-300)<1e-6);var line=(LineSegment)original[index];
        var moved=endpoints?new LineSegment(line.X1-15,line.Y1,line.X2,line.Y2):new LineSegment(line.X1-15,line.Y1,line.X2-15,line.Y2);
        var history=new UndoRedoManager();history.Record(d);d.UpdateOuterContourLine(original,index,moved);Closed(d);
        if(d.OuterContour.Segments.Count!=original.Count+(endpoints?1:2)||!arcs.All(a=>d.OuterContour.Segments.Contains(a)))Fail("ARC neighbor gap lines");
        var snapshot=d.OuterContour.Segments.ToList();d.Recalculate();d.Recalculate();if(!snapshot.SequenceEqual(d.OuterContour.Segments))Fail("duplicate connector lines");
        // A further move uses the connected contour and keeps the same count.
        var connectedIndex=d.OuterContour.Segments.IndexOf(moved);d.UpdateOuterContourLine(snapshot,connectedIndex,new(moved.X1-5,moved.Y1,endpoints?moved.X2:moved.X2-5,moved.Y2));Closed(d);
        if(d.OuterContour.Segments.Count!=snapshot.Count)Fail("repeated move connector duplication");
        var undo=history.Undo(d)??throw new Exception();if(!undo.OuterContour.Segments.SequenceEqual(original))Fail("gap connector undo");
        var redo=history.Redo(undo)??throw new Exception();Closed(redo);
        var path=Path.Combine(Path.GetTempPath(),"gap-closure-"+Guid.NewGuid().ToString("N")+".dxf");
        try{VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path);Closed(loaded);if(!loaded.OuterContour.Segments.SequenceEqual(d.OuterContour.Segments))Fail("connector DXF");}
        finally{if(File.Exists(path))File.Delete(path);}
        if(BentSurfaceEngine.Build(d).Triangles.Count==0)Fail("connector 3D");
    }
    var removed=Rounded();var arc=removed.OuterContour.Segments.OfType<ArcSegment>().First();var before=removed.OuterContour.Segments.Count;
    if(!removed.DeleteObject(new GeometryObject{Geometry=arc}))Fail("ARC deletion");
    Closed(removed);if(removed.OuterContour.Segments.Contains(arc)||removed.OuterContour.Segments.Count!=before)Fail("ARC deletion chord");
    var open=Rounded();var first=open.OuterContour.Segments[0];open.OuterContour.Segments.RemoveAt(0);open.Recalculate();Closed(open);
    if(!open.OuterContour.Segments.Any(s=>s is LineSegment l&&(new System.Windows.Point(l.X1,l.Y1)-Point(first,false)).Length<1e-6&&(new System.Windows.Point(l.X2,l.Y2)-Point(first,true)).Length<1e-6))Fail("first/last gap closure");
}
static void ClippedSectionEditing(){var d=new VCuttingDocument();d.AddBend(SectionAxis.H,100,BendDirection.Left);var selectorY=d.Material.Height-10;var cut=d.AddHole("Rectangle",290,selectorY);d.UpdateCut(cut,290,selectorY,20,20,7);if(!d.TryMergeBoundaryCut(cut))Fail("clipped edit fixture");var geometry=SectionGeometryEngine.Build(d,SectionAxis.H,false,290);var oldEnd=geometry.EndPosition;if(!d.UpdateClippedSectionSegment(SectionAxis.H,geometry,0,110,290))Fail("clipped section edit");Eq(110,d.Bends.Single().Position);var changed=ContourSectionEngine.MaterialInterval(d.OuterContour.Segments,SectionAxis.H,290,d.Material.Height);Eq(oldEnd+10,changed.End);Eq(geometry.SegmentLengths[1],changed.End-d.Bends.Single().Position);}
static void ContourSections(){var d=new VCuttingDocument();d.AddBend(SectionAxis.H,100,BendDirection.Left);var selectorY=d.Material.Height-10;var expectedH=d.Material.Height-20;var cut=d.AddHole("Rectangle",290,selectorY);d.UpdateCut(cut,290,selectorY,20,20,7);if(!d.TryMergeBoundaryCut(cut))Fail("section fixture notch merge");var full=ContourSectionEngine.MaterialInterval(d.OuterContour.Segments,SectionAxis.H,150,d.Material.Height);Eq(0,full.Start);Eq(d.Material.Height,full.End);var h=ContourSectionEngine.MaterialInterval(d.OuterContour.Segments,SectionAxis.H,290,d.Material.Height);Eq(0,h.Start);Eq(expectedH,h.End);var w=ContourSectionEngine.MaterialInterval(d.OuterContour.Segments,SectionAxis.W,selectorY,d.Material.Width);Eq(0,w.Start);Eq(280,w.End);var geometry=SectionGeometryEngine.Build(d,SectionAxis.H,true,290);if(!geometry.IsClipped(d.Material.Height)||geometry.Bends.Count!=1)Fail("selected H section clipping");Eq(expectedH,geometry.SegmentLengths.Sum());var unchanged=SectionGeometryEngine.Build(d,SectionAxis.H,true,150);Eq(d.HSegments.Sum(x=>x.Length),unchanged.SegmentLengths.Sum());}
static void BentSurface(){var plain=new VCuttingDocument();var rectangle=BentSurfaceEngine.Build(plain);if(rectangle.Points.Count!=4||rectangle.Triangles.Count!=6||rectangle.Edges.Count!=4||rectangle.BendEdges.Count!=0)Fail("rectangular 3D surface");var cut=plain.AddHole("Rectangle",290,290);plain.UpdateCut(cut,290,290,20,20,7);if(!plain.TryMergeBoundaryCut(cut))Fail("3D notch fixture");var notched=BentSurfaceEngine.Build(plain);if(notched.Triangles.Count<=rectangle.Triangles.Count||notched.Points.Any(p=>Math.Abs(p.X-300)<1e-6&&Math.Abs(p.Y-300)<1e-6))Fail("3D notch contour");plain.AddBend(SectionAxis.W,150,BendDirection.Up);plain.AddBend(SectionAxis.H,120,BendDirection.Right);var bent=BentSurfaceEngine.Build(plain);if(bent.Points.All(p=>Math.Abs(p.Z)<1e-6)||!bent.BendEdges.Any(e=>e.Layer=="V")||!bent.BendEdges.Any(e=>e.Layer=="V1"))Fail("3D colored bend edges");}
static void SelectionHighlight(){var d=new VCuttingDocument();var w=d.AddBend(SectionAxis.W,40,BendDirection.Up);if(!FlatDesignerView.TrySelectedLine(d,w,out var wl))Fail("W selection");Eq(40,wl.X1);Eq(0,wl.Y1);Eq(d.Material.Height,wl.Y2);var h=d.AddBend(SectionAxis.H,25,BendDirection.Right);if(!FlatDesignerView.TrySelectedLine(d,h,out var hl))Fail("H selection");Eq(25,hl.Y1);Eq(d.Material.Width,hl.X2);var go=new GeometryObject{Id="L001",Geometry=new LineSegment(0,0,1,0)};if(!FlatDesignerView.TrySelectedLine(d,go,out var gl)||gl.X2!=1)Fail("L selection");}
static void DeleteObjects(){var d=new VCuttingDocument();var bend=d.AddBend(SectionAxis.W,40,BendDirection.Up);var cut=d.AddHole("Circle",20,20);var joint=new MicroJoint{Id="MJ001",Position=2,Width=.1};d.MicroJoints.Add(joint);var history=new UndoRedoManager();history.Record(d);if(!d.DeleteObject(bend)||d.Bends.Count!=0||d.WSegments.Count!=1)Fail("delete bend");if(!d.DeleteObject(cut)||d.Cuts.Count!=0||d.InnerContours.Count!=0)Fail("delete cut");if(!d.DeleteObject(joint)||d.MicroJoints.Count!=0)Fail("delete joint");var restored=history.Undo(d)??throw new InvalidOperationException();if(restored.Bends.Count!=1||restored.Cuts.Count!=1||restored.MicroJoints.Count!=1||restored.OuterContour.Segments.Count!=4)Fail("undo deleted objects");}
static void UndoRedo(){var d=new VCuttingDocument();var history=new UndoRedoManager(3);history.Record(d);d.AddBend(SectionAxis.W,50,BendDirection.Up);history.Record(d);d.ChangeThickness(.3);var undo1=history.Undo(d)??throw new InvalidOperationException();Eq(.2,undo1.Material.Thickness);if(undo1.Bends.Count!=1)Fail("undo thickness");var undo2=history.Undo(undo1)??throw new InvalidOperationException();if(undo2.Bends.Count!=0)Fail("undo bend");var redo=history.Redo(undo2)??throw new InvalidOperationException();if(redo.Bends.Count!=1)Fail("redo bend");}
static void MicroJoints(){var joints=new[]{new MicroJoint{Position=5,Width=2}};var l=MicroJointGeometryEngine.Split(new LineSegment(0,0,10,0),joints);if(l.Count!=2)Fail("line split");Eq(4,l[0].X2);Eq(6,l[1].X1);var arcs=MicroJointGeometryEngine.Split(new CircleSegment(0,0,10),new[]{new MicroJoint{Position=10,Width=2},new MicroJoint{Position=30,Width=2}});if(arcs.Count!=3)Fail("circle split");}
static void RecentFiles(){var s=new VCuttingSettings();for(var i=0;i<12;i++)UserSettingsStore.UpdateRecentList(s,Path.Combine(Path.GetTempPath(),$"recent-{i}.dxf"));if(s.RecentFiles.Count!=10||!s.RecentFiles[0].EndsWith("recent-11.dxf"))Fail("recent limit/order");var duplicate=s.RecentFiles[5];UserSettingsStore.UpdateRecentList(s,duplicate.ToUpperInvariant());if(s.RecentFiles.Count!=10||!string.Equals(s.RecentFiles[0],Path.GetFullPath(duplicate.ToUpperInvariant()),StringComparison.OrdinalIgnoreCase))Fail("recent deduplication");}
static void SettingsPersistence(){var s=new VCuttingSettings{ShowRuler=false,ShowGrid=false,ShowStatusBar=true,WDimensions=false,WBent=true,WRotation=3,HDimensions=true,HBent=true,HRotation=2,DefaultUnit=MeasurementUnit.Millimeter,NewWidth=1200,NewHeight=800,NewThickness=2,RecentFileCount=20,ZoomStepPercent=25,SelectionTolerancePixels=9,HandleRadiusPixels=8,Transparent3DDefault=true,Edge3DScale=1.5,HorizontalRulerIntervalMetres=.1,VerticalRulerIntervalMetres=.2,GridIntervalMetres=.05,RecentFiles=[Path.Combine(Path.GetTempPath(),"one.dxf"),"\0invalid"]};var loaded=UserSettingsStore.Deserialize(UserSettingsStore.Serialize(s));if(loaded.ShowRuler||loaded.ShowGrid||!loaded.ShowStatusBar||loaded.RecentFiles.Count!=1||loaded.WDimensions||!loaded.WBent||loaded.WRotation!=3||!loaded.HDimensions||!loaded.HBent||loaded.HRotation!=2||loaded.DefaultUnit!=MeasurementUnit.Millimeter||loaded.NewWidth!=1200||loaded.RecentFileCount!=20||loaded.ZoomStepPercent!=25||!loaded.Transparent3DDefault||loaded.Edge3DScale!=1.5||loaded.HorizontalRulerIntervalMetres!=.1||loaded.VerticalRulerIntervalMetres!=.2||loaded.GridIntervalMetres!=.05)Fail("settings persistence/invalid path cleanup");var corrupt=UserSettingsStore.Deserialize("{not json");if(!corrupt.ShowRuler||!corrupt.ShowGrid||!corrupt.ShowStatusBar||corrupt.RecentFiles.Count!=0||!corrupt.WDimensions||!corrupt.HDimensions)Fail("corrupt settings fallback");}
static void ViewportCoordinates(){var pan=new System.Windows.Vector(17,-9);var t=FlatViewportEngine.Calculate(900,700,300,200,true,1.75,pan);var design=new System.Windows.Point(123.45,67.89);var screen=t.ToScreen(design);var roundTrip=t.ToDesign(screen);Eq(design.X,roundTrip.X);Eq(design.Y,roundTrip.Y);var zoomed=FlatViewportEngine.Calculate(900,700,300,200,true,2.5,pan);var correction=FlatViewportEngine.PanKeepingAnchor(t,zoomed,screen);var corrected=zoomed with{Origin=zoomed.Origin+correction};var anchored=corrected.ToScreen(design);Eq(screen.X,anchored.X);Eq(screen.Y,anchored.Y);if(FlatViewportEngine.GridStep(t.Scale)<=0)Fail("viewport grid step");Eq(10,FlatViewportEngine.DisplayStep(10,10,8));Eq(500,FlatViewportEngine.DisplayStep(.1,10,45));Eq(FlatViewportEngine.GridStep(2),FlatViewportEngine.DisplayStep(2,0,45));}
static void SectionViewportCoordinates(){var state=new SectionViewportState(1,new(12,-4));var anchor=new System.Windows.Point(240,170);var before=SectionViewportEngine.Transform(640,420,state);var inverse=before;inverse.Invert();var design=inverse.Transform(anchor);var zoomed=SectionViewportEngine.ZoomAt(640,420,state,anchor,1.5);var after=SectionViewportEngine.Transform(640,420,zoomed).Transform(design);Eq(anchor.X,after.X);Eq(anchor.Y,after.Y);var centered=SectionViewportEngine.CenteredZoom(state,1.5);Eq(1.5,centered.Zoom);Eq(0,centered.Pan.X);Eq(0,centered.Pan.Y);Eq(.92,SectionViewportEngine.BentFitScale(765,210,150,150));Eq(1.5,SectionViewportEngine.BentFitScale(765,500,150,150));if(!SectionViewportEngine.IsInsideFlatMaterial(SectionAxis.W,new(200,210),640,420)||SectionViewportEngine.IsInsideFlatMaterial(SectionAxis.W,new(200,100),640,420))Fail("W section hit bounds");if(!SectionViewportEngine.IsInsideFlatMaterial(SectionAxis.H,new(320,200),640,420)||SectionViewportEngine.IsInsideFlatMaterial(SectionAxis.H,new(100,200),640,420))Fail("H section hit bounds");}
static void SectionSelection(){Eq(0,SectionSelectionEngine.Clamp(-3,100));Eq(45,SectionSelectionEngine.Clamp(45,100));Eq(100,SectionSelectionEngine.Clamp(130,100));}
static void RoundTrip(){var path=Path.Combine(Path.GetTempPath(),$"cosmic-{Guid.NewGuid():N}.dxf");try{var d=new VCuttingDocument();d.AddBend(SectionAxis.W,12,BendDirection.Up);d.AddBend(SectionAxis.W,42,BendDirection.Down);d.UpdateSegment(d.WSegments[0],10);d.AddHole("Circle",30,40);d.AddHole("Hexagon",50,60);d.MicroJoints.Add(new(){Id="MJ001",ParentContourId="OUTER",Position=1.2,Width=.1,Sequence=5});d.WView.Dimensions=false;d.WView.BentMode=true;d.WView.RotationQuarterTurns=3;d.HView.Dimensions=true;d.HView.BentMode=true;d.HView.RotationQuarterTurns=2;d.ChangeThickness(.3);VCuttingDxfSerializer.Save(d,path);var text=File.ReadAllText(path,Encoding.ASCII);if(!text.Contains("\r\nL\r\n")||!text.Contains("\r\nV\r\n")||!text.Contains("CIRCLE")||!text.Contains("COSMIC_DESIGNER_JSON:")||!text.Contains("\r\n5\r\n"))Fail("DXF layers/metadata/units");var r=VCuttingDxfSerializer.Load(path);Eq(.3,r.Material.Thickness);if(r.Bends.Count!=2||r.Cuts.Count!=2||r.InnerContours.Count!=2||r.MicroJoints.Count!=1||r.WSegments.Count!=3)Fail("metadata roundtrip");Eq(0,r.WSegments[0].Index);Eq(1,r.WSegments[1].Index);Eq(10,r.WSegments[0].Length);if(r.Cuts[1].Shape!="Hexagon"||r.Cuts[1].Geometry.Count!=6)Fail("hole roundtrip");if(r.WView.Dimensions||!r.WView.BentMode||r.WView.RotationQuarterTurns!=3||!r.HView.Dimensions||!r.HView.BentMode||r.HView.RotationQuarterTurns!=2)Fail("section view state roundtrip");}finally{if(File.Exists(path))File.Delete(path);}}
static void GeneralDxfImport(){var path=Path.Combine(Path.GetTempPath(),$"import-{Guid.NewGuid():N}.dxf");var saved=path+".saved.dxf";try{var pairs=new[]{(0,"SECTION"),(2,"ENTITIES"),(0,"LINE"),(8,"-L-"),(10,"10"),(20,"20"),(11,"110"),(21,"20"),(0,"LINE"),(8,"L"),(10,"110"),(20,"20"),(11,"110"),(21,"70"),(0,"LINE"),(8,"L"),(10,"110"),(20,"70"),(11,"10"),(21,"70"),(0,"LINE"),(8,"L"),(10,"10"),(20,"70"),(11,"10"),(21,"20"),(0,"CIRCLE"),(8,"L"),(10,"40"),(20,"40"),(40,"5"),(0,"LINE"),(8,"V"),(10,"35"),(20,"20"),(11,"35"),(21,"70"),(0,"LINE"),(8,"-V1-"),(10,"10"),(20,"55"),(11,"110"),(21,"55"),(0,"ENDSEC"),(0,"EOF")};File.WriteAllText(path,string.Join("\r\n",pairs.SelectMany(x=>new[]{x.Item1.ToString(),x.Item2}))+"\r\n",Encoding.ASCII);if(VCuttingDxfSerializer.HasMetadata(path))Fail("plain DXF metadata detection");var result=GeneralDxfImportEngine.Import(path);Eq(10,result.Document.Material.Width);Eq(5,result.Document.Material.Height);Eq(.2,result.Document.Material.Thickness);if(result.Document.OuterContour.Segments.Count!=4||result.Document.Cuts.Count!=1||result.Document.Bends.Count!=2)Fail("general DXF import counts");if(result.Document.Bends.Count(x=>x.Axis==SectionAxis.W)!=1||result.Document.Bends.Count(x=>x.Axis==SectionAxis.H)!=1)Fail("general DXF bend axes");VCuttingDxfSerializer.Save(result.Document,saved);var loaded=VCuttingDxfSerializer.Load(saved);if(loaded.Cuts.Single().Geometry.Single() is not CircleSegment||loaded.OuterContour.Segments.Count!=4)Fail("imported geometry metadata roundtrip");Eq(10,loaded.Material.Width);}finally{if(File.Exists(path))File.Delete(path);if(File.Exists(saved))File.Delete(saved);}}
static void FlatMaterialMask(){var d=new VCuttingDocument();d.AddHole("Circle",150,150);var transform=new FlatViewportTransform(1,new(0,300));var mask=FlatMaterialMaskEngine.Build(d,transform);if(!mask.FillContains(transform.ToScreen(new(10,10)))||mask.FillContains(transform.ToScreen(new(150,150)))||mask.FillContains(transform.ToScreen(new(-1,10))))Fail("flat material cut mask");var corner=d.AddHole("Rectangle",290,290);d.UpdateCut(corner,290,290,20,20,7);if(!d.TryMergeBoundaryCut(corner))Fail("flat mask notch fixture");mask=FlatMaterialMaskEngine.Build(d,transform);if(mask.FillContains(transform.ToScreen(new(295,295))))Fail("flat material outer notch mask");}
static void ExactCutSurface()
{
    double Area(SurfaceMeshData mesh)
    {
        var area=0d;for(var i=0;i<mesh.Triangles.Count;i+=3)
        {
            var a=mesh.Points[mesh.Triangles[i]];var b=mesh.Points[mesh.Triangles[i+1]];var c=mesh.Points[mesh.Triangles[i+2]];
            area+=Math.Abs((b.X-a.X)*(c.Y-a.Y)-(b.Y-a.Y)*(c.X-a.X))/2;
        }
        return area;
    }
    foreach(var shape in new[]{"Circle","Triangle","Rectangle","Diamond","Parallelogram"})
    {
        var d=new VCuttingDocument();var cut=d.AddHole(shape,137,143);d.UpdateCut(cut,137,143,73,shape=="Circle"?73:91,7);
        var mesh=BentSurfaceEngine.Build(d);
        var cutArea=cut.Geometry.SingleOrDefault(g=>g is CircleSegment) is CircleSegment circle?
            12*circle.Radius*circle.Radius*Math.Sin(Math.PI/12):
            Math.Abs(cut.Geometry.OfType<LineSegment>().Sum(l=>l.X1*l.Y2-l.X2*l.Y1))/2;
        Eq(90000-cutArea,Area(mesh));
        // Every displayed boundary midpoint must lie on an actual material triangle edge.
        foreach(var (aIndex,bIndex) in mesh.Edges)
        {
            var a=mesh.Points[aIndex];var b=mesh.Points[bIndex];var mx=(a.X+b.X)/2;var my=(a.Y+b.Y)/2;
            var found=false;
            for(var i=0;i<mesh.Triangles.Count&&!found;i+=3)for(var j=0;j<3;j++)
            {
                var p=mesh.Points[mesh.Triangles[i+j]];var q=mesh.Points[mesh.Triangles[i+(j+1)%3]];
                var cross=(q.X-p.X)*(my-p.Y)-(q.Y-p.Y)*(mx-p.X);
                if(Math.Abs(cross)<1e-6&&mx>=Math.Min(p.X,q.X)-1e-6&&mx<=Math.Max(p.X,q.X)+1e-6&&my>=Math.Min(p.Y,q.Y)-1e-6&&my<=Math.Max(p.Y,q.Y)+1e-6)found=true;
            }
            if(!found)Fail(shape+" surface/outline gap");
        }
        d.AddBend(SectionAxis.W,137,BendDirection.Up);d.AddBend(SectionAxis.H,143,BendDirection.Right);
        var bent=BentSurfaceEngine.Build(d);if(bent.Points.All(p=>Math.Abs(p.Z)<1e-6)||bent.Triangles.Count==0)Fail(shape+" clipped folded surface");
    }
    var overlap=new VCuttingDocument();
    var geometry=new System.Windows.Media.RectangleGeometry(new System.Windows.Rect(0,0,300,300)) as System.Windows.Media.Geometry;
    foreach(var shape in new[]{"Diamond","Parallelogram"})
    {
        var cut=overlap.AddHole(shape,shape=="Diamond"?140:160,150);overlap.UpdateCut(cut,cut.CenterX,150,100,80,7);
        var lines=cut.Geometry.Cast<LineSegment>().ToList();var figure=new System.Windows.Media.PathFigure{StartPoint=new(lines[0].X1,lines[0].Y1),IsClosed=true};
        figure.Segments.Add(new System.Windows.Media.PolyLineSegment(lines.Select(l=>new System.Windows.Point(l.X2,l.Y2)),true));
        geometry=System.Windows.Media.Geometry.Combine(geometry,new System.Windows.Media.PathGeometry(new[]{figure}),System.Windows.Media.GeometryCombineMode.Exclude,System.Windows.Media.Transform.Identity,1e-9,System.Windows.Media.ToleranceType.Absolute);
    }
    // WPF Boolean geometry quantizes coordinates; allow its small area quantization error.
    if(Math.Abs(geometry.GetArea(1e-9,System.Windows.Media.ToleranceType.Absolute)-Area(BentSurfaceEngine.Build(overlap)))>.001)Fail("overlapping cut union area");
    var diagonal=new VCuttingDocument();var triangle=diagonal.AddHole("Triangle",20,275);diagonal.UpdateTriangleVertices(triangle,[new(0,250),new(0,300),new(40,300)]);
    if(!diagonal.TryMergeBoundaryCut(triangle))Fail("diagonal outer fixture");
    var outerArea=Math.Abs(diagonal.OuterContour.Segments.Cast<LineSegment>().Sum(l=>l.X1*l.Y2-l.X2*l.Y1))/2;
    Eq(outerArea,Area(BentSurfaceEngine.Build(diagonal)));
}
static void CutSurface(){var d=new VCuttingDocument();var cut=d.AddHole("Circle",150,150);d.UpdateCut(cut,150,150,40,40,7);var mesh=BentSurfaceEngine.Build(d);if(mesh.Triangles.Count==0||mesh.Edges.Count!=28)Fail("3D cut boundary edges");for(var i=0;i<mesh.Triangles.Count;i+=3){var a=mesh.Points[mesh.Triangles[i]];var b=mesh.Points[mesh.Triangles[i+1]];var c=mesh.Points[mesh.Triangles[i+2]];var x=(a.X+b.X+c.X)/3;var y=(a.Y+b.Y+c.Y)/3;if((x-150)*(x-150)+(y-150)*(y-150)<19*19)Fail("3D cut face remained");}}
static void Eq(double a,double b){if(Math.Abs(a-b)>1e-6)Fail($"{a}!={b}");}static void Fail(string s)=>throw new InvalidOperationException(s);
