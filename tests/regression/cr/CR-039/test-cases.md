# CR-039 Test Cases — 3D 천공 면과 윤곽선 정합성 교정

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-039-001
- Related CR: CR-039
- Status: ACTIVE
- 목적: 3D 천공 면과 윤곽선 정합성 교정 — ExactCutSurface 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-039-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::ExactCutSurface
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-039-002
- Related CR: CR-039
- Status: ACTIVE
- 목적: 3D 천공 면과 윤곽선 정합성 교정 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-039/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

- 원·삼각형·사각형·마름모·평행사변형의 면 제거 영역과 윤곽선이 일치한다.
- 실제 면적이 외곽 면적에서 천공 근사 윤곽 면적을 뺀 값과 일치한다.
- 겹친 천공, 비직교 외곽 및 절곡 교차에서도 재료 면이 경계를 넘지 않는다.
- 기존 자동 검증 및 Release 빌드 통과.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
