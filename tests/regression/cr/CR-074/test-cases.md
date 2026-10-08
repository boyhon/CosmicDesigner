# CR-074 accumulated cases
## TC-074-001
Execution Type: AUTO. Status: ACTIVE.
Purpose: direction, preview/persistence, existing ellipse edits and UI contract.
Preconditions: Development Release build; bundled en/ko.
Procedure: VCutting.Verification --tests TC-074-001.
Expected: horizontal/vertical/four diagonal axis endpoints/2:1/perpendicular axes; 48 closed LINE preview equals create/inner; boundary uniform fitting, tiny/nonfinite rejected; move/affine resize/unit/DXF/Undo preserve contour; Flat mask and 3D exclude holes; toolbar order/icon and en/ko text consistent.
Implementation: VCutting.Verification/EllipseTests.cs::Run.
## TC-074-002
Execution Type: MANUAL. Status: ACTIVE. Result: PENDING_MANUAL.
Preconditions: CR-074 Development build; fresh/existing profiles; 100%/150% DPI.
Procedure: choose Ellipse immediately after Circle; drag horizontally, vertically and in four diagonals. Inspect dashed preview vs release and Select restoration, short-click/Esc cancel, near-boundary clipping. Move/resize/Undo/Redo/save/reopen; inspect tilted cut in Flat and 3D. Switch English/한국어 and check icon/tooltip/accessibility/layout.
Expected: supplied-reference-style outline icon, one correctly tilted ellipse, no duplicate cuts/overflow; geometry and hole remain consistent; old shapes unaffected.
Reviewer/time/evidence: pending; never infer human PASS from AUTO.
