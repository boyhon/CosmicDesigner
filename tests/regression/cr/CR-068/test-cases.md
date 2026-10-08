# CR-068 accumulated cases

## TC-068-001
Execution Type: AUTO. Status: ACTIVE.
Purpose: Installer default folder and upgrade contract
Preconditions: Windows .NET 10; CR-069 build
Procedure: --tests TC-068-001
Expected: DefaultDirName VCutting and legacy AppId/previous folder reuse
Implementation: VCutting.Verification/Program.cs::InstallerBranding
Result: PASS (2026-10-06; see results/CR-069-2026-10-06.md)

## TC-068-002
Execution Type: MANUAL. Status: ACTIVE.
Purpose: New installation default path
Preconditions: Windows .NET 10; CR-069 build
Procedure: User checks UI/Help, existing settings and DXF; installs/updates/removes candidate on test machine.
Expected: Uninstall then new install proposes C:\Program Files\VCutting
Implementation: Human review
Result: PENDING_MANUAL

CR-069 approves branding changes in TC-024/064/065/066; CR-068 approves default folder. IDs, purpose and non-branding assertions retained.
