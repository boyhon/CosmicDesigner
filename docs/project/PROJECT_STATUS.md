# VCutting 개발 — Project Status

## Baseline

- Date: `2026-10-01`
- Document: [BASELINE-2026-10-01.md](BASELINE-2026-10-01.md)
- Basis: current source, solution, verification projects, installer definition, and repository documents available in the workspace
- Git note: no `.git` directory was available in this workspace when the baseline documentation was introduced; no commit hash can therefore be asserted

## Current Version

- Internal development version: `1.20.29` (release/Version.props).
- Customer version: not assigned; ordinary builds display `Development`.
- First future approved Freeze target: `1.0.0-rc.1`; no Freeze/release allocated by CR-070.
- Installer builds now require an approved immutable Freeze; historical 1.20.29-rc1 and CR-069 candidate artifacts remain preserved.

## Current Development Phase

CR-070 implemented: internal 1.20.29 / customer Development; next target 1.0.0-rc.1, not issued. New/affected/full AUTO 1/1, 6/6, 78/78 PASS; final Release 0 warnings/errors. Mandatory human TC-070-002 pending. No new Freeze/installer/release performed. See releases/VERSIONING.md and tests/regression/results/CR-070-2026-10-06.md.

CR-069: VCutting 이름 변경 구현 완료; 필수 사람 검증 대기. 공식 관리명 VCutting 개발. 아래 기존 Freeze 설명은 2026-10-06 변경 전 기록이다.

2026-10-06 release-management operation: preparing 1.20.29-rc1 Freeze of all managed work through CR-064 on boyhon for draft PR to main. No new CR number consumed; next remains CR-065. Full AUTO preflight 73/73 PASS. Source/Help/resources/tests and installer definitions are frozen together; human acceptance is not promoted by this operation. See releases/RC-1.20.29-rc1.md.

CosmicDesigner 1.20.29 source includes CR-064 Help baseline alongside CR-001~063 implementation. CR-064 is unreleased; required browser and actual installer acceptance remains pending. The left object/property area shows OBJECTS above PROPERTIES. Flat Designer adds Line/Arc/Polyline open L-layer slit tools using the user-supplied PNG icons. Required human UI acceptance remains pending.

## Recovery Audit — 2026-10-01

- Confirmed that there was no `In Progress` or `Open` CR and did not create a new CR.
- Reconciled CR-002/CR-003 settings behavior with the safe-fallback requirement: malformed JSON and invalid recent-file paths now fall back or are discarded without preventing startup or later recent-list updates.
- Extracted the Flat Designer viewport calculation into a shared, testable transform and added coordinate round-trip, anchored Zoom, and grid-step regression checks for CR-004/CR-005.
- Rebuilt the CosmicDesigner application and verification executable with zero warnings and errors; all automated verification checks passed.
- Republished the current `1.2.0` application to `artifacts/CosmicDesigner-1.2.0`.
- A running CosmicDesigner process holds the conventional `CosmicDesigner/bin/Release` executable open, so verification used isolated output directories and did not terminate the user's running application.

## 1.3.0 Implementation — 2026-10-01

- Implemented CR-004 through CR-010 without changing the DXF metadata format.
- Added Flat Designer ruler isolation, explicit Select/Hole mode handling, Flat Delete routing and Object Tree selection synchronization.
- Added global H/W Section view persistence and View-menu synchronization while retaining file-metadata precedence.
- Added common Section wheel Zoom, material-body Bend Hit Testing and single-value dimension rendering.
- Added translucent acrylic 3D surfaces and opaque gray edge geometry.
- Release build completed with zero warnings and errors; all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.3.0/CosmicDesigner.exe` with file version `1.3.0.0`.

## 1.3.1 Correction — 2026-10-01

- Corrected CR-006 Bent Mode dimension geometry so extension lines use measurable exterior polygon edges rather than center-axis bend points.
- Removed unselected red/blue bend dots from Bent Mode and changed selected-bend feedback to a boundary across material thickness.
- Published `artifacts/CosmicDesigner-1.3.1/CosmicDesigner.exe` with file version `1.3.1.0`.

## 1.3.2 3D Preview Correction — 2026-10-01

- Replaced the dark preview space with a white background.
- Simplified the material to low-opacity, two-sided acrylic surfaces without directional lighting or shading.
- Changed all exterior and bend-edge geometry to opaque black.
- Published `artifacts/CosmicDesigner-1.3.2/CosmicDesigner.exe` with file version `1.3.2.0`.

## 1.3.3 3D Visibility Correction — 2026-10-01

- Increased the light-gray acrylic surface opacity so geometry remains visible against the white background.
- Increased black exterior/bend Edge thickness and switched Edge material to black diffuse rendering.
- Release build and all verification checks passed; native Windows screenshot automation was unavailable for final visual acceptance.
- Published `artifacts/CosmicDesigner-1.3.3/CosmicDesigner.exe` with file version `1.3.3.0`.

## 1.3.4 Section Dimension Correction — 2026-10-01

- Aligned Bent dimension witness points and editor anchors with the rendered dark material surface edges rather than invisible center-axis coordinates.
- Corrected each Bent segment span to include the opposing exterior contact faces, leaving the required material-thickness offset between adjacent dimension baselines.
- Added the material-thickness dimension to W Section Designer Flat and Bent views.
- Release build and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.3.4/CosmicDesigner.exe` with file version `1.3.4.0`.
- Published the follow-up caliper-measurement correction separately at `artifacts/CosmicDesigner-1.3.4-caliper-fix` because the original artifact was in use.

## 1.4.0 Section Selector Linkage — 2026-10-01

- Added labeled H and W dashed section selectors to Flat Designer.
- H Section moves horizontally by X coordinate; W Section moves vertically by Y coordinate.
- Linked selector movement to the corresponding Section Designer position display and refresh.
- Added material-bound clamping and automated selection-position checks.
- Release build and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.4.0/CosmicDesigner.exe` with file version `1.4.0.0`.

## 1.4.1 Bent Fit Scale Correction — 2026-10-01

- Replaced the fixed 190-pixel Bent viewport deduction with adaptive horizontal and vertical dimension margins.
- Applied one shared Fit scale to Bent geometry and dimension editors.
- Added short-wide W Section and maximum-scale regression checks.
- Release build and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.4.1/CosmicDesigner.exe` with file version `1.4.1.0`.

## 1.4.2 Centered Section Zoom — 2026-10-01

- Changed H/W Section wheel Zoom from pointer-anchored Pan accumulation to viewport-center scaling.
- Reset Section Pan on each Zoom step so geometry remains centered through repeated scaling.
- Preserved Flat Designer pointer-anchored Zoom and Section scale bounds.
- Release build and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.4.2/CosmicDesigner.exe` with file version `1.4.2.0`.

## 1.5.0 Boundary Cut Merge — 2026-10-02

- Converted boundary-touching Rectangle Cuts on the basic rectangular material into open Outer Contour notches.
- Removed merged Cut and Inner Contour objects; corner and edge-center notches produce six and eight LINEs respectively.
- Preserved fully interior Rectangle Cuts and declined automatic merges that would split the material across opposite boundaries.
- Release build, complete verification and DXF metadata round-trip checks passed.
- Published `artifacts/CosmicDesigner-1.5.0/CosmicDesigner.exe` with file version `1.5.0.0`.

## 1.5.1 Explicit Boundary Merge Workflow — 2026-10-02

- Kept boundary-touching Rectangle Cuts editable instead of merging immediately.
- Added orange merge-candidate rendering, status guidance and an explicit right-click merge command.
- Generalized sequential Rectangle merging to the current rectilinear Outer Contour.
- Added candidate-state, four-corner sequential merge, Undo and DXF round-trip regression coverage.
- Release build and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.5.1/CosmicDesigner.exe` with file version `1.5.1.0`.
- Implementation commit: `ade8b61`.

