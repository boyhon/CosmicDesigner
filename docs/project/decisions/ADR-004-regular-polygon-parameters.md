# ADR-004 RegularPolygon parameter authority and DXF interchange
## Status
Accepted — CR-078, user attachment 2026-10-07.
## Context
Legacy Polygon cuts store arbitrary sampled edges; the requested regular shape must remain a single parameter-editable entity, without changing old documents or generic polyline interpretation.
## Decision
Use existing CutOperation with Shape/EntityType RegularPolygon, center, integer sides3..512, positive radius and CCW first vertex angle normalized0..360. Layer L and closed semantics match existing Hole tools. One shared vertex calculator is authoritative; Geometry stores only derived LINE cache for existing renderer/boolean/contour/mesh. No edge children in object tree or vertex deformation. Move changes center only, size edits use radius. Fillet/union explicitly turn modified shape into Compound.
Add optional Radius/Rotation fields to existing DXF JSON metadata and snapshots; old non-RegularPolygon documents retain legacy handling. Load and history rebuild regular vertices from parameters. Unit physical conversion scales center/radius. No new file format.
Export regular shape as one closed LWPOLYLINE, group90 count=N, group70=1, AcDbEntity/AcDbPolyline subclasses, N unique ordered vertices and original L layer; default OCS +Z, design +Y matches existing DXF, screen transforms already invert Y. Generic default-OCS straight LWPOLYLINE parser emits existing generic GeometryLine edges (not RegularPolygon). Curved bulges/nondefault normals/malformed counts are rejected as unsupported instead of silently losing geometry.
## Consequences
Current app preserves logical metadata parameters and reads older cuts. Older apps may ignore new fields and use cached LINE geometry, but may not interpret regular semantics or LWPOLYLINE interchange; not guaranteed. Max512 prevents unbounded contour/mesh work while supporting hundreds of sides. Preview/creation/edit/export must share calculator; AUTO and real UI/AutoCAD verification separated.
## Standard source
Autodesk LWPOLYLINE DXF reference: https://help.autodesk.com/cloudhelp/2017/ENU/AutoCAD-DXF/files/GUID-748FC305-F3F2-4F74-825A-61F04D757A50.htm
