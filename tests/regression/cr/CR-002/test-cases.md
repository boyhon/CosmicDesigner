# CR-002 Test Cases — 최근 파일 10개 열기 및 지속성

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-002-001
- Related CR: CR-002
- Status: ACTIVE
- 목적: 최근 파일 10개 열기 및 지속성 — RecentFiles 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-002-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::RecentFiles
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-002-002
- Related CR: CR-002
- Status: ACTIVE
- 목적: 최근 파일 10개 열기 및 지속성 — SettingsPersistence 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-002-002 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::SettingsPersistence
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-002-003
- Related CR: CR-002
- Status: ACTIVE
- 목적: 최근 파일 10개 열기 및 지속성 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-002/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. `File > Open` 바로 다음에 `Recent` 메뉴가 표시된다.
2. 성공적으로 연 파일만 목록에 추가된다.
3. 중복 경로는 하나만 존재하며 다시 열면 최상단으로 이동한다.
4. 목록은 최신순 10개를 초과하지 않는다.
5. 프로그램 재실행 후 경로와 순서가 복원된다.
6. Recent를 통한 열기가 일반 Open과 동일한 결과를 만든다.
7. 누락 파일 처리 후 프로그램이 정상 동작한다.
8. 기존 DXF 열기·Import·저장 기능의 회귀 테스트가 통과한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
