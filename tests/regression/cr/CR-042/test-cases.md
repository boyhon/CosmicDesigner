# CR-042 Test Cases — Outer Contour LINE 삭제 후 폐합 및 재료 영역 정합성

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-042-001
- Related CR: CR-042
- Status: ACTIVE
- 목적: Outer Contour LINE 삭제 후 폐합 및 재료 영역 정합성 — OuterLineDeletion 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-042-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::OuterLineDeletion
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-042-002
- Related CR: CR-042
- Status: ACTIVE
- 목적: Outer Contour LINE 삭제 후 폐합 및 재료 영역 정합성 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-042/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

- 경계 통합된 사각형 노치의 한 변 삭제 후 모든 인접 LINE 끝점이 연결된다.
- 새 경계 외부는 흰색이고 내부는 회색이며 실제 LINE과 채움 경계가 일치한다.
- 첫/마지막 LINE 삭제와 Undo/Redo 및 DXF 왕복이 통과한다.
- 최소 외곽 또는 무효 결과 삭제는 원본을 유지한다.
- 전체 자동 검증 및 Release 빌드 통과.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
