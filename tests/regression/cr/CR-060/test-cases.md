# CR-060
## TC-060-001
Execution Type: AUTO. Status: ACTIVE.
Procedure: --tests TC-060-001.
Expected: V/V1 horizontal/vertical mappings, bounds, move Undo/Redo, layer/direction/position DXF persistence, unsupported drags rejected.
Implementation: Program.cs::FlatBendTools.
## TC-060-002
Execution Type: MANUAL. Status: ACTIVE.
Procedure: on fresh material, V then right/down drags; repeat V1. Inspect H left/right and W up/down wedges. Click each line, drag along/across it and outside material. Observe property position, selection tree, 3D and Undo/Redo. Esc cancel, switch Hole/Slit/Fillet/Select; save/open and repeat.
Expected: correct colors/layers/wedges, perpendicular-only movement with clamped X/Y, preserved geometry and mode behavior.
Result: PENDING_MANUAL.
