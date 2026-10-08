# CR-046 Test Cases — Outer Contour 편집 후 틈 자동 LINE 연결

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-046-001
- Related CR: CR-046
- Status: ACTIVE
- 목적: Outer Contour 편집 후 틈 자동 LINE 연결 — OuterGapClosure 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-046-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::OuterGapClosure
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-046-002
- Related CR: CR-046
- Status: ACTIVE
- 목적: Outer Contour 편집 후 틈 자동 LINE 연결 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-046/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

- 양 끝이 Fillet ARC와 연결된 LINE 몸체 이동 시 두 틈에 LINE이 추가된다.
- 한 끝점 이동은 해당 틈만 연결하며 기존 ARC를 변경하지 않는다.
- ARC 삭제 및 첫/마지막 틈도 실제 LINE으로 폐합된다.
- 재계산과 반복 편집은 연결 LINE을 중복 생성하지 않는다.
- Undo/Redo, 저장/재열기 및 전체 자동 검증 통과.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
