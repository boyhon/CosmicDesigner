# Freeze Result

## 1. Freeze Baseline
Branch: `boyhon` → `main`.
Commit: `8fe11260f62932021ef92210dd32552abdffd93a` (immutable source Freeze; no amend).
Tree: `b943d308f4a59aebaaf68342f20ad7bc03ea317c`.
Version: `1.20.29-rc1` candidate/documentation, CosmicDesigner `1.20.29`, installer suite `1.1.0` retained.
Date: 2026-10-06 (Asia/Seoul).
The source Freeze contains all then-current managed work. This result is a later documentation commit; binaries were built while HEAD was exactly the Freeze SHA and tracked inputs were unchanged.

## 2. Freeze Scope
302 managed inputs; 225 changed files in the Freeze commit. Classification is recorded in artifact source-manifest.json:

| Category | Files |
|---|---:|
| Source / Configuration | 60 |
| Help / User Documentation | 43 |
| Resources | 10 |
| Tests | 95 |
| Project Management Artifacts | 86 |
| Build / Installer | 8 |

Source includes five applications, shared modules, projects/solution and settings; Help includes HTML/CSS/JS plus internal traceability/gaps; Resources include icons, Slit PNG and user-provided reference PowerPoint; Tests include verification executables' source and accumulated immutable TC specifications/results; management includes AGENTS, CR-001~064, requirements, policies, historical manuals and source requests; Installer includes scripts/config/resources. No new functionality added.

## 3. Excluded Files
Build intermediates/output, IDE/user settings, .dotnet/cache/log/temp/secret files, EXE/DLL/MSI/ZIP outputs and Office ~$ lock file excluded. No original reference material deleted. Binaries remain in ignored artifacts output. Secret/private-user-path/resource audit: 302 candidates, 292 text/Office sources, 0 findings after repository-relative historical scripts and redacted temporary-path prefixes. This is a pattern audit, not a claim of an exhaustive security review. Historical non-secret machine paths in test evidence are context, not required build dependencies. Whitespace-only normalization fixed 45 documentation EOF warnings without changing test expectations.

## 4. Build Result
Executables: `stage/{CosmicDesigner,DXFExplorer,DXFViewer,DXFSimulator,DXFDrawer}.exe`.
Build Configuration: Release / win-x64 / self-contained / .NET 10.
Build Status: PASS for all five publishes and hash-based shared-file collision checks. Verification project builds: 0 warnings, 0 errors.
Rebuilt or Reused: rebuilt all five. Reason: prior outputs did not provide exact current Freeze source provenance.
CosmicDesigner.exe FileVersion: `1.20.29.0`; ProductVersion: `1.20.29+8fe11260f62932021ef92210dd32552abdffd93a`, confirming build/source linkage.
Stage: 452 files / 148,701,771 bytes. Main executable SHA256: `6dcd5170f78e452db761e1ec306fe85ebfe820f7868a4a6de8cb361847e7a98c`.

