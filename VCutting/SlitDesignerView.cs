using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace VCutting;

public sealed partial class FlatDesignerView
{
    public string? ActiveSlitShape { get; private set; }
    public event EventHandler<SlitPlacementEventArgs>? SlitPlacementRequested;
    readonly List<Point> _slitPoints = [];
    Point _slitHover;

    public void BeginSlitDrawing(string shape)
    {
        CancelSlitDrawing();ActiveSlitShape=shape;Cursor=Cursors.Cross;InvalidateVisual();
    }
    public void CancelSlitDrawing()
    {
        ActiveSlitShape=null;_slitPoints.Clear();InvalidateVisual();
    }
    public bool HandleSlitKey(Key key)
    {
        if(ActiveSlitShape is null)return false;
        if(key==Key.Escape){CancelSlitDrawing();HoleToolCancelled?.Invoke(this,EventArgs.Empty);return true;}
        if(key==Key.Enter&&ActiveSlitShape=="Polyline"){FinishSlit();return true;}
        return false;
    }
    bool HandleSlitClick(Point screen,Point design,MouseButtonEventArgs e)
    {
        if(Document is null)return false;
        if(ActiveSlitShape is not null)
        {
            e.Handled=true;
            if(!Inside(design)|| (ShowRuler&&(screen.X<39||screen.Y<25)))return true;
            if(_slitPoints.Count>0&&(ToScreen(_slitPoints[^1])-screen).Length<4)return true;
            _slitPoints.Add(design);_slitHover=design;
            if((ActiveSlitShape=="Line"&&_slitPoints.Count==2)||(ActiveSlitShape=="Arc"&&_slitPoints.Count==3))
            {
                if(!FinishSlit())_slitPoints.RemoveAt(_slitPoints.Count-1);
            }
            InvalidateVisual();return true;
        }
        if(ActiveHoleShape is not null||ActiveBendLayer is not null||FilletMode)return false;
        if(TrySelectSlitAt(screen)){e.Handled=true;return true;}
        return false;
    }
    public bool TrySelectSlitAt(Point screen)
    {
        if(Document is null||ActiveSlitShape is not null||ActiveHoleShape is not null||ActiveBendLayer is not null||FilletMode
            ||screen.X<0||screen.Y<0||screen.X>ActualWidth||screen.Y>ActualHeight
            ||ShowRuler&&(screen.X<39||screen.Y<25))return false;
        var slit=SlitHitTesting.Find(Document.Slits,Transform(),screen,SelectionTolerance);
        if(slit is null)return false;
        SelectedObject=slit;ObjectSelected?.Invoke(this,slit);InvalidateVisual();return true;
    }
    bool FinishSlit()
    {
        if(Document is null||ActiveSlitShape is null||!SlitDrawingEngine.TryCreate(ActiveSlitShape,_slitPoints,out var geometry)||!SlitDrawingEngine.Fits(geometry,Document.Material.Width,Document.Material.Height))return false;
        var shape=ActiveSlitShape;CancelSlitDrawing();SlitPlacementRequested?.Invoke(this,new(shape,geometry));return true;
    }
    void DrawSlits(DrawingContext dc,Func<double,double,Point> p)
    {
        if(Document is null)return;
        foreach(var slit in Document.Slits)
            foreach(var geometry in slit.Geometry)
                DrawGeometry(dc,geometry,p,new Pen(ReferenceEquals(SelectedObject,slit)?Brushes.Gold:Brushes.Black,ReferenceEquals(SelectedObject,slit)?4:2));
        if(ActiveSlitShape is null)return;
        var preview=_slitPoints.ToList();
        if(preview.Count>0&&(preview[^1]-_slitHover).Length>1e-6)preview.Add(_slitHover);
        var pen=new Pen(Brushes.DodgerBlue,2){DashStyle=DashStyles.Dash};
        if(SlitDrawingEngine.TryCreate(ActiveSlitShape,preview,out var geometryPreview))
            foreach(var g in geometryPreview)DrawGeometry(dc,g,p,pen);
        else if(preview.Count==2)dc.DrawLine(pen,p(preview[0].X,preview[0].Y),p(preview[1].X,preview[1].Y));
        foreach(var point in _slitPoints)dc.DrawEllipse(Brushes.White,new Pen(Brushes.DodgerBlue,2),p(point.X,point.Y),4,4);
        ViewText.Draw(dc,this,ActiveSlitShape switch{"Arc"=>Localization.Text("ui.0186"),"Polyline"=>Localization.Text("ui.0188"),_=>Localization.Text("ui.0189")},new(ShowRuler?46:8,ShowRuler?28:8),Brushes.DarkGoldenrod,12);
    }
}
