# CR-004 Test Cases — Flat Designer 기본 포인터, Pan, Zoom, 눈금자와 격자

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-004-001
- Related CR: CR-004
- Status: ACTIVE
- 목적: Flat Designer 기본 포인터, Pan, Zoom, 눈금자와 격자 — ViewportCoordinates 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-004-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::ViewportCoordinates
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-004-002
- Related CR: CR-004
- Status: ACTIVE
- 목적: Flat Designer 기본 포인터, Pan, Zoom, 눈금자와 격자 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-004/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. 기본 상태에서 기본 포인터가 표시되고 빈 공간 좌클릭·드래그로 데이터가 변경되지 않는다.
2. 중복 크기 텍스트가 제거되며 Material 속성은 유지된다.
3. 오른쪽 버튼 Pan은 설계 좌표를 변경하지 않고 모든 화면 요소를 함께 이동한다.
4. Pan 종료 시 캡처와 포인터가 정상 복원된다.
5. 휠 및 메뉴 Zoom에 유효한 최소·최대 배율이 적용된다.
6. Fit to Window가 전체 재료와 설계 객체를 화면 안에 배치한다.
7. Ruler/Grid 버튼과 View 메뉴 체크가 양방향 동기화된다.
8. 모든 배율과 Pan 상태에서 눈금, 객체, 생성 좌표와 Hit Test가 허용 오차 내에서 일치한다.
9. 기존 선택, Hole 생성, Delete, Property 편집과 Section/3D 기능의 회귀 테스트가 통과한다.
10. 최대 확대 상태와 Pan 경계에서도 도면 및 선택 표시가 눈금자 영역을 침범하지 않는다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