## 1.6.0 Selected Local Section — 2026-10-03

- Connected H/W Section geometry to the actual scan-line intersection of the selected Flat Designer coordinate and current Outer Contour.
- Shortened Flat and Bent profiles at merged boundary notches and excluded bends outside the remaining local material interval.
- Preserved stored SectionSegment lengths and editable dimensions for uncut full sections; clipped contour-derived dimensions are read-only.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.6.0/CosmicDesigner.exe` with file version `1.6.0.0`.
- Implementation commit: `daad64f`.

## 1.7.0 Outer Contour Direct Editing — 2026-10-03

- Added direct Flat Designer Hit Testing and selection for Outer Contour LINE objects.
- Added orientation-aware cursors and perpendicular body dragging for LINE movement.
- Added endpoint-handle Resize with material-bound and minimum-length constraints.
- Propagated changed shared vertices to connected neighboring LINEs so the Outer Contour remains connected.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.7.0/CosmicDesigner.exe` with file version `1.7.0.0`.
- Implementation commit: `2b1a9d3`.

## 1.8.0 Clipped Section Dimension Editing — 2026-10-03

- Replaced read-only clipped Section dimensions with editable Flat/Bent dimension controls.
- Propagated an edited local length through subsequent bend stations and the intersected Outer Contour boundary.
- Reused connected-vertex editing so both LINEs attached to a moved boundary remain connected.
- Preserved other local segment lengths and the existing Bent exterior-thickness correction policy.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.8.0/CosmicDesigner.exe` with file version `1.8.0.0`.
- Implementation commit: `037bce9`.

## 1.9.0 Contour-aware 3D Preview — 2026-10-03

- Replaced the full rectangular tensor-grid surface with Outer Contour-clipped cell triangulation.
- Removed 3D faces from merged notch areas and generated black exterior edges from the resulting mesh boundary.
- Split the mesh at W/H bend stations and retained visible fold edges after contour clipping.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.9.0/CosmicDesigner.exe` with file version `1.9.0.0`.
- Implementation commit: `0ecd6cd`.

## 1.10.0 File Command Shortcuts — 2026-10-03

- Added global Ctrl+N, Ctrl+O and Ctrl+S handling through the existing New/Open/Save command paths.
- Added matching shortcut text to the File menu items.
- Retained Save-to-Save-As fallback and existing Ctrl+Z/Ctrl+Y behavior.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.10.0/CosmicDesigner.exe` with file version `1.10.0.0`.
- Implementation commit: `24762b7`.

## 1.11.0 Unsaved Changes Confirmation — 2026-10-03

- Added document dirty-state tracking for geometry changes, Undo/Redo and file-specific Section view changes.
- Added one Save/Discard/Cancel confirmation path for New, Open, Recent, File Exit and window close.
- Prevented document replacement or shutdown when the user cancels confirmation, cancels Save As or saving fails.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.11.0/CosmicDesigner.exe` with file version `1.11.0.0`.
- Implementation commit: `882f035`.

## 1.12.0 Settings and Document Units — 2026-10-03

- Added Edit > Settings with persistent General, Editing, 3D Preview and New Document preferences without duplicating transient View commands.
- Added Edit > Document Units for mm/cm/m using either preserved numbers or converted values that preserve physical size.
- Stored the active unit in CosmicDesigner metadata and DXF `$INSUNITS`, including Undo/Redo and legacy compatibility.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.12.0/CosmicDesigner.exe` with file version `1.12.0.0`.
- Implementation commit: `c72934b`.

## 1.12.1 Document-first Window Title — 2026-10-03

- Changed the title order to `Filename.dxf - CosmicDesigner` and use `Untitled` for a new document.
- Added an immediate leading `*` indicator whenever the current document has unsaved changes; saving removes it.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.12.1/CosmicDesigner.exe` with file version `1.12.1.0`.
- Implementation commit: `41151ca`.

## 1.13.0 Ruler and Grid Spacing — 2026-10-03

- Added a Settings `Ruler & Grid` tab with independent Auto/custom horizontal Ruler, vertical Ruler and Grid intervals.
- Stored custom intervals as physical lengths and converted them to the active document unit at render time.
- Added adaptive 1·2·5 thinning with 8px Grid and 45px Ruler density thresholds.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.13.0/CosmicDesigner.exe` with file version `1.13.0.0`.
- Implementation commit: `f1486b2`.

## 1.13.1 Flat Cut-area Mask — 2026-10-03

- Added an Even-Odd material mask from the actual Outer Contour and all Cut contours.
- Rendered Cut interiors and Outer Contour exterior as white background, with Grid and bend lines clipped to remaining material.
- Preserved visible contour boundaries and H/W Section selection overlays.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.13.1/CosmicDesigner.exe` with file version `1.13.1.0`.
- Implementation commit: `cf205bf`.

## 1.13.2 Transparent 3D Cut Regions — 2026-10-03

- Added all internal Cut contours to 3D mesh partitioning and excluded triangles whose centers lie in a Cut region.
- Added Cut boundary edges while preserving Outer Contour notches, folds and existing Preview interaction.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.13.2/CosmicDesigner.exe` with file version `1.13.2.0`.
- Implementation commit: `d178aff`.

## 1.13.3 Clean 3D Boundary Edges — 2026-10-03

- Removed internal bend-station and triangulation edges from the 3D Preview.
- Retained only one-sided mesh boundary edges for the Outer Contour and Cut contours while preserving folded surface geometry.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.13.3/CosmicDesigner.exe` with file version `1.13.3.0`.
- Implementation commit: `0592574`.

## 1.13.4 Explicit 3D Contour Edges — 2026-10-03

- Replaced inferred one-sided mesh edges with edges generated explicitly from Outer Contour and Cut contours.
- Split explicit contour edges at bend stations so they follow folded surfaces without adding internal or radial lines.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.13.4/CosmicDesigner.exe` with file version `1.13.4.0`.
- Refinement commit: `94d9fcd`.

## 1.14.0 Colored 3D Bend Edges — 2026-10-03

- Added material-clipped bend edges generated from actual W/H bend stations.
- Rendered V bend edges in red and V1 bend edges in blue while retaining black Outer/Cut contour edges.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.14.0/CosmicDesigner.exe` with file version `1.14.0.0`.
- Implementation commit: `e33bb68`.

## 1.15.0 Triangle Vertex Editing — 2026-10-03

- Replaced Triangle Cut bounding-box Resize with three direct vertex handles.
- Preserved the edited arbitrary triangle while moving the whole shape and kept all vertices within material bounds.
- Kept the Triangle as a closed three-LINE contour for Flat masking, persistence and 3D Cut rendering.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.15.0/CosmicDesigner.exe` with file version `1.15.0.0`.
- Implementation commit: `62bc5b6`.

## 1.16.0 Drag-sized Cut Creation — 2026-10-03

- Added Mouse Down/drag/Mouse Up creation for Circle, Triangle and Rectangle Cuts.
- Added a dashed live preview and ignored drags shorter than four screen pixels.
- Returned to Select mode after one Cut is created to prevent accidental duplicates.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.16.0/CosmicDesigner.exe` with file version `1.16.0.0`.
- Implementation commit: `6fbf06b`.

## 1.17.0 Triangle Boundary Merge — 2026-10-03

- Extended safe boundary merge detection from Rectangle to Triangle Cuts.
- Reused orange candidate highlighting and explicit right-click confirmation.
- Preserved non-orthogonal Triangle edges in the resulting Outer Contour.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.17.0/CosmicDesigner.exe` with file version `1.17.0.0`.
- Implementation commit: `e244747`.

## 1.18.0 Outer Contour Fillet — 2026-10-03

- Added radius input, Fillet mode, applicable-corner Hit Testing and orange dashed preview.
- Trimmed the adjacent LINEs and inserted an exact tangent quarter-circle ARC.
- Preserved the exact ARC in DXF while representing it as a straight chord in 3D Preview.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.18.0/CosmicDesigner.exe` with file version `1.18.0.0`.
- Implementation commit: `09c2abb`.

