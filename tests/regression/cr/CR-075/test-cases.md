# CR-075 cases

## TC-075-001
Execution Type: AUTO. Status: ACTIVE.
Preconditions: Development Release build.
Procedure: VCutting.Verification --tests TC-075-001.
Purpose/Expected: AUTO: infer actual axes/angle from legacy/tilted and affine-resized samples; independent center/major/minor/angle edits and wrap; invalid/nonfinite/min/order/bounds unchanged; inner/history/units/DXF; deformed contours refused.
Implementation: EllipsePropertyTests.

## TC-075-002
Execution Type: MANUAL. Status: ACTIVE. Result: PENDING_MANUAL.
Preconditions: Development build, existing/fresh profile, 100%/150% DPI.
Procedure/Expected:  actual property labels English/한국어, no LINE children, length vs radius, invalid restore/no dirty/no Undo; actual preview/move/resize/save/3D and DPI.
Reviewer/time/evidence: pending; no human PASS inferred.
