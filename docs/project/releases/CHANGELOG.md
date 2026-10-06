# Cosmic Designer 개발 — Changelog

Release-relevant changes after the baseline are recorded here. CR identifiers must accompany functional entries.

## Unreleased

- 2026-10-06: Freeze current source, HTML Help, resources, accumulated regression/project records and installer definitions as 1.20.29-rc1 on boyhon. No new product features; isolate RC installer output, redact personal temporary-path references and make historical manual scripts repository-relative. Installer suite remains 1.1.0; manual acceptance remains pending.

- CR-064: Add a Korean offline HTML user manual with 22 current-source pages, workflows, troubleshooting, terminology, shortcuts, local navigation/search and print styles. Connect four Help routes, include CosmicDesigner/help in installer configuration and track documentation impact for all past CRs. Automated regression 73/73 PASS; required browser/UI and actual installation review pending.

## 1.20.26 — 2026-10-05

- CR-054 follow-up: Improve Flat Line/Arc/Polyline slit click selection with exact LINE/ARC distance and screen-space tolerance. Slits take selection priority over overlapping bends while drawing tools retain their input. Existing gold highlight and tree/property synchronization apply. Manual UI acceptance pending.

## 1.20.20 — 2026-10-05

- CR-055: Reorder the left object/property area to OBJECTS above PROPERTIES (Material and Selected object), preserving selection/property controls and scrolling. Manual UI acceptance pending.

## 1.20.19 — 2026-10-05

- CR-054: Add supplied-image Line, Arc and Polyline slit buttons with click-based drawing, dashed preview, Enter completion and Esc cancellation. Store open L paths separately from holes, preserving material area and supporting selection/Delete, Undo/Redo, document units, DXF and folded 3D edge display. Manual UI acceptance pending.

## 1.20.13 — 2026-10-05

- CR-048: Circle properties show one editable Radius R in document units instead of Width/Height. Edits retain center and reject invalid/out-of-material values; other shapes retain size properties.

## 1.20.12 — 2026-10-05

- CR-047 follow-up: Triangle Width/Height edits preserve lower-left X/Y and the unedited dimension. Scale existing vertices from the lower-left anchor, preserving orientation; reject out-of-material sizes and unsupported ARC scaling without changing the contour.

## 1.20.11 — 2026-10-05

- CR-047: Triangle position properties use the bounding rectangle lower-left X/Y in document units. Position edits translate the existing contour, preserving arbitrary vertices and Fillet geometry, with bounds, Undo/Redo and DXF persistence.

## 1.20.10 — 2026-10-05

- CR-046: Automatically insert explicit LINE connectors at Outer Contour LINE/ARC gaps after editing or deletion, preserving exact ARC geometry and aligning actual boundaries with material masks and 3D previews.

## 1.20.9 — 2026-10-05

- CR-045: Restored Rectangle/Triangle/Parallelogram boundary merging on Fillet ARC Outer Contours. Exact ARC geometry is retained, with direct intersections trimming ARC angle ranges; orange selection and explicit merge workflow remain available.

## 1.20.8 — 2026-10-04

- CR-044: Outer and Hole Fillet ARCs now appear rounded in 3D through maximum 5-degree tessellation shared by surface clipping and outline edges; exact Flat/DXF ARCs are preserved.

## 1.20.7 — 2026-10-04

- CR-043: Restored Rectangle boundary merge availability on diagonal LINE-based Outer Contours after line deletion or Triangle/Parallelogram merging, retaining orange candidate selection and explicit context-menu integration.

## 1.20.6 — 2026-10-04

- CR-042: Reconnect neighboring Outer Contour LINE geometry after edge deletion so gray material and white exterior match the actual closed outline; retain Undo/Redo and DXF persistence.

## 1.20.5 — 2026-10-04

- CR-041: Triangle holes now point in the vertical drag direction, with matching preview/result vertices and preserved editing, Undo/Redo and persistence.

## 1.20.4 — 2026-10-04

- CR-040: Boundary-touching Parallelogram Cuts now use orange selection and the explicit Outer Contour merge command, preserving diagonal edges and existing Undo/Redo and persistence behavior.

## 1.20.3 — 2026-10-04

- CR-039: Replaced whole-triangle centroid deletion with contour-aligned surface splitting, correcting gaps/overlap between 3D Cut openings and outline edges for sloped and circular shapes.

## 1.20.2 — 2026-10-04

