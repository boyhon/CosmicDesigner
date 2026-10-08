# CR-045 Test Cases — Fillet ARC 외곽의 천공 통합

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-045-001
- Related CR: CR-045
- Status: ACTIVE
- 목적: Fillet ARC 외곽의 천공 통합 — CurvedBoundaryMerge 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-045-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::CurvedBoundaryMerge
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-045-002
- Related CR: CR-045
- Status: ACTIVE
- 목적: Fillet ARC 외곽의 천공 통합 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-045/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

- Fillet 네 코너 적용 후 세 도형을 순차 통합할 수 있다.
- 관련 없는 ARC 형상/반지름이 그대로 유지된다.
- ARC 직접 교차에서도 남은 ARC의 원/반지름과 정확한 교점이 유지된다.
- 내부/분리 Cut 거부, 폐합, Undo/Redo 및 DXF ARC 왕복 검증 통과.
- 전체 자동 검증 및 Release 빌드 통과.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
