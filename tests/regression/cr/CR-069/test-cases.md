# CR-069 accumulated cases

## TC-069-001
Execution Type: AUTO. Status: ACTIVE.
Purpose: Rename and legacy persistence compatibility
Preconditions: Windows .NET 10; CR-069 build
Procedure: --tests TC-069-001
Expected: VCutting project/assembly/UI bindings; legacy settings/metadata/AppId contracts preserved
Implementation: VCutting.Verification/Program.cs::RenameCompatibility
Result: PASS (2026-10-06; see results/CR-069-2026-10-06.md)

## TC-069-002
Execution Type: MANUAL. Status: ACTIVE.
Purpose: Renamed UI and real installation/data acceptance
Preconditions: Windows .NET 10; CR-069 build
Procedure: User checks UI/Help, existing settings and DXF; installs/updates/removes candidate on test machine.
Expected: VCutting titles/About/Help, previous Recent/settings/DXF intact; install/upgrade/uninstall correct
Implementation: Human review
Result: PENDING_MANUAL

CR-069 approves branding changes in TC-024/064/065/066; CR-068 approves default folder. IDs, purpose and non-branding assertions retained.
