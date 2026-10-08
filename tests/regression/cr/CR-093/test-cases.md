# CR-093 cases
## TC-093-001
Execution Type: AUTO. Status: ACTIVE.
Purpose: integration build validation and no customer version allocation.
Procedure: invoke actual PowerShell builder with -ValidateOnly on fresh artifacts path, invalid identifier, outside artifacts path and existing output directory. Compare release/freezes filenames before/after.
Expected: fresh valid path reports INTEGRATION_TEST/Development/internal numeric1.20.29.0; invalid/reused paths fail before output modification; no Freeze file created.
Implementation: VCutting.Verification/PackageTests.cs::IntegrationInstaller.
## TC-093-002
Execution Type: MANUAL. Status: ACTIVE.
Purpose: install/use verification before Freeze decision.
Procedure: run delivered VCuttingSetup-IntegrationTest.exe; follow installation flow, launch CosmicDesigner/CosmicConvert and auxiliary apps; inspect About/Help Development and recorded BuildIdentity; test Fit/Reset, SVG conversion/import/save/2D/3D and intended work; verify settings and upgrade/uninstall using TC-090-002. Report observations with reviewer/time/build ID; request fixes if needed, then explicitly approve Freeze when satisfied.
Expected: real installation and use work; no customer release implied, correct build identifiable.
Result: PENDING_MANUAL; human evidence not supplied.
