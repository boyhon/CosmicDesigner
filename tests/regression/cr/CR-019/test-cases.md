# CR-019 Test Cases — 3D Preview Outer Contour 컷 형상 반영

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-019-001
- Related CR: CR-019
- Status: ACTIVE
- 목적: 3D Preview Outer Contour 컷 형상 반영 — BentSurface 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-019-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::BentSurface
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-019-002
- Related CR: CR-019
- Status: ACTIVE
- 목적: 3D Preview Outer Contour 컷 형상 반영 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-019/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. 기본 직사각형 재료는 기존과 같은 완전한 3D 면을 표시한다.
2. 모서리 notch가 통합되면 해당 모서리 face가 3D Preview에서 제거된다.
3. notch의 새 경계 LINE이 검은 외곽 Edge로 표시된다.
4. W/H 절곡선은 컷 형상과 함께 올바르게 절곡되고 fold Edge를 유지한다.
5. CR-017/018에 의한 Outer Contour 변경 후 Preview가 즉시 다시 생성된다.
6. 회전, Zoom, Fit/Reset과 Semi Transparent 옵션에 회귀가 없다.
7. 전체 자동 회귀 테스트가 통과한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
