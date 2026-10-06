# Cosmic Designer 개발 — Current Requirements

Only behavior confirmed by current source, tests, project configuration, or existing product documentation is listed here. Proposed future behavior belongs in a Change Request or the proposed-requirements backlog.

- Main Window Title은 새 문서에서 `Untitled - CosmicDesigner`, 파일 문서에서 `Filename.dxf - CosmicDesigner` 형식을 사용하고 저장되지 않은 변경이 있으면 맨 앞에 `*`를 표시한다.
- Settings에서 Flat Designer의 가로 Ruler, 세로 Ruler와 Grid 간격을 각각 Auto 또는 물리 길이 기준 사용자 값으로 설정하며 축소 시 과밀 표시를 자동 생략한다.
- Flat Designer는 Outer Contour 내부에서 Cut Object를 제외한 실제 재료 영역에만 재료색, Grid와 절곡선을 표시하고 절삭 영역은 흰색으로 표시한다.
- 3D Preview는 Outer Contour 외부와 모든 내부 Cut Object에 surface face를 생성하지 않아 잘려 나간 영역을 투명하게 표시한다.
- 3D 면은 Outer/Cut의 표시 윤곽과 같은 경계에서 분할하여 천공 공간과 경계선을 일치시킨다. Circle의 24각형 및 Fillet ARC의 최대 5도 간격 곡선 근사을 면과 선에 동일하게 적용한다.
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

- Select 모드에서 Slit 직선/원호/연결선의 실제 선을 클릭하여 경로 전체 선택, 금색 강조 및 OBJECTS/Selected object 연동을 한다. LINE 구간/ARC signed sweep 거리와 화면 픽셀 tolerance로 확대·축소/Pan에 대응하며 겹친 절곡보다 절개 선택 우선, 경로 간에는 가장 가까운 것/동률 마지막 경로를 선택한다 (CR-054 follow-up).

- 좌측 객체 속성 영역은 OBJECTS 제목 및 객체 트리가 위, PROPERTIES 제목과 Material/Selected object가 아래에 배치된다 (CR-055).

