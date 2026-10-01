# ARC Entity 지원 개선 보고서

## 누락 원인

기존 Entity dispatch는 `LINE`, `CIRCLE`만 Geometry로 변환했고 그 외 타입을 `UnsupportedCounts`에 기록했다. Geometry 모델과 WPF 렌더러에도 원호 타입이 없었다.

## 수정 내용

- `GeometryArc`: 중심 XYZ, 반지름, 원본 시작/끝 각도, Layer, 원본 group data 보존
- `NormalizeAngle`: 임의 각도를 `[0, 360)`으로 정규화
- `ContainsAngle`: CCW sweep과 0° crossing을 고려한 각도 포함 판정
- `PointAt`: degree 각도의 원호 좌표 계산
- `ExpandBounds`: 시작점, 끝점과 구간에 포함되는 0/90/180/270° 극점만 Bounds에 반영
- `DxfDocumentParser.CreateArc`: group code 8, 10, 20, 30, 40, 50, 51 파싱
- `DxfViewport`: WPF `StreamGeometry`와 `ArcSegment`로 실제 곡선 렌더링

화면은 DXF Y축을 반전하므로 DXF의 CCW sweep은 화면 좌표에서 `SweepDirection.Clockwise`로 그린다. 색상, 선 두께, 표시 여부는 다른 Entity와 동일하게 `LayerStyleProvider`를 사용한다.

## 자동 검증

각도 정규화, 180→270, 270→0, 350→10, 0→180 구간과 시작/끝 좌표, quarter/half/cross-zero Bounds를 검사하며 tolerance는 `0.000001`이다.

## 실제 DXF 결과

| 파일 | LINE | CIRCLE | ARC | Bounds | 미지원 |
|---|---:|---:|---:|---|---:|
| 삿갓방수함1-상.dxf | 27 | 0 | 2 | 300 × 317.5 | 0 |
| 삿갓방수함1.dxf | 33 | 0 | 0 | 300 × 681 | 0 |
| 3번 노출함 몸체.dxf | 28 | 0 | 0 | 300 × 657 | 0 |
| 3번 노출함 문짝.dxf | 29 | 0 | 0 | 359 × 349 | 0 |

대상 파일의 ARC는 다음과 같이 확인했다.

- Center `(40, 10)`, R10, `180° → 270°`: `(30, 10) → (40, 0)`
- Center `(260, 10)`, R10, `270° → 0°`: `(260, 0) → (270, 10)`

`기둥통바_전개도(1).dxf`는 검색 가능한 샘플 경로에서 발견되지 않아 회귀 검증에서 제외했다.

## 아직 렌더링하지 않는 Entity

`LWPOLYLINE`, `POLYLINE`, `SPLINE`, `ELLIPSE`, `HATCH` 등은 종류와 개수를 로그에 기록하고 나머지 도형을 계속 표시한다.
