# Repository Instructions

## Project Identity

- The official project-management name is **VCutting 개발**.
- Keep the existing repository name, namespaces, solution/project names, executable names, and product names unless an approved Change Request explicitly changes them.

## Project Management

This repository uses Git-based project management documentation.

Before modifying functional behavior, read in this order:

1. `AGENTS.md`
2. `docs/project/DEVELOPMENT_POLICY.md`
3. `docs/project/PROJECT_STATUS.md`
4. relevant files under `docs/project/requirements/`
5. relevant files under `docs/project/change-requests/`
6. relevant ADRs under `docs/project/decisions/`
7. the affected source code

Functional changes after the 2026-10-01 project baseline should normally be tracked through a Change Request.

Users may submit changes in natural language and do not need to know CR numbers or templates. For each functional request, apply the Change Request Intake procedure in `docs/project/DEVELOPMENT_POLICY.md`: check existing CRs, update or split as appropriate, assign the next unused number automatically, preserve source context, and update the CR index and project status before or alongside implementation.

After implementation:

1. Run proportionate tests.
2. Update the related Change Request.
3. Update `docs/project/PROJECT_STATUS.md`.
4. Update requirements or ADRs when the approved design changed them.
5. Decide whether `docs/project/releases/CHANGELOG.md` needs an entry.
6. Inspect Git changes and keep documentation consistent with the source.

Do not create an ADR for routine small changes. Do not change application behavior solely to satisfy project-management documentation.

## CR Regression Test Suite
Follow docs/project/REGRESSION_POLICY.md for CR impact analysis, immutable TC IDs, accumulated regression cases, new/affected/full execution, merge gates and completion reporting. Never delete or change existing test expectations solely to accommodate an implementation.
Every test case must specify Execution Type (AUTO / MANUAL / SEMI_AUTO / NOT_AUTOMATED). Report automated and human-reviewed results separately. Never invent human PASS; keep required human verification pending and do not close the CR until it is recorded.

## User Documentation Management
Every CR must evaluate User Visible Change and Manual Impact as Yes/No, including a reason for No. HTML under help-content/help is the official user manual source. When Manual Impact = Yes, identify and update affected HTML pages, workflows, troubleshooting and screenshots as applicable, then link sections and tests in the CR and manual-traceability.md. Current implementation takes precedence over older CR descriptions. Keep internal implementation details and gaps out of user navigation; record unverified information in DOCUMENTATION_GAPS.md. Internal changes without user impact should not unnecessarily change the manual.

CR Definition of Done = Implementation Complete + Relevant Automated Tests Passed + Required Manual/Human Tests Completed + Regression Tests Passed + User Documentation Updated (when Manual Impact = Yes). A CR with required human verification or documentation outstanding is not fully complete and must not be Verified/Closed.

## Release Version Authority — CR-070
Internal development version and customer release version are independent. Read release/Version.props and docs/project/releases/VERSIONING.md before development/Freeze/installer/release work. Customer versions are issued only by explicitly user-authorized Freeze (first target 1.0.0-rc.1) or separately authorized production promotion. Never allocate a Freeze merely because a build was requested. This policy implementation does not authorize a Freeze or release.

Installer builds consume immutable release/freezes records and must block input/commit/build-setting/version-property mismatches before modifying outputs. Rebuilding the same Freeze keeps customer version and records a new build ID/checksum. Any executable-source/Help/installer/dependency change requires a new authorized Freeze even when no new CR exists. Do not rewrite old records, distributed packages or overwrite prior output roots. Ordinary builds display Development; About/installer/Help derive version from the same authority.

Freeze source must be clean and committed. Record approval/source SHA/inputs hashes/included CRs/settings/verification; append build checksums under release/builds. Ledger-only commits are allowed after the source anchor; other changed source blocks builds. Production promotion requires complete actual automated and human verification plus separate user approval; never invent reviewer/evidence. Stable Windows mapping and future release increments follow ADR-001. Preserve historical 1.20.29-rc1 records/artifacts and user data/AppId.

## Package and Application Identity — User correction 2026-10-06
VCutting은 여러 프로그램을 포함하는 패키지 이름이다. 현재 이 공식 사용자 도움말의 대상 실행 프로그램은 CosmicDesigner이며, 실행 프로그램 이름을 패키지 이름과 혼동하지 않는다. CosmicExplorer는 향후 패키지에 추가할 계획이며 현재 구현/배포된 프로그램으로 안내하지 않는다. 사용자 2026-10-06 정정이 기존 CR-069의 주 프로그램명 VCutting 해석보다 우선한다. 기존 기술 경로/실행 산출물 이름이 아직 다르면 내부 문서에 불일치를 기록하고 별도 구현 변경에서 처리한다.

## Display Localization — CR-072
CosmicDesigner user-visible strings use external Languages/en.json and ko.json, stable keys and validated composite formats. User overrides: LocalApplicationData/CosmicDesigner/Languages. Do not translate IDs, DXF symbols, paths or numeric input. Never change numeric culture for display language. New user defaults mm; valid legacy settings and stored documents retain units/physical sizes.

## Identity reminder — 2026-10-07
VCutting은 프로젝트/패키지 이름이며 현재 개발 프로그램은 CosmicDesigner이다. CR-074는 타원 Hole 추가이며 실행 파일/네임스페이스 이름 변경은 포함하지 않는다.

## Executable naming approval — User instruction 2026-10-07
사용자가 다음부터 주 프로그램 실행 파일을 VCutting.exe 대신 CosmicDesigner.exe로 만들도록 명시적으로 승인했다. 이후 주 프로그램 빌드·게시·설치 작업에는 CosmicDesigner.exe 이름을 적용하고 관련 참조를 일치시킨다. 실제 구현 변경은 CR Intake를 통해 추적한다. VCutting 프로젝트/패키지 명칭은 유지하며, 이 지침 기록만으로 기존 산출물이 변경되었다고 주장하지 않는다.

## Build identification in completion reports — User instruction 2026-10-07
실행 파일/빌드 결과를 안내할 때 항상 해당 산출물의 Help > About에 표시되는 Build identifier를 실행 파일 경로와 함께 보고한다. 추측하거나 과거 빌드 값을 복사하지 말고 해당 빌드의 AssemblyMetadata BuildIdentity를 확인한다. 동시에 실행 중인 개발 빌드를 구별할 수 있도록 개발 산출물에는 기존 VCuttingBuildIdentity override로 작업/시각 등을 포함한 구별 가능한 식별자를 부여하고 실제 값과 경로를 기록한다. 이는 고객 버전/RC/Freeze 발급 승인이 아니며 승인된 Freeze 빌드의 기존 식별 규칙은 유지한다.
