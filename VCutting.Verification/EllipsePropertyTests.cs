using System.IO;
using System.Windows;
using VCutting;
using Localization = VCutting.Localization;

static class EllipsePropertyTests
{
    static void Check(bool v,string reason){if(!v)throw new InvalidOperationException(reason);}
    static void Eq(double a,double b)=>Check(Math.Abs(a-b)<1e-6,$"ellipse property {a} != {b}");
    static EllipseProperties Get(CutOperation cut){Check(EllipsePropertiesEngine.TryGet(cut,out var p),"valid ellipse properties");return p;}
    public static void Run()
    {
        foreach(var angle in new[]{0d,30,90,135,179})
        {
            var doc=new VCuttingDocument();var cut=doc.AddHole("Ellipse",150,150);
            doc.UpdateCut(cut,150,150,80,40,7);var p=Get(cut);Eq(80,p.MajorLength);Eq(40,p.MinorLength);
            var target=new EllipseProperties(150,150,80,40,angle);
            Check(doc.TryUpdateEllipse(cut,target),"angle edit");p=Get(cut);Eq(angle,p.RotationDegrees);Eq(80,p.MajorLength);Eq(40,p.MinorLength);Eq(150,p.CenterX);
            var notifications=0;void Changed(object? sender,EventArgs e)=>notifications++;doc.Changed+=Changed;
            var unchanged=cut.Geometry.ToList();Check(doc.TryUpdateEllipse(cut,p),"same-value accepted");Check(notifications==0&&unchanged.SequenceEqual(cut.Geometry),"no-op no dirty notification");doc.Changed-=Changed;
            var history=new UndoRedoManager();history.Record(doc);var before=cut.Geometry.ToList();
            Check(doc.TryUpdateEllipse(cut,p with {MajorLength=100}),"major edit");p=Get(cut);Eq(100,p.MajorLength);Eq(40,p.MinorLength);Eq(angle,p.RotationDegrees);Eq(150,p.CenterY);
            var undo=history.Undo(doc)!;Check(undo.Cuts.Single().Geometry.SequenceEqual(before),"axis undo");var redo=history.Redo(undo)!;Eq(100,Get(redo.Cuts.Single()).MajorLength);
            Check(doc.TryUpdateEllipse(cut,p with {MinorLength=20}),"minor edit");p=Get(cut);Eq(100,p.MajorLength);Eq(20,p.MinorLength);Eq(angle,p.RotationDegrees);
            Check(doc.TryUpdateEllipse(cut,p with {CenterX=160,CenterY=140}),"center edit");p=Get(cut);Eq(160,p.CenterX);Eq(140,p.CenterY);Eq(angle,p.RotationDegrees);
            Check(doc.InnerContours.Single().Segments.SequenceEqual(cut.Geometry),"inner sync");
            foreach(var invalid in new[]{p with {CenterX=0},p with {MajorLength=10},p with {MinorLength=.09},p with {MajorLength=double.NaN},p with {RotationDegrees=double.PositiveInfinity},p with {CenterY=double.NaN},p with {MinorLength=-1}})
            {before=cut.Geometry.ToList();Check(!doc.CanUpdateEllipse(cut,invalid)&&!doc.TryUpdateEllipse(cut,invalid),"invalid rejected");Check(before.SequenceEqual(cut.Geometry),"invalid no geometry change");}
            doc.ChangeUnit(MeasurementUnit.Millimeter,UnitChangeMode.PreservePhysicalSize);p=Get(cut);Eq(1000,p.MajorLength);Eq(200,p.MinorLength);Eq(1600,p.CenterX);Eq(angle,p.RotationDegrees);
            var path=Path.Combine(Path.GetTempPath(),"ellipse-props-"+Guid.NewGuid().ToString("N")+".dxf");
            try{VCuttingDxfSerializer.Save(doc,path);var loaded=VCuttingDxfSerializer.Load(path);Check(loaded.Cuts.Single().Geometry.SequenceEqual(cut.Geometry),"legacy DXF exact geometry");Eq(1000,Get(loaded.Cuts.Single()).MajorLength);}
            finally{File.Delete(path);}
        }
        var d=new VCuttingDocument();var c=d.AddHole("Ellipse",150,150);d.UpdateCut(c,150,150,60,30,7);
        Check(d.TryUpdateEllipse(c,new(150,150,60,30,-30)),"negative angle");Eq(150,Get(c).RotationDegrees);
        Check(d.TryUpdateEllipse(c,new(150,150,60,30,390)),"angle wrap");Eq(30,Get(c).RotationDegrees);
        Check(d.TryUpdateEllipse(c,new(150,150,60,.1,30)),"minimum minor length accepted");Eq(.1,Get(c).MinorLength);
        Check(d.TryUpdateEllipse(c,new(160,140,60,.1,30)),"minimum minor remains editable");
        // A contour vertex edit must not be silently replaced by an ideal ellipse.
        c.Geometry[0]=new LineSegment(150,150,150,150);Check(!EllipsePropertiesEngine.TryGet(c,out _),"deformed legacy refused");
        var edge=new EllipseProperties(25,25,60,20,45);Check(edge.Fits(300,300),"rotated analytic bounds");Check(!(edge with {CenterX=20}).Fits(300,300),"analytic outside even if sampled bounds differ");
        Eq(0,EllipseProperties.NormalizeAngle(360));Eq(90,EllipseProperties.NormalizeAngle(-90));
        var root=Root();var source=File.ReadAllText(Path.Combine(root,"VCutting","MainWindow.xaml.cs"));
        Check(source.Contains("ShowEllipseProperties(c,ellipse);return;")&&source.Contains("EllipsePropertiesEngine.TryGet(c,out _)?0:c.Geometry.Count"),"ellipse UI branch and collapsed LINE children");
        Localization.SetLanguage("ko");Check(Localization.Format("ellipse.major","mm")=="장축 길이 (mm)","Korean axis units");Localization.SetLanguage("en");Check(Localization.Text("ellipse.angle")=="Rotation angle (°)","English angle");
    }
    static string Root(){var d=new DirectoryInfo(AppContext.BaseDirectory);while(d!=null&&!File.Exists(Path.Combine(d.FullName,"VCutting.sln")))d=d.Parent;return d!.FullName;}
    public static void Identity()
    {
        Check(typeof(VCuttingDocument).Assembly.GetName().Name=="CosmicDesigner","actual main assembly name");
        Check(File.Exists(Path.Combine(AppContext.BaseDirectory,"CosmicDesigner.exe"))&&File.Exists(Path.Combine(AppContext.BaseDirectory,"CosmicDesigner.dll")),"actual output identity");
        var root=Root();var iss=File.ReadAllText(Path.Combine(root,"installer","VCutting.iss"));
        Check(iss.Contains("{app}\\CosmicDesigner.exe")&&!iss.Contains("{app}\\VCutting.exe"),"installer new executable references");
        Check(iss.Contains("DefaultDirName={autopf}\\VCutting")&&iss.Contains("AppId={{72D5CE59-6E07-47D0-88C6-ADE37D3368A1}"),"package and upgrade compatibility");
    }
}