## 1.18.1 Sequential Fillet Correction — 2026-10-03

- Removed the incorrect requirement that the entire Outer Contour contain only LINE segments.
- Kept validation local to the selected LINE–LINE corner while including existing ARC starts in contour orientation calculation.
- Added four-corner sequential Fillet and four-ARC persistence regression coverage.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.18.1/CosmicDesigner.exe` with file version `1.18.1.0`.
- Implementation commit: `41e53f3`.

## 1.19.0 Fillet ARC Selection and Radius Editing — 2026-10-03

- Added gold ARC highlighting and tangent endpoint handles for Object Tree selection.
- Added the editable `Radius R` property using the active document unit.
- Reconstructed the original corner and recalculated both adjacent LINE endpoints when changing radius.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.19.0/CosmicDesigner.exe` with file version `1.19.0.0`.
- Implementation commit: `d57839a`.

## 1.20.0 Arbitrary-angle and Hole Fillets — 2026-10-03

- Generalized tangent-point calculation to arbitrary convex LINE–LINE interior angles.
- Applied the same Fillet workflow to LINE-based internal Hole contours.
- Added Cut child geometry to the Object Tree for Hole ARC selection and radius editing.
- Approximated Hole ARCs as straight chords in the overview-oriented 3D Preview.
- Release build completed with zero warnings and errors, and all CosmicDesigner verification checks passed.
- Published `artifacts/CosmicDesigner-1.20.0/CosmicDesigner.exe` with file version `1.20.0.0`.
- Implementation commit: `45557d9`.

## Regression Suite Management — 2026-10-05

- Execution Type is mandatory: AUTO / MANUAL / SEMI_AUTO / NOT_AUTOMATED. Registered TC counts: AUTO 49, MANUAL 46, SEMI_AUTO 0, NOT_AUTOMATED 0; six BASE checks are AUTO.
- Automated results: PASS 55, FAIL 0, NOT_RUN 0 (existing recorded run). Manual results: PASS 0, FAIL 0, NOT_RUN 46. No human PASS is inferred.
- Each CR has a separate WAITING FOR USER VERIFICATION gate for newly registered manual acceptance; historical lifecycle/approval records are retained. Required human confirmation must be recorded before Verified/Closed promotion.

- Applied the user CR Regression policy and retrospectively mapped CR-001 through CR-047 to immutable TC IDs.
- Registered 49 automated TC mappings and 46 manual acceptance cases; six unmapped baseline checks remain in the full suite.
- Historical lifecycle states are retained; manual cases are NOT RUN. Harness Release build passed with zero warnings/errors; manifest IDs match --list. CR-046 automatic selection 1/1, affected selection 5/5 and full automatic suite 55/55 reported IDs passed. See tests/regression/results/2026-10-05.md.
- This management/test-harness migration does not change application behavior or consume CR-047.

## In Progress

- None.

## Open Change Requests

- None.

## 1.20.1 Hole Toolbar — 2026-10-04

- CR-037: Removed Arc, Sector, Ellipse, Pentagon, Hexagon and Polygon tool buttons; replaced the remaining five shape labels with outline icons and retained tooltips/accessibility names.
- Release build completed with zero warnings/errors; all CosmicDesigner verification checks passed.
- Published artifacts/CosmicDesigner-1.20.1/CosmicDesigner.exe; manual UI acceptance remains.

## 1.20.2 Diamond and Parallelogram Drag Creation — 2026-10-04

- CR-038: Extended shared drag preview/Mouse Up creation to Diamond and Parallelogram; retained click rejection, boundary clamping, Select return and Undo.
- Release build: zero warnings/errors. Complete verification passed, including new shape bounds/contour/Undo checks.
- Published artifacts/CosmicDesigner-1.20.2/CosmicDesigner.exe; manual UI acceptance remains.

## 1.20.3 Contour-aligned 3D Cut Surfaces — 2026-10-04

- CR-039: Split each surface cell at actual contour intersections before triangulation to align removed Cut areas with their outline edges.
- Preserved bend partitions and existing Circle/ARC preview approximations.
- Release build: zero warnings/errors. All verification passed, including five-shape area/boundary, overlapping Cut union, diagonal Outer Contour and folded surface checks.
- Published artifacts/CosmicDesigner-1.20.3/CosmicDesigner.exe; manual UI acceptance remains.

## 1.20.4 Parallelogram Boundary Merge — 2026-10-04

- CR-040: Added Parallelogram to the polygon boundary-merge workflow, including orange candidate selection and explicit right-click merge.
- Release build: zero warnings/errors. All verification passed, including slant/closure, interior/splitting rejection, Undo/Redo, DXF roundtrip, Section and 3D checks.
- Published artifacts/CosmicDesigner-1.20.4/CosmicDesigner.exe; manual UI acceptance remains.

## 1.20.5 Triangle Drag Direction — 2026-10-04

- CR-041: Triangle preview and creation now share explicit vertices: downward drags create downward-pointing triangles, upward drags create upward-pointing triangles.
- Release build: zero warnings/errors. All verification passed, including four drag directions, preview/result equality, movement, Undo/Redo and DXF persistence.
- Published artifacts/CosmicDesigner-1.20.5/CosmicDesigner.exe; manual UI acceptance remains.

## 1.20.6 Closed Contour LINE Deletion — 2026-10-04

- CR-042: Deleting an Outer Contour LINE now reconnects the following LINE so actual edges match the material mask instead of leaving an implicit closing gap.
- Release build: zero warnings/errors. All verification passed, including notch boundary/mask/3D, first/last deletion, minimum contour rejection, Undo/Redo and DXF roundtrip.
- Published artifacts/CosmicDesigner-1.20.6/CosmicDesigner.exe; manual UI acceptance remains.

## 1.20.7 Rectangle Merge Recovery — 2026-10-04

- CR-043: Added polygon subtraction fallback for Rectangle merging against diagonal and triangular LINE-based Outer Contours, restoring orange candidate selection and the context command after contour editing.
- Release build: zero warnings/errors. All verification passed, including deletion/polygon merge fixtures, closure/slants, Undo/Redo, DXF and unsafe/interior rejection.
- Published artifacts/CosmicDesigner-1.20.7/CosmicDesigner.exe; manual UI acceptance remains.

## 1.20.8 Rounded 3D Fillets — 2026-10-04

- CR-044: Replaced single-chord Fillet previews with shared ARC sampling at maximum 5-degree intervals for Outer and Hole surface/edge generation.
- Final Release build: zero warnings/errors. All verification passed, including rounded corner/area, Hole radius updates, fold intersections and exact ARC persistence.
- Published artifacts/CosmicDesigner-1.20.8/CosmicDesigner.exe; manual UI acceptance remains.

## 1.20.9 Fillet-preserving Boundary Merge — 2026-10-05

- CR-045: Added analytic LINE/ARC boundary subtraction for Rectangle, Triangle and Parallelogram cuts, restoring orange candidates and the explicit merge command on Fillet contours.
- Original ARC geometry is preserved; direct cut intersections trim ARC angle ranges at calculated intersections.
- Release build: zero warnings/errors. Complete verification passed, including sequential shape merging, original/trimmed ARCs, invalid-result rejection, Undo/Redo, DXF, Flat mask and 3D generation.
- Published artifacts/CosmicDesigner-1.20.9/CosmicDesigner.exe; manual UI acceptance remains.

## 1.20.10 Outer Contour Gap Closure — 2026-10-05

- CR-046: Added explicit LINE connectors for successive LINE/ARC gaps after movement, endpoint editing, deletion and recalculation, preserving ARC geometry.
- Updated drag selection IDs to account for inserted connector lines.
- Final Release build: zero warnings/errors. All verification passed, including two/single gap closure, ARC deletion, wraparound repair, repeated-edit stability, Undo/Redo and DXF.
- Published artifacts/CosmicDesigner-1.20.10/CosmicDesigner.exe; manual UI acceptance remains.

