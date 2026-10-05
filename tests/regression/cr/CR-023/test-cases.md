# CR-023 Test Cases — 문서 단위와 두 가지 단위 변경 방식

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-023-001
- Related CR: CR-023
- Status: ACTIVE
- 목적: 문서 단위와 두 가지 단위 변경 방식 — DocumentUnits 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-023-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::DocumentUnits
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-023-002
- Related CR: CR-023
- Status: ACTIVE
- 목적: 문서 단위와 두 가지 단위 변경 방식 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-023/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. cm 300을 실제 크기 유지로 mm로 바꾸면 3000 mm가 된다.
2. cm 300을 숫자 유지로 mm로 바꾸면 300 mm가 된다.
3. 모든 geometry 및 제조 관련 길이값이 실제 크기 유지 방식에서 같은 비율로 변환된다.
4. DXF `$INSUNITS`와 메타데이터가 문서 단위를 기록한다.
5. 저장 후 다시 열면 단위와 숫자가 동일하다.
6. Undo/Redo가 단위와 숫자를 함께 복원한다.
7. 전체 자동 회귀 테스트가 통과한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
