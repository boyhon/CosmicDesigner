using Microsoft.Win32;
using System.Windows;

namespace VCutting;
public partial class MainWindow
{
    void ImportSvg_Click(object sender,RoutedEventArgs e)
    {
        var picker=new OpenFileDialog{Filter=Localization.Text("svg.file")+"|*.svg",InitialDirectory=ValidFolder(_settings.DefaultOpenFolder)};
        if(picker.ShowDialog()!=true)return;
        SvgImportResult result;
        try{result=SvgImportEngine.Import(picker.FileName,_settings.DefaultUnit,_settings.NewThickness*_settings.DefaultUnit.Metres()*1000);}
        catch(Exception ex){MessageBox.Show(this,ex.Message,Localization.Text("svg.title"),MessageBoxButton.OK,MessageBoxImage.Error);return;}
        var message=Localization.Format("svg.confirm",result.Document.Material.Width,result.Document.Material.Height,result.Document.Unit.Symbol(),result.ClosedObjects,result.OpenObjects);
        if(result.Warnings.Count>0)message+=Environment.NewLine+Environment.NewLine+string.Join(Environment.NewLine,result.Warnings);
        if(MessageBox.Show(this,message,Localization.Text("svg.title"),MessageBoxButton.OKCancel,MessageBoxImage.Information)!=MessageBoxResult.OK||!ConfirmSaveChanges())return;
        _history.Clear();ApplySectionDefaults(result.Document);Attach(result.Document);_path=null;_titlePath=picker.FileName;
        _dirtyState.MarkChanged();UpdateWindowTitle();StatusText.Text=Localization.Text("svg.imported");
    }
}
