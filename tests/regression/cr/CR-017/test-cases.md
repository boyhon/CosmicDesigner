# CR-017 Test Cases — Flat Designer Outer Contour LINE 직접 편집

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-017-001
- Related CR: CR-017
- Status: ACTIVE
- 목적: Flat Designer Outer Contour LINE 직접 편집 — OuterContourEditing 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-017-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::OuterContourEditing
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-017-002
- Related CR: CR-017
- Status: ACTIVE
- 목적: Flat Designer Outer Contour LINE 직접 편집 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-017/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. Flat Designer에서 Outer Contour LINE 클릭 선택이 된다.
2. 수직 선택선 몸체의 커서는 `ew-resize`이고 좌우 드래그가 된다.
3. 수직 선택선 끝점의 커서는 `ns-resize`이고 상하 Resize가 된다.
4. 수평선에는 회전 대칭인 커서와 조작이 적용된다.
5. 몸체 이동 시 양 끝에 연결된 두 인접선이 함께 늘거나 줄어든다.
6. 끝점 이동 시 해당 끝에 연결된 인접선이 함께 변경된다.
7. Outer Contour는 연결 상태와 LINE 개수를 유지한다.
8. 편집 전 상태는 Undo로 복원할 수 있다.
9. 전체 자동 회귀 테스트가 통과한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
