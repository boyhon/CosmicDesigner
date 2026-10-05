# CR-007 Test Cases — W/H Section Designer 보기 상태의 파일별 저장

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-007-001
- Related CR: CR-007
- Status: ACTIVE
- 목적: W/H Section Designer 보기 상태의 파일별 저장 — RoundTrip 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-007-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::RoundTrip
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-007-002
- Related CR: CR-007
- Status: ACTIVE
- 목적: W/H Section Designer 보기 상태의 파일별 저장 — SettingsPersistence 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-007-002 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::SettingsPersistence
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-007-003
- Related CR: CR-007
- Status: ACTIVE
- 목적: W/H Section Designer 보기 상태의 파일별 저장 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-007/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. W/H의 Dimensions, Bent Mode, Rotation을 서로 다른 조합으로 저장할 수 있다.
2. 닫고 다시 열면 각 축의 CheckBox와 실제 렌더링 상태가 동일하게 복원된다.
3. 새 문서가 정의된 기본 상태로 시작한다.
4. 새 필드가 없는 기존 CosmicDesigner 파일이 오류 없이 기본 상태로 열린다.
5. 전역 Ruler/Grid/Status Bar 설정과 파일별 Section 상태가 섞이지 않는다.
6. 저장 포맷 변경이 필요한 경우 호환성 분석과 사용자 승인 기록이 존재한다.
7. DXF 열기·Import·저장, Section 편집, Undo/Redo와 3D Preview 회귀 테스트가 통과한다.
8. View 메뉴의 H/W 항목과 각 Section Designer 컨트롤이 양방향으로 동기화된다.
9. 프로그램 재시작 후 마지막 H/W Dimensions, Bent, Rotation 상태가 복원된다.
10. 파일별 상태가 있는 문서를 열면 전역 기본 상태보다 문서 상태가 우선한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
