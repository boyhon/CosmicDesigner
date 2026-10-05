# CR-040 Test Cases — 평행사변형 Cut Outer Contour 통합

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-040-001
- Related CR: CR-040
- Status: ACTIVE
- 목적: 평행사변형 Cut Outer Contour 통합 — ParallelogramBoundaryCut 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-040-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::ParallelogramBoundaryCut
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-040-002
- Related CR: CR-040
- Status: ACTIVE
- 목적: 평행사변형 Cut Outer Contour 통합 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-040/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

- 상하 경계 및 모서리에 겹친 평행사변형이 후보가 된다.
- 후보 선택색은 주황색이고 외곽에서 떨어지면 일반 선택색으로 돌아간다.
- 명시적 통합만 Cut/Inner Contour를 제거하며 새 외곽은 폐합 및 사선을 유지한다.
- 내부 Hole과 재료를 분리하는 Cut은 후보가 아니다.
- Undo/Redo 및 DXF 왕복 검증과 전체 자동 검증 통과.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
