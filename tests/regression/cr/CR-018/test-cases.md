# CR-018 Test Cases — 잘린 Section 치수 편집과 Outer Contour 연쇄 갱신

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-018-001
- Related CR: CR-018
- Status: ACTIVE
- 목적: 잘린 Section 치수 편집과 Outer Contour 연쇄 갱신 — ClippedSectionEditing 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-018-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::ClippedSectionEditing
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-018-002
- Related CR: CR-018
- Status: ACTIVE
- 목적: 잘린 Section 치수 편집과 Outer Contour 연쇄 갱신 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-018/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. 잘린 H Section의 Flat 및 Bent 치수 값을 편집할 수 있다.
2. 잘린 W Section에도 같은 기능이 적용된다.
3. Bent 외곽 치수 입력에 재료 두께 보정이 적용된다.
4. 편집 대상 이후의 bend 위치와 잘린 contour 경계가 변화량만큼 이동한다.
5. 변경되지 않아야 할 다른 로컬 segment 길이는 유지된다.
6. 이동한 contour 경계에 연결된 양쪽 LINE 끝점이 함께 갱신된다.
7. Outer Contour 연결성과 LINE 개수가 유지된다.
8. 편집을 Undo로 되돌릴 수 있다.
9. 전체 자동 회귀 테스트가 통과한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
