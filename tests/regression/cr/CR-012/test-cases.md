# CR-012 Test Cases — Flat Designer H/W 단면 선택선 및 연동

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-012-001
- Related CR: CR-012
- Status: ACTIVE
- 목적: Flat Designer H/W 단면 선택선 및 연동 — SectionSelection 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-012-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::SectionSelection
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-012-002
- Related CR: CR-012
- Status: ACTIVE
- 목적: Flat Designer H/W 단면 선택선 및 연동 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-012/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. Flat Designer에 H 세로 파선과 W 가로 파선이 표시된다.
2. H 선을 잡으면 좌우 크기 조절 커서가 보이고 좌우 드래그가 된다.
3. W 선을 잡으면 상하 크기 조절 커서가 보이고 상하 드래그가 된다.
4. 선은 재료 경계 안으로 제한된다.
5. H/W Section Designer가 대응 위치를 즉시 표시하고 다시 그린다.
6. 기존 Flat Designer 직접 조작과 Zoom/Pan 회귀가 없다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