- Each document explicitly uses millimeters, centimeters or meters. Legacy metadata without a unit retains its historical millimeter-to-centimeter migration behavior.
- A new document starts with `300 × 300 cm` material and `0.2 cm` thickness.
- The document manages one outer contour, inner contours/cuts, bend objects, W/H section segments, and micro joints.
- Material-thickness changes recalculate thickness-dependent bend properties.
- Section dimensions and corresponding bend positions are editable and synchronized.
- Flat W/H is the exact sum of the corresponding non-Bent Section segment lengths; material-thickness compensation is applied only when converting those developed lengths to Bent exterior measurement dimensions.
- Bent section previews render a closed, thickness-bearing profile.
- Cut geometry includes the supported hole-shape tools and is emitted on the L layer.
- Hole toolbar exposes Circle, Semicircle, QuarterCircle, Triangle, Rectangle, Diamond and Parallelogram as small outline icon buttons with shape-name tooltips and accessibility names. Arc, Sector, Ellipse, Pentagon, Hexagon and Polygon buttons are omitted pending individual future requests.
- Slit toolbar uses the user-provided PNG icons for Line (two clicks), Arc (start/through/end clicks) and Polyline (successive clicks, Enter to finish). Esc cancels the unfinished path; completion returns to Select. Open slits are stored separately from holes as exact L-layer LINE/ARC entities, retain material surface area, support direct/tree selection, Delete, Undo/Redo, units and DXF round-trip, and appear as sampled cutting lines on the folded 3D surface (CR-054).
- Supported design objects can be selected through the object tree and edited through the Property panel.
- Supported selected objects can be deleted with the Delete key; deletions participate in Undo/Redo.
- A mergeable boundary-touching Rectangle Cut remains editable and is shown with an orange selection; moving it away restores the normal gold selection.
- A mergeable Rectangle Cut provides an `Outer Contour로 통합` right-click command. Only that explicit command converts it into an open notch and removes the Cut and matching Inner Contour.
- Fillet ARC-based Outer Contours support Rectangle, Triangle and Parallelogram boundary merging with original ARC centers/radii preserved; intersections trim exact ARC angle ranges rather than replace curves with LINEs.
- A Triangle or Parallelogram Cut whose subtraction leaves one closed Outer Contour uses the same orange candidate and explicit right-click merge workflow, preserving diagonal Cut edges and remaining Fillet ARCs in the resulting contour.
- Rectangle notches can be merged repeatedly into the current LINE/ARC Outer Contour, including diagonal edges and Fillet ARCs; fully interior rectangles and cuts that would produce multiple material components remain Cut objects.
- Outer Contour editing/recalculation connects every gap between successive LINE/ARC endpoints, including the final/first pair, with an explicit LINE. Fillet ARCs remain unchanged when adjacent LINEs move; deleting an ARC replaces its gap with a LINE. Connector geometry participates in Undo/Redo and DXF persistence.
- Deleting an Outer Contour LINE reconnects the following LINE to the deleted edge start, keeping the rendered material mask and actual closed boundary aligned; invalid or minimum-contour deletions are rejected.
- Flat Designer supports direct selection of Outer Contour LINE objects. Dragging a selected line body moves it perpendicular to itself, while dragging either endpoint changes its length; every shared endpoint is propagated to the connected neighboring LINE so the contour stays connected.
- Flat Designer provides a numeric-radius Fillet tool for convex right-angle Outer Contour LINE corners, with orange dashed preview, tangent quarter-circle ARC insertion, invalid-radius rejection and Undo support.
- Existing Fillet ARCs do not prevent additional eligible LINE–LINE corners on the same Outer Contour from receiving subsequent Fillets.
- Selecting an Outer Contour Fillet ARC in the Object Tree highlights the arc and its tangent endpoints in Flat Designer, and Selected object provides an editable `Radius R` that recalculates both adjacent LINE endpoints while preserving contour closure.
- Fillet supports convex non-parallel LINE–LINE corners at arbitrary angles on both the Outer Contour and LINE-based internal Hole contours; Cut child LINE/ARC objects can be selected in the Object Tree and their ARC radii remain editable.
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
- Triangle 최초 생성은 드래그 시작 Y에 밑변, 종료 Y에 꼭짓점을 배치하여 아래로 드래그하면 역삼각형, 위로 드래그하면 바로 선 삼각형이 되며 미리보기와 생성 결과가 같다.
- Circle Cut 전체 선택은 Width/Height 대신 현재 문서 단위의 `Radius R` 하나로 크기를 표시·편집한다. 중심을 유지하고 동일 지름으로 윤곽을 동기화한다. 반지름 .05 미만/비유한/재료 범위 초과 입력은 변경 없이 거부한다 (CR-048).
- Triangle Cut 전체 선택은 현재 문서 단위의 `Lower-left X/Y`로 외접 사각형 좌하 좌표를 표시·편집하며, 위치 변경은 편집된 윤곽과 방향을 유지하여 평행 이동하고 재료 경계에 제한한다 (CR-047). Width/Height 속성 변경은 좌하 X/Y와 반대 치수를 고정하고 기존 세 LINE을 축별 확대/축소한다. 범위 초과/부적합 값 및 원형 ARC를 포함한 비지원 크기 변경은 도형을 이동하거나 재생성하지 않고 거부한다.
- Triangle Cut은 폐합된 세 LINE으로 유지되며, 선택 시 세 꼭짓점 핸들을 각각 이동하여 임의·직각·이등변 삼각형으로 편집할 수 있고 몸체 드래그는 현재 모양을 유지한 채 전체를 이동한다.
- Circle, Triangle, Rectangle, Diamond와 Parallelogram Hole 도구는 Flat Designer에서 기준점부터 드래그한 크기를 점선으로 미리 표시하고 Mouse Up 때 Cut을 한 개 생성하며, 짧은 클릭은 무시하고 생성 후 Select 모드로 복귀한다.
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
- Outer Contour Fillet ARCs are sampled at a maximum 5-degree interval in 3D Preview; the same sampled curve defines both surface clipping and outline edges while exact ARC geometry remains in Flat Designer and DXF.
- Internal Hole Fillet ARCs use the same sampled curve in 3D Preview, including radius changes and bend-station splitting; Flat Designer and DXF retain exact ARCs.

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

- CosmicDesigner Help provides executable-relative local User Manual, Getting Started, Keyboard Shortcuts and Troubleshooting HTML routes with missing-file/browser-failure guidance (CR-064). The official user documentation source is help-content/help; HTML/assets copy to build/publish output while internal Markdown/JSON are excluded. Every CR evaluates user documentation impact and updates affected pages before completion.

- Undo/Redo uses bounded FIFO-style history with a configured capacity of 100 in the main window.
- Functional changes should be covered by the relevant verification project and regression build.

## Proposed requirements, not current behavior

The following document is a structured proposal backlog and is not evidence that its items are implemented:

- [CosmicDesigner 변경 개발 요건서](../../COSMIC_DESIGNER_CHANGE_REQUIREMENTS.md)

The source bundle and subsequent natural-language requests are tracked in the [Change Request Index](../change-requests/README.md). Registration alone does not make a proposal a current implemented requirement.

- Semicircle 도구는 첫 클릭을 지름 중심으로 사용하고 주축 드래그 방향(상/하/좌/우)에 볼록한 정확한 180도 ARC와 지름 LINE 천공을 생성한다. 이동량은 반지름이며 재료 범위로 제한한다. 대각선은 큰 축(동률 세로) 기준, 점선 미리보기, Mouse Up 단일 생성, 짧은 클릭 무시 및 Select 복귀를 적용한다. 이동/크기 편집 및 DXF/Undo/Redo는 방향과 반원 비율을 보존한다 (CR-049).

