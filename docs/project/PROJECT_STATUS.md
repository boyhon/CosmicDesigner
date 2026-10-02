# Cosmic Designer 개발 — Project Status

## Baseline

- Date: `2026-10-01`
- Document: [BASELINE-2026-10-01.md](BASELINE-2026-10-01.md)
- Basis: current source, solution, verification projects, installer definition, and repository documents available in the workspace
- Git note: no `.git` directory was available in this workspace when the baseline documentation was introduced; no commit hash can therefore be asserted

## Current Version

- DXFExplorer installer suite: `1.1.0` (`installer/DXFExplorer.iss`)
- CosmicDesigner: `1.9.0`

## Current Development Phase

CosmicDesigner 1.9.0 implements all registered CRs through CR-019. The 3D Preview surface and black edges now follow the current cut and edited Outer Contour instead of always rendering a full rectangle.

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

## In Progress

- None.

## Open Change Requests

- None.

## Implemented — Verification Pending

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
2. Complete final UI acceptance for CR-011 through CR-019 using the latest `1.9.0` artifact.

## Next Change Request Number

`CR-020`
