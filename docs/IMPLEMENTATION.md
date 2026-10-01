# DXFViewer 1차 구현 보고서

> 이 문서는 1차 구현 당시의 기록이다. 현재 지원 범위와 프로젝트 상태는 `README.md` 및 `docs/project/` 문서를 우선한다.

## 요구사항과 범위

판금 DXF에 기록된 절단선, Hole/Cutout, V 가공선을 CAD 지식 없이 확인할 수 있는 2D 작업도 뷰어를 구현했다. 3D 절곡 결과 복원은 범위에서 제외했다.

지원 DXF subset:

- ASCII DXF
- HEADER: `$ACADVER`, `$DWGCODEPAGE`, `$MEASUREMENT`, `$INSUNITS`
- ENTITIES: `LINE`, `CIRCLE`
- Layer: 모든 이름을 보존하며 `-L-`, `-V-`, `-V1-`에 기본 제조 의미와 스타일을 제공

## 프로그램 구조

```text
DXF File
  → DxfDocumentParser
  → DxfDocument / GeometryEntity
  → LayerStyleProvider / ManufacturingOperation
  → DxfViewport
  → WPF UI
```

원본 group code/value 목록은 각 Geometry의 `OriginalEntity`에 보존된다. 지원하지 않는 Entity는 중단을 일으키지 않고 `UnsupportedCounts`와 로그에 기록된다.

## Geometry와 Bounds

- `GeometryLine`: Start/End XYZ와 원본 Layer 보존
- `GeometryCircle`: Center XYZ, Radius, Layer 보존
- LINE Bounds는 양 끝점을 포함한다.
- CIRCLE Bounds는 `center ± radius`를 포함한다.
- 역방향을 포함한 동일 LINE은 tolerance `0.000001`로 탐지하며 삭제하지 않고 `IsDuplicate`, `DuplicateOf`만 기록한다.

## Layer Mapping

| DXF Layer | 제조 의미 | 기본 색상 |
|---|---|---|
| `-L-` | 절단 형상 | 검정 |
| `-V-` | V 가공/절곡선 | 빨강 |
| `-V1-` | 별도 V 가공선 | 파랑 |
| 기타 | 미지정 작업 | 자동 팔레트 |

매핑은 `LayerStyleProvider`에 있어 파서 및 렌더러와 분리되어 있다.

## Rendering과 UI

DXF 좌표는 수정하지 않는다. Viewport가 단일 Scale, Translate, Y축 반전을 적용한다. X/Y 배율이 같으므로 CIRCLE의 원형과 전체 종횡비가 유지된다.

UI는 파일 정보/Entity/Layer 제어, 중앙 작업도, 분석 결과/로그로 구성된다. 파일 열기, Fit, 확대/축소, 휠 Zoom, 드래그 Pan, DXF 마우스 좌표, Layer On/Off, Bounds On/Off를 지원한다.

## 테스트 결과

확보된 실제 샘플 3개를 `DXFExplorer.Verification`으로 검증했다.

| 파일 | LINE | CIRCLE | Layer | Bounds | 중복 LINE |
|---|---:|---:|---|---|---:|
| 3번 노출함 몸체.dxf | 28 | 0 | `-L-`, `-V-` | 300 × 657 | 0 |
| 3번 노출함 문짝.dxf | 29 | 0 | `-L-`, `-V-` | 359 × 349 | 0 |
| 삿갓방수함1.dxf | 33 | 0 | `-L-`, `-V-`, `-V1-` | 300 × 681 | 3 |

위 결과는 개발 지시서의 기대 Entity 수와 Bounds에 일치한다. `기둥통바_전개도(1).dxf`는 검색 가능한 프로젝트 범위에서 발견되지 않아 실제 CIRCLE 샘플 검증은 수행하지 못했다. CIRCLE 코드 경로는 중심과 반지름을 파싱하고 반지름을 Bounds 및 화면 반경에 적용하도록 구현했다.

Release 솔루션 빌드 결과: 경고 0, 오류 0.

## 알려진 제약과 확장 방향

- 바이너리 DXF 미지원
- ARC, LWPOLYLINE, POLYLINE, SPLINE, ELLIPSE, HATCH 렌더링 미지원
- 외곽 Contour와 내부 Cutout의 의미론적 자동 분류 미지원
- Layer Mapping 편집/저장 UI 미지원
- DXF 단위 자동 변환 미지원

향후 Geometry 파생 클래스를 추가해 Entity 지원을 확장하고, 닫힌 Contour 분석과 작업 Layer 매핑 설정 저장 기능을 추가할 수 있다.
