// Exact nested DTO declarations copied from VCuttingDxfSerializer (Engines.cs), CR-087.
// Alias types are used only in empty Bend/MicroJoint collections; no invented serialized data.
using MicroJoint = System.Text.Json.JsonElement;
using SectionAxis = System.Int32;
using BendDirection = System.Int32;
namespace CosmicConvert;
internal static partial class DesignerMetadata
{
record DocumentDto(double Thickness,List<SegmentDto> WSegments,List<SegmentDto> HSegments,List<BendDto> Bends,List<CutDto>? Cuts,List<MicroJoint> MicroJoints,List<GeometryDto>? Outer=null,string? Unit=null,ViewDto? WView=null,ViewDto? HView=null,List<SlitDto>? Slits=null);
    record SlitDto(string Id,int Sequence,string Shape,List<GeometryDto> Geometry);
    record ViewDto(bool Dimensions,bool BentMode,int RotationQuarterTurns);
    record SegmentDto(string Id,int Index,double Length); record BendDto(string Id,int Sequence,SectionAxis Axis,BendDirection Direction,double Position);record CutDto(string Id,int Sequence,string Shape,double CenterX,double CenterY,double Width,double Height,int Sides,List<GeometryDto>? Geometry=null,double? Radius=null,double? Rotation=null,int? StarStep=null);record GeometryDto(string Type,double A,double B,double C,double D,double Radius);

}
