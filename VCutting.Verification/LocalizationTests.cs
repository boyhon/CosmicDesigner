using VCutting;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Globalization;

internal static class LocalizationTests
{
    static void Check(bool value,string reason){if(!value)throw new InvalidOperationException(reason);}
    public static void Languages()
    {
        var root=new DirectoryInfo(AppContext.BaseDirectory);while(root is not null&&!File.Exists(Path.Combine(root.FullName,"VCutting.sln")))root=root.Parent;
        var folder=Path.Combine(root!.FullName,"artifacts","language-tests-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(folder);
        var bundled=Path.Combine(AppContext.BaseDirectory,"Languages");var culture=CultureInfo.CurrentCulture;
        void Pack(string code,object strings)=>File.WriteAllText(Path.Combine(folder,code+".json"),JsonSerializer.Serialize(new{code,name=code,strings}),new UTF8Encoding(false));
        try
        {
            Localization.Initialize("en",bundled,folder);Check(Localization.Text("ui.0197")=="_File","English menu retained");
            Localization.SetLanguage("ko");Check(Localization.Text("ui.0197")=="파일(_F)","Korean menu");Check(Localization.Format("ui.0018","B001","V")=="V 레이어에 B001 생성","dynamic arguments/order");
            Check(Localization.Kind("Circle")=="원","kind display");Check(Localization.Kind("L001")=="L001","identifiers unchanged");
            Check(Localization.Kind("Polygon")=="다각형"&&Localization.Kind("Hexagon")=="육각형"&&Localization.Kind("Ellipse")=="타원","legacy supported shape names localized");
            var state=new VCuttingSettings{DisplayLanguage="ko"};Check(UserSettingsStore.Deserialize(UserSettingsStore.Serialize(state)).DisplayLanguage=="ko","language persistence");Check(UserSettingsStore.Deserialize("{}").DisplayLanguage=="en","legacy language English");
            var count=0;void Changed()=>count++;Localization.Changed+=Changed;Localization.SetLanguage("en");Localization.SetLanguage("ko");Localization.Changed-=Changed;Check(count==2,"live change notifications");
            Exception? uiError=null;
            var uiThread=new System.Threading.Thread(()=>{try{
                Localization.SetLanguage("en");var label=new System.Windows.Controls.TextBlock{Text=Localization.Text("ui.0237")};
                var input=new System.Windows.Controls.TextBox{Text="3000",ToolTip=Localization.Text("ui.0020")};
                var unitLabel=new System.Windows.Controls.TextBlock();System.Windows.Data.BindingOperations.SetBinding(unitLabel,System.Windows.Controls.TextBlock.TextProperty,new System.Windows.Data.Binding{Source=MeasurementUnit.Millimeter,Converter=new UnitLabelConverter()});
                var panel=new System.Windows.Controls.StackPanel();panel.Children.Add(label);panel.Children.Add(input);panel.Children.Add(unitLabel);
                var window=new System.Windows.Window{Title=Localization.Format("window.title","",Localization.DisplayText("window.untitled")),Content=panel};
                LocalizationUi.RefreshElement(window);Localization.SetLanguage("ko");LocalizationUi.RefreshElement(window);
                Check(label.Text=="준비"&&input.Text=="3000"&&input.ToolTip.ToString()=="재료","live labels/tooltip keep numeric input");Check(window.Title=="제목 없음 - CosmicDesigner","live dynamic window title");Check(unitLabel.Text=="밀리미터 (mm)","unit label binding updated without selection reset");
                Localization.SetLanguage("en");LocalizationUi.RefreshElement(window);Check(label.Text=="Ready"&&window.Title=="Untitled - CosmicDesigner","live English restoration");window.Close();
            }catch(Exception ex){uiError=ex;}});uiThread.SetApartmentState(System.Threading.ApartmentState.STA);uiThread.Start();uiThread.Join();if(uiError is not null)throw uiError;
            Pack("ko",new Dictionary<string,string>{{"ui.0197","내 파일(_F)"},{"ui.0018","bad {5}"},{"ui.0237","깨진 {"},{"unknown.key","unknown"}});
            Localization.SetLanguage("ko");Check(Localization.Text("ui.0197")=="내 파일(_F)","user override");Check(Localization.Format("ui.0018","B001","V")=="V 레이어에 B001 생성","invalid slots fall back");Check(Localization.Text("ui.0237")=="준비","invalid format fallback");Check(Localization.Diagnostics.Count>0,"diagnostics retained");
            File.WriteAllText(Path.Combine(folder,"ko.json"),"{\"code\":\"ko\",\"name\":\"한국어\",\"strings\":{\"ui.0197\":\"a\",\"ui.0197\":\"b\"}}");Localization.SetLanguage("ko");Check(Localization.Text("ui.0197")=="파일(_F)","duplicate keys rejected");
            File.WriteAllBytes(Path.Combine(folder,"ko.json"),[0xff,0xfe,0xff]);Localization.SetLanguage("ko");Check(Localization.Text("ui.0197")=="파일(_F)","invalid UTF8 safe");
            File.WriteAllText(Path.Combine(folder,"ko.json"),"{broken");Localization.SetLanguage("ko");Check(Localization.Text("ui.0197")=="파일(_F)","bad JSON safe");File.Delete(Path.Combine(folder,"ko.json"));Localization.SetLanguage("ko");Check(Localization.Text("ui.0197")=="파일(_F)","override removal restores default");
            Pack("ko",new Dictionary<string,string>{{"file.dxf","my|files"},{"ui.0197","\0"}});Localization.SetLanguage("ko");Check(Localization.DxfFilter.Split('|').Length==2&&Localization.DxfFilter.EndsWith("|*.dxf"),"translated filter keeps invariant DXF pattern");Check(Localization.Text("ui.0197")=="파일(_F)","NUL text rejected safely");File.Delete(Path.Combine(folder,"ko.json"));
            Pack("fr",new Dictionary<string,string>{{"ui.0197","Fichier(_F)"}});Localization.Initialize("fr",bundled,folder);Check(Localization.Languages.Any(x=>x.Code=="fr"),"new language discovery");Check(Localization.Text("ui.0197")=="Fichier(_F)","new language lookup");Check(Localization.Text("ui.0237")=="Ready","missing translation English fallback");
            Pack("en",new Dictionary<string,string>{{"ui.0237","Prepared"}});Localization.SetLanguage("fr");Check(Localization.Text("ui.0237")=="Prepared","user English fallback layer");File.Delete(Path.Combine(folder,"en.json"));
            Localization.SetLanguage("../escape");Check(Localization.Language=="en","invalid language path blocked");
            foreach(var name in new[]{"en","ko"})
            {
                var path=Path.Combine(bundled,name+".json");Check(File.Exists(path),"catalog deployed");Check(File.ReadAllBytes(path).SequenceEqual(File.ReadAllBytes(Path.Combine(root.FullName,"VCutting","Languages",name+".json"))),"catalog source/output match");
                using var doc=JsonDocument.Parse(File.ReadAllText(path));var keys=doc.RootElement.GetProperty("strings").EnumerateObject().Select(x=>x.Name).Order().ToArray();using var english=JsonDocument.Parse(File.ReadAllText(Path.Combine(bundled,"en.json")));Check(keys.SequenceEqual(english.RootElement.GetProperty("strings").EnumerateObject().Select(x=>x.Name).Order()),"catalog key parity");
            }
            Localization.Initialize("ko",bundled,folder);var first=new VCuttingDocument();first.AddHole("Circle",100,100);var pathDxf=Path.Combine(folder,"before.dxf");VCuttingDxfSerializer.Save(first,pathDxf);var bytes=File.ReadAllBytes(pathDxf);Localization.SetLanguage("en");VCuttingDxfSerializer.Save(first,pathDxf);Check(bytes.SequenceEqual(File.ReadAllBytes(pathDxf)),"language cannot alter DXF");Check(CultureInfo.CurrentCulture.Equals(culture),"numeric culture unchanged");
            Localization.Initialize("ko",Path.Combine(folder,"missing"),folder);Check(Localization.Text("ui.0197")=="_File","missing bundled files recover embedded English");
        }
        finally{Localization.Initialize("en");Directory.Delete(folder,true);}
    }
    public static void Millimeters()
    {
        var s=new VCuttingSettings();Check(s.DefaultUnit==MeasurementUnit.Millimeter&&s.NewWidth==3000&&s.NewHeight==3000&&s.NewThickness==2,"fresh mm defaults");
        var document=new VCuttingDocument();document.ConfigureNew(s.NewWidth,s.NewHeight,s.NewThickness,s.DefaultUnit);Check(document.Unit==MeasurementUnit.Millimeter&&document.Material.Width==3000&&Math.Abs(document.Material.Width*document.Unit.Metres()-3)<1e-10,"same physical size");
        foreach(var legacy in new[]{"{}","{\"DefaultUnit\":1}"}){var restored=UserSettingsStore.Deserialize(legacy);Check(restored.DefaultUnit==MeasurementUnit.Centimeter&&restored.NewWidth==300&&restored.NewHeight==300&&restored.NewThickness==.2,"legacy absent dimensions/unit retain cm meaning");}
        var custom=UserSettingsStore.Deserialize("{\"DefaultUnit\":2,\"NewWidth\":5,\"NewHeight\":4,\"NewThickness\":0.003}");Check(custom.DefaultUnit==MeasurementUnit.Meter&&custom.NewWidth==5&&custom.NewThickness==.003,"explicit stored values untouched");
        var legacyCustom=UserSettingsStore.Deserialize("{\"NewWidth\":250,\"NewHeight\":200,\"NewThickness\":0.3}");Check(legacyCustom.DefaultUnit==MeasurementUnit.Centimeter&&legacyCustom.NewWidth==250&&legacyCustom.NewThickness==.3,"partial legacy settings preserve values");
        Check(UserSettingsStore.Deserialize("broken").DefaultUnit==MeasurementUnit.Millimeter,"corrupt file fresh defaults");
        var existing=new VCuttingDocument();Check(existing.Unit==MeasurementUnit.Centimeter&&existing.Material.Width==300,"legacy model defaults unchanged");existing.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);Check(existing.Material.Width==3000&&existing.Material.Thickness==2,"no double conversion");existing.ChangeUnit(MeasurementUnit.Centimeter,UnitChangeMode.PreservePhysicalSize);Check(Math.Abs(existing.Material.Width-300)<1e-9,"conversion roundtrip");
        var loaded=UserSettingsStore.Deserialize(UserSettingsStore.Serialize(s));Check(loaded.DefaultUnit==s.DefaultUnit&&loaded.NewWidth==s.NewWidth,"new settings roundtrip");
    }
}
