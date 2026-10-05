# CR-049 Test Cases

## TC-049-001
Execution Type: AUTO; Status: ACTIVE
Purpose: cardinal and dominant-axis direction, exact closed ARC/LINE, preview/commit parity, boundary clamp, tiny drag rejection, material mask, 3D creation, move/resize direction, Undo/Redo and DXF round-trip.
Preconditions: Windows/.NET 10. Implementation: CosmicDesigner.Verification/Program.cs::SemicircleCreation.
Input: anchor (100,100), axial displacement ±20, diagonal (5,20)/(20,5), boundary anchor (5,100).
Expected: 180-degree ARC, matching closing diameter, direction maintained through editing/persistence; r=5 at boundary. Runner: --tests TC-049-001.

## TC-049-002
Execution Type: MANUAL; Status: ACTIVE; Result: PENDING_MANUAL
Preconditions: artifacts/CosmicDesigner-1.20.14/CosmicDesigner.exe.
1. Verify Circle → Semicircle → Triangle order, left semicircle outline matching attached image, tooltip and accessibility name.
2. Choose Semicircle, press within material and drag up/down/right/left, including purely axial motion. Verify dashed preview and matching single final hole; Select mode returns.
3. Verify tiny click produces no cut; diagonal drag follows dominant axis; material boundary limits radius.
4. Move and resize each direction, Undo/Redo, save/reopen DXF; verify circular arc and orientation retained.
5. Verify holes remove material in Flat and 3D at multiple zooms and with bends.
Expected: all requested directions, icon and interaction agree; no unintended existing shape changes. Human reviewer/date/evidence required before PASS.
