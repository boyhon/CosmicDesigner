# CR-073 Test Cases
## TC-073-001
Execution Type: AUTO
Status: ACTIVE
Purpose: fresh mm defaults, missing-unit legacy cm, explicit stored dimensions, corrupt settings recovery, conversion/physical size invariance.
Preconditions: Release build; no production user settings writes.
Procedure: VCutting.Verification --tests TC-073-001
Expected: requirements above pass; disposable artifacts fixture only.
Implementation: VCutting.Verification/LocalizationTests.cs
## TC-073-002
Execution Type: MANUAL
Status: ACTIVE
Procedure: clean user profile first launch mm/3000×3000/2; old cm/m profiles preserved; old DXF physical sizes; new document and Restore Defaults mm; property dimension labels current unit.
Expected: readable correct text/unchanged workflows and physical sizes; actual installed resource loading works.
Result: PENDING_MANUAL; no reviewer evidence.
