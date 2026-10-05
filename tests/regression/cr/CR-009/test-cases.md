# CR-009 Test Cases — Section Designer 쐐기 생성 클릭 범위 제한

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-009-001
- Related CR: CR-009
- Status: ACTIVE
- 목적: Section Designer 쐐기 생성 클릭 범위 제한 — SectionViewportCoordinates 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-009-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::SectionViewportCoordinates
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-009-002
- Related CR: CR-009
- Status: ACTIVE
- 목적: Section Designer 쐐기 생성 클릭 범위 제한 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-009/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. W/H Flat Mode의 재료 도형 내부 클릭은 기존과 같이 V/V1 객체를 생성한다.
2. 재료 도형 외부, 여백 및 패널 배경 클릭은 객체를 생성하지 않는다.
3. 길이 편집 필드를 클릭하거나 값을 편집해도 새 객체가 생성되지 않는다.
4. Bent Mode 클릭은 객체를 생성하지 않는다.
5. 무효 클릭은 Undo/Redo 기록을 추가하지 않는다.
6. 유효 클릭 위치와 생성된 Position 값이 허용 오차 내에서 일치한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
