# CR-011 Test Cases — Section Designer CAD 외곽 치수 기준 및 판재 두께 표시

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-011-001
- Related CR: CR-011
- Status: ACTIVE
- 목적: Section Designer CAD 외곽 치수 기준 및 판재 두께 표시 — ExteriorDimensions 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-011-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: CosmicDesigner.Verification/Program.cs::ExteriorDimensions
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-011-002
- Related CR: CR-011
- Status: ACTIVE
- 목적: Section Designer CAD 외곽 치수 기준 및 판재 두께 표시 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-011/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. `CD_3.dxf` H Bent 형상의 구간 치수 값이 `20 / 10 / 100 / 10 / 20 cm`이다.
2. 모든 치수 연장선은 화면에 보이는 진한 재료 표면선의 외곽 모서리에서 시작한다.
3. 중심축이나 표면선과 분리된 이론 좌표에서 시작하는 치수선이 없다.
4. `20` 치수의 절곡부 기준선과 인접한 `100` 치수의 기준선은 동일 선이 아니며 화면상의 재료 두께만큼 어긋난다.
5. 상하 `10` 치수도 인접 외측 표면까지 재료 두께를 포함한다.
6. 끝단 치수 `20`은 외측 절곡 모서리부터 재료 끝단까지 표시한다.
7. W Section Designer의 Flat 및 Bent 보기에서 재료 두께 `0.2`가 표시된다.
8. 기존 치수 편집, 휠 Zoom, 회전 및 쐐기 생성 범위에 회귀가 없다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.
