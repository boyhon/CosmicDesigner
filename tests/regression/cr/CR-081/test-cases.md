# CR-081 cases

## TC-081-001
Execution Type AUTO; Status ACTIVE; Windows .NET10. Procedure --tests TC-081-001.
Expected Slit start/end/CW/unwrapped edit and invalid/no-op atomicity, exact cardinal bounds; Outer/Inner closed reconnection and adjacent ARC connector lines; signed ARC DXF/history/units and edited mesh.
Implementation AngleEditingTests::Arcs.

## TC-081-002
Execution Type MANUAL; Status ACTIVE.
Procedure Edit ARC start/end from Slit and Outer/Inner tree, see signed sweep/connection changes, clockwise and crossing0 angles, errors, undo, DXF/CAD; Korean/English layout/Flat/3D. Confirm changing connected Fillet angles produces ordinary ARC and tangent radius resize may reject.
Expected valid angles/edit results, unchanged physical units/data workflow, natural labels/unclipped controls. Reviewer/date/evidence required. PENDING_MANUAL; no human PASS.