## 1.20.11 Triangle Lower-left Properties — 2026-10-05

- CR-047: Triangle 전체 선택의 위치를 Lower-left X/Y와 현재 문서 단위로 표시하며 기존 윤곽을 평행 이동한다.
- Release 빌드 경고 0/오류 0. 신규 자동 1/1, 영향 6/6, 전체 56/56 PASS. TC-047-002는 PENDING_MANUAL; WAITING FOR USER VERIFICATION.
- artifacts/CosmicDesigner-1.20.11/CosmicDesigner.exe 게시.

## 1.20.12 Triangle Size Anchor Correction — 2026-10-05

- CR-047 follow-up: Width/Height edits anchor the existing Triangle at its lower-left coordinates and preserve the other dimension. Invalid/unsupported resize is rejected without changing geometry.
- Release build: zero warnings/errors. New AUTO 1/1, affected 7/7, full 57/57 PASS. CR-047 MANUAL cases: PENDING_MANUAL 2; WAITING FOR USER VERIFICATION.
- Suite: AUTO 51 mappings + six BASE, MANUAL 48; SEMI_AUTO/NOT_AUTOMATED 0. No new human PASS inferred.
- Published artifacts/CosmicDesigner-1.20.12/CosmicDesigner.exe.

## 1.20.13 Circle Radius Property — 2026-10-05

- CR-048: Circle Width/Height replaced by Radius R in document units; center preserved, invalid/out-of-material values rejected; other shapes unchanged.
- Release warning 0/error 0. New AUTO 1/1, affected 7/7, full 58/58 PASS; FAIL 0/NOT_RUN 0.
- TC-048-002 MANUAL PENDING_MANUAL; WAITING FOR USER VERIFICATION. Suite: AUTO 52 mappings + six BASE, MANUAL 49; SEMI_AUTO/NOT_AUTOMATED 0.
- Published artifacts/CosmicDesigner-1.20.13/CosmicDesigner.exe.

## Implemented — Verification Pending

- [CR-055](change-requests/CR-055.md) — 객체 속성 영역 OBJECTS / PROPERTIES 순서; TC-055-001 PENDING_MANUAL.

- [CR-054](change-requests/CR-054.md) — 직선·원호·연결선 절개; TC-054-002 PENDING_MANUAL.

- [CR-048](change-requests/CR-048.md) — Circle 반지름 속성

- [CR-047](change-requests/CR-047.md) — Triangle Cut 좌하단 위치 속성

- [CR-004](change-requests/CR-004.md) — 확대 시 Flat Designer 도면과 눈금자 겹침 방지
- [CR-005](change-requests/CR-005.md) — Hole/선택 모드 전환, 직접 조작, Delete 및 선택 동기화 보완
- [CR-006](change-requests/CR-006.md) — Section 치수 값 중복 표시 제거
- [CR-007](change-requests/CR-007.md) — H/W Section View 메뉴 및 재시작 상태 지속성
- [CR-008](change-requests/CR-008.md) — W/H Section Designer 마우스 휠 확대·축소
- [CR-009](change-requests/CR-009.md) — Section Designer 쐐기 생성 클릭 범위 제한
- [CR-010](change-requests/CR-010.md) — 3D Preview 투명 아크릴 표현
- [CR-011](change-requests/CR-011.md) — Section Designer CAD 외곽 치수 기준 및 판재 두께 표시
- [CR-012](change-requests/CR-012.md) — Flat Designer H/W 단면 선택선 및 연동
- [CR-013](change-requests/CR-013.md) — W Section Bent 적응형 화면 배율 교정
- [CR-014](change-requests/CR-014.md) — Section Designer 중앙 고정 Zoom
- [CR-015](change-requests/CR-015.md) — Outer Contour 경계 천공 병합
- [CR-016](change-requests/CR-016.md) — 선택 위치의 실제 Outer Contour 단면 표시
- [CR-017](change-requests/CR-017.md) — Flat Designer Outer Contour LINE 직접 편집
- [CR-018](change-requests/CR-018.md) — 잘린 Section 치수 편집과 Outer Contour 연쇄 갱신
- [CR-019](change-requests/CR-019.md) — 3D Preview Outer Contour 컷 형상 반영
- [CR-020](change-requests/CR-020.md) — File 메뉴 New/Open/Save 단축키
- [CR-021](change-requests/CR-021.md) — 미저장 변경 사항 저장 확인
- [CR-022](change-requests/CR-022.md) — Edit Settings 사용자 환경 설정창
- [CR-023](change-requests/CR-023.md) — 문서 단위와 두 가지 단위 변경 방식
- [CR-024](change-requests/CR-024.md) — 문서명 우선 Window Title
- [CR-025](change-requests/CR-025.md) — Ruler 및 Grid 간격 설정
- [CR-026](change-requests/CR-026.md) — Flat Designer 절삭 영역 흰색 표시
- [CR-027](change-requests/CR-027.md) — 3D Preview 내부 Cut 투명 표시
- [CR-028](change-requests/CR-028.md) — 3D Preview 내부 절곡 Edge 제거
- [CR-029](change-requests/CR-029.md) — 3D Preview V/V1 절곡선 색상 표시
- [CR-030](change-requests/CR-030.md) — 삼각형 Cut 꼭짓점 직접 편집
- [CR-031](change-requests/CR-031.md) — 원·삼각형·사각형 Cut 드래그 생성
- [CR-032](change-requests/CR-032.md) — Triangle Cut Outer Contour 명시적 통합
- [CR-033](change-requests/CR-033.md) — Outer Contour 직각 모서리 Fillet
- [CR-034](change-requests/CR-034.md) — 다중 모서리 연속 Fillet 교정
- [CR-035](change-requests/CR-035.md) — Fillet ARC 선택 표시 및 반지름 편집
- [CR-036](change-requests/CR-036.md) — 임의 각도 및 내부 Hole Fillet

- [CR-037](change-requests/CR-037.md) — 천공 도구 정리 및 도형 아이콘 버튼

- [CR-038](change-requests/CR-038.md) — 다이아몬드·평행사변형 드래그 생성

- [CR-039](change-requests/CR-039.md) — 3D 천공 면과 윤곽선 정합성 교정

- [CR-040](change-requests/CR-040.md) — 평행사변형 Cut Outer Contour 통합

- [CR-041](change-requests/CR-041.md) — 삼각형 천공 드래그 방향 반영

- [CR-042](change-requests/CR-042.md) — Outer Contour LINE 삭제 후 폐합 및 재료 영역 정합성

- [CR-043](change-requests/CR-043.md) — 사선 Outer Contour의 사각형 통합 복구

- [CR-044](change-requests/CR-044.md) — 3D Preview Fillet 곡선 표시

- [CR-045](change-requests/CR-045.md) — Fillet ARC 외곽의 천공 통합

- [CR-046](change-requests/CR-046.md) — Outer Contour 편집 후 틈 자동 LINE 연결

## Completed Change Requests

- [CR-001](change-requests/CR-001.md) — policy gate verified; no CDF implementation was required for version 1.2.0
- [CR-002](change-requests/CR-002.md) — 최근 파일 10개 열기 및 지속성; user acceptance test passed
- [CR-003](change-requests/CR-003.md) — View 메뉴와 전역 표시 상태 지속성; user acceptance test passed

## Known Issues

- CR-011 through CR-015 share implementation commit `f1c55a0` because they were completed as one continuous user-review sequence before final Git recording.
- Manual UI acceptance remains for CR-004 through CR-010, particularly cursor transitions, extreme Pan/Zoom clipping, Section visual alignment, restart persistence and 3D transparency quality.
- The shared DXF geometry parser supports ASCII `LINE`, `CIRCLE`, and `ARC`; unsupported entities are reported rather than converted.
- Binary DXF is not supported by the current parser.
- `docs/IMPLEMENTATION.md` is a historical first-phase report and may describe an earlier support boundary; current requirements and source take precedence.

