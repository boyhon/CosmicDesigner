# CR-064 Documentation Test Cases

## TC-064-001
Execution Type: AUTO. Status: ACTIVE.
Purpose: local manual integrity, deployment equivalence, all-page navigation and Help route/package configuration regression.
Preconditions: Windows/.NET 10; repository Release verification build in isolated output; HTML baseline and manual-sections.json present.
Procedure: dotnet <output>/VCutting.Verification.dll --tests TC-064-001.
Expected: valid UTF-8 declaration/bytes; every HTML href/src and fragment resolves inside deployed root; no external dependencies; source HTML/CSS/JS/images byte-identical to output; all 22 sections linked from every manual page; Korean language/mobile metadata, Home/previous/next/related links; allowed menu routes resolve from arbitrary install location; route traversal rejected; installer includes CosmicDesigner and recursive help; internal Markdown/JSON excluded.
Implementation: VCutting.Verification/Program.cs::UserDocumentation.
Coverage: automated asset and integration contract only; does not launch browser or install an application.

## TC-064-002
Execution Type: MANUAL. Status: ACTIVE. Result: PENDING_MANUAL.
Purpose: user help menu, real browser/local-file layout and content acceptance.
Preconditions: CR-064 Release app with help folder, Edge/Chromium configured, existing design copy.
Procedure:
1. Launch from a directory other than executable directory. Help > User Manual, Getting Started, Keyboard Shortcuts, Troubleshooting each opens corresponding local HTML; About still opens.
2. Use index cards, sidebar search (search 천공 and clear), previous/next/Home/related links. Disconnect internet; links still work. Open index directly with JavaScript disabled; navigation remains usable.
3. Compare screenshots/text with actual menus and objects; walk new design, Hole, Slit, V/V1, Fillet, boundary merge, units, Settings, DXF save/import on a disposable design.
4. Check Korean text, 100%/200% browser zoom, narrow 375px viewport, keyboard focus/skip link and Ctrl+P print preview. Check no clipped essential text; print omits navigation.
5. Temporarily rename help/index.html in a disposable copy, verify missing-help message; restore. Set unsupported HTML association only in disposable environment if feasible and check friendly failure.
Expected: all local targets, understandable correct UI instructions, readable content and print, reversible failure handling; no geometry behavior change.
Evidence required: reviewer, date/time, app/output path/version, browser, observed result per step and images when appropriate. Codex must not invent PASS.

## TC-064-003
Execution Type: MANUAL. Status: ACTIVE. Result: PENDING_MANUAL.
Purpose: actual installer lifecycle coverage.
Preconditions: freshly built installer including CR-064, disposable Windows environment and existing install backup.
Procedure: install fresh to default and custom path with spaces; verify VCutting.exe and all help HTML/assets, launch via Start menu, open four Help routes; upgrade previous package after saving work; confirm help/current assets remain correct; uninstall and confirm installed Help/app removed while externally saved DXF remains.
Expected: installation paths independent, current HTML and CosmicDesigner shipped, shortcuts usable, clean upgrade/removal with user documents preserved.
Evidence required: reviewer/time, installer hash/version, paths, observed fresh/custom/upgrade/removal results. Configuration checks do not count as actual installed PASS.

CR-069 supersedes prior product/output branding expectations; CR-068 supersedes the installer default folder. TC IDs/purpose and other assertions remain unchanged. Historical reports preserve their original results.
