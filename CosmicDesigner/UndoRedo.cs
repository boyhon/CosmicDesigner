namespace CosmicDesigner;

public sealed record MaterialSnapshot(double Thickness);
public sealed record SegmentSnapshot(string Id,SectionAxis Axis,int Index,double Length);
public sealed record BendSnapshot(string Id,int Sequence,SectionAxis Axis,BendDirection Direction,double Position);
public sealed record CutSnapshot(string Id,int Sequence,CutKind Kind,string Shape,string ParentContourId,double CenterX,double CenterY,double Width,double Height,int Sides,IReadOnlyList<GeometrySegment> Geometry);
public sealed record SlitSnapshot(string Id,int Sequence,string Shape,IReadOnlyList<GeometrySegment> Geometry);
public sealed record JointSnapshot(string Id,int Sequence,string ParentContourId,double Position,double Width,bool Enabled);
public sealed record ViewSnapshot(bool Dimensions,bool BentMode,int RotationQuarterTurns);
public sealed record DocumentSnapshot(MaterialSnapshot Material,MeasurementUnit Unit,IReadOnlyList<SegmentSnapshot> Segments,IReadOnlyList<BendSnapshot> Bends,IReadOnlyList<CutSnapshot> Cuts,IReadOnlyList<JointSnapshot> Joints,IReadOnlyList<GeometrySegment> Outer,ViewSnapshot WView,ViewSnapshot HView,IReadOnlyList<SlitSnapshot>? Slits=null)
{
    public static DocumentSnapshot Capture(CosmicDesignerDocument d)=>new(new(d.Material.Thickness),d.Unit,d.WSegments.Concat(d.HSegments).Select(s=>new SegmentSnapshot(s.Id,s.Axis,s.Index,s.Length)).ToList(),d.Bends.Select(b=>new BendSnapshot(b.Id,b.Sequence,b.Axis,b.Direction,b.Position)).ToList(),d.Cuts.Select(c=>new CutSnapshot(c.Id,c.Sequence,c.Kind,c.Shape,c.ParentContourId,c.CenterX,c.CenterY,c.Width,c.Height,c.Sides,c.Geometry.ToList())).ToList(),d.MicroJoints.Select(j=>new JointSnapshot(j.Id,j.Sequence,j.ParentContourId,j.Position,j.Width,j.Enabled)).ToList(),d.OuterContour.Segments.ToList(),new(d.WView.Dimensions,d.WView.BentMode,d.WView.RotationQuarterTurns),new(d.HView.Dimensions,d.HView.BentMode,d.HView.RotationQuarterTurns),d.Slits.Select(s=>new SlitSnapshot(s.Id,s.Sequence,s.Shape,s.Geometry.ToList())).ToList());
    public CosmicDesignerDocument Restore()
    {
        var d=new CosmicDesignerDocument();d.WSegments.Clear();d.HSegments.Clear();d.Bends.Clear();d.Cuts.Clear();d.MicroJoints.Clear();
        foreach(var s in Segments){var value=new SectionSegment{Id=s.Id,Axis=s.Axis,Index=s.Index,Length=s.Length};if(s.Axis==SectionAxis.W)d.WSegments.Add(value);else d.HSegments.Add(value);}
        foreach(var b in Bends)d.Bends.Add(new BendObject{Id=b.Id,Sequence=b.Sequence,Axis=b.Axis,Direction=b.Direction,Position=b.Position});
        foreach(var c in Cuts){var cut=new CutOperation{Id=c.Id,Sequence=c.Sequence,Kind=c.Kind,Shape=c.Shape,ParentContourId=c.ParentContourId,CenterX=c.CenterX,CenterY=c.CenterY,Width=c.Width,Height=c.Height,Sides=c.Sides};if(c.Geometry.Count>0)cut.Geometry.AddRange(c.Geometry);else HoleGeometryEngine.Rebuild(cut);d.Cuts.Add(cut);var contour=new ContourObject{Id=cut.Id,Kind=ContourKind.Inner};contour.Segments.AddRange(cut.Geometry);d.InnerContours.Add(contour);}
        foreach(var s in Slits??[]){var slit=new SlitOperation{Id=s.Id,Sequence=s.Sequence,Shape=s.Shape};slit.Geometry.AddRange(s.Geometry);d.Slits.Add(slit);}
        foreach(var j in Joints)d.MicroJoints.Add(new MicroJoint{Id=j.Id,Sequence=j.Sequence,ParentContourId=j.ParentContourId,Position=j.Position,Width=j.Width,Enabled=j.Enabled});
        d.SetUnit(Unit);d.ChangeThickness(Material.Thickness);d.PreserveOuterGeometry(Outer);d.WView.Dimensions=WView.Dimensions;d.WView.BentMode=WView.BentMode;d.WView.RotationQuarterTurns=WView.RotationQuarterTurns;d.HView.Dimensions=HView.Dimensions;d.HView.BentMode=HView.BentMode;d.HView.RotationQuarterTurns=HView.RotationQuarterTurns;return d;
    }
}

public sealed class UndoRedoManager
{
    readonly List<DocumentSnapshot> _undo=[];readonly List<DocumentSnapshot> _redo=[];readonly int _capacity;
    public UndoRedoManager(int capacity=100)=>_capacity=Math.Max(1,capacity);
    public bool CanUndo=>_undo.Count>0;public bool CanRedo=>_redo.Count>0;
    public void Record(CosmicDesignerDocument document){_undo.Add(DocumentSnapshot.Capture(document));if(_undo.Count>_capacity)_undo.RemoveAt(0);_redo.Clear();}
    public CosmicDesignerDocument? Undo(CosmicDesignerDocument current){if(!CanUndo)return null;_redo.Add(DocumentSnapshot.Capture(current));var state=_undo[^1];_undo.RemoveAt(_undo.Count-1);return state.Restore();}
    public CosmicDesignerDocument? Redo(CosmicDesignerDocument current){if(!CanRedo)return null;_undo.Add(DocumentSnapshot.Capture(current));var state=_redo[^1];_redo.RemoveAt(_redo.Count-1);return state.Restore();}
    public void Clear(){_undo.Clear();_redo.Clear();}
}