- Semicircle 지름이 Outer Contour LINE과 양의 길이로 겹치고 하나의 폐합 재료 경계로 차집합이 가능하면 주황 후보와 `Outer Contour로 통합` 우클릭 명령을 제공한다. 중복 지름을 제거하고 정확한 ARC 홈을 외곽에 연결하며 Cut/Inner Contour를 제거한다. 내부/점 접촉/재료 외부/분리/퇴화는 거부한다. 원호 진행 방향을 Flat mask/표시, 3D, DXF, Undo/Redo에 보존하고 단면은 ARC 및 사선 LINE의 정확한 교점을 사용한다 (CR-050).

- Semicircle 전체 선택 속성은 현재 문서 단위로 실제 ARC 중심인 Center X/Y와 Radius R을 제공하며 Width/Height를 생략한다. 반지름 편집은 원호 중심/지름 위치/방향을 유지하고 중심 편집은 평행 이동한다. 비유한/반지름 .1 미만/재료 경계 초과 값은 이동 또는 축소 없이 거부한다. 기존 외접 중심 저장 표현은 호환 유지한다 (CR-051).

- QuarterCircle 도구는 Semicircle 다음의 4분원 윤곽 아이콘이며 첫 클릭을 실제 원호 중심으로 사용한다. X/Y 이동 부호로 1/2/3/4사분면을 정하고 min(abs(dx),abs(dy)) 반지름의 정확한 90도 ARC 및 두 반지름 LINE을 생성한다. 축상/짧은 드래그 거부, 재료 범위 제한, 점선 미리보기와 단일 생성/Select 복귀. 어느 반지름이든 외곽 LINE과 양의 길이로 겹치고 차집합이 단일 폐합 경계면 주황 후보/명시적 우클릭 통합. Width/Height 대신 현재 단위의 실제 Center X/Y 및 Radius R을 편집하며 반지름 변경은 중심/방향을 보존하고 비유한/.1 미만/재료 초과 값은 변경 없이 거부한다. 이동/resize/단위/DXF/Undo에서 원형과 방향을 유지한다 (CR-052).
- CR-056: 현재 재료 W/H와 일치하는 기본 사각 Outer Contour는 DXF 또는 Undo/Redo 복구 후에도 Section 길이 편집에 맞춰 갱신한다. 별도 편집 외곽 보존 정책은 유지한다.
- CR-057: Hole/Slit toolbar 아이콘은 동일한 색과 1.8 DIP 선을 사용하며 Hole 높이는 최대 18 DIP로 버튼 내부에서 전체 윤곽을 표시한다.
- CR-058: Bent H/W Section 윤곽과 치수 기준은 실제 thickness × scale을 사용한다. 최소 화면 두께 10px 과장 정책은 Bent 표시에서 제거한다. Flat schematic strip은 기존 정책을 유지한다.
- CR-059: Bent H/W 치수값은 치수선 길이 방향 중앙에 배치한다. 같은 쪽의 겹치는 평행 치수선은 큰 길이부터 바깥쪽 단계로 배치하며 그림과 editor는 같은 layout을 사용한다.
- CR-060: Slit 다음 V/V1 버튼의 오른쪽 드래그는 H 수평 절곡(Left/Right), 아래쪽 드래그는 W 수직 절곡(Up/Down)을 생성한다. Flat에서 클릭 선택하고 선에 수직인 방향으로만 X/Y Position을 이동하며 재료 범위와 최소 .01 구간 길이를 유지한다. 기존 쐐기/속성/3D/DXF/Undo 연동.

- CR-061: 홈으로 분리된 날개와 중앙 연결부의 복합 절곡은 연결부의 현재 3D 힌지로 이미 접힌 재료를 함께 회전한다. 일직선 힌지가 아닌 연속 교차 절곡은 기존 표시로 fallback한다.

- CR-062: H/W 선택 단면은 Outer Contour의 모든 LINE/ARC 교점을 기준으로 모든 재료 구간을 표시하고 실제 빈 구간의 위치/간격을 유지한다. Flat/Bent 분리 경로 및 구간별 치수를 사용하며 빈 공간에는 절곡을 생성하지 않는다. 단일 구간 치수 편집/3D 기존 API 호환을 유지한다.

- CR-063: Cut 몸체 이동 중, 중심을 가로지르는 인접 평행 외곽 LINE/V/V1 사이 중앙과 1.5 DIP 이내면 빨간 점선 가로/세로 중앙 안내와 중심 십자를 표시한다. 두 축은 독립. 확대/Pan 대응, 자동 흡착 없음, resize/이동 종료/다른 선택에는 표시하지 않는다. Semicircle/QuarterCircle은 실제 ARC 중심 사용.
- CR-065: 설치 출력 CosmicDesignerSetup.exe; Freeze 설치 버전 1.20.29-rc1, 숫자형 FileVersion 1.20.29.0. 기존 AppId/제품명 유지.
