# CR-010 Test Cases — 3D Preview 투명 아크릴 표현

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-010-001
- Related CR: CR-010
- Status: ACTIVE
- 목적: 3D Preview 투명 아크릴 표현 — BentSurface 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-010-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::BentSurface
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-010-002
- Related CR: CR-010
- Status: ACTIVE
- 목적: 3D Preview 투명 아크릴 표현 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-010/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

1. 앞쪽 면을 통해 뒤쪽 면과 절곡 구조를 식별할 수 있다.
2. 외곽과 주요 절곡 모서리가 회색 윤곽선으로 선명하게 표시된다.
3. 마우스 드래그 회전 중 투명 면이 심하게 깜박이거나 사라지지 않는다.
4. Fit/Reset과 기존 회전 조작이 유지된다.
5. 배경과 도형 간 대비가 충분하고 전체 공간 형상을 기존보다 쉽게 판별할 수 있다.
6. Flat/Section Designer 및 저장 데이터에는 영향을 주지 않는다.
7. 3D Preview 배경이 흰색이며 방향성 조명에 따른 명암이나 그늘이 나타나지 않는다.
8. 외곽 및 절곡 모서리가 검정 실선으로 표시된다.
9. 기본 투명 상태에서도 연회색 면이 흰 배경과 육안으로 명확히 구별된다.
10. 재료 크기와 관계없이 검정 Edge에 가시적인 최소 화면 굵기가 확보된다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.


## 요구 변경 추적
CR-029 이후 V/V1 절곡 Edge 색상 기대는 CR-029 기준을 적용한다. 기존 전 Edge 검정 요구 부분은 SUPERSEDED.
기존 TC ID는 유지하며 나머지 검증 목적은 ACTIVE. 위 폐기된 기대값으로 새 구현을 실패 판정하지 않는다.
