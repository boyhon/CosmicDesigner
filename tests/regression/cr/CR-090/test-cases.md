# CR-090 tests

## TC-090-001
Execution Type: AUTO. Status: ACTIVE.
Purpose: prevent omission of converter during full package assembly.
Precondition: repository with installer and verification build.
Procedure: --tests TC-090-001.
Expected: publish list and required-stage checks name CosmicConvert; Start Menu points to CosmicConvert.exe; main executable remains CosmicDesigner.exe.
Implementation: VCutting.Verification/PackageTests.cs. Actual frozen publish/compile is recorded separately.

## TC-090-002
Execution Type: MANUAL. Status: ACTIVE.
Purpose: actual installation lifecycle and shortcut acceptance.
Precondition: generated RC installer; test system and existing user settings backup.
Procedure: install/upgrade, launch CosmicDesigner and CosmicConvert from Start Menu; check Help/About/version; convert an SVG; uninstall and verify settings retained.
Expected: all six programs available, correct RC/build identities, existing upgrade/settings preserved.
Result: PENDING_MANUAL; no human reviewer/time/evidence supplied.
