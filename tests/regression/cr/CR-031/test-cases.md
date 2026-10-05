# CR-031 Test Cases — 원·삼각형·사각형 Cut 드래그 생성

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-031-001
- Related CR: CR-031
- Status: ACTIVE
- 목적: 원·삼각형·사각형 Cut 드래그 생성 — HoleDragCreation 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-031-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::HoleDragCreation
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-031-002
- Related CR: CR-031
- Status: ACTIVE
- 목적: 원·삼각형·사각형 Cut 드래그 생성 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-031/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

- Circle, Triangle, Rectangle은 드래그 완료 전까지 문서에 추가되지 않는다.
- 드래그 중 실제 생성될 크기의 미리보기가 보인다.
- 생성된 Cut의 중심, 너비와 높이는 드래그 범위와 일치한다.
- Circle의 너비와 높이는 같다.
- 단순 클릭이나 너무 짧은 드래그로 Cut이 생성되지 않는다.
- 한 개 생성 후 Select 모드로 복귀한다.
- Undo는 생성 작업 전체를 한 번의 편집으로 처리한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
