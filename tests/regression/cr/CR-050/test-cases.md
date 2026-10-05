# CR-050 Test Cases
## TC-050-001
Execution Type: AUTO; Status: ACTIVE
Purpose: four directions, exact reversed ARC, closed contour, Cut/Inner removal, interior/outward/tangent rejection, sequential merges, Fillet preservation, multiple collinear edges and partial overlap, material mask, sampled 3D area, exact local Section notch depth, Undo/Redo, DXF geometry and CCW export.
Preconditions: Windows/.NET 10. Input: r=20 semicircles on all four edges of 300x300 material; additional boundary fixtures.
Expected: safe one-loop subtraction and unchanged existing ARC geometry; correct area and persistence.
Implementation: CosmicDesigner.Verification/Program.cs::SemicircleBoundaryMerge. Run --tests TC-050-001.
## TC-050-002
Execution Type: MANUAL; Status: ACTIVE; Result: PENDING_MANUAL
Preconditions: artifacts/CosmicDesigner-1.20.15/CosmicDesigner.exe.
1. Create inward semicircles on each of the four material edges with diameter coinciding with Outer Contour. Select: orange candidate and status guidance.
2. Right click → Outer Contour로 통합. Verify only arc notch remains, no duplicate diameter or Cut/Inner tree objects.
3. Check Flat/3D and local Sections, multiple zooms, Undo/Redo, save/reopen, and external DXF viewer.
4. Repeat after Fillet and prior notch; try interior and arc-only tangent holes. Only eligible diameter overlaps offer merge.
Expected: four-direction notch shape and preserved exact arcs; existing triangle/rectangle/parallelogram workflow unchanged. Record reviewer/date/evidence before PASS.
