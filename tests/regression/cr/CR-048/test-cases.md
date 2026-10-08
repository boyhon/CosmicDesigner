# CR-048 Test Cases

## TC-048-001
Execution Type: AUTO; Status: ACTIVE.
Purpose: radius edit invariants. Preconditions: Windows/.NET 10 Release.
Procedure: --tests TC-048-001. Expected: 23.59 radius = 47.18 diameter; unchanged center; invalid/boundary inputs rejected without mutation; synchronized Inner Contour; Undo/Redo, DXF, mm/m; non-Circle rejected.
Implementation: VCutting.Verification/Program.cs::CircleRadius.

## TC-048-002
Execution Type: MANUAL; Status: ACTIVE; Result: PENDING_MANUAL.
Purpose: single radius UI. Preconditions: 1.20.13, Circle diameter 47.18 inside material.
Procedure: select Circle through Flat and Object Tree, verify Radius R (cm) 23.59 with no Width/Height; edit to 20, leave focus, confirm diameter 40 and unchanged center. Undo/Redo; enter zero/negative/out-of-material radius and confirm original value restored. Select Triangle/Rectangle and confirm Width/Height remain. Convert to mm/m and verify label/value/edit. Save/reopen and check Circle.
Expected: single editable radius with active unit, center preserved, invalid value restored, other shapes unchanged, no stale values after focus changes.
Implementation: MainWindow.ShowProperties; Models.TryResizeCircleRadius.
