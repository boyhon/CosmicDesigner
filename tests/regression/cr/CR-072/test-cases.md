# CR-072 Test Cases
## TC-072-001
Execution Type: AUTO
Status: ACTIVE
Purpose: language catalog loading, override/reset/new-language, persistence, malformed UTF8/JSON/duplicate/placeholder fallback, numeric culture and DXF byte invariance, resource shipping.
Preconditions: Release build; no production user settings writes.
Procedure: VCutting.Verification --tests TC-072-001
Expected: requirements above pass; disposable artifacts fixture only.
Implementation: VCutting.Verification/LocalizationTests.cs
## TC-072-002
Execution Type: MANUAL
Status: ACTIVE
Procedure: Settings English/한국어 switching, cancel restores previous language, restart persistence, all menus/dialogs/properties/status/tooltip/drawing guidance; narrow/dpi layout, Korean wording/access keys; custom user resources and actual installation.
Expected: readable correct text/unchanged workflows and physical sizes; actual installed resource loading works.
Result: PENDING_MANUAL; no reviewer evidence.
