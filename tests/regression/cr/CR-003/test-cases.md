# CR-003 Test Cases — View 메뉴와 전역 표시 상태 지속성

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-003-001
- Related CR: CR-003
- Status: ACTIVE
- 목적: View 메뉴와 전역 표시 상태 지속성 — SettingsPersistence 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-003-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::SettingsPersistence
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-003-002
- Related CR: CR-003
- Status: ACTIVE
- 목적: View 메뉴와 전역 표시 상태 지속성 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-003/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. 명세와 동일한 View 메뉴 구조가 제공된다.
2. Ruler, Grid, Status Bar 메뉴는 체크형이며 실제 표시 상태와 일치한다.
3. 메뉴 변경이 현재 화면에 즉시 반영된다.
4. 다른 파일을 열어도 표시 상태가 유지된다.
5. 프로그램 재실행 후 상태가 복원된다.
6. 설정 부재 또는 손상 시 프로그램이 기본값으로 정상 실행된다.
7. Zoom 메뉴가 CR-004의 Zoom In, Zoom Out, Fit 동작을 실행한다.
8. 전역 설정과 파일별 Section 상태가 섞이지 않는 저장·재열기 테스트가 통과한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
