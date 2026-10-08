# CR-026 Test Cases — Flat Designer 절삭 영역 흰색 표시

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-026-001
- Related CR: CR-026
- Status: ACTIVE
- 목적: Flat Designer 절삭 영역 흰색 표시 — FlatMaterialMask 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-026-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::FlatMaterialMask
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-026-002
- Related CR: CR-026
- Status: ACTIVE
- 목적: Flat Designer 절삭 영역 흰색 표시 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-026/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

- 원형과 다각형 Cut 내부가 흰색으로 표시된다.
- Outer Contour 병합으로 제거된 노치가 흰색으로 표시된다.
- 절삭 영역에 Grid 및 절곡선이 표시되지 않는다.
- 재료 윤곽선과 선택 표시는 기존처럼 보인다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
