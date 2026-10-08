# CR-034 Test Cases — 다중 모서리 연속 Fillet 교정

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-034-001
- Related CR: CR-034
- Status: ACTIVE
- 목적: 다중 모서리 연속 Fillet 교정 — OuterContourFillet 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-034-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::OuterContourFillet
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-034-002
- Related CR: CR-034
- Status: ACTIVE
- 목적: 다중 모서리 연속 Fillet 교정 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-034/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

- 기본 사각형의 네 모서리에 R10 Fillet을 연속 적용할 수 있다.
- 결과는 네 LINE과 네 ARC로 이루어진 하나의 폐곡선이다.
- 각 ARC의 반지름이 10으로 유지된다.
- DXF 저장·재열기 후 네 ARC가 모두 보존된다.
- 3D 현 근사와 기존 회귀 검증이 통과한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
