# CR-014 Test Cases — Section Designer 중앙 고정 Zoom

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-014-001
- Related CR: CR-014
- Status: ACTIVE
- 목적: Section Designer 중앙 고정 Zoom — SectionViewportCoordinates 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-014-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::SectionViewportCoordinates
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-014-002
- Related CR: CR-014
- Status: ACTIVE
- 목적: Section Designer 중앙 고정 Zoom 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-014/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. Section Zoom 후 Pan X/Y가 `0`이다.
2. 서로 다른 포인터 위치에서 반복 Zoom해도 도형 중심이 뷰 중심에 유지된다.
3. H/W, Flat/Bent 및 회전 상태에서 동일하게 동작한다.
4. Zoom 범위가 `0.4`–`5.0`을 벗어나지 않는다.
5. 전체 자동 회귀 테스트가 통과한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
