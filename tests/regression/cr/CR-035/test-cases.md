# CR-035 Test Cases — Fillet ARC 선택 표시 및 반지름 편집

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-035-001
- Related CR: CR-035
- Status: ACTIVE
- 목적: Fillet ARC 선택 표시 및 반지름 편집 — OuterContourFillet 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-035-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::OuterContourFillet
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-035-002
- Related CR: CR-035
- Status: ACTIVE
- 목적: Fillet ARC 선택 표시 및 반지름 편집 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-035/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

- Object Tree의 `L ARC` 선택 시 해당 호만 굵은 노란색으로 표시된다.
- 호 양 끝에 흰색/노란색 접점 핸들이 보인다.
- Selected object에서 `Radius R`을 편집할 수 있다.
- R10을 R20으로 변경하면 ARC 중심과 두 LINE 접점이 함께 이동한다.
- 변경 후 Outer Contour가 연결된 폐곡선을 유지한다.
- 과도한 반지름은 거부되고 Undo/Redo 및 DXF 왕복이 유지된다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
