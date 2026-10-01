# DXFViewer

판금 DXF의 제조 작업정보를 2D 작업 지시도 형태로 보여주는 .NET WPF 프로그램입니다. DXF를 이미지로 변환하지 않고 LINE/CIRCLE/ARC Entity를 내부 Geometry로 파싱해 렌더링합니다.

## 실행

Visual Studio에서 `DXFViewer.sln`을 열고 `DXFExplorer`를 시작 프로젝트로 실행합니다.

```powershell
dotnet run --project .\DXFExplorer\DXFExplorer.csproj
```

DXF 경로를 실행 인자로 넘겨 바로 열 수도 있습니다.

```powershell
dotnet run --project .\DXFExplorer\DXFExplorer.csproj -- "C:\drawing.dxf"
```

## 구현 기능

- ASCII DXF HEADER와 ENTITIES 분석
- LINE, CIRCLE, ARC Geometry 및 반지름을 포함한 Bounds 계산
- `-L-` 검정, `-V-` 빨강, `-V1-` 파랑 제조 레이어 매핑
- 알 수 없는 레이어의 자동 색상 배정
- 레이어별 표시/숨김, 범례, Entity 통계, 오류 로그
- 회색 작업 범위와 전체 크기 표시
- 종횡비 보존, Y축 변환, 화면 맞춤, 버튼/휠 확대·축소, 드래그 이동
- 마우스 위치의 원본 DXF 좌표 표시
- 방향이 반대인 동일 LINE을 포함한 중복 Geometry 탐지(원본 보존)
- 지원하지 않는 Entity를 집계하고 나머지 Geometry는 계속 표시

## 주요 구조

- `DxfDocumentParser`: HEADER/Entity 파싱과 오류 복구
- `DxfDocument`, `GeometryLine`, `GeometryCircle`, `GeometryArc`: 렌더링과 분리된 데이터 모델
- `LayerStyleProvider`: DXF Layer, 제조 의미, 화면 스타일 매핑
- `DxfViewport`: 좌표 변환, 렌더링, Fit/Zoom/Pan
- `DXFExplorer.Verification`: 실제 DXF용 명령행 검증 도구

## 현재 제약사항

현재 공통 파서 범위는 ASCII DXF의 LINE, CIRCLE, ARC입니다. 바이너리 DXF 및 POLYLINE, SPLINE, ELLIPSE, HATCH는 아직 렌더링하지 않으며 종류와 개수를 로그에 남깁니다. 공통 파서는 좌표를 강제 변환하지 않으며, CosmicDesigner의 일반 DXF Import가 `$INSUNITS`를 해석해 센티미터로 변환합니다.

상세 설계와 테스트 결과는 [docs/IMPLEMENTATION.md](docs/IMPLEMENTATION.md)를 참고하십시오.

CosmicDesigner의 반복 테스트 및 변경 개발 요건은 [docs/COSMIC_DESIGNER_CHANGE_REQUIREMENTS.md](docs/COSMIC_DESIGNER_CHANGE_REQUIREMENTS.md)에서 ID, 상태, 수용 기준과 변경 이력으로 관리합니다.

프로젝트 관리 문서는 [docs/project/PROJECT_STATUS.md](docs/project/PROJECT_STATUS.md)에서 시작합니다. Baseline 이후의 기능 변경은 [Development Policy](docs/project/DEVELOPMENT_POLICY.md)에 따라 Change Request로 관리합니다.
