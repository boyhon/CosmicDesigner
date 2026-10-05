# CR-030 Test Cases — 삼각형 Cut 꼭짓점 직접 편집

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-030-001
- Related CR: CR-030
- Status: ACTIVE
- 목적: 삼각형 Cut 꼭짓점 직접 편집 — TriangleEditing 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-030-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::TriangleEditing
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-030-002
- Related CR: CR-030
- Status: ACTIVE
- 목적: 삼각형 Cut 꼭짓점 직접 편집 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-030/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

- Triangle Cut 선택 시 정확히 세 꼭짓점 편집 핸들이 표시된다.
- 어느 꼭짓점도 독립적으로 이동할 수 있고 세 LINE은 항상 폐합된다.
- 삼각형 내부 드래그는 변형 없이 전체 도형을 이동한다.
- 꼭짓점은 재료 범위 밖으로 이동하지 않는다.
- 편집한 임의 삼각형이 저장 후 다시 열어도 보존된다.
- 기존 Cut, Flat 절삭 영역 및 3D Preview 회귀 검증이 통과한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
