# CR-005 Test Cases — 모든 Hole 객체의 선택, 이동과 Resize 직접 조작

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-005-001
- Related CR: CR-005
- Status: ACTIVE
- 목적: 모든 Hole 객체의 선택, 이동과 Resize 직접 조작 — HoleOperations 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-005-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::HoleOperations
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-005-002
- Related CR: CR-005
- Status: ACTIVE
- 목적: 모든 Hole 객체의 선택, 이동과 Resize 직접 조작 — SelectionHighlight 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-005-002 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::SelectionHighlight
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-005-003
- Related CR: CR-005
- Status: ACTIVE
- 목적: 모든 Hole 객체의 선택, 이동과 Resize 직접 조작 — DeleteObjects 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-005-003 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::DeleteObjects
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-005-004
- Related CR: CR-005
- Status: ACTIVE
- 목적: 모든 Hole 객체의 선택, 이동과 Resize 직접 조작 — UndoRedo 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-005-004 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::UndoRedo
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-005-005
- Related CR: CR-005
- Status: ACTIVE
- 목적: 모든 Hole 객체의 선택, 이동과 Resize 직접 조작 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-005/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. Hole 모드의 Status Bar 좌표와 생성 중심 좌표가 Pan/Zoom 상태에서도 일치한다.
2. 생성 즉시 세 UI 영역의 선택 상태가 일치하고 작업이 Undo/Redo된다.
3. 선택 객체의 모서리 제어점과 적절한 대각 Resize 커서가 표시된다.
4. 모서리 드래그가 폭과 높이를 함께 변경한다.
5. 가로/세로 경계 드래그가 각각 높이/폭만 변경한다.
6. 객체 내부 드래그가 형상을 변형하지 않고 위치만 변경한다.
7. 지원하는 모든 Hole 형상에서 Move와 Resize 후 고유 형상 및 폐곡선이 유지된다.
8. 한 번의 드래그가 하나의 Undo/Redo 항목으로 기록된다.
9. Property, Object Tree, L Geometry 및 저장 데이터가 결과와 동기화된다.
10. 저장 후 재열기에서 위치와 크기가 보존된다.
11. 생성, Hit Test, Move와 Resize 좌표 변환 자동 테스트가 통과한다.
12. 기존 DXF, Delete, Section Designer와 3D Preview 회귀 테스트가 통과한다.
13. 선택 모드 토글 후 선택 객체에 Move/Resize 커서와 동작이 복원된다.
14. Flat Designer에 포커스된 선택 객체가 `Delete`로 삭제되고 Undo할 수 있다.
15. Flat Designer와 OBJECT 트리의 선택 강조가 항상 동일 객체를 가리킨다.
16. Flat Designer의 포인터/포커스 이탈 시 Hole 모드와 crosshair가 해제된다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
