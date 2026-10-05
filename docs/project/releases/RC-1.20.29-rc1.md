# Cosmic Designer 개발 — 1.20.29-rc1 Freeze Baseline

Freeze Date: 2026-10-06 (Asia/Seoul)
Freeze Branch: boyhon
Target Branch: main (GitHub default; no open boyhon PR at intake)
Freeze Commit: the commit introducing this document; resolve with `git log --diff-filter=A --format=%H -- docs/project/releases/RC-1.20.29-rc1.md`. Do not amend it. Artifact release-notes/checksums record its exact SHA after creation.
Candidate ID / Documentation Baseline: 1.20.29-rc1
Application Version: CosmicDesigner 1.20.29
Installer Suite Version: DXFExplorer 1.1.0 (existing separate product-suite version retained)
Build: Release / win-x64 / self-contained, five WPF applications, .NET 10 SDK
Status: Release Candidate, required human acceptance pending; not a production release or merge approval.

## Purpose and source
Freeze all currently managed source, documentation, resources, tests and reproducible build/installer definitions. User original request: FREEZE_REQUEST-2026-10-06.md. No new functional CR; this is release management. Only minimal build portability, output isolation, ignore/security fixes are allowed. Existing CR-064 Help integration is included, not reimplemented.

## Freeze scope
Include all tracked inputs and inspected untracked implementation through CR-064: application/shared source and projects/solution, icons/SVG/PNG, help-content HTML/CSS/JS and internal traceability/gaps, AGENTS/project requirements/CR/status/ADR/changelog, regression manifest/spec/results, verification projects, installer source/scripts/docs, historical manuals and user-provided reference PowerPoint. The PowerPoint is design/reference evidence for recent CRs; preserve it. Its ~$ Office lock file is excluded without deletion.

Exclude bin/obj, IDE/user settings, artifacts, installer publish/stage/output, caches/log/temp, credentials and reproducible EXE/DLL/archives. Release binaries are outside Git under artifacts/release/1.20.29-rc1. No source/resource deletions, history rewrites or force pushes.

## Included CRs and functionality
CR-001~064 current implementation and records. Principal additions since last committed CR-036: cleaned Hole toolbar and direct sizing/properties, boundary/Fillet/contour fixes, semicircle and quarter circle, Slit Line/Arc/Polyline, Flat V/V1 drawing/movement, section dimensions/thickness/gaps, compound fold preview, midpoint guides, HTML Help baseline. Historical lifecycle and acceptance remain unchanged; implemented does not mean human-verified.

## Build and reproducibility
Windows x64, .NET 10 SDK, Inno Setup compiler; NuGet packages restored using installer/NuGet.Publish.Config. Python 3 stdlib runs source audit.

```powershell
python installer/Verify-Freeze.py
dotnet build CosmicDesigner.Verification -c Release -p:BaseOutputPath=artifacts/rc1-build/
dotnet artifacts/rc1-build/Release/net10.0-windows/CosmicDesigner.Verification.dll
./installer/Build-Installer.ps1 -OutputRoot "$PWD/artifacts/release/1.20.29-rc1"
./installer/Verify-Help.ps1 -Root artifacts/release/1.20.29-rc1/stage
```

Use an absolute BaseOutputPath if required by local MSBuild. Installer reset validates its resolved publish/stage paths inside the selected repository output root before recursive deletion; existing normal output is not touched when using RC OutputRoot. Missing compiler/network/privileges must be reported, not bypassed. Default installation retains existing AppId and admin/all-users policy.

## Verification plan and status
AUTO: existing CosmicDesigner full 73 checks including TC-064-001; shared DXF ARC/simulation self-tests; DXFDrawer geometry tests; Help source/stage links and source/runtime assets. Preserve existing expectations.
MANUAL: TC-064-002/003 and earlier required UI cases remain pending absent human reviewer/time/evidence. Automated silent installation or process startup, if possible, is supplementary and does not convert MANUAL to PASS.
Preflight and exact frozen-source build/installer results are recorded in FREEZE-RESULT-2026-10-06.md in a subsequent documentation commit and the ignored artifact release notes. This initial baseline records the scope before exact-commit artifact generation.

## Known limitations and documentation
22 CosmicDesigner HTML pages; 35 total product HTML assets. Five documented gaps: current screenshots, actual installation lifecycle, Micro Joint creation UI, remaining cm labels on non-cm input, compound/crossed fold and general DXF edge cases. ASCII LINE/CIRCLE/ARC import only; no binary DXF; general import treats non-cm units as mm. About has older hardcoded version text; assembly/project version is the baseline authority. Freeze does not change these normal behaviors.

## Git and artifacts
One source Freeze commit on boyhon, normal push to origin, draft PR to main while required human verification remains. Executables + installer + SHA256 checksums + exact source/tree SHA + release notes are artifact outputs. Final source manifest/hashes and complete managed file inventory are derived from the Freeze commit. No amend; any reporting/fixes use separate commits.
