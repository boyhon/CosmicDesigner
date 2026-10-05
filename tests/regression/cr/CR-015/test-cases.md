# CR-015 Test Cases — Outer Contour 경계 천공 병합

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-015-001
- Related CR: CR-015
- Status: ACTIVE
- 목적: Outer Contour 경계 천공 병합 — BoundaryCuts 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-015-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::BoundaryCuts
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-015-002
- Related CR: CR-015
- Status: ACTIVE
- 목적: Outer Contour 경계 천공 병합 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-015/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. 모서리에 닿은 Rectangle Cut이 사라지고 Inner Contour도 제거된다.
2. 기존 4-LINE Outer Contour가 6-LINE 노치 외곽으로 바뀐다.
3. 한 변 중앙에 닿은 Rectangle은 8-LINE 외곽이 된다.
4. 내부 Rectangle Cut은 자동 병합되지 않는다.
5. 저장된 DXF의 L 레이어에는 새 Outer Contour가 반영된다.
6. 전체 자동 회귀 테스트가 통과한다.
7. 경계 접촉만으로 자동 병합되지 않고 Cut을 계속 이동·Resize할 수 있다.
8. 병합 가능 상태는 주황색, 불가능하거나 경계에서 떨어진 상태는 노란색으로 표시된다.
9. 우클릭 메뉴 선택 시에만 병합되고 Undo로 이전 상태를 복원할 수 있다.
10. 첫 병합 후 변경된 Outer Contour의 다른 모서리에도 후속 Rectangle Cut을 병합할 수 있다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
