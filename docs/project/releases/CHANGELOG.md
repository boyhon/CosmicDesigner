# Cosmic Designer 개발 — Changelog

Release-relevant changes after the baseline are recorded here. CR identifiers must accompany functional entries.

## Unreleased

- No changes registered after 1.3.3.

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
