# ADR-001 — VCutting 릴리스 버전과 불변 Freeze

## Status
Accepted — CR-070, 2026-10-06 user-approved policy.

## Context
개발 단위 버전 1.20.29와 고객 버전이 혼재한다. 설치 반복 횟수나 CR 생성 여부로 고객 번호를 부여하면 같은 버전의 내용이 달라질 수 있다. 기존 숫자 버전 1.20.29.0 업그레이드 순서도 유지해야 한다.

## Decision
release/Version.props를 단일 버전 정책 원본으로 둔다. DevelopmentVersion과 다음 TargetCustomerVersion은 독립적이다. 새 고객 버전은 명시적 승인 증거와 clean committed source에서 versioning.py freeze가 발급한다. 첫 목표 1.0.0, 아직 발급 없음. MAJOR.MINOR.PATCH-rc.N, 별도 승인 promote로 정식 전환. RC 숫자 증가는 Freeze 동작에만 있다.

등록된 release/freezes/<version>.json은 exclusive-create로 작성하며 재작성하지 않는다. 소스 커밋/입력별 SHA256/개발 버전/전체 CR 목록/빌드 설정/승인/검증 상태를 저장한다. 각 빌드 결과의 체크섬과 시각은 release/builds/<version>/<build-number>.json에 append-only로 연결한다. 기록을 Git에 커밋할 수 있도록 원장 경로만 Freeze 입력에서 제외한다. 원장 전용 후속 커밋은 허용하되 다른 소스 변경과 현재 입력 불일치는 차단한다. 빌드는 정확한 원본 소스 커밋을 표시한다.

Windows 숫자형 매핑: (customer major + epoch 1).minor.patch.revision. RC revision=1..65534, 정식 revision=65535. 고객 1.0.0-rc.1 → 2.0.0.1, rc.10 → 2.0.0.10, 1.0.0 → 2.0.0.65535, 1.0.1-rc.1 → 2.0.1.1. 기존 1.20.29.0보다 첫 후보가 크다. 범위 초과 시 자동 wrap 없이 오류. epoch는 출시 후 변경하지 않는다.

승인 Freeze 없이 일반 빌드는 Development 및 내부 개발 정보로 표시한다. 설치는 FreezeRecord/BuildNumber 필수이며 변경 입력, 변경 버전 속성, 다른 build settings를 차단한다. 앱/설치 문자열/숫자 메타데이터 및 실행 시 Help 표시를 같은 기록에서 파생한다.

정식 전환에는 정확한 후보/커밋, 누적 ACTIVE AUTO 및 MANUAL/SEMI_AUTO 전체 PASS와 사람 확인자/시각/관측 근거, 별도 출시 승인 증거가 필요하다. NOT_AUTOMATED ACTIVE는 미해결 상태이므로 차단한다. 사람이 수행한 결과를 Codex가 만들지 않는다.

## Alternatives Considered
개발 버전 고객 노출, 빌드마다 RC 증가, 설치 파일 내 수동 버전 각각 배제: 개발/출시 의미 혼동, 재현성 및 일관성 부족. 고객 1.0.0을 숫자 1.0.0.1로 직접 매핑하면 기존 설치 숫자보다 감소하므로 epoch 적용.

## Consequences / Compatibility and Migration
이전 Freeze와 산출물은 보존한다. 새 정책 구현은 새 Freeze/출시를 의미하지 않는다. 이전 Freeze 재빌드는 그 소스 checkout/도구로 수행한다. 개발 빌드는 고객 설치 패키지를 생성하지 않는다. 사용자 설정, DXF, AppId, 제품명 변화 없음. 원장 해시는 변조/실수 탐지이며 외부 서명이나 보안 권한 시스템을 대체하지 않는다.

## Related Change Request
[CR-070](../change-requests/CR-070.md)

Toolchain: Freeze records exact selected .NET SDK and policy-pinned Inno Setup engine; validate checks SDK, installer probes engine before compilation. Framework/runtime selection follows the frozen SDK; source dependency declarations and feed configuration are hashed. Future package dependencies must use exact versions/committed locks.