## Next Work

1. User-test CR-004 through CR-010 and promote passing CRs to Verified.
2. Complete final UI acceptance for CR-011 through CR-048 using the latest `1.20.13` artifact.

## Next Change Request Number

`CR-089`

## CR-049 — 2026-10-05
Implemented: 원 다음에 반원 윤곽 버튼, 지름 중심에서 네 방향 드래그 생성. 이동/크기/DXF/Undo에서 방향 유지. 자동 신규 1/1, 영향 11/11, 전체 59/59 PASS. Release build 0 warnings/errors. Published artifacts/CosmicDesigner-1.20.14/CosmicDesigner.exe.
Verification Status: WAITING FOR USER VERIFICATION (TC-049-002 PENDING_MANUAL). Next CR: CR-050.

## CR-050 — 2026-10-05
Implemented: 반원 지름과 외곽 LINE 겹침 시 주황 후보/우클릭 통합. 정확한 ARC 홈, Cut/Inner 제거, Flat/3D/단면/DXF/Undo 방향 유지. 신규 1/1, 영향 14/14, 전체 AUTO 60/60 PASS; Release build 0 warnings/errors. Published artifacts/CosmicDesigner-1.20.15/CosmicDesigner.exe.
Verification Status: WAITING FOR USER VERIFICATION (TC-050-002 PENDING_MANUAL). Next CR-051.

## CR-051 — 2026-10-05
Implemented: 반원 전체 속성에서 외접 중심/Width/Height 대신 실제 ARC Center X/Y 및 Radius R을 현재 문서 단위로 제공. 반지름 편집은 중심/지름/방향 보존; 부적합/재료 초과 입력은 변경 없이 거부. 신규 1/1, 영향 6/6, 전체 AUTO 61/61 PASS; Release build 0 warnings/errors. Published artifacts/CosmicDesigner-1.20.16/CosmicDesigner.exe.
Verification Status: WAITING FOR USER VERIFICATION (TC-051-002 PENDING_MANUAL). Next CR-052.

## CR-052 — 2026-10-05
Implemented: 반원 다음 4분원 아이콘, 첫 클릭 중심의 사분면 방향 90도 ARC 생성, 두 반지름 각각/동시 경계 겹침의 명시적 통합, Center X/Y 및 Radius R 속성. Release build 0 warnings/errors. 신규 AUTO 1/1, 영향 11/11, 전체 62/62 PASS. TC-052-002 PENDING_MANUAL; Verification Status: WAITING FOR USER VERIFICATION. Published artifacts/CosmicDesigner-1.20.17/CosmicDesigner.exe. Next CR-053.

## CR-053 — 2026-10-05
Implemented: non-Bent Section 구간 길이 합을 Flat W/H로 사용하여 절곡 두께 보정의 중복 차감을 제거했다. `199 / 1998 / 199 = 2396` W/H 대칭 검증을 추가했으며 Bent 외곽 치수 보정은 유지한다. Release build 0 warnings/errors. 신규 AUTO 1/1, 영향 11/11, 전체 63/63 PASS. TC-053-002 PENDING_MANUAL; Verification Status: WAITING FOR USER VERIFICATION. Published artifacts/CosmicDesigner-1.20.18/CosmicDesigner.exe. Next CR-054.

## CR-054 — 2026-10-05
Implemented: 첨부 PNG의 직선·원호·연결선 버튼, 클릭 작성/점선 미리보기/Enter 완료/Esc 취소, 별도 열린 L 경로 선택/삭제/DXF/단위/Undo/3D. 재료 면적 유지. Release build 0 warnings/errors. 신규 AUTO 1/1, 영향 8/8, 전체 64/64 PASS; FAIL 0/NOT_RUN 0. TC-054-002 PENDING_MANUAL; Verification Status: WAITING FOR USER VERIFICATION. Published artifacts/CosmicDesigner-1.20.19/CosmicDesigner.exe. Next CR-055.

## CR-055 — 2026-10-05
Implemented: 객체 속성 영역을 상단 OBJECTS / 하단 PROPERTIES(Material, Selected object) 순서로 배치. Release build 0 warnings/errors. 신규 AUTO N/A, 영향 AUTO 3/3, 전체 AUTO 64/64 PASS. TC-055-001 PENDING_MANUAL; Verification Status: WAITING FOR USER VERIFICATION. Published artifacts/CosmicDesigner-1.20.20/CosmicDesigner.exe. Next CR-061.

- [CR-056](change-requests/CR-056.md) — Section 기본 사각 외곽 동기화; Implemented; 1.20.21. AUTO 신규 1/1, 영향 7/7, 전체 65/65 PASS. MANUAL PENDING_MANUAL; WAITING FOR USER VERIFICATION.

- CR-057 — Hole/Slit 아이콘 굵기 및 크기; Implemented; 1.20.22. 영향 AUTO 3/3, 전체 65/65 PASS; MANUAL PENDING_MANUAL; WAITING FOR USER VERIFICATION.

- CR-058 — Bent Section 실제 두께 축척 정합성; Implemented; 1.20.23. New AUTO 1/1, affected 7/7, full 66/66 PASS. TC-058-002 PENDING_MANUAL; WAITING FOR USER VERIFICATION.

- CR-059 — Section 치수 중앙 및 겹침 배치; Implemented; 1.20.24. New AUTO 1/1, affected 6/6, full 67/67 PASS. TC-059-002 PENDING_MANUAL; WAITING FOR USER VERIFICATION.

- CR-060 — Flat V/V1 절곡선 생성 및 이동; Implemented; 1.20.25. New AUTO 1/1, affected 9/9, full 68/68 PASS; TC-060-002 PENDING_MANUAL; WAITING FOR USER VERIFICATION.

## CR-054 follow-up — 2026-10-05
Implemented: Flat 직선/원호/연결선 직접 클릭을 정확한 LINE/ARC 거리로 판정하며 겹친 절곡보다 절개 선택 우선. 금색/트리/속성 기존 연동. Release build 0 warnings/errors; 신규 AUTO 1/1, 영향 5/5, 전체 69/69 PASS. TC-054-004 PENDING_MANUAL; WAITING FOR USER VERIFICATION. Published artifacts/CosmicDesigner-1.20.26/CosmicDesigner.exe. 신규 CR 번호 미소비.

## CR-061 — 2026-10-05
Implemented: 실제 연결부 힌지 기준 날개 회전 및 90도 방향 대칭; 1.20.27. Build 0 warnings/errors; New AUTO 1/1, affected 9/9, full 70/70 PASS; TC-061-002 PENDING_MANUAL / WAITING FOR USER VERIFICATION. Non-collinear crossed hinges retain legacy fallback.

- [CR-062](change-requests/CR-062.md) — Implemented: Section의 모든 재료 구간과 빈 공간 표시; 1.20.28; New AUTO 1/1, affected 10/10, full 71/71 PASS. TC-062-002 PENDING_MANUAL / WAITING FOR USER VERIFICATION.

- CR-063 — Implemented: Cut 이동 중앙 안내선; 1.20.29. New AUTO 1/1, affected 9/9, full 72/72 PASS; TC-063-002 PENDING_MANUAL / WAITING FOR USER VERIFICATION.

## CR-064 — 2026-10-06
Implemented: HTML 사용자 도움말 22개, 과거 CR 63건 영향 평가(Yes 62), Help 메뉴/배포 및 Living Documentation. Build 0 warnings/errors; New AUTO 1/1, affected 5/5, full 73/73 PASS. TC-064-002/003 PENDING_MANUAL / WAITING FOR USER VERIFICATION. Installer configuration/publish verified; actual install NOT_RUN. Report: HELP_SYSTEM_REPORT-2026-10-06.md. Next CR-065.

