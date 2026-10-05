# CR-051 Test Cases
## TC-051-001
Execution Type: AUTO; Status: ACTIVE
Purpose: actual ARC center and radius in all directions; source example X=300 vs bounds center=279.209; center-preserving resize, diameter/merge candidate/Inner preservation; invalid rejection without mutation; move, units, Undo/Redo and DXF.
Preconditions: Windows/.NET 10. Inputs: cardinal edge centers, radius 41.582 → 30, invalid radii/centers, cm → mm.
Expected: actual center remains on edge after radius edit, shape direction unchanged, no clamping for invalid property edits. Implementation: Program.cs::SemicircleProperties.
## TC-051-002
Execution Type: MANUAL; Status: ACTIVE; Result: PENDING_MANUAL
Preconditions: artifacts/CosmicDesigner-1.20.16/CosmicDesigner.exe.
1. Select right-edge left-facing semicircle. Verify Center X=300, Center Y=diameter midpoint, Radius R=41.582 for attached example; Width/Height absent.
2. Edit radius; diameter stays at X=300, direction and orange merge candidate remain. Undo/Redo.
3. Edit actual center X/Y; shape translates. Invalid/out-of-bounds values reject without move/shrink.
4. Repeat four directions and mm/cm/m, save/reopen and merge; verify circle/triangle properties unchanged.
Expected: values represent ARC center and radius in current document units. Human reviewer/date/evidence required for PASS.
