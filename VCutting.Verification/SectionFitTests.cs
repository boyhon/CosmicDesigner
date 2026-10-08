using System.Reflection;
using System.Windows;
using VCutting;

static class SectionFitTests
{
    public static void Run()
    {
        Exception? failure=null;
        var thread=new Thread(()=>{try{Verify();}catch(Exception e){failure=e;}});
        thread.SetApartmentState(ApartmentState.STA);thread.Start();thread.Join();
        if(failure is not null)throw failure;
    }
    static void Verify()
    {
        var zoom=typeof(SectionDesignerView).GetField("_viewZoom",BindingFlags.Instance|BindingFlags.NonPublic)!;
        var pan=typeof(SectionDesignerView).GetField("_viewPan",BindingFlags.Instance|BindingFlags.NonPublic)!;
        foreach(var axis in new[]{SectionAxis.H,SectionAxis.W})
        foreach(var bent in new[]{false,true})
        foreach(var rotation in Enumerable.Range(0,4))
        {
            var doc=new VCuttingDocument();
            var view=new SectionDesignerView{Document=doc,Axis=axis,BentMode=bent,SectionPosition=100};
            view.Measure(new Size(765,420));view.Arrange(new Rect(0,0,765,420));view.SetRotation(rotation);
            var baseline=view.Children.Cast<UIElement>().Select(e=>new Point(System.Windows.Controls.Canvas.GetLeft(e),System.Windows.Controls.Canvas.GetTop(e))).ToArray();
            zoom.SetValue(view,5d);pan.SetValue(view,new Vector(400,-300));view.Refresh();
            view.FitToWindow();view.FitToWindow();
            if((double)zoom.GetValue(view)! !=1||(Vector)pan.GetValue(view)! !=new Vector())throw new Exception("Fit state");
            var actual=view.Children.Cast<UIElement>().Select(e=>new Point(System.Windows.Controls.Canvas.GetLeft(e),System.Windows.Controls.Canvas.GetTop(e))).ToArray();
            if(!baseline.SequenceEqual(actual))throw new Exception("Fit dimension editor alignment");
            if(view.ViewRotationQuarterTurns!=rotation||view.BentMode!=bent||view.SectionPosition!=100||!ReferenceEquals(doc,view.Document))throw new Exception("Fit changed design/options");
        }
        new SectionDesignerView().FitToWindow();
    }
}