## Release Candidate Freeze Result — 2026-10-06
Source Freeze `8fe11260f62932021ef92210dd32552abdffd93a`, candidate 1.20.29-rc1, 302 managed inputs. Pushed boyhon; [Draft PR #1](https://github.com/boyhon/CosmicDesigner/pull/1) to main. Five Release/win-x64/self-contained app publishes and Inno Setup compile PASS; stage has 35 HTML. Full AUTO 73/73 plus shared/Drawer checks and startup 5/5 PASS. Installer 46,789,650 bytes; 453 artifact checksums recorded. Actual clean install/upgrade/uninstall and human UI/Help review remain pending; same-AppId existing install records, non-admin host and no Sandbox. See [Freeze report](releases/FREEZE-RESULT-2026-10-06.md). Freeze not amended; follow-up changes are documentation only. Next CR-065.

## CR-065 — 2026-10-06
Implemented: CosmicDesignerSetup.exe, Freeze 1.20.29-rc1 (numeric Windows version 1.20.29.0). New AUTO 1/1, affected 1/1, full 74/74 PASS; build 0 warnings/errors; Inno compile and Help links PASS. Output artifacts/release/1.20.29-rc1-cr065/output/CosmicDesignerSetup.exe, 46,789,675 bytes. Original Freeze preserved; packaging follow-up includes updated installation help. TC-065-002 / TC-064-003 PENDING_MANUAL; WAITING FOR USER VERIFICATION. Next CR-066. See tests/regression/results/CR-065-2026-10-06.md.

## CR-066 — 2026-10-06
Implemented: 설치 화면/제품명/기본 폴더/그룹/선택형 바탕 화면/완료 후 실행 CosmicDesigner. Existing AppId/upgrade path reuse retained. Freeze 1.20.29-rc1. New AUTO 1/1, affected 2/2, full 75/75 PASS; build 0 warnings/errors; Inno compile and Help PASS. Output artifacts/release/1.20.29-rc1-cr066/output/CosmicDesignerSetup.exe. TC-066-002 / TC-064-003 PENDING_MANUAL; WAITING FOR USER VERIFICATION. Next CR-067.

## CR-067 — 2026-10-06
Implemented: SetupClassicIcon.ico applied; Freeze 1.20.29-rc1 rebuilt. Affected AUTO 3/3 and full 75/75 PASS. Actual installer progress window title-bar classic icon observed via computer-use. Taskbar/user visual confirmation pending; TC-067-001 SEMI_AUTO PENDING_MANUAL. No actual installation action performed by Codex; user advanced installer independently. Output artifacts/release/1.20.29-rc1-cr067/output/CosmicDesignerSetup.exe. Next CR-068.

CR-067 verification update: user confirmed taskbar classic icon in conversation (2026-10-06); TC-067-001 SEMI_AUTO PASS, CR-067 Verified. Initial language-dialog visual capture coverage limited; no installation lifecycle PASS asserted.

## CR-068 — 2026-10-06 (original request; implemented with CR-069)
다음 설치 빌드 전에 기본 디렉토리를 C:\Program Files\VCutting으로 적용한다. 현재 재빌드/기존 설치 이동 없음. 사용자 제거 후 재설치 방식 승인. 제품명/파일명 유지. installer/README.md의 다음 빌드 지침 및 change-requests/CR-068.md 참조. Next CR-069.

## CR-069 — 2026-10-06
Implemented: VCutting 개발/제품·솔루션 이름 변경 및 CR-068 기본 설치 폴더 적용. Release build 0 warnings/errors; new AUTO 2/2, affected 10/10, full 77/77 PASS; Viewer/Drawer verification PASS; Help 35 HTML PASS; VCuttingSetup.exe compile/version PASS. Existing settings/metadata/AppId and prior Freeze preserved. TC-069-002/TC-068-002 PENDING_MANUAL; WAITING FOR USER VERIFICATION. Next CR-070. Result: tests/regression/results/CR-069-2026-10-06.md.

- [CR-070](change-requests/CR-070.md) — Implemented / WAITING FOR USER VERIFICATION: 고객 버전/Freeze 정책; 이번 작업에서 실제 Freeze/출시 없음. Next CR-071.

- CR-071: 설치 매뉴얼 6단계 원본 화면 구현; New/affected/full AUTO 1/1, 2/2, 79/79 PASS; Release build 0 warnings/errors; TC-071-002 PENDING_MANUAL. Next CR-072. Freeze/출시 없음.

## Identity clarification — 2026-10-06
패키지 VCutting, 현재 사용자 매뉴얼 대상 CosmicDesigner, 향후 CosmicExplorer 계획. AGENTS 및 매뉴얼 정정 완료. 주 앱/installer의 현재 VCutting.exe 산출물 정합성 변경은 남아 있다. CR-071 follow-up, 새 CR 번호 미소비. 이전 테스트는 당시 결과로 보존; 사람 검증 대기.

2026-10-07 CR-071 명칭 정정 후 검증: 공식 22페이지 패키지/프로그램 구분 및 CosmicDesigner.exe 안내 PASS; Help 35 HTML 로컬 링크 PASS; Release verification build 0 warnings/errors; 영향 AUTO TC-064-001/TC-071-001 2/2 PASS; full AUTO 79/79 PASS. 기존 원본 설치 화면 유지. TC-071-002 사람 브라우저/인쇄 검증 PENDING_MANUAL; 사람 PASS 기록 없음. 실제 코드/설치 VCutting.exe를 CosmicDesigner.exe와 일치시키는 작업은 미구현 상태로 내부 Gap에 기록.

2026-10-07 CR-072 다국어 / CR-073 기본 mm Implemented / WAITING FOR USER VERIFICATION. 별도 요구 추적. Next CR-074. Freeze 발급 없음.

CR-072/073 final: new AUTO 2/2, affected 7/7, full 81/81 PASS; solution Release 0 warnings/errors. Human TC-072-002/TC-073-002 pending. 319 external keys; user override compatibility and legacy cm semantics retained. No new Freeze/installer/release. Next CR-074. Result: tests/regression/results/CR-072-073-2026-10-07.md.
`nCR-074 In Progress — CosmicDesigner 타원 Hole 방향 드래그 생성. Next CR-075. 프로젝트 VCutting / 프로그램 CosmicDesigner 유지.

## CR-074 — 2026-10-07 final
Implemented / WAITING FOR USER VERIFICATION: CosmicDesigner 원 옆 Ellipse outline 아이콘과 장축 방향 드래그 생성. 기존 48 LINE 구조/설정/DXF/Undo/unit 호환. Build 0 warnings/errors; new AUTO 1/1, affected 13/13, full 82/82 PASS. Help 35 HTML links PASS. TC-074-002 MANUAL PENDING_MANUAL; 사람 PASS 없음. 고객 버전/Freeze/installer 발급 없음. 실행: artifacts/cr074-build/Release/net10.0-windows/VCutting.exe (프로그램 CosmicDesigner; 기존 binary naming 후속 항목 유지). Next CR-075. Report tests/regression/results/CR-074-2026-10-07.md.
`nCR-075/076 In Progress — 타원 고유 속성 및 사전 승인된 주 프로그램 executable 이름 적용. Next CR-077.

## CR-075 / CR-076 — 2026-10-07 final
Implemented / WAITING FOR USER VERIFICATION. CosmicDesigner 타원 중심 X/Y, 장축/단축 전체 길이, 반시계 회전각, 읽기 전용 비율. 정상 타원 단일 트리 객체, invalid/no-op 안전 처리. Native DXF ELLIPSE 저장은 미구현 후속 항목; 기존 LINE/metadata 저장 호환 유지. 사전 승인된 주 앱 실행 파일명 CosmicDesigner.exe 적용, package/project/namespace/AppId/settings 유지. New AUTO 2/2, affected 14/14, full 84/84 PASS. Build 0 warnings/errors; Help 35 HTML PASS. Human TC-075-002/TC-076-002 PENDING_MANUAL. No new installer/Freeze/customer version. Run artifacts/cr075-build/Release/net10.0-windows/CosmicDesigner.exe. Next CR-077. Result tests/regression/results/CR-075-076-2026-10-07.md.
## CR-077 — 2026-10-07 final
Implemented / WAITING FOR USER VERIFICATION. 내부 겹침 연결 그룹 주황색 선택/윤곽 및 명시적 우클릭 통합. 하나의 Compound Cut/Inner Contour, analytic LINE/ARC 및 재료 섬 보존; 이동/동일비율 곡선 resize/직선 축별 resize/한 Undo/Redo/DXF/단위 처리. New AUTO 1/1, affected 17/17, full 85/85 PASS; solution Release 0 errors/warnings; Viewer/Drawer checks and Help 35 HTML PASS. TC-077-002 MANUAL PENDING_MANUAL; no human PASS. Multi-loop Compound Fillet 제한 및 과거 앱의 새 compound loop 해석 비보장은 ADR-003/Gap에 기록. No customer version/Freeze/installer/release. Run artifacts/cr077-build/Release/net10.0-windows/CosmicDesigner.exe. Next CR-078. Result tests/regression/results/CR-077-2026-10-07.md.

## CR-078 — 2026-10-07 final
Implemented / WAITING FOR USER VERIFICATION. CosmicDesigner Regular Polygon icon/Sides6 input, center/first-vertex two-click preview, logical center/sides3..512/radius/CCW rotation properties; whole selection/body move/delete and single Undo/Redo. Shared vertex calculator, closed LWPOLYLINE and optional JSON/history parameters, generic straight polyline import without regular inference. Existing contour/units/Flat/3D/localization preserved; Fillet/union transitions to Compound. New11/11, affected20/20, full96/96 AUTO PASS. Final solution Release0 errors/0 warnings; actual CosmicDesigner.exe startup AUTO PASS; Viewer/Drawer and Help35 HTML PASS. TC-078-012 MANUAL PENDING_MANUAL including actual UI/AutoCAD. 341 keys en/ko; ADR-004 and original attachment retained. No customer version/Freeze/installer/release/commit/push. Run artifacts/cr078-build/Release/net10.0-windows/CosmicDesigner.exe. Next CR-079. Result tests/regression/results/CR-078-2026-10-07.md.
2026-10-07 CR-079 intake: crossing regular star tool approved; CR-078 vertex calculator reused. No Freeze.

## CR-079 — 2026-10-07 final
Implemented / WAITING FOR USER VERIFICATION. CosmicDesigner Regular Star crossing icon, Sides5..512/Step2..floor((N−1)/2), default {5/2}, center/first-vertex preview. One logical parametric object, gcd cycles, center/radius/CCW rotation/N/Step properties, movement/deletion/history/units; one closed DXF LWPOLYLINE per cycle plus metadata. Even-odd Flat/3D preserves material islands. Self-intersection split before internal union; nonmanifold rejected. Direct star Fillet/Outer merge unavailable. New3/3, affected22/22, full99/99 AUTO PASS; solution0 errors/warnings, actual startup AUTO PASS, Viewer/Drawer PASS, Help35 HTML links PASS, en/ko346 keys. MANUAL TC-079-004 PENDING_MANUAL (UI/Korean layout/external CAD/screenshots), no human PASS. No Freeze/customer version/installer/commit/push. Run artifacts/cr079-build/Release/net10.0-windows/CosmicDesigner.exe. Next CR-080. Report tests/regression/results/CR-079-2026-10-07.md.

CR-079 follow-up In Progress: user explicitly requires solid interior hole; star nonzero winding replaces prior even-odd, Compound unchanged. Next CR-080.

CR-079 follow-up final: user-requested full star interior/center cut via nonzero winding in Flat/3D/hit/internal union; existing Compound islands preserved. Old saved stars use corrected rule, geometry/metadata/history unchanged. New1/1 affected11/11 full99/99 AUTO PASS; final build0 errors/warnings, Help35 PASS; MANUAL006 pending. Implemented / WAITING FOR USER VERIFICATION. Execute artifacts/cr079-solid-build/Release/net10.0-windows/CosmicDesigner.exe. No Freeze/customer version/installer/commit/push. Next CR-080. Report tests/regression/results/CR-079-solid-2026-10-07.md.

CR-079 cutting-profile follow-up In Progress: internal cutting paths removed, only exterior star boundary retained. Next CR-080.

CR-079 profile-only final: internal star cutting lines removed; one analytic closed2N-edge exterior profile shared by preview/Flat/3D/DXF. N/K/R/CCW rotation/center/history/units preserved; old stars regenerate optimized paths on load. New2/2 affected12/12 full100/100 AUTO PASS, final solution0 errors/warnings, Help35 PASS, Viewer/Drawer PASS, active manifest100 IDs match. Required MANUAL009 pending. Implemented / WAITING FOR USER VERIFICATION. Run artifacts/cr079-profile-build/Release/net10.0-windows/CosmicDesigner.exe; report tests/regression/results/CR-079-profile-2026-10-07.md. No Freeze/customer version/installer/commit/push; Next080.

CR-080 In Progress: move Parallelogram between Diamond/RegularPolygon, always report actual BuildIdentity. Next CR-081.

CR-080 final: Diamond→Parallelogram→RegularPolygon→RegularStar, unchanged button appearance/handler/shape. Persistent AGENTS exact artifact BuildIdentity+path completion report rule. Existing override distinct development ID 7dd14f5-working-tree.cr080.20261007T060727Z. Run artifacts/cr080-build/Release/net10.0-windows/CosmicDesigner.exe. Build0 errors/warnings, affected6/6 full100/100 AUTO PASS, Help35 PASS, Viewer/Drawer PASS, metadata match. MANUAL080001 pending, Implemented / WAITING FOR USER VERIFICATION. No Freeze/customer version/installer/commit/push. Next081. Report tests/regression/results/CR-080-2026-10-07.md.

CR-081/082 In Progress: all ARC start/end angles and Rectangle rotation; next083.

CR-081/082 final: Implemented / WAITING FOR USER VERIFICATION. All ARC start/end signed angle editing with valid contour reconnection; Rectangle local dimensions/CCW rotation shared across hit/cache/DXF/history/units/merge. New2/2 affected18/18 full102/102 AUTO PASS, solution0 errors/warnings, Help35 links/Viewer/Drawer PASS. MANUAL081002/082002 PENDING_MANUAL; no human PASS. Run artifacts/cr081-082-build/Release/net10.0-windows/CosmicDesigner.exe; actual About Build identifier 7dd14f5-working-tree.cr081-082.20261007T095210Z. Report tests/regression/results/CR-081-082-2026-10-07.md. No Freeze/customer version/installer/commit/push. Next CR-083.

CR-083 In Progress: selected shape Copy/Paste, Windows clipboard and units/history compatibility. Next CR-084.

CR-083 final: Implemented / WAITING FOR USER VERIFICATION. Ctrl+C/V and Edit Copy/Paste for selected Cut/Slit/contour segments and imported DXF; independent geometry/IDs/joints/sequence, physical units, 10mm cascade/clamp, one Undo/Redo/DXF, text clipboard retained. New1/1 affected14/14 full103/103 AUTO PASS; solution0 warnings/errors, Help35/Viewer/Drawer PASS, en/ko361 keys, manifest103 IDs match. TC-083-002 MANUAL pending, no human PASS. Run artifacts/cr083-build/Release/net10.0-windows/CosmicDesigner.exe; actual About Build identifier 7dd14f5-working-tree.cr083.20261007T100959Z. Report tests/regression/results/CR-083-2026-10-07.md. No Freeze/customer version/installer/commit/push. Next CR-084.

CR-084 In Progress: SVG import editable geometry and physical units. Next CR-085.

CR-084 final: Implemented / WAITING FOR USER VERIFICATION. File Import SVG: viewport/units/groups/path primitives, winding-resolved editable Cut/open Slit, native similarity Circle and nominal0.01mm curve LINE approximation; data-only bounded XML/unsupported warnings/Save As DXF. New3/3 affected9/9 full106/106 AUTO PASS, final -m:1 solution0 errors/warnings; Help35/Viewer/Drawer PASS, en/ko373 keys, manifest106 IDs match. MANUAL084004 pending, no human PASS. Execute artifacts/cr084-build/Release/net10.0-windows/CosmicDesigner.exe; actual About Build identifier 7dd14f5-working-tree.cr084.20261007T102842Z. Report tests/regression/results/CR-084-2026-10-07.md. No Freeze/customer version/installer/commit/push. Next CR-085.

CR-085 In Progress — separate CosmicConvert.exe, certified curve optimization, quiet/GUI, tests/help. User2026-10-08; no release/commit. Next CR-086.

CR-085 Implemented / WAITING FOR USER VERIFICATION — standalone CosmicConvert.exe; new4/4 affected6/6 existingfull106/106 + new4 = accumulated110/110 AUTO PASS, Viewer/Drawer/Help PASS. TC-085-005 MANUAL pending; no merge/commit/release. Apple846shapeLINE→LINE70+ARC97; .0499256865mm bound; original hashes preserved. Output artifacts/cr085-final-build/Release/net10.0-windows/CosmicConvert.exe; actual BuildIdentity 7dd14f5-working-tree.cr085.20261007T160928Z. Report tests/regression/results/CR-085-2026-10-08.md. Mixed contours import as individual entities; crossing fill unsupported. Next CR-086.

CR-086 In Progress: CosmicConvert output frame with10mm margin all sides, rigid relocation. Next CR-087.

CR-086 Implemented / WAITING FOR USER VERIFICATION: frame margin10mm, Apple82.1222108×94.0460344mm / shape167+frame4=171entities. New1/1, affected converter3PASS/1NOT_RUN + Designer2/2, full existing106/106 + converter4PASS/1NOT_RUN =110PASS,0FAIL,1NOT_RUN. Historical850LINE Apple DXF replaced externally by167entity file; expectation retained, not fabricated PASS. HumanTC-086-002 pending. Build0warnings/errors, actual BuildIdentity7dd14f5-working-tree.cr086.20261007T163951Z at artifacts/cr086-build/Release/net10.0-windows/CosmicConvert.exe. Report tests/regression/results/CR-086-2026-10-08.md. Next CR-087; no release/commit.

CR-087 In Progress: CosmicConvert native Compound/Slit metadata; actual selection/3D integration. Next CR-088.
CR-087 Implemented / WAITING FOR USER VERIFICATION: native Compound/Slit metadata; Apple1Compound3loops,171entities,10mm frame/error unchanged. Full112PASS/0FAIL/1NOT_RUN historical850LINE fixture. TC087003 human pending. Build cr087.20261008.native-compat (actual metadata), artifacts/cr087-build/Release/net10.0-windows/CosmicConvert.exe. Evidence tests/regression/results/CR-087-2026-10-08.md. NextCR088; no Freeze/release/commit.
CR-088 In Progress: invisible auxiliary leaf text blocks outlined SVG; nextCR089.
CR-088 Implemented / WAITING FOR USER VERIFICATION: invisible auxiliary leaf text and bounded fill topology broad phase. Provided 양영권회장 sample1Compound493entities,error<=.05mm, original preserved. New1/1, affected4/4, full113PASS/0FAIL/1historicalNOT_RUN. TC088002 human pending. Actual BuildIdentity cr088.20261008.invisible-text; artifacts/cr088-build/Release/net10.0-windows/CosmicConvert.exe. Report tests/regression/results/CR-088-2026-10-08.md; nextCR089. No release/Freeze/installer/commit.

CR-089 Implemented / WAITING FOR USER VERIFICATION: H/W Fit/Reset; new1/1 affected8/8 Designer full107/107 AUTO PASS; humanTC089002 pending. Development CosmicDesigner.exe artifacts/cr089-build/Release/net10.0-windows; actual BuildIdentity cr089.20261008.section-fit. Next CR-090.
CR-089 accumulated regression final: 114 AUTO PASS, 0 FAIL, 1 historical NOT_RUN (TC-085-004). TC-089-002 MANUAL pending. Evidence tests/regression/results/CR-089-2026-10-08.md.
CR-090 In Progress: all-work commit/push, six-program package; explicit user RC1 Freeze approval2026-10-08. NextCR091.
CR-090 Implemented / WAITING FOR USER VERIFICATION: converter included in package and Start Menu; solution0warnings/errors, new1/1 affected7/7 fullDesigner108/108 PASS, accumulated115PASS/1historicalNOT_RUN. User-approved RC1 build pending source anchor; ledger under release/freezes and release/builds is authoritative. NextCR091.
CR-091 Implemented: WPF generated temporary csproj source-mismatch correction; focused isolated test PASS. RC1 failed before installer compile, preserved. Replacement RC2 Freeze approval required; NextCR092. User visible/manual impact No (internal build correction); actual installer human090002pending.
CR-091 final pre-Freeze: policy5/5PASS; fullDesigner109/109PASS, accumulated116PASS/1historicalNOT_RUN. User explicitly approved1.0.0-rc.2. Source build0errors/1cachedNU1900warning; actual RC publish/compile evidence will be appended under release/builds. No production promotion. NextCR092.
CR-092 In Progress: repair duplicate generated Help version source paths during referenced-app publishing; RC2 failed before installer compile. Six-project pre-Freeze publish in progress. NextCR093.
CR-092 Implemented: full policy6/6PASS, Designer110/110AUTO PASS, accumulated117PASS/1historicalNOT_RUN. Actual six-program self-contained publish and combined stage collision/Help37 verification PASS. Final build0warnings/errors. Replacement RC3 approval pending; human090002pending. NextCR093.
CR-093 In Progress: user requires installation/use verification before Freeze; Development integration installer authorized, pending RC3 request superseded (no RC3 issued). NextCR094.
CR-093 Implemented / WAITING FOR USER VERIFICATION: pre-Freeze Development integration installer pathway; new1/1affected6/6fullDesigner111/111AUTO PASS, accumulated118PASS/1historicalNOT_RUN. Build0warnings/errors, Help37/source audit PASS. Actual integration artifact/source/build ID/checksum in release/builds/development; no RC3 issued. User manual installation/use review pending; Freeze only afterward. NextCR094.
CR-093 actual integration installer generated: artifacts/integration/manual-20261008-01/output/VCuttingSetup-IntegrationTest.exe,47380492bytes; Development1.20.29.0. Source2b502cf83d5e1ab4b4d00a9caab87f31020fcc4a; actual About BuildIdentity2b502cf83d5e1ab4b4d00a9caab87f31020fcc4a.integration.manual-20261008-01. All6self-contained publish/metadata/Help37/compile/version PASS; ledger release/builds/development/manual-20261008-01.json. Actual install/use human093002/090002pending; no new Freeze/RC3 issued. NextCR094.

## Authorized Freeze — 2026-10-08 / 1.0.0-rc.3
User explicitly instructed '네, Freeze 하십시오.' after receiving Development integration installer manual-20261008-01. Approval release/approvals/2026-10-08-rc3.txt. Current source is being anchored and registered as immutable RC3; release/freezes/1.0.0-rc.3.json is authoritative for exact source SHA/input hashes/toolchain/settings. Prior RC1/RC2 records retained. Executable/Help/installer/dependency inputs match the delivered integration build; prior AUTO111Designer+7converterPASS/1historicalNOT_RUN evidence retained. Individual human observations not supplied, so human gates remain pending and no Verified/Closed/production promotion is asserted. No new functional CR; nextCR094.
