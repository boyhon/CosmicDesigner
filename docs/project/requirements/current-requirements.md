# Cosmic Designer 개발 — Current Requirements

Only behavior confirmed by current source, tests, project configuration, or existing product documentation is listed here. Proposed future behavior belongs in a Change Request or the proposed-requirements backlog.

- Main Window Title은 새 문서에서 `Untitled - CosmicDesigner`, 파일 문서에서 `Filename.dxf - CosmicDesigner` 형식을 사용하고 저장되지 않은 변경이 있으면 맨 앞에 `*`를 표시한다.
- Settings에서 Flat Designer의 가로 Ruler, 세로 Ruler와 Grid 간격을 각각 Auto 또는 물리 길이 기준 사용자 값으로 설정하며 축소 시 과밀 표시를 자동 생략한다.
- Flat Designer는 Outer Contour 내부에서 Cut Object를 제외한 실제 재료 영역에만 재료색, Grid와 절곡선을 표시하고 절삭 영역은 흰색으로 표시한다.
- 3D Preview는 Outer Contour 외부와 모든 내부 Cut Object에 surface face를 생성하지 않아 잘려 나간 영역을 투명하게 표시한다.
- 3D Preview의 검은 Edge는 Outer Contour와 Cut 경계에만 표시하며 내부 절곡선과 메시 분할선은 표시하지 않는다.
- 3D Preview는 실제 재료 위의 V 절곡선을 적색, V1 절곡선을 청색 Edge로 표시한다.

## Project identity

- The project-management name is **Cosmic Designer 개발**.
- Existing technical names and product outputs remain unchanged unless an approved CR changes them.

## Platform and composition

- Applications are Windows WPF projects targeting .NET 10.
- Existing outputs include DXFExplorer, DXFViewer, DXFSimulator, DXFDrawer, and CosmicDesigner.
- Shared DXF models and parsing are supplied by the DXFExplorer project to dependent applications.

## DXF processing

- ASCII DXF is supported; binary DXF is rejected with an explanatory error.
- The shared geometry parser supports `LINE`, `CIRCLE`, and `ARC` entities.
- Entity layer names are retained and unsupported entity types are reported.
- Manufacturing geometry uses L for cutting and V/V1 for opposite bend directions.

## CosmicDesigner domain behavior

