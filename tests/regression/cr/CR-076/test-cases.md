# CR-076 cases

## TC-076-001
Execution Type: AUTO. Status: ACTIVE.
Preconditions: Development Release build.
Procedure: VCutting.Verification --tests TC-076-001.
Purpose/Expected: AUTO: actual assembly name and CosmicDesigner.exe/dll, installer references/AppId/package path.
Implementation: EllipsePropertyTests.

## TC-076-002
Execution Type: MANUAL. Status: ACTIVE. Result: PENDING_MANUAL.
Preconditions: Development build, existing/fresh profile, 100%/150% DPI.
Procedure/Expected:  start CosmicDesigner.exe; future authorized installer clean install/upgrade and shortcut/run/uninstall; legacy settings and document preserved. Actual installer not created by this task.
Reviewer/time/evidence: pending; no human PASS inferred.
