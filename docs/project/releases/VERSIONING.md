# VCutting version / Freeze / release procedure

Authority: release/Version.props, release/versioning.py, ADR-001. Internal development version is independent of customer version. Current state: Development, next customer target 1.0.0. No VCutting 1.0.0 RC has been issued by CR-070.

## Development
Ordinary `dotnet build VCutting.sln -c Release` remains Development even in Release configuration. About and generated Help version display Development. Internal version and source/build identity appear in diagnostics. Never edit the installer version manually or infer customer version from CR count.

## Authorized Freeze
Only after the human explicitly requests Freeze, preserve that approval as a text file with user/source/date. Commit source and check clean worktree. Retain any pending human acceptance in the record; RC may be prepared for acceptance but is not a production release. Set the next release target only under approved scope (first 1.0.0, later PATCH fixes/MINOR features/MAJOR incompatible changes). Do not alter epoch after release.

Use installed Python 3 (no dependency packages):

```powershell
python release/versioning.py freeze --approval <user-approval-text-file>
```

This allocates the next RC and exclusive-creates release/freezes/1.0.0-rc.1.json. Commit that record without amending the source commit. Input comparison excludes only release/freezes and release/builds, allowing ledger-only commits. Code/resources/installer/dependencies/Help/policies/documents/untracked inputs are included. Dirty sources must be committed before allocation. Record all approvals truthfully.

## Frozen installer build / rebuild
```powershell
python release/versioning.py validate --freeze release/freezes/1.0.0-rc.1.json
./installer/Build-Installer.ps1 -FreezeRecord release/freezes/1.0.0-rc.1.json -BuildNumber <unique-build-id> -OutputRoot artifacts/release/<unique-build-id> -PythonPath <python-executable> -IsccPath <ISCC.exe>
```

No allocation occurs in a build. It validates before any output reset, creates version properties in artifacts, validates each app build and validates again before compile/evidence. Requires Release/win-x64/self-contained. Same source/Freeze uses same customer version; each build has unique identity/checksum evidence. Do not overwrite previously distributed artifact directories. Build evidence is exclusive-create; duplicate build identifiers are rejected.

Changed input or version mapping blocks the selected Freeze. Restore/check out exact source for a rebuild or perform a new explicitly authorized Freeze after committing changes. Existing CR corrections also count; lack of a new CR does not authorize reusing a released version for changed content.

Generated app metadata and Help/js/version.js derive from the Freeze. Static source Help shows Development; delivered Help executes the local version script and shows the customer version. JS-disabled Help retains Development fallback; do not claim this fallback is an approved RC. Windows numeric mapping and ordering: ADR-001.

## Production release
Require separate explicit user release approval and complete verification JSON containing exact candidateVersion/sourceCommit, automated[] and human[] results. Each result has testId/result PASS; human also reviewer/observedAt/evidence. Include every ACTIVE AUTO/BASE and MANUAL/SEMI_AUTO ID from the manifest; unresolved ACTIVE NOT_AUTOMATED blocks release.

```powershell
python release/versioning.py promote --freeze release/freezes/1.0.0-rc.2.json --approval <release-approval-text-file> --verification <actual-verification-json>
```

This creates immutable 1.0.0 metadata without changing source. Build and verify the final package through the same installer procedure. Keep candidate and final records/artifacts/checksums separately. Publishing/shipping remains a separate explicitly authorized action. Record final tests before claiming release completion. Next target is explicitly approved; no increment driven solely by build counts or CR counts.

## Regression
Set VCUTTING_RELEASE_PYTHON to the installed Python executable, then run VCutting.Verification --tests TC-070-001. The Python fixture simulates approval and human evidence only inside temporary Git repositories. It cannot certify actual human acceptance or issue a production Freeze for this repository.
TC-065-001 validates installer gates by default; to validate an actually approved package, set COSMIC_INSTALLER_PATH and VCUTTING_INSTALLER_BUILD_INFO to its installer/build.json. No selected candidate means actual new installer compile is NOT_RUN. Historical packages are not validated as newly approved releases.

## Frozen toolchain
Each Freeze records the selected dotnet SDK and the policy-pinned Inno Setup engine (release/Version.props). SDK changes block builds; installer builds probe and reject a different Inno engine before compile. Dependency versions/configuration are source inputs; retain exact versions and committed lock files for future package dependencies. Freeze/build evidence has a checksum linked to its immutable source record.

## Development artifact reporting — user2026-10-07
Use existing -p:VCuttingBuildIdentity=<commit>-working-tree.<task>.<UTC-or-unique-id> for separately identifiable development artifacts. Record ID/output path and verify actual AssemblyMetadata BuildIdentity; include exact value in completion report so Help > About can identify a running window. Rebuilds of same development artifact may retain its identifier; distinguish separate delivered artifacts. Do not modify approved Freeze identity rules or allocate customer versions.
