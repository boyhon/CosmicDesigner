using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace VCutting;
public partial class MainWindow
{
    void PolygonSidesChanged(object sender,TextChangedEventArgs e)
    {
        if(FlatView is null)return;
        var valid=int.TryParse(PolygonSidesBox.Text,NumberStyles.Integer,CultureInfo.CurrentCulture,out var sides)&&sides>=3&&sides<=RegularPolygonParameters.MaximumSides;
        FlatView.RegularPolygonSides=valid?sides:0;
        if(FlatView.ActiveHoleShape is "RegularPolygon" or "RegularStar")StatusText.Text=Localization.Text(FlatView.ActiveHoleShape=="RegularStar"?"star.instructions":valid?"polygon.instructions":"polygon.invalid");
    }
    void RegularPolygonInputRejected(object? sender,EventArgs e)=>StatusText.Text=Localization.Text(FlatView.ActiveHoleShape=="RegularStar"?"star.invalid":"polygon.invalid");
    void RegularPolygonPlacementRequested(object? sender,RegularPolygonParameters value)
    {
        if(!value.Fits(_document.Material.Width,_document.Material.Height))return;
        RecordEdit();var cut=_document.AddRegularPolygon(value);if(cut is null)return;
        FlatView.SelectedObject=cut;WSection.SelectedObject=cut;HSection.SelectedObject=cut;
        ActivateSelectMode(Localization.Format("status.holeCreated",Localization.DisplayKind("RegularPolygon"),cut.Id));RefreshAll();SelectTreeObject(cut.Id);ShowProperties(cut);UpdateHistoryUi();
        if(_document.CanMergeBoundaryCut(cut))StatusText.Text=Localization.Text("ui.0066");else if(_document.CanMergeInternalCuts(cut))StatusText.Text=Localization.Text("merge.candidate");
    }
    void ShowRegularPolygonProperties(CutOperation cut)
    {
        var unit=_document.Unit.Symbol();
        EditEllipseNumber(Localization.Format("ellipse.centerX",unit),cut.CenterX,v=>EditRegularPolygon(cut,RegularPolygonGeometry.Parameters(cut) with {CenterX=v}));
        EditEllipseNumber(Localization.Format("ellipse.centerY",unit),cut.CenterY,v=>EditRegularPolygon(cut,RegularPolygonGeometry.Parameters(cut) with {CenterY=v}));
        EditEllipseNumber(Localization.Text("polygon.sides"),cut.Sides,v=>{if(!double.IsFinite(v)||v!=Math.Truncate(v)||v<3||v>RegularPolygonParameters.MaximumSides){RegularPolygonRejected(cut);return;}EditRegularPolygon(cut,RegularPolygonGeometry.Parameters(cut) with {Sides=(int)v});});
        EditEllipseNumber(Localization.Format("polygon.radius",unit),cut.Radius,v=>EditRegularPolygon(cut,RegularPolygonGeometry.Parameters(cut) with {Radius=v}));
        EditEllipseNumber(Localization.Text("polygon.rotation"),cut.Rotation,v=>EditRegularPolygon(cut,RegularPolygonGeometry.Parameters(cut) with {Rotation=v}));
    }
    void RegularPolygonRejected(CutOperation cut){ShowProperties(cut);Dispatcher.BeginInvoke(()=>StatusText.Text=Localization.Text("polygon.invalid"));}
    void EditRegularPolygon(CutOperation cut,RegularPolygonParameters value)
    {
        if(!_document.CanUpdateRegularPolygon(cut,value)){RegularPolygonRejected(cut);return;}
        if(RegularPolygonGeometry.Parameters(cut)==value.Normalized)return;
        RecordEdit();_document.TryUpdateRegularPolygon(cut,value);ShowProperties(cut);UpdateHistoryUi();
    }
}
