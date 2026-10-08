# CR-022 Test Cases — Edit Settings 사용자 환경 설정창

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-022-001
- Related CR: CR-022
- Status: ACTIVE
- 목적: Edit Settings 사용자 환경 설정창 — SettingsPersistence 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-022-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::SettingsPersistence
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-022-002
- Related CR: CR-022
- Status: ACTIVE
- 목적: Edit Settings 사용자 환경 설정창 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-022/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. Edit > Settings와 Ctrl+,로 창이 열린다.
2. 설정값은 재시작 후 유지된다.
3. Zoom, 선택 허용 거리, 핸들 크기와 3D 표현 설정이 현재 화면에 적용된다.
4. New는 설정된 기본 크기, 두께와 단위로 문서를 만든다.
5. Cancel은 변경을 저장하지 않는다.
6. Restore Defaults가 기본값을 입력한다.
7. 전체 자동 회귀 테스트가 통과한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
