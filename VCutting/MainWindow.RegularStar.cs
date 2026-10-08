using System.Globalization;
using System.Windows.Controls;

namespace VCutting;
public partial class MainWindow
{
    void StarStepChanged(object sender,TextChangedEventArgs e)
    {
        if(FlatView is null)return;
        FlatView.RegularStarStep=int.TryParse(StarStepBox.Text,NumberStyles.Integer,CultureInfo.CurrentCulture,out var step)?step:0;
        FlatView.Refresh();
        if(FlatView.ActiveHoleShape=="RegularStar")StatusText.Text=Localization.Text("star.instructions");
    }
    void RegularStarPlacementRequested(object? sender,RegularStarParameters value)
    {
        if(!value.Fits(_document.Material.Width,_document.Material.Height))return;
        RecordEdit();var cut=_document.AddRegularStar(value);if(cut is null)return;
        FlatView.SelectedObject=cut;WSection.SelectedObject=cut;HSection.SelectedObject=cut;
        ActivateSelectMode(Localization.Format("status.holeCreated",Localization.DisplayKind("RegularStar"),cut.Id));RefreshAll();SelectTreeObject(cut.Id);ShowProperties(cut);UpdateHistoryUi();
        if(_document.CanMergeInternalCuts(cut))StatusText.Text=Localization.Text("merge.candidate");
    }
    void ShowRegularStarProperties(CutOperation cut)
    {
        var unit=_document.Unit.Symbol();
        void Polygon(System.Func<RegularPolygonParameters,RegularPolygonParameters> change)=>EditRegularStar(cut,RegularStarGeometry.Parameters(cut) with {Polygon=change(RegularPolygonGeometry.Parameters(cut))});
        EditEllipseNumber(Localization.Format("ellipse.centerX",unit),cut.CenterX,v=>Polygon(p=>p with {CenterX=v}));
        EditEllipseNumber(Localization.Format("ellipse.centerY",unit),cut.CenterY,v=>Polygon(p=>p with {CenterY=v}));
        EditEllipseNumber(Localization.Text("polygon.sides"),cut.Sides,v=>{if(!Integer(v,5,512)){StarRejected(cut);return;}Polygon(p=>p with {Sides=(int)v});});
        EditEllipseNumber(Localization.Text("star.step"),cut.StarStep,v=>{if(!Integer(v,2,(cut.Sides-1)/2)){StarRejected(cut);return;}EditRegularStar(cut,RegularStarGeometry.Parameters(cut) with {Step=(int)v});});
        EditEllipseNumber(Localization.Format("polygon.radius",unit),cut.Radius,v=>Polygon(p=>p with {Radius=v}));
        EditEllipseNumber(Localization.Text("polygon.rotation"),cut.Rotation,v=>Polygon(p=>p with {Rotation=v}));
    }
    static bool Integer(double value,int min,int max)=>double.IsFinite(value)&&value==System.Math.Truncate(value)&&value>=min&&value<=max;
    void StarRejected(CutOperation cut){ShowProperties(cut);Dispatcher.BeginInvoke(()=>StatusText.Text=Localization.Text("star.invalid"));}
    void EditRegularStar(CutOperation cut,RegularStarParameters value)
    {
        if(!_document.CanUpdateRegularStar(cut,value)){StarRejected(cut);return;}
        if(RegularStarGeometry.Parameters(cut)==value.Normalized)return;
        RecordEdit();_document.TryUpdateRegularStar(cut,value);ShowProperties(cut);UpdateHistoryUi();
    }
}