## 5. Installer Result
Installer: `artifacts/release/1.20.29-rc1/output/DXFExplorerSetup.exe`.
Version: suite `1.1.0`, candidate `1.20.29-rc1`; unchanged existing AppId/product name.
Compiler: Inno Setup 6.7.3, official signed installer checked before user-scope tooling installation ([official download source](https://jrsoftware.org/isdl.php)). Chocolatey machine installation failed due non-admin permissions; official user-scope tooling succeeded.
Compile: PASS (34.656 seconds); 46,789,650 bytes.
SHA256: `1f5a2094db73d95a1de5326c306ac61af97b98908a4c398ca822a216c0f95299`.
Help Included: all 35 product HTML assets plus CSS/JS, including 22 CosmicDesigner pages; original-vs-stage content verification PASS. Internal Help Markdown/JSON excluded from runtime distribution.
Runtime Resources Included: five applications, .NET/Desktop runtime and DLLs, localized resources and embedded application resources. No developer-source directory fallback required by CosmicDesigner Help.

## 6. Installation Test
Install: NOT_RUN. Current process is not an administrator, same-AppId install records already exist in both machine/user registry locations, Windows Sandbox is unavailable. Installing with the same AppId could overwrite existing registration even with a separate folder; existing installation and applications were preserved.
Program Start: supplemental AUTO PASS for all five stage EXEs from an empty working directory, main windows created/input-idle, normal CloseMainWindow/exit observed.
Help: AUTO stage/distribution link/asset checks PASS; actual installed menu/browser acceptance PENDING_MANUAL.
Resource Load: AUTO startup/main-window creation plus source-resource/project audit PASS; visual acceptance remains pending.
Result: packaging and portable-stage startup verified; actual clean install/upgrade/uninstall and installed UI behavior require disposable Windows environment. Do not describe these as installed human PASS.
The first startup probe was inconclusive due asynchronous initialization/close timing; bounded waits corrected the probe, and all five passed. No application fix was needed.

## 7. Test Result
Unit / Integration: ARC unit checks, DXF line analyzer, simulation self-tests, DXFDrawer geometry/round-trip PASS.
Regression: CosmicDesigner full 73/73 AUTO PASS; FAIL 0 / NOT_RUN 0. All existing expectations preserved. Exact Freeze source checked unchanged before/after packaging.
Manual: no new human execution; manifest MANUAL 67 NOT_RUN in this operation; prior historical acceptance preserved. TC-064-002/003 PENDING_MANUAL. SEMI_AUTO/NOT_AUTOMATED 0.
Documentation: source, stage, distribution 35 HTML PASS; links/anchors/UTF-8/assets/navigation/EXE-relative Help routes covered by TC-064-001 and existing Verify-Help. Frozen Help source-stage hashes compared.
Startup: supplementary AUTO 5/5 PASS (not MANUAL TC approval).
Raw artifact evidence: regression-{preflight,frozen}.txt, startup-smoke.json; source-manifest.json, checksums.txt and package-manifest.json.

## 8. Git Result
Commit: source Freeze `8fe1126`; one logical source Baseline includes CR-037~064 implementation/documents plus all previously managed inputs. CR lifecycle unchanged.
Push: source Freeze successfully pushed, normal `git push origin boyhon`, no force.
Remote Branch: `origin/boyhon`.
Follow-up: result/CR linkage documentation is a separate commit. Freeze commit is never amended; application, Help, resource, test and installer inputs do not change in the follow-up.

## 9. Pull Request
Title: Release candidate: freeze source, help, resources and installer baseline (1.20.29-rc1).
Target Branch: main.
PR URL: [boyhon/CosmicDesigner #1](https://github.com/boyhon/CosmicDesigner/pull/1).
Status: Draft; baseline review requested. Required human acceptance pending, not production release or READY FOR MERGE. Attached to this chat.

## 10. Release Artifacts
Root: `artifacts/release/1.20.29-rc1/` (ignored, outside Git source management).
Executable: `stage/CosmicDesigner.exe` plus all four suite apps and runtime files in stage.
Installer: `output/DXFExplorerSetup.exe`.
Checksum: `checksums.txt` (all 452 staged files + installer), `package-manifest.json`.
Release Note: `release-notes.md`.
Freeze inventory: `source-manifest.json`, all 302 source paths/categories/hashes and exact Commit/Tree.
Installation instructions: run installer on Windows x64 with administrator approval, preserve any current work first; disposable clean environment required for acceptance. The self-contained stage also runs without separately installing .NET.

## 11. Known Issues
Existing ASCII LINE/CIRCLE/ARC subset, non-cm general import treated as mm, unsupported binary DXF, compound crossed-hinge fallback and legacy cm labels. About has older hardcoded version text; built assembly metadata is authoritative. No behavior changes made to fix these during Freeze.

## 12. Documentation Gaps
Retain Help's five gaps: current screenshots, actual installer lifecycle, Micro Joint creation UI, non-cm label verification, complex fold/general-DXF cases. Installer compile/package/startup is now verified, but installed human review remains outstanding. No invented screenshots or human PASS.

## 13. Remaining Risks
Clean/default/custom path install, upgrade/uninstall, browser display/print/readability, actual Help menu clicks and historical GUI acceptance are pending. Installer/binaries unsigned by the project; compiler download signature is a separate check. Artifacts are local outputs, not published GitHub Release assets. NuGet/toolchain restoration needs network on a new machine; deterministic byte-identical builds are not promised. Draft PR preserves the Freeze for review, and manual verification gates remain enforced.
