# CR-021 Test Cases — 미저장 변경 사항 저장 확인

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-021-001
- Related CR: CR-021
- Status: ACTIVE
- 목적: 미저장 변경 사항 저장 확인 — DirtyState 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-021-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::DirtyState
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-021-002
- Related CR: CR-021
- Status: ACTIVE
- 목적: 미저장 변경 사항 저장 확인 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-021/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. 변경 후 New를 실행하면 저장 확인창이 표시된다.
2. 변경 후 Open 또는 Recent를 실행하면 저장 확인창이 표시된다.
3. 변경 후 File Exit 또는 창 X 닫기를 실행하면 저장 확인창이 표시된다.
4. Yes와 성공한 Save 후 요청 작업이 계속된다.
5. No 선택 시 저장하지 않고 요청 작업이 계속된다.
6. Cancel 또는 Save As 취소/실패 시 현재 작업이 유지된다.
7. 변경이 없으면 확인창 없이 요청 작업이 실행된다.
8. 저장 성공 후에는 추가 변경 전까지 확인창이 표시되지 않는다.
9. 전체 자동 회귀 테스트가 통과한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
