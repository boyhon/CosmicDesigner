# CR-008 Test Cases — W/H Section Designer 마우스 휠 확대·축소

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-008-001
- Related CR: CR-008
- Status: ACTIVE
- 목적: W/H Section Designer 마우스 휠 확대·축소 — SectionViewportCoordinates 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-008-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::SectionViewportCoordinates
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-008-002
- Related CR: CR-008
- Status: ACTIVE
- 목적: W/H Section Designer 마우스 휠 확대·축소 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-008/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. H/W 영역의 휠 위·아래 입력으로 확대와 축소가 동작한다.
2. 포인터 아래 단면 좌표가 확대 전후 허용 오차 내에서 유지된다.
3. 최소·최대 배율을 벗어나지 않는다.
4. Flat/Bent 및 0/90/180/270도 회전에서 형상과 치수 표시가 일치한다.
5. 치수 입력, 쐐기 선택과 기존 Flat Designer Zoom 동작에 회귀가 없다.
6. View 메뉴에 Section Zoom 항목이 생기지 않는다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