- CR-038: Diamond and Parallelogram now preview and create a single Cut sized by dragging, matching the existing Circle/Triangle/Rectangle interaction; click rejection, Select return and Undo are preserved.

## 1.20.1 — 2026-10-04

- CR-037: Removed Arc, Sector, Ellipse, Pentagon, Hexagon and Polygon Hole buttons; converted the five remaining shapes to compact outline icon buttons with tooltips and accessibility names.

## 1.20.0 — 2026-10-03

- CR-036: Generalized Fillet tangent calculations from right angles to arbitrary convex LINE–LINE angles.
- Added Fillet preview, application, Object Tree child geometry and radius editing for LINE-based internal Hole contours.
- Added straight-chord 3D approximation for internal Hole ARC segments.
- CosmicDesigner assembly version is now `1.20.0`.

## 1.19.0 — 2026-10-03

- CR-035: Added gold Flat Designer highlighting and tangent endpoint handles for selected Outer Contour ARC objects.
- Added editable `Radius R` properties that rebuild the tangent ARC and both adjacent LINE endpoints while preserving contour closure.
- CosmicDesigner assembly version is now `1.19.0`.

## 1.18.1 — 2026-10-03

- CR-034: Allowed additional eligible corners to receive Fillets after the Outer Contour already contains one or more ARC segments.
- Added four-corner sequential Fillet and four-ARC DXF round-trip regression coverage.
- CosmicDesigner assembly version is now `1.18.1`.

## 1.18.0 — 2026-10-03

- CR-033: Added numeric-radius Fillet creation for convex right-angle Outer Contour corners with live dashed preview.
- Saved exact tangent quarter-circle ARCs in DXF while using straight chord approximations in the overview-oriented 3D Preview.
- Corrected Flat Designer ARC rendering to scale radius with the current viewport Zoom.
- CosmicDesigner assembly version is now `1.18.0`.

## 1.17.0 — 2026-10-03

- CR-032: Extended orange candidate highlighting and explicit right-click Outer Contour merging to Triangle Cuts.
- Preserved Triangle diagonal edges while rejecting fully internal holes or multi-contour subtraction results.
- CosmicDesigner assembly version is now `1.17.0`.

## 1.16.0 — 2026-10-03

- CR-031: Added drag-to-size creation with live previews for Circle, Triangle and Rectangle Cuts.
- Ignored short clicks and returned to Select mode after creation to prevent accidental duplicate Cuts.
- CosmicDesigner assembly version is now `1.16.0`.

## 1.15.0 — 2026-10-03

- CR-030: Replaced Triangle Cut bounding-box resizing with three independent vertex handles over its closed three-LINE contour.
- Preserved an edited triangle's exact shape during whole-object movement, DXF metadata round-trip, Flat masking and 3D cut rendering.
- CosmicDesigner assembly version is now `1.15.0`.

## 1.14.0 — 2026-10-03

- CR-029: Added red V and blue V1 bend edges to the 3D Preview, clipped to remaining material.
- Retained black edges only for actual Outer and Cut contours.
- CosmicDesigner assembly version is now `1.14.0`.

## 1.13.4 — 2026-10-03

- CR-028 refinement: Replaced inferred one-sided mesh edges with explicit Outer Contour and Cut contour edges, removing residual radial lines around circular holes.
- CosmicDesigner assembly version is now `1.13.4`.

## 1.13.3 — 2026-10-03

- CR-028: Removed internal bend and mesh-partition lines from the 3D Preview while retaining exterior and Cut boundary edges.
- CosmicDesigner assembly version is now `1.13.3`.

## 1.13.2 — 2026-10-03

- CR-027: Removed 3D surface faces from internal Cut Object regions and generated visible hole boundary edges.
- Preserved Outer Contour notch and folded-surface behavior.
- CosmicDesigner assembly version is now `1.13.2`.

## 1.13.1 — 2026-10-03

- CR-026: Rendered Cut interiors and areas outside the actual Outer Contour as white removed material.
- Grid and bend lines are now clipped to the remaining material region.
- CosmicDesigner assembly version is now `1.13.1`.

## 1.13.0 — 2026-10-03

- CR-025: Added independent Auto/custom spacing preferences for horizontal Ruler, vertical Ruler and Grid.
- Custom display spacing retains its physical length across document-unit changes and safely thins at low Zoom.
- CosmicDesigner assembly version is now `1.13.0`.

