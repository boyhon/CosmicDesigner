# CR-082 cases

## TC-082-001
Execution Type AUTO; Status ACTIVE; Windows .NET10. Procedure --tests TC-082-001.
Expected 45deg orthogonal corners/local dimensions, parametric validation/cache/invalid/no-op; move/resize/normalization; metadata/history/units/mask/mesh, accurate WPF hit, fillet/internal/Outer polygon merge.
Implementation AngleEditingTests::Rectangles.

## TC-082-002
Execution Type MANUAL; Status ACTIVE.
Procedure Rectangle rotation0/30/45/90/negative; local width/height/body move, property resize, angle0 old handles and rotated no handles, selection outside actual polygon, invalid boundaries, undo/redo, old saved rectangle reopen, Fillet/Merge/CAD; English/Korean layout.
Expected valid angles/edit results, unchanged physical units/data workflow, natural labels/unclipped controls. Reviewer/date/evidence required. PENDING_MANUAL; no human PASS.
