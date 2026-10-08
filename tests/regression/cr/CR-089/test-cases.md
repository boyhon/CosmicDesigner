# CR-089 regression

## TC-089-001
Execution Type: AUTO. Status: ACTIVE.
Purpose: restore actual SectionDesignerView viewport and editor alignment without altering design/options.
Preconditions: Windows .NET 10 WPF build.
Procedure: run VCutting.Verification --tests TC-089-001. STA thread creates H/W views, Flat/Bent and all four rotations; records editor positions; sets zoom5/pan400,-300; refreshes and fits twice; also fits an empty view.
Expected: zoom1/pan0, identical baseline editor positions, rotation/mode/section position/document reference preserved; no exception.
Implementation: VCutting.Verification/SectionFitTests.cs.

## TC-089-002
Execution Type: MANUAL. Status: ACTIVE.
Purpose: actual button and visual fit acceptance.
Preconditions: CR-089 development app; H/W sections with bends and an outer notch.
Procedure: in each H/W panel zoom until clipped, click its Fit/Reset. Repeat Flat/Bent, all rotations, dimensions on/off, narrow/wide resized panels and disconnected sections. Verify opposite panel remains unchanged. Repeat in English/Korean; edit a dimension after fitting; confirm Help instructions.
Expected: entire section centered with original proportions, suitable horizontal/vertical margins, aligned usable dimension editors, current options/geometry retained; button visible and operable in both languages.
Result: PENDING_MANUAL. Reviewer/time/evidence: not yet supplied.