## 1.12.1 — 2026-10-03

- CR-024: Changed the Main Window Title to `Filename.dxf - CosmicDesigner`, uses `Untitled` for new documents, and prefixes unsaved documents with `*`.
- CosmicDesigner assembly version is now `1.12.1`.

## 1.12.0 — 2026-10-03

- `CR-022`: added Edit > Settings for persistent folders, Recent count, interaction tolerances, Zoom step, 3D presentation and new-document defaults.
- `CR-023`: added mm/cm/m document units with numeric-preserving rescale and physical-size-preserving conversion modes.
- Persisted units in DXF `$INSUNITS` and CosmicDesigner metadata with Undo/Redo and legacy-file compatibility.
- CosmicDesigner assembly version is now `1.12.0`.

## 1.11.0 — 2026-10-03

- `CR-021`: added unsaved-change tracking and Save/Discard/Cancel confirmation before New, Open/Recent, Exit and window close.
- Continued the requested operation only after successful saving or an explicit discard choice.
- Kept the current design open when confirmation, Save As or saving is cancelled or fails.
- CosmicDesigner assembly version is now `1.11.0`.

## 1.10.0 — 2026-10-03

- `CR-020`: added global Ctrl+N, Ctrl+O and Ctrl+S shortcuts for File > New, Open and Save.
- Displayed each shortcut beside its corresponding File menu item.
- Reused existing command handlers, including Save As fallback for unsaved documents.
- CosmicDesigner assembly version is now `1.10.0`.

## 1.9.0 — 2026-10-03

- `CR-019`: clipped the 3D Preview surface mesh to the current rectilinear Outer Contour.
- Removed triangle faces from merged boundary-notch areas and regenerated black exterior edges from the actual mesh boundary.
- Preserved W/H bend deformation and visible fold edges across the contour-aware mesh.
- CosmicDesigner assembly version is now `1.9.0`.

## 1.8.0 — 2026-10-03

- `CR-018`: made clipped H/W Section dimensions editable in Flat and Bent modes.
- Length changes propagate through local bend stations and the selected Outer Contour boundary while preserving the other local segment lengths.
- Connected contour LINE endpoints follow the moved boundary and Bent exterior values retain material-thickness correction.
- CosmicDesigner assembly version is now `1.8.0`.

## 1.7.0 — 2026-10-03

- `CR-017`: added direct Flat Designer selection and editing for Outer Contour LINE objects.
- Selected LINE bodies move perpendicular to themselves and endpoint handles resize their length with orientation-aware cursors.
- Connected neighboring LINE endpoints follow every shared-vertex change, preserving contour connectivity and Undo support.
- CosmicDesigner assembly version is now `1.7.0`.

## 1.6.0 — 2026-10-03

- `CR-016`: H/W Section Designer now uses the actual local material interval where its Flat Designer selector intersects the current Outer Contour.
- Merged boundary notches shorten Flat/Bent section profiles and omit bends outside the selected material interval.
- Existing full-section segment calculations remain editable; contour-derived clipped dimensions are displayed read-only.
- CosmicDesigner assembly version is now `1.6.0`.

## 1.5.1 — 2026-10-02

- `CR-015`: replaced automatic boundary merging with an orange merge-candidate state and explicit `Outer Contour로 통합` right-click command.
- Generalized Rectangle subtraction to the current rectilinear Outer Contour so later Cuts can merge after earlier notches.
- Kept candidate Cuts editable until confirmation and retained Undo support.
- CosmicDesigner assembly version is now `1.5.1`.

## 1.5.0 — 2026-10-02

- `CR-015`: converts boundary-touching Rectangle Cuts into open Outer Contour notches.
- Removes the merged Cut and matching Inner Contour while preserving interior Rectangle Cuts.
- Adds 6-LINE corner-notch and 8-LINE edge-notch regression coverage.
- CosmicDesigner assembly version is now `1.5.0`.

## 1.4.2 — 2026-10-01

- `CR-014`: changed Section Designer wheel Zoom to remain fixed on the viewport center.
- Cleared accumulated Section Pan on every Zoom step so repeated scaling does not drift toward a corner.
- Kept Flat Designer pointer-anchored Zoom unchanged.
- CosmicDesigner assembly version is now `1.4.2`.

## 1.4.1 — 2026-10-01

