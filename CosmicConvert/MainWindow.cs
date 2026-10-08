using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
namespace CosmicConvert;
public sealed class MainWindow : Window
{
    readonly DrawingPreview preview=new();readonly TextBlock status=new(){Margin=new Thickness(8),TextWrapping=TextWrapping.Wrap};
    readonly ProgressBar progress=new(){Height=5,Visibility=Visibility.Collapsed,IsIndeterminate=true};
    readonly MenuItem open=new(){Header="Open"},save=new(){Header="Save",IsEnabled=false},exit=new(){Header="Exit"};
    SvgDrawing? drawing;string? input;bool busy;CancellationTokenSource? cancellation;
    public MainWindow()
    {
        Title="CosmicConvert — "+VCuttingRelease.VersionInfo.Display+" — "+VCuttingRelease.VersionInfo.Build;Width=800;Height=650;
        var panel=new DockPanel();var menu=new Menu();menu.Items.Add(open);menu.Items.Add(save);menu.Items.Add(exit);DockPanel.SetDock(menu,Dock.Top);panel.Children.Add(menu);
        var bottom=new StackPanel();bottom.Children.Add(progress);bottom.Children.Add(status);DockPanel.SetDock(bottom,Dock.Bottom);panel.Children.Add(bottom);panel.Children.Add(preview);Content=panel;
        status.Text="Open an SVG. Save converts to DXF (mm, tolerance 0.05 mm).";open.Click+=Open;save.Click+=Save;exit.Click+=(_,_)=>Close();Closing+=OnClosing;
    }
    async void Open(object sender,RoutedEventArgs args)
    {
        var picker=new OpenFileDialog{Filter="SVG files|*.svg"};if(picker.ShowDialog(this)!=true)return;SetBusy(true);
        try{var loaded=await Task.Run(()=>SvgReader.Read(picker.FileName));drawing=loaded;input=picker.FileName;preview.Drawing=loaded;preview.InvalidateVisual();status.Text=$"{input}\nViewport {loaded.Width:0.###} × {loaded.Height:0.###} mm; {loaded.Outlines.Count} paths. 96 px/in; Y converted to bottom-left coordinates.";}
        catch(Exception ex){status.Text=ex.Message;MessageBox.Show(this,ex.Message,"Open failed");}finally{SetBusy(false);}
    }
    async void Save(object sender,RoutedEventArgs args)
    {
        if(drawing is null||input is null)return;var picker=new SaveFileDialog{Filter="DXF files|*.dxf",DefaultExt=".dxf",AddExtension=true,FileName=Path.GetFileNameWithoutExtension(input)+".dxf",InitialDirectory=Path.GetDirectoryName(input),OverwritePrompt=true};if(picker.ShowDialog(this)!=true)return;
        cancellation=new();SetBusy(true);var report=new Progress<string>(s=>status.Text=s);
        try
        {
            var source=drawing;var result=await Task.Run(()=>{var converted=Optimizer.Convert(source,cancellation.Token,report);DxfWriter.Save(converted,picker.FileName,cancellation.Token,report);return converted;});
            var layout=ExportLayout.Create(result);status.Text=$"Saved {picker.FileName}\n"+string.Join(", ",layout.Entities.GroupBy(e=>e.Kind).Select(g=>$"{g.Key}: {g.Count()}"))+$"; 10 mm margin, frame {layout.Width:0.###} × {layout.Height:0.###} mm; conservative curve error bound ≤ {result.ErrorBound:G4} mm.";
        }
        catch(OperationCanceledException){status.Text="Cancelled; existing output preserved.";}catch(Exception ex){status.Text=ex.Message;MessageBox.Show(this,ex.Message,"Conversion failed");}finally{cancellation.Dispose();cancellation=null;SetBusy(false);}
    }
    void SetBusy(bool value){busy=value;open.IsEnabled=!value;save.IsEnabled=!value&&drawing is not null;progress.Visibility=value?Visibility.Visible:Visibility.Collapsed;}
    void OnClosing(object? sender,CancelEventArgs e){if(busy){e.Cancel=true;cancellation?.Cancel();status.Text="Finishing/cancelling safely. Close again when processing stops.";}}
}
sealed class DrawingPreview : FrameworkElement
{
    public SvgDrawing? Drawing{get;set;}
    protected override void OnRender(DrawingContext dc)
    {
        dc.DrawRectangle(Brushes.White,null,new Rect(RenderSize));if(Drawing is not{} drawing)return;
        var path=new PathGeometry();foreach(var outline in drawing.Outlines)
        {
            if(outline.Curves.Count==0)continue;var f=new PathFigure{StartPoint=outline.Curves[0].At(0),IsClosed=outline.Closed,IsFilled=false};
            foreach(var c in outline.Curves)
            {
                if(c.Bezier is {Length:2} p)f.Segments.Add(new LineSegment(p[1],true));
                else if(c.Bezier is{} b)f.Segments.Add(new BezierSegment(b[1],b[2],b[3],true));
                else{int n=Math.Clamp((int)Math.Ceiling(Math.Abs(c.Sweep)*100),2,1000);f.Segments.Add(new PolyLineSegment(Enumerable.Range(1,n).Select(i=>c.At(i/(double)n)),true));}
            }path.Figures.Add(f);
        }
        var bounds=path.Bounds;if(bounds.IsEmpty)return;double scale=Math.Min((ActualWidth-30)/Math.Max(bounds.Width,1e-6),(ActualHeight-30)/Math.Max(bounds.Height,1e-6));if(scale<=0)return;
        var matrix=new Matrix(scale,0,0,-scale,(ActualWidth-bounds.Width*scale)/2-bounds.X*scale,(ActualHeight-bounds.Height*scale)/2+bounds.Bottom*scale);
        path.Transform=new MatrixTransform(matrix);dc.DrawGeometry(null,new Pen(Brushes.SteelBlue,1.5),path);
    }
}
