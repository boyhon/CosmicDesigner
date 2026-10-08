using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace VCutting;
public readonly record struct FlatBendPlacement(SectionAxis Axis,double Position,BendDirection Direction);
public static class FlatBendDrawingEngine
{
    public static bool TryCalculate(string layer,Point start,Point end,double width,double height,out FlatBendPlacement placement)
    {
        placement=default;
        if(layer is not ("V" or "V1")||!double.IsFinite(start.X)||!double.IsFinite(start.Y)||!double.IsFinite(end.X)||!double.IsFinite(end.Y)||start.X<0||start.Y<0||start.X>width||start.Y>height)return false;
        var dx=end.X-start.X;var dy=end.Y-start.Y;
        if(Math.Max(Math.Abs(dx),Math.Abs(dy))<.01)return false;
        if(Math.Abs(dx)>=Math.Abs(dy))
        {
            if(dx<=0||start.Y<=0||start.Y>=height)return false;
            placement=new(SectionAxis.H,start.Y,layer=="V"?BendDirection.Left:BendDirection.Right);
        }
        else
        {
            if(dy>=0||start.X<=0||start.X>=width)return false;
            placement=new(SectionAxis.W,start.X,layer=="V"?BendDirection.Up:BendDirection.Down);
        }
        return true;
    }
    public static double MovePosition(SectionAxis axis,double original,Point start,Point current,double width,double height)
            {
        var extent=axis==SectionAxis.H?height:width;var margin=Math.Min(.01,extent/2);
        return Math.Clamp(original+(axis==SectionAxis.H?current.Y-start.Y:current.X-start.X),margin,extent-margin);
    }
}
public sealed partial class FlatDesignerView
{
    public string? ActiveBendLayer {get;private set;}
    bool _bendCreating;BendObject? _movingBend;Point _bendStart,_bendCurrent,_bendStartScreen;double _bendOriginal;bool _bendRecorded;
    public void BeginBendDrawing(string layer){CancelBendDrawing();CancelSlitDrawing();ActiveHoleShape=null;FilletMode=false;ActiveBendLayer=layer;Cursor=Cursors.Cross;Focus();InvalidateVisual();}
    public void CancelBendDrawing(){ActiveBendLayer=null;ResetBendDrag();if(IsMouseCaptured)ReleaseMouseCapture();InvalidateVisual();}
    void ResetBendDrag(){_bendCreating=false;_movingBend=null;_bendRecorded=false;}
    public bool HandleBendKey(Key key){if(key!=Key.Escape||ActiveBendLayer is null&&_movingBend is null)return false;CancelBendDrawing();HoleToolCancelled?.Invoke(this,EventArgs.Empty);return true;}
    bool HandleBendDown(Point screen,Point design,MouseButtonEventArgs e)
    {
        if(Document is null)return false;
        if(ActiveBendLayer is not null)
        {
            e.Handled=true;if(!Inside(design)||ShowRuler&&(screen.X<39||screen.Y<25))return true;
            _bendCreating=true;_bendStart=_bendCurrent=design;_bendStartScreen=screen;CaptureMouse();return true;
        }
        if(ActiveHoleShape is not null||ActiveSlitShape is not null||FilletMode||!Inside(design))return false;
        foreach(var bend in Document.Bends.Reverse())
        {
            var a=bend.Axis==SectionAxis.H?ToScreen(0,bend.Position):ToScreen(bend.Position,0);
            var b=bend.Axis==SectionAxis.H?ToScreen(Document.Material.Width,bend.Position):ToScreen(bend.Position,Document.Material.Height);
            if(DistanceToSegment(screen,a,b)>SelectionTolerance)continue;
            _movingBend=bend;_bendStart=design;_bendOriginal=bend.Position;_bendRecorded=false;
            SelectedObject=bend;ObjectSelected?.Invoke(this,bend);CaptureMouse();e.Handled=true;return true;
        }
        return false;
    }
    bool HandleBendMove(Point screen,Point design,MouseEventArgs e)
    {
        if(Document is null)return false;
        if(_bendCreating){_bendCurrent=design;InvalidateVisual();return true;}
        if(_movingBend is {} bend&&e.LeftButton==MouseButtonState.Pressed)
        {
            var position=FlatBendDrawingEngine.MovePosition(bend.Axis,_bendOriginal,_bendStart,design,Document.Material.Width,Document.Material.Height);
            if(Math.Abs(position-bend.Position)>1e-7)
            {
                if(!_bendRecorded){BeforeEdit?.Invoke(this,EventArgs.Empty);_bendRecorded=true;}
                Document.UpdateBend(bend,position,bend.Direction,bend.Sequence);ObjectSelected?.Invoke(this,bend);
            }
            Cursor=bend.Axis==SectionAxis.H?Cursors.SizeNS:Cursors.SizeWE;InvalidateVisual();return true;
        }
        if(ActiveBendLayer is not null){Cursor=Cursors.Cross;return true;}
        return false;
    }
    bool HandleBendUp(Point screen,MouseButtonEventArgs e)
    {
        if(!_bendCreating&&_movingBend is null)return false;
        var creating=_bendCreating;var layer=ActiveBendLayer;var start=_bendStart;var dragged=(screen-_bendStartScreen).Length>=4;
        ResetBendDrag();if(IsMouseCaptured)ReleaseMouseCapture();e.Handled=true;
        if(creating&&dragged&&Document is not null&&layer is not null&&FlatBendDrawingEngine.TryCalculate(layer,start,ToDesign(screen),Document.Material.Width,Document.Material.Height,out var placement))
        {
            BeforeEdit?.Invoke(this,EventArgs.Empty);
            var bend=Document.AddBend(placement.Axis,placement.Position,placement.Direction);
            ActiveBendLayer=null;HoleToolCancelled?.Invoke(this,EventArgs.Empty);
            SelectedObject=bend;ObjectSelected?.Invoke(this,bend);
        }
        Cursor=ActiveBendLayer is null?Cursors.Arrow:Cursors.Cross;InvalidateVisual();return true;
    }
    void DrawBendPreview(DrawingContext dc,Func<double,double,Point> p)
    {
        if(!_bendCreating||ActiveBendLayer is null||Document is null||!FlatBendDrawingEngine.TryCalculate(ActiveBendLayer,_bendStart,_bendCurrent,Document.Material.Width,Document.Material.Height,out var placement))return;
        var pen=new Pen(ActiveBendLayer=="V"?Brushes.Red:Brushes.DodgerBlue,2){DashStyle=DashStyles.Dash};
        if(placement.Axis==SectionAxis.H)dc.DrawLine(pen,p(0,placement.Position),p(Document.Material.Width,placement.Position));
        else dc.DrawLine(pen,p(placement.Position,0),p(placement.Position,Document.Material.Height));
    }
}