- Each document explicitly uses millimeters, centimeters or meters. Legacy metadata without a unit retains its historical millimeter-to-centimeter migration behavior.
- A new document starts with `300 × 300 cm` material and `0.2 cm` thickness.
- The document manages one outer contour, inner contours/cuts, bend objects, W/H section segments, and micro joints.
- Material-thickness changes recalculate thickness-dependent bend properties.
- Section dimensions and corresponding bend positions are editable and synchronized.
- Bent section previews render a closed, thickness-bearing profile.
- Cut geometry includes the supported hole-shape tools and is emitted on the L layer.
- Supported design objects can be selected through the object tree and edited through the Property panel.
- Supported selected objects can be deleted with the Delete key; deletions participate in Undo/Redo.
- A mergeable boundary-touching Rectangle Cut remains editable and is shown with an orange selection; moving it away restores the normal gold selection.
- A mergeable Rectangle Cut provides an `Outer Contour로 통합` right-click command. Only that explicit command converts it into an open notch and removes the Cut and matching Inner Contour.
- Rectangle notches can be merged repeatedly into the current rectilinear Outer Contour; fully interior rectangles and cuts that would produce multiple material components remain Cut objects.
- Flat Designer supports direct selection of Outer Contour LINE objects. Dragging a selected line body moves it perpendicular to itself, while dragging either endpoint changes its length; every shared endpoint is propagated to the connected neighboring LINE so the contour stays connected.
- The 3D preview supports mouse-driven viewpoint rotation and wheel zoom.
- File > Recent retains up to 10 successfully opened files in a user-level persistent setting.
- File > New, Open and Save display and execute the global keyboard shortcuts `Ctrl+N`, `Ctrl+O` and `Ctrl+S` respectively.
- Edit > Settings provides persistent user preferences for default folders, Recent count, interaction tolerances, Zoom step, 3D opacity/edge scale and new-document dimensions/unit without duplicating transient View commands.
- Edit > Document Units changes the active document between mm/cm/m either by preserving numeric values (rescaling physical interpretation) or by converting every length value to preserve physical size; the operation participates in Undo/Redo.
- When the current design has unsaved document changes, New, Open/Recent, Exit and window close ask whether to save. Yes completes saving before continuing, No discards the pending changes, and Cancel or a cancelled/failed Save As leaves the current document open.
- View provides Zoom In, Zoom Out, Fit to Window, Ruler, Grid, and Status Bar controls.
- Ruler, Grid, and Status Bar visibility are global persistent settings.
- Flat Designer supports right-button Pan and wheel/menu Zoom with a shared centimeter coordinate transform.
- Flat Designer supports Cut Object selection, control handles, Move, corner Resize, and edge Resize across the supported Hole shapes.
- Triangle Cut은 폐합된 세 LINE으로 유지되며, 선택 시 세 꼭짓점 핸들을 각각 이동하여 임의·직각·이등변 삼각형으로 편집할 수 있고 몸체 드래그는 현재 모양을 유지한 채 전체를 이동한다.
- Flat Designer keeps design geometry outside the ruler bands and provides explicit Select/Hole mode switching.
- Flat Designer selection is synchronized with the Object Tree and supports focused Delete; leaving the designer cancels Hole mode.
- Flat Designer displays labeled H and W section selectors as colored dashed lines; H moves horizontally, W moves vertically, and both are clamped to the material bounds.
- Moving a Flat Designer section selector immediately refreshes the corresponding H or W Section Designer and displays its selected X or Y coordinate.
- H/W Section Designer intersects the selected Flat Designer section line with the current Outer Contour, so boundary notches shorten the displayed local Flat/Bent profile and exclude bends outside that material interval.
- Dimensions in a clipped local H/W Section remain editable. Changing one propagates the delta through subsequent local bend positions and the intersected Outer Contour boundary (or symmetrically from the opposite clipped boundary), while connected contour LINE endpoints and unaffected local lengths remain consistent.
- Section dimension editors use a transparent idle appearance with focus feedback and are the single rendered dimension value.
- Bent Section dimensions use measurable thickness-polygon exterior edges; center-axis bend dots are not rendered.
- Bent dimension witness points align with the rendered dark material surface edges and never use the invisible center axis as a measurement reference.
- Bent segment dimensions span the opposing exterior contact faces a caliper can measure; adjacent segment endpoints at a bend are offset by the rendered material thickness rather than sharing one miter point.
- W Section Designer shows the material thickness dimension in Flat and Bent views.
- H/W Section Designers support center-fixed mouse-wheel Zoom that keeps geometry centered and only create bends from clicks inside the flat material body.
- Bent H/W Section views use a shared adaptive Fit scale that preserves dimension space without collapsing geometry in short, wide panels.
- View provides synchronized H/W Dimensions, Bent and Rotation controls.
- The 3D preview uses a white background, visibly light-gray translucent acrylic surfaces and sufficiently thick opaque black exterior/bend edges without directional lighting or shadow-like shading.
- The 3D preview surface mesh is clipped to the current rectilinear Outer Contour, so merged boundary cuts and subsequent contour edits remove the corresponding 3D faces and update exterior black edges while preserving fold edges.

## Persistence and compatibility

- CosmicDesigner saves DXF in centimeters and embeds CosmicDesigner metadata for application-specific state.
- A DXF containing valid CosmicDesigner metadata reopens as a design document.
- A general ASCII DXF without metadata can be imported from L/V/V1 geometry after user confirmation.
- Millimeter or unit-unspecified general DXF input is converted to centimeters; centimeter DXF input remains in centimeters.
- General import requires Save As so the source is not overwritten implicitly.
- Legacy CosmicDesigner metadata without a unit marker is interpreted as the earlier millimeter representation and converted to centimeters.
- W/H Section Dimensions, Bent Mode, and rotation states are saved independently in backward-compatible optional DXF metadata fields.
- The last-used W/H Section view states are also stored globally for new/imported documents; file metadata takes precedence when present.

## History and verification

- Undo/Redo uses bounded FIFO-style history with a configured capacity of 100 in the main window.
- Functional changes should be covered by the relevant verification project and regression build.

## Proposed requirements, not current behavior

The following document is a structured proposal backlog and is not evidence that its items are implemented:

- [CosmicDesigner 변경 개발 요건서](../../COSMIC_DESIGNER_CHANGE_REQUIREMENTS.md)

The source bundle and subsequent natural-language requests are tracked in the [Change Request Index](../change-requests/README.md). Registration alone does not make a proposal a current implemented requirement.
