# CR-025 Test Cases — Ruler 및 Grid 간격 설정

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-025-001
- Related CR: CR-025
- Status: ACTIVE
- 목적: Ruler 및 Grid 간격 설정 — ViewportCoordinates 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-025-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::ViewportCoordinates
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-025-002
- Related CR: CR-025
- Status: ACTIVE
- 목적: Ruler 및 Grid 간격 설정 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-025/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

- 세 가지 간격을 독립적으로 저장하고 재시작 후 복원한다.
- Auto에서는 기존 자동 간격을 사용한다.
- 단위 변경 전후의 사용자 지정 물리 간격이 동일하다.
- 과도하게 축소해도 조밀한 선이나 중첩 숫자를 무제한 렌더링하지 않는다.
- Settings 적용 직후 Flat Designer가 갱신되고 문서 제목에 `*`가 추가되지 않는다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
