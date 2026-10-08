# CR-043 Test Cases — 사선 Outer Contour의 사각형 통합 복구

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-043-001
- Related CR: CR-043
- Status: ACTIVE
- 목적: 사선 Outer Contour의 사각형 통합 복구 — RectangleAfterDiagonal 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-043-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::RectangleAfterDiagonal
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-043-002
- Related CR: CR-043
- Status: ACTIVE
- 목적: 사선 Outer Contour의 사각형 통합 복구 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-043/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

- LINE 삭제 후 사선 외곽에서 사각형 후보 및 통합이 동작한다.
- 삼각형·평행사변형 통합 후 사각형도 순차 통합된다.
- 내부 사각형과 재료 분리 사각형은 통합되지 않는다.
- 사선/폐합 유지, Undo/Redo, DXF 왕복 및 기존 사각형 통합 검증이 통과한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