- `CR-013`: replaced the fixed 190-pixel Bent viewport deduction with an adaptive W/H Fit calculation.
- Kept Bent geometry and dimension-editor placement on the same scale.
- Added short-wide and maximum-scale regression checks.
- CosmicDesigner assembly version is now `1.4.1`.

## 1.4.0 — 2026-10-01

- `CR-012`: added draggable, labeled H/W section selector lines to Flat Designer.
- Linked H selector X positions and W selector Y positions to the corresponding Section Designer views.
- Added material-bound clamping and section-position feedback.
- CosmicDesigner assembly version is now `1.4.0`.

## 1.3.4 — 2026-10-01

- Corrected Bent dimension witness points to align with the rendered dark material surface edges instead of invisible center-axis coordinates.
- Extended each Bent segment dimension through its bend ends to the opposing exterior contact faces, so adjacent dimensions are separated by the material thickness as in caliper measurement.
- Added a material-thickness dimension to W Section Designer Flat and Bent views.
- CosmicDesigner assembly version is now `1.3.4`.

## 1.3.3 — 2026-10-01

- `CR-010`: corrected the nearly invisible 1.3.2 preview with higher-opacity light-gray acrylic surfaces.
- Increased the minimum and model-relative thickness of black exterior and bend edges.
- Changed edge rendering to black diffuse geometry for reliable contrast under uniform ambient light.
- CosmicDesigner assembly version is now `1.3.3`.

## 1.3.2 — 2026-10-01

- `CR-010`: simplified the 3D preview to a white-background acrylic skeleton view.
- Reduced surface opacity so rear faces remain visible through front faces.
- Removed directional lighting and shading, and changed all exterior/bend edges to opaque black.
- CosmicDesigner assembly version is now `1.3.2`.

## 1.3.1 — 2026-10-01

- `CR-006`: removed Bent Mode center-axis bend dots and changed dimension anchors to the measurable exterior edges of the thickness-bearing section polygon.
- Selected bends are now indicated by a boundary across material thickness instead of a center-axis point.
- CosmicDesigner assembly version is now `1.3.1`.

## 1.3.0 — 2026-10-01

- `CR-004`: isolated the Flat Designer drawing viewport from ruler bands during Zoom and Pan.
- `CR-005`: added an explicit Select mode, Hole-mode exit behavior, Flat Delete routing and Object Tree selection synchronization.
- `CR-006`: removed duplicated static Section dimension values while retaining transparent editable controls.
- `CR-007`: added H/W Section View menus and persistent last-used Dimensions, Bent and Rotation settings with file-state precedence.
- `CR-008`: added pointer-anchored mouse-wheel Zoom to both Section Designers.
- `CR-009`: restricted V/V1 creation to clicks inside the rendered flat material body.
- `CR-010`: changed the 3D preview to translucent acrylic surfaces with opaque gray edges.
- CosmicDesigner assembly version is now `1.3.0`.

## 1.2.0 — 2026-10-01

- `CR-001`: established the approved CDF proposal gate while retaining DXF metadata for this release.
- `CR-002`: added a persistent, deduplicated Recent Files menu with a 10-file limit.
- `CR-003`: added View menu commands and persistent Ruler, Grid, and Status Bar settings.
- `CR-004`: added Flat Designer Fit, Zoom, right-button Pan, centimeter ruler and adaptive grid.
- `CR-005`: added Hole selection handles, Move, corner Resize, edge Resize, pointer coordinates, and gesture-level Undo recording.
- `CR-006`: made Section dimension editors transparent when idle with hover/focus feedback.
- `CR-007`: persisted independent W/H Dimensions, Bent Mode, and rotation in backward-compatible DXF metadata.
- CosmicDesigner assembly version is now `1.2.0`.

## Baseline — 2026-10-01

- Established the observable current source state as the project-management baseline.
- Introduced CR, ADR, current-requirements, project-status, and release documentation policies.
- Did not change application logic or rename existing technical/product artifacts.

## 1.20.14 — 2026-10-05
- CR-049: 원 천공 다음에 첨부 윤곽 형태의 반원 천공 버튼. 네 방향 드래그로 정확한 반원 생성, 점선 미리보기, 재료 경계 제한, 방향 보존 편집/Undo/DXF.
- 자동 신규/영향/전체 회귀 PASS; UI 수동 검증 대기.

