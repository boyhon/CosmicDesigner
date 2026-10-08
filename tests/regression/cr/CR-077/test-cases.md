# CR-077 accumulated regression cases

## TC-077-001
Execution Type: AUTO. Status: ACTIVE.
Preconditions: Windows .NET 10 Development Release build.
Procedure: VCutting.Verification --tests TC-077-001.
Expected: circle–rectangle–circle connected group merges transitively; independent object/Outer unchanged; exact ARC and LINE preserved; one selected Cut/Inner; containment/duplicate/shared-edge union; point contact/disjoint/boundary-cut rejected; all supported shape union; one Undo/Redo; repeated merge and IDs; curve uniform scaling rejects affine distortion; LINE compound affine scaling; ring material island loops/Flat/3D remain; DXF metadata and physical units preserved; localized labels.
Implementation: VCutting.Verification/CutUnionTests.cs::Run.

## TC-077-002
Execution Type: MANUAL. Status: ACTIVE. Result: PENDING_MANUAL.
Preconditions: CosmicDesigner.exe Development, English/한국어, 100%/150% DPI.
Procedure/Expected: reproduce attached circle–rectangle–circle and separate tilted ellipse. Select any connected object: orange selection and connected outlines; right-click → Merge overlapping cuts/겹친 천공 통합 creates one Inner object without seams, ellipse/Outer unchanged. Undo once restores all, Redo repeats. Move/resize compound; curved shape keeps arc curvature and equal ratio, LINE-only shape allows independent axes. Repeat merge, save/load, flat/3D preview; ring keeps material island and no fake cutting bridge. Move apart removes orange/menu. Check outer merge remains unchanged, point contact does not offer merge. Inspect Korean labels/layout and context menu. Multiple-loop fillet is unavailable; single-loop fillet remains supported. Actual installer validation only after separately authorized Freeze.
Reviewer/date/evidence: pending; no human PASS inferred.
