# CR-041 Test Cases — 삼각형 천공 드래그 방향 반영

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-041-001
- Related CR: CR-041
- Status: ACTIVE
- 목적: 삼각형 천공 드래그 방향 반영 — TriangleDragDirection 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-041-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::TriangleDragDirection
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-041-002
- Related CR: CR-041
- Status: ACTIVE
- 목적: 삼각형 천공 드래그 방향 반영 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-041/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

- 상하 드래그 및 좌우 반대 방향에서 요구한 방향의 삼각형이 생성된다.
- 미리보기와 생성된 세 LINE의 꼭짓점이 일치한다.
- 재료 경계 제한 및 단순 클릭/작은 드래그 거부가 유지된다.
- 방향은 이동·Undo/Redo·DXF 저장/재열기에서도 유지된다.
- Release 빌드 및 전체 자동 검증 통과.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
