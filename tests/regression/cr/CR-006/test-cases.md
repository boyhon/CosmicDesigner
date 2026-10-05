# CR-006 Test Cases — Section Designer 치수 편집기의 투명 표시

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-006-001
- Related CR: CR-006
- Status: ACTIVE
- 목적: Section Designer 치수 편집기의 투명 표시 — ExteriorDimensions 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-006-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::ExteriorDimensions
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-006-002
- Related CR: CR-006
- Status: ACTIVE
- 목적: Section Designer 치수 편집기의 투명 표시 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-006/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. 비포커스 TextBox의 배경과 외곽선이 도형과 치수선을 가리지 않는다.
2. Hover/Focus 시 편집 가능 상태를 식별할 수 있다.
3. Flat Mode와 Bent Mode에서 값 입력과 적용이 정상 동작한다.
4. 잘못된 값 복원, Enter 처리와 Undo/Redo 동작이 유지된다.
5. Section 형상, 거리 배치와 기존 기능 회귀 테스트가 통과한다.
6. 각 구간 길이는 동일 위치에 한 번만 표시되며 옅은 회색의 중복 값이 남지 않는다.
7. Bent Mode에서 중심축 절곡점의 빨강/파랑 점이 표시되지 않는다.
8. Bent Mode 치수 연장선은 중심축이 아니라 실제 두께 다각형의 외측 모서리에서 시작한다.
9. 선택된 절곡은 중심점이 아니라 재료 두께를 가로지르는 외곽 간 경계로 식별된다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
