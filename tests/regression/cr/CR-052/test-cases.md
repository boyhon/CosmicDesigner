# CR-052 Test Cases
## TC-052-001
Execution Type: AUTO; Status: ACTIVE.
Purpose: 4분원 생성/편집/통합과 기존 저장 경로 확인.
Preconditions: Windows .NET 10 Release.
Procedure: --tests TC-052-001. QuarterCircleOperations assertions execute all four quadrants; both individual radii and corner merges, invalid/interior/tangent rejection, preview equality, mask/mesh, DXF, units and Undo/Redo.
Expected: all assertions PASS; exit 0.
Implementation: CosmicDesigner.Verification/Program.cs::QuarterCircleOperations.
## TC-052-002
Execution Type: MANUAL; Status: ACTIVE; Result: PENDING_MANUAL.
Preconditions: 1.20.17 application, new 300x300 document.
Procedure: verify Quarter Circle outline button immediately after Semicircle; drag from (100,100) into each quadrant and compare dashed preview/final; short click produces no cut, completed drag returns Select. Select cut: Center X/Y and Radius R in current units, no Width/Height. Edit radius and move/resize with handles; direction and circular arc persist. Place center on each material edge with inward quadrant and then a corner; orange candidate and right-click merge remove Cut/Inner. Undo/Redo and DXF reopen. Inspect Flat, sections and 3D cutout/arc; repeat after Fillet and earlier notches.
Expected: requested geometry/direction, radius properties and explicit merge work without gaps or stale selections.
Evidence: human reviewer/date/observations required; no human result recorded.
