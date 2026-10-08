using System.Windows.Controls;

namespace VCutting;
public partial class MainWindow
{
    void EditArcNumber(string name,double value,Action<double> apply)=>EditEllipseNumber(name,value,apply,"arc.invalid");
    void EditRectangleNumber(string name,double value,Action<double> apply)=>EditEllipseNumber(name,value,apply,"rectangle.invalid");
    void ShowArcAngles(string parentId,ArcSegment arc,DesignObject owner)
    {
        EditArcNumber(Localization.Text("arc.start"),arc.StartDegrees,v=>EditArcAngles(parentId,arc,v,arc.EndDegrees,owner));
        EditArcNumber(Localization.Text("arc.end"),arc.EndDegrees,v=>EditArcAngles(parentId,arc,arc.StartDegrees,v,owner));
        ReadOnlyRow(Localization.Text("arc.sweep"),arc.EndDegrees-arc.StartDegrees);
    }
    void EditArcAngles(string parentId,ArcSegment arc,double start,double end,DesignObject owner)
    {
        if(!_document.CanEditArc(parentId,arc,start,end,out var value,out _)){ShowProperties(owner);Dispatcher.BeginInvoke(()=>StatusText.Text=Localization.Text("arc.invalid"));return;}
        if(value==arc)return;RecordEdit();if(!_document.TryEditArc(parentId,arc,start,end,out var updated))return;
        DesignObject selected=owner;
        if(owner is GeometryObject old)
        {
            var geometry=parentId=="OUTER"?_document.OuterContour.Segments:_document.Cuts.First(c=>c.Id==parentId).Geometry;
            var index=geometry.IndexOf(updated);var id=parentId=="OUTER"?$"L{index+1:000}":$"{parentId}-L{index+1:000}";
            selected=new GeometryObject{Id=id,Sequence=index+1,ParentContourId=parentId,Layer=old.Layer,EntityType="ARC",Geometry=updated};
        }
        FlatView.SelectedObject=selected;WSection.SelectedObject=selected;HSection.SelectedObject=selected;RefreshAll();SelectTreeObject(selected.Id);ShowProperties(selected);UpdateHistoryUi();
    }
    void ShowRectangleProperties(CutOperation cut)
    {
        var unit=_document.Unit.Symbol();
        EditRectangleNumber(Localization.Format("ellipse.centerX",unit),cut.CenterX,v=>EditRectangle(cut,RotatedRectangleGeometry.Parameters(cut) with {CenterX=v}));
        EditRectangleNumber(Localization.Format("ellipse.centerY",unit),cut.CenterY,v=>EditRectangle(cut,RotatedRectangleGeometry.Parameters(cut) with {CenterY=v}));
        EditRectangleNumber(Localization.Format("rectangle.width",unit),cut.Width,v=>EditRectangle(cut,RotatedRectangleGeometry.Parameters(cut) with {Width=v}));
        EditRectangleNumber(Localization.Format("rectangle.height",unit),cut.Height,v=>EditRectangle(cut,RotatedRectangleGeometry.Parameters(cut) with {Height=v}));
        EditRectangleNumber(Localization.Text("rectangle.rotation"),cut.Rotation,v=>EditRectangle(cut,RotatedRectangleGeometry.Parameters(cut) with {Rotation=v}));
    }
    void EditRectangle(CutOperation cut,RectangleParameters value)
    {
        if(!_document.CanUpdateRectangle(cut,value)){ShowProperties(cut);Dispatcher.BeginInvoke(()=>StatusText.Text=Localization.Text("rectangle.invalid"));return;}
        if(RotatedRectangleGeometry.Parameters(cut)==value.Normalized)return;RecordEdit();_document.TryUpdateRectangle(cut,value);ShowProperties(cut);UpdateHistoryUi();
    }
}
