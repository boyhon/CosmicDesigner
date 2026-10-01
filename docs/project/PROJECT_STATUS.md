# Cosmic Designer 개발 — Project Status

## Baseline

- Date: `2026-10-01`
- Document: [BASELINE-2026-10-01.md](BASELINE-2026-10-01.md)
- Basis: current source, solution, verification projects, installer definition, and repository documents available in the workspace
- Git note: no `.git` directory was available in this workspace when the baseline documentation was introduced; no commit hash can therefore be asserted

## Current Version

- DXFExplorer installer suite: `1.1.0` (`installer/DXFExplorer.iss`)
- CosmicDesigner: `1.3.3`

## Current Development Phase

CosmicDesigner 1.3.3 implements all registered CRs through CR-010, including the CR-006 Bent exterior-dimension correction and the visibility-corrected CR-010 acrylic skeleton preview. CR-001 through CR-003 are Verified; CR-004 through CR-010 passed automated/build verification and await user UI acceptance.

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

## Completed Change Requests

- [CR-001](change-requests/CR-001.md) — policy gate verified; no CDF implementation was required for version 1.2.0
- [CR-002](change-requests/CR-002.md) — 최근 파일 10개 열기 및 지속성; user acceptance test passed
- [CR-003](change-requests/CR-003.md) — View 메뉴와 전역 표시 상태 지속성; user acceptance test passed

## Known Issues

- Git metadata is absent from the current workspace, so history and related commits cannot be inspected or recorded here.
- Manual UI acceptance remains for CR-004 through CR-010, particularly cursor transitions, extreme Pan/Zoom clipping, Section visual alignment, restart persistence and 3D transparency quality.
- The shared DXF geometry parser supports ASCII `LINE`, `CIRCLE`, and `ARC`; unsupported entities are reported rather than converted.
- Binary DXF is not supported by the current parser.
- `docs/IMPLEMENTATION.md` is a historical first-phase report and may describe an earlier support boundary; current requirements and source take precedence.

## Next Work

1. Initialize or connect the intended Git repository without renaming technical artifacts solely for the management-name change.
2. User-test CR-004 through CR-010 in the CosmicDesigner 1.3.0 artifact and promote passing CRs to Verified.
3. Use `CR-011` for the next newly registered change request.

## Next Change Request Number

`CR-011`
