# CR-020 Test Cases — File 메뉴 New/Open/Save 단축키

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-020-001
- Related CR: CR-020
- Status: ACTIVE
- 목적: File 메뉴 New/Open/Save 단축키 — FileShortcuts 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-020-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::FileShortcuts
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-020-002
- Related CR: CR-020
- Status: ACTIVE
- 목적: File 메뉴 New/Open/Save 단축키 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-020/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. `Ctrl+N`으로 새 문서가 생성된다.
2. `Ctrl+O`로 Open 대화상자가 열린다.
3. `Ctrl+S`로 현재 문서가 저장된다.
4. 저장 경로가 없으면 `Ctrl+S`가 기존 Save As 대화상자를 연다.
5. New, Open, Save 메뉴에 각각 `Ctrl+N`, `Ctrl+O`, `Ctrl+S`가 표시된다.
6. Undo/Redo 단축키가 계속 동작한다.
7. 전체 자동 회귀 테스트가 통과한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
