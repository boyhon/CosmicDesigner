# CR-071 Test Cases
## TC-071-001
Execution Type: AUTO
Status: ACTIVE
Purpose: supplied screenshot order/accessibility/provenance/shipping integrity.
Preconditions: Release build inside repository.
Procedure: VCutting.Verification --tests TC-071-001
Expected: two pages have six numbered figures in requested order, nonempty alt; original checksum and PNG signature valid; deployed bytes identical.
Implementation: VCutting.Verification/Program.cs::InstallationScreenshots
## TC-071-002
Execution Type: MANUAL
Status: ACTIVE
Preconditions: offline browser and built Help.
Procedure: open getting-started.html and installation.html; compare all six images/captions/buttons to supplied screens; inspect at desktop and 390px width and print preview.
Expected: legible, correct order, no clipped images; optional selections and historic capture version understood.
Result: PENDING_MANUAL; reviewer/date/evidence not yet supplied.
