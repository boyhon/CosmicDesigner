# Cosmic Designer 개발 — Changelog

Release-relevant changes after the baseline are recorded here. CR identifiers must accompany functional entries.

## Unreleased

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
