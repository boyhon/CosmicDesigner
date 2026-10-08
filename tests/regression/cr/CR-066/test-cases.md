# CR-066 Cases

TC-066-001: ACTIVE; Execution Type: AUTO. Purpose: setup branding and launch contract. Preconditions: repository build. Procedure: --tests TC-066-001. Expected: CosmicDesigner AppName/default folder/group/desktop/run and unchanged AppId. Implementation: Program.cs::InstallerBranding.

TC-066-002: ACTIVE; Execution Type: MANUAL. Purpose: actual installer branding. Preconditions: isolated Windows environment. Procedure: open Korean/English installer, review title/body/desktop task, install, open desktop shortcut and postinstall run; verify installed-app name and upgrade previous installation. Expected: CosmicDesigner wording and launch, same-AppId upgrade. Result: PENDING_MANUAL. Reviewer/date/evidence required.

CR-069 supersedes prior product/output branding expectations; CR-068 supersedes the installer default folder. TC IDs/purpose and other assertions remain unchanged. Historical reports preserve their original results.
