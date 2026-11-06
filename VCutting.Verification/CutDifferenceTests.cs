using System.IO;
using System.Windows;
using VCutting;
using VCutting.Viewer;

static class CutDifferenceTests
{
    static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
    static CutOperation Rect(VCuttingDocument d,double x,double y,double w,double h){var c=d.AddHole("Rectangle",x,y);d.UpdateCut(c,x,y,w,h,7);return c;}
    static bool Filled(VCuttingDocument d,double x,double y)=>d.Cuts.Any(c=>CutContourEngine.Contains(c,new Point(x,y)));
    static double Area(VCuttingDocument d)=>d.Cuts.Sum(c=>c.Geometry.Sum(g=>
    {
        if(g is LineSegment l)return (l.X1*l.Y2-l.X2*l.Y1)/2;
        var a=(ArcSegment)g;var start=a.StartDegrees*Math.PI/180;var end=a.EndDegrees*Math.PI/180;
        return (a.Radius*a.Cx*(Math.Sin(end)-Math.Sin(start))+a.Radius*a.Cy*(Math.Cos(start)-Math.Cos(end))+a.Radius*a.Radius*(end-start))/2;
    }));
    public static void Run(int n)
    {
        var d=new VCuttingDocument();CutOperation s;
        switch(n)
        {
            case 1:
                s=Rect(d,100,100,40,40);Rect(d,120,100,40,40);
                Check(d.TrySubtractInternalCuts(s,out var r)&&r.Count==1,"partial difference");Check(Math.Abs(Area(d)-800)<1e-6&&!Filled(d,100,100)&&Filled(d,135,100),"B minus A direction and area");break;
            case 2:
                s=Rect(d,150,150,20,20);Rect(d,150,150,80,80);
                Check(d.TrySubtractInternalCuts(s,out r)&&r.Count==1&&CutContourEngine.Loops(r[0].Geometry).Count==2,"hole loops");Check(Math.Abs(Area(d)-6000)<1e-6&&!Filled(d,150,150)&&Filled(d,120,120),"hole parity");break;
            case 3:
                s=Rect(d,150,150,80,80);Rect(d,150,150,20,20);Check(d.TrySubtractInternalCuts(s,out r)&&r.Count==0&&d.Cuts.Count==0&&d.InnerContours.Count==0,"empty succeeds and consumes sources");break;
            case 4:
                s=Rect(d,150,150,40,40);Rect(d,120,150,40,40);Rect(d,180,150,40,40);Check(d.TrySubtractInternalCuts(s,out r)&&r.Count==2&&Math.Abs(Area(d)-2400)<1e-6,"(A union C) minus B");break;
            case 5:
                s=Rect(d,150,150,20,100);Rect(d,150,150,80,80);Check(d.TrySubtractInternalCuts(s,out r)&&r.Count==2&&Math.Abs(Area(d)-4800)<1e-6,"two independent islands");Check(r.Select(c=>c.Id).Distinct().Count()==2&&r.All(c=>c.Sequence==s.Sequence&&c.Kind==s.Kind&&c.Layer==s.Layer),"identity and properties");break;
            case 6:
                s=Rect(d,100,100,40,40);Rect(d,200,200,40,40);Check(!d.TrySubtractInternalCuts(s,out r)&&d.Cuts.Count==2&&d.Cuts[0]==s,"disjoint no-op");break;
            case 7:
                s=Rect(d,100,100,40,40);Rect(d,140,100,40,40);Check(d.CanMergeInternalCuts(s)&&d.TrySubtractInternalCuts(s,out r)&&Math.Abs(Area(d)-1600)<1e-6,"edge touch shares merge policy");break;
            case 8:
                s=Rect(d,100,100,40,40);Rect(d,140,140,40,40);Check(!d.CanMergeInternalCuts(s)&&!d.TrySubtractInternalCuts(s,out r)&&d.Cuts.Count==2,"vertex touch no-op");break;
            case 9:
                s=Rect(d,90,150,50,40);Rect(d,130,150,50,40);Rect(d,170,150,50,40);Rect(d,210,150,50,40);
                Check(d.InternalMergeGroup(s).Count==4&&d.TrySubtractInternalCuts(s,out r)&&Filled(d,225,150)&&!Filled(d,100,150)&&Math.Abs(Area(d)-4800)<1e-6,"transitive chain");break;
            case 10:
                s=Rect(d,150,150,20,100);Rect(d,150,150,80,80);Check(d.TrySubtractInternalCuts(s,out r),"save fixture");
                var path=Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".dxf");
                try{VCuttingDxfSerializer.Save(d,path);var loaded=VCuttingDxfSerializer.Load(path);Check(loaded.Cuts.Count==2&&loaded.Cuts.Zip(d.Cuts).All(p=>p.First.Geometry.SequenceEqual(p.Second.Geometry)),"save reload exact geometry");VCuttingDxfSerializer.Save(loaded,path);var again=VCuttingDxfSerializer.Load(path);Check(Math.Abs(Area(again)-4800)<1e-6,"second DXF roundtrip");var parsed=DxfDocumentParser.Parse(path);Check(parsed.UnsupportedCounts.Count==0&&parsed.EntityCounts["LINE"]==12,"DXF parser reload contains frame plus two result contours only");var raw=File.ReadAllText(path);Check(raw.Contains("LINE")&&!raw.Contains("LWPOLYLINE"),"actual analytic DXF entities");}finally{File.Delete(path);}break;
            case 11:
                s=Rect(d,150,150,20,100);Rect(d,150,150,80,80);d.HistorySelectionId=s.Id;
                var history=new UndoRedoManager();var before=DocumentSnapshot.Capture(d);history.Record(d);Check(d.TrySubtractInternalCuts(s,out r),"history fixture");d.HistorySelectionId=r[0].Id;var after=DocumentSnapshot.Capture(d);
                var undone=history.Undo(d)!;Check(undone.HistorySelectionId==s.Id&&undone.Cuts.Zip(before.Cuts).All(p=>p.First.Id==p.Second.Id&&p.First.Sequence==p.Second.Sequence&&p.First.Geometry.SequenceEqual(p.Second.Geometry)),"undo order properties geometry selection");var redone=history.Redo(undone)!;Check(redone.Cuts.Zip(after.Cuts).All(p=>p.First.Geometry.SequenceEqual(p.Second.Geometry))&&redone.HistorySelectionId==after.SelectionId,"redo exact");break;
            case 12:CutUnionTests.Run();break;
            case 13:
                s=d.AddHole("Circle",150,150);d.UpdateCut(s,150,150,20,20,7);Rect(d,150,150,80,80);Check(d.TrySubtractInternalCuts(s,out r)&&r[0].Geometry.OfType<ArcSegment>().Count()==2&&!Filled(d,150,150),"exact circle hole arcs");
                Check(Math.Abs(Area(d)-(6400-Math.PI*100))<1e-6,"analytic circular area");break;
        }
    }
}
