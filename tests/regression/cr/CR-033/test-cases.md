# CR-033 Test Cases — Outer Contour 직각 모서리 Fillet

2026-10-05 기존 검증 코드/수용 기준의 사후 매핑. Test ID는 발급 후 유지한다.

## TC-033-001
- Related CR: CR-033
- Status: ACTIVE
- 목적: Outer Contour 직각 모서리 Fillet — OuterContourFillet 기존 assertion
- 사전 조건: Windows, .NET 10, Release 검증 빌드
- 입력/절차: 검증 executable을 --tests TC-033-001 로 실행
- 기대 결과: 함수의 모든 assertion PASS, exit code 0
- Execution Type: AUTO
- 구현 위치: VCutting.Verification/Program.cs::OuterContourFillet
- 범위: 함수에 실제 구현된 검사만 포함; CR 전체/UI 승인과 구분

## TC-033-002
- Related CR: CR-033
- Status: ACTIVE
- 목적: Outer Contour 직각 모서리 Fillet 수용 기준/시각 동작 확인
- 사전 조건: CR 본문의 입력/시나리오와 현재 실행 파일, 사용자 문서는 사본 사용
- Execution Type: MANUAL
- 구현 위치: tests/regression/cr/CR-033/test-cases.md
- 입력/절차 및 기대 결과: 아래 기준을 순서대로 재현하고 항목마다 실제 결과/PASS/FAIL을 기록한다. UI 표시 항목은 실제 화면에서 확인하고, 지속성 항목은 저장/종료/재열기 후 확인한다.

- 기본 사각 재료의 네 볼록 모서리에 유효한 Fillet 미리보기가 나타난다.
- R10 적용 시 두 접점이 원래 꼭짓점에서 각각 10만큼 떨어지고 ARC 반지름이 10이다.
- 결과 Outer Contour는 연결된 LINE/ARC 폐곡선을 유지한다.
- 너무 큰 반지름과 오목/비직각 모서리는 거부한다.
- Undo로 적용 전 Outer Contour를 복원할 수 있다.
- 저장된 DXF와 CosmicDesigner 메타데이터에 ARC가 보존된다.
- 3D Preview가 비어 있거나 끊어지지 않고 현 근사 외곽을 표시한다.

- 실행 결과: NOT_RUN — 이관 시점에 새 수동 실행을 주장하지 않음.


## 요구 변경 추적
3D ARC 단일 chord 기대 부분은 CR-044에 의해 SUPERSEDED. 정확한 Flat/DXF ARC 기준은 ACTIVE이며 3D 곡선은 TC-044-001/TC-044-002 기준을 적용한다.
기존 TC ID는 유지하며 나머지 검증 목적은 ACTIVE. 위 폐기된 기대값으로 새 구현을 실패 판정하지 않는다.
