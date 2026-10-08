using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace VCutting;
public sealed partial class FlatDesignerView
{
    readonly RegularPolygonDrawingState _regularDrawing=new();
    public int RegularPolygonSides {get=>_regularDrawing.Sides;set{_regularDrawing.Sides=value;InvalidateVisual();}}
    public int RegularStarStep {get;set;}=2;
    public event EventHandler<RegularStarParameters>? RegularStarPlacementRequested;
    public event EventHandler<RegularPolygonParameters>? RegularPolygonPlacementRequested;
    public event EventHandler? RegularPolygonInputRejected;
    public void CancelRegularPolygonDrawing(){_regularDrawing.Cancel();InvalidateVisual();}
    public bool HandleRegularPolygonKey(Key key){if(key!=Key.Escape||ActiveHoleShape is not ("RegularPolygon" or "RegularStar"))return false;CancelRegularPolygonDrawing();ActiveHoleShape=null;HoleToolCancelled?.Invoke(this,EventArgs.Empty);return true;}
    bool HandleRegularPolygonClick(Point point)
    {
        if(ActiveHoleShape is not ("RegularPolygon" or "RegularStar")||Document is null)return false;
        if(ActiveHoleShape=="RegularStar"&&(RegularPolygonSides<5||RegularStarStep<2||RegularStarStep>(RegularPolygonSides-1)/2)){RegularPolygonInputRejected?.Invoke(this,EventArgs.Empty);return true;}
        var hadCenter=_regularDrawing.Center is not null;
        if(_regularDrawing.Click(point,Document.Material.Width,Document.Material.Height,out var value)){if(ActiveHoleShape=="RegularStar")RegularStarPlacementRequested?.Invoke(this,new(value,RegularStarStep));else RegularPolygonPlacementRequested?.Invoke(this,value);}
        else if(hadCenter||RegularPolygonSides<3||RegularPolygonSides>RegularPolygonParameters.MaximumSides)RegularPolygonInputRejected?.Invoke(this,EventArgs.Empty);
        InvalidateVisual();return true;
    }
    void DrawRegularPolygonPreview(DrawingContext dc,Func<double,double,Point> toScreen)
    {
        if(ActiveHoleShape is not ("RegularPolygon" or "RegularStar")||_regularDrawing.Center is not Point center)return;
        var pen=new Pen(Brushes.DodgerBlue,2){DashStyle=DashStyles.Dash};var points=RegularPolygonGeometry.Vertices(_regularDrawing.Preview);
        var cycles=ActiveHoleShape=="RegularStar"?new IReadOnlyList<Point>[] {RegularStarGeometry.Profile(new(_regularDrawing.Preview,RegularStarStep))}:new IReadOnlyList<Point>[] {points};
        foreach(var cycle in cycles)for(var i=0;i<cycle.Count;i++)dc.DrawLine(pen,toScreen(cycle[i].X,cycle[i].Y),toScreen(cycle[(i+1)%cycle.Count].X,cycle[(i+1)%cycle.Count].Y));
        dc.DrawEllipse(Brushes.White,pen,toScreen(center.X,center.Y),3,3);
    }
}