## 1.20.15 — 2026-10-05
- CR-050: 반원 지름이 외곽과 겹치면 명시적 우클릭으로 정확한 ARC 홈 통합. 부분 겹침/연속 LINE/Fillet 유지, Undo/DXF 지원.
- 반대 진행 방향 ARC의 Flat 표시와 DXF 출력을 일치시키고 단면에 원호 홈의 교점을 반영. 신규/영향/전체 자동 회귀 PASS; UI 수동 검증 대기.

## 1.20.16 — 2026-10-05
- CR-051: 반원 속성을 실제 원호 Center X/Y와 Radius R로 개선. 우측 외곽 접촉 반원은 Center X=300 표시. 반지름 변경 시 중심 및 지름 유지, 유효 범위 검증.
- 신규/영향/전체 AUTO 1/1, 6/6, 61/61 PASS; UI 수동 검증 대기.

## 1.20.17 — 2026-10-05
- CR-052: 반원 다음 4분원 도구, 드래그 사분면의 정확한 원호, 두 반지름 경계 겹침 통합 및 Radius R 속성.
- Release 0 warnings/errors; new AUTO 1/1, affected 11/11, full 62/62 PASS. Manual UI acceptance PENDING_MANUAL.
## 1.20.18 — 2026-10-05

- Corrected Flat W/H to equal the exact sum of non-Bent Section segment lengths instead of subtracting bend thickness compensation a second time.
- Preserved material-thickness compensation exclusively for Bent exterior measurement dimensions.
- Added CR-053 automated and manual regression cases for the `199 / 1998 / 199 = 2396` scenario.
- Release build passed with zero warnings/errors; new 1/1, affected 11/11 and full AUTO 63/63 passed. Manual UI acceptance remains pending.
## 1.20.21 — 2026-10-05
- CR-056: 불러오기/Undo 이후 기본 사각 재료의 Section 치수와 Flat 외곽 불일치를 수정했다.
- Release build 0 warnings/errors. New AUTO 1/1, affected 7/7, full 65/65 PASS. MANUAL PENDING_MANUAL.
## 1.20.22 — 2026-10-05
- CR-057: Slit 아이콘 선을 Hole과 동일한 1.8 DIP로 통일하고 Hole 아이콘을 줄여 하단 잘림을 방지했다.
- Release warning/error 0. Affected AUTO 3/3, Full AUTO 65/65 PASS; manual visual acceptance pending.
## 1.20.23 — 2026-10-05
- CR-058: H/W Bent 단면의 과장된 최소 화면 두께를 제거하고 윤곽과 치수 기준을 실제 재료 두께/배율로 통일했다.
- Release 0 warnings/errors; new AUTO 1/1, affected 7/7, full 66/66 PASS. MANUAL PENDING_MANUAL.
## 1.20.24 — 2026-10-05
- CR-059: Section Bent 치수값 중앙 정렬 및 겹치는 큰 치수선 바깥 배치.
- Release 0 warnings/errors; new 1/1, affected 6/6, full AUTO 67/67 PASS; MANUAL PENDING_MANUAL.
## 1.20.25 — 2026-10-05
- CR-060: Flat V/V1 생성 버튼, 방향별 Section 쐐기 연동, 절곡선 클릭 선택 및 평행 이동.
- Release 0 warnings/errors; new 1/1, affected 9/9, full AUTO 68/68 PASS; MANUAL PENDING_MANUAL.

## 1.20.27 — 2026-10-05
- CR-061: 홈 사이 연결부의 실제 절곡축으로 나비 날개 3D 회전 교정. 신규 1/1, 영향 9/9, 전체 70/70 AUTO PASS. 사용자 시각 검증 대기.

## 1.20.28 — 2026-10-05
- CR-062: H/W Section의 모든 분리 구간과 Outer Contour 홈 표시; 빈 공간 절곡 생성 방지. 신규 1/1, 영향 10/10, 전체 71/71 AUTO PASS; 사용자 도면 시각 검증 대기.

## 1.20.29 — 2026-10-06
- CR-063: Cut 이동 중 인접 외곽/절곡선 중앙 점선 안내. 신규 1/1, 영향 9/9, 전체 72/72 AUTO PASS; 사용자 UI 확인 대기.

## CR-065 — 2026-10-06
설치 파일을 CosmicDesignerSetup.exe로 생성하고 Freeze 버전 1.20.29-rc1을 적용한다 (Windows FileVersion 1.20.29.0).

## CR-066 — 2026-10-06
설치 화면/기본 경로/그룹/바탕 화면/완료 후 실행을 CosmicDesigner로 변경.
