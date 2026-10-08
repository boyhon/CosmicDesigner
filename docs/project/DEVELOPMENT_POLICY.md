# VCutting 개발 — Development Policy

## 1. Purpose and scope

This document defines change management after the **2026-10-01 baseline**. The official management name is `VCutting 개발`. Existing technical and product names remain unchanged until an approved Change Request authorizes a rename.

## 2. Required reading order

Before changing functional behavior, review:

1. repository `AGENTS.md`
2. this policy
3. `PROJECT_STATUS.md`
4. relevant current requirements
5. relevant Change Requests
6. relevant ADRs
7. affected source and tests

## 3. Change Request policy

After the baseline, the following work is normally managed through a Change Request (CR):

- new functionality;
- changes to existing behavior;
- UI/UX changes;
- calculation-policy changes;
- data-model changes;
- file-format changes;
- bug fixes;
- architectural or design changes;
- compatibility-affecting changes.

Pure spelling fixes, clearly non-functional comment changes, and equivalently trivial documentation corrections may omit a CR. If there is doubt about behavioral impact, create a CR.

Introducing this management structure is not itself a functional product change and does not consume a CR number.

## 4. Change Request Intake

The user is not required to provide a Change Request number or use the formal Change Request template.

When the user requests a functional change:

1. Interpret the user's natural-language request.
2. Read `docs/project/change-requests/` and check existing Change Requests for duplication, dependency, or overlap.
3. Determine whether the request should create a new CR, update an existing CR, or be split into multiple logically independent CRs.
4. If a new CR is required, determine the next number from both the CR files and the recorded next number, then assign the next unused sequential CR number.
5. Convert the request into the standard Change Request format, including Background, Current Behavior, Required Behavior, Scope, Implementation Notes, and testable Acceptance Criteria.
6. Preserve the user's original intent, terminology, constraints, and source context. Do not silently narrow or broaden the requested outcome.
7. Ask for clarification only when implementation cannot safely proceed without it. Unknown non-blocking details may be recorded as `TBD` or an explicit implementation decision.
8. Preserve an original source document and reference its repository-relative path and relevant section from every derived CR. Never delete or overwrite a source document merely because CRs were created from it.
9. If the request exists only in conversation, identify the source as a dated user request and retain a concise faithful quotation or summary in the CR.
10. Update the CR index and `PROJECT_STATUS.md`, including Open/In Progress lists and the next available CR number.

Intake status rules:

- Use `Open` when the user is registering or refining a requirement without asking for immediate implementation.
- Use `In Progress` when the user explicitly requests implementation and work begins in the same task.
- Updating an overlapping CR must preserve its history; do not replace its original request without recording the added source and scope change.
- Splitting one source into multiple CRs requires a source-to-CR mapping so no requirement is lost or implemented twice.

## 5. CR numbering and files

- Use monotonically increasing IDs: `CR-001`, `CR-002`, and so on.
- Store each CR as `docs/project/change-requests/CR-NNN.md`.
- Never reuse an issued number, including rejected or abandoned requests.
- The next available number is recorded in `PROJECT_STATUS.md` and the change-request index.

## 6. CR lifecycle

Allowed states:

```text
Open → In Progress → Implemented → Verified → Closed
  └────────────────────────────────────────→ Rejected
```

- `Open`: recorded but not being implemented.
- `In Progress`: implementation has started.
- `Implemented`: code and documentation changes are complete, but final verification is pending.
- `Verified`: acceptance criteria have passed.
- `Closed`: all records, release linkage, and commits are complete.
- Required MANUAL or human-reviewed SEMI_AUTO tests still pending: retain Implemented and record `Verification Status: WAITING FOR USER VERIFICATION`. Automated PASS alone cannot promote a CR to Verified/Closed; record actual human verification evidence first.
- `Rejected`: the request will not be implemented; the reason must remain documented.

## 7. Required CR structure

Every CR must contain at least:

```markdown
# CR-xxx Change title

## Status
Open | In Progress | Implemented | Verified | Closed | Rejected

## Request

## Background

## Current Behavior

## Required Behavior

## Scope

## Implementation Notes

## Acceptance Criteria

## Modified Files

## Test Result

## Related Commit

## Release
```

Acceptance criteria must be observable or testable. Unknown values must be marked `TBD`; they must not be invented.

## 8. Git linkage

- Prefer one logical CR per commit.
- Include the CR ID in the commit subject, for example: `CR-017: Recalculate properties after material thickness change`.
- If a CR needs multiple commits, record all commit hashes in the CR.
- Do not claim a commit exists until Git confirms it.
- Review the working tree before and after implementation, preserving unrelated user changes.

## 9. Requirements management

- `requirements/current-requirements.md` contains only requirements confirmed by current source, tests, or approved product documentation.
- Proposed changes are not current requirements until approved and implemented.
- When an approved CR changes an existing requirement, update the current-requirements document after implementation.
- Do not infer historical intent that cannot be supported by repository evidence.

## 10. Architecture Decision Records

Consider an ADR for long-lived decisions involving:

- the core data model;
- calculation algorithms;
- file-format policy;
- major UI structure;
- framework or library selection;
- compatibility policy.

Use `ADR-NNN-short-title.md`. Do not create ADRs for routine small fixes. ADR identifiers are never reused.

## 11. File-format policy

DXF remains the current interchange format. If a required feature cannot be represented safely in DXF, propose the `.cdf` **Cosmic Design Format** with its compatibility and migration impact. Implementation or adoption of CDF requires explicit user approval and an approved CR; see the proposed policy in `../COSMIC_DESIGNER_CHANGE_REQUIREMENTS.md`.

## 12. Completion procedure

After implementing a CR:

1. run relevant automated and manual tests;
2. record actual modified files and test results in the CR;
3. update `PROJECT_STATUS.md`;
4. update current requirements or ADRs if necessary;
5. add a changelog entry when the change is release-relevant;
6. inspect Git changes;
7. keep source, CR status, requirements, status, ADRs, and changelog mutually consistent.

## 13. CR-based Regression Test Suite
The user-approved [Regression Policy](REGRESSION_POLICY.md) is mandatory. CRs include Impact Analysis, Test Cases, Affected Regression Tests and Regression Result; execute new, affected and full automated cases in that order. Stable TC IDs accumulate in tests/regression; supersession requires a recorded CR and reason. Merge readiness is separate from the existing lifecycle.

## 14. User Documentation Management
Every CR includes Documentation Impact: User Visible Change (Yes/No), Manual Impact (Yes/No with reason), Affected Feature, Affected Manual Sections, Related Tests/Regression Tests and Manual Status. Required CR structure includes this section; evaluate at intake and revisit when scope changes. The official source is HTML in help-content/help; Word/PDF are derivative outputs only.

For Manual Impact Yes, update relevant HTML, workflow, troubleshooting, screenshots when needed and manual-traceability.md before full completion. Use current source/tests over superseded historical descriptions; preserve original CR history. Unconfirmed material goes in DOCUMENTATION_GAPS.md. Internal non-user changes should not cause unnecessary user manual edits. See help-content/help/README.md for navigation and validation rules.

Definition of Done: Implementation Complete + Relevant Automated Tests Passed + Required Manual/Human Tests Completed + Regression Tests Passed + User Documentation Updated (when Manual Impact Yes). Keep Implemented / WAITING FOR USER VERIFICATION when required people review remains. No Verified/Closed while required documentation remains outstanding.

## Release Version Authority — CR-070
Internal development version and customer release version are independent. Read release/Version.props and docs/project/releases/VERSIONING.md before development/Freeze/installer/release work. Customer versions are issued only by explicitly user-authorized Freeze (first target 1.0.0-rc.1) or separately authorized production promotion. Never allocate a Freeze merely because a build was requested. This policy implementation does not authorize a Freeze or release.

Installer builds consume immutable release/freezes records and must block input/commit/build-setting/version-property mismatches before modifying outputs. Rebuilding the same Freeze keeps customer version and records a new build ID/checksum. Any executable-source/Help/installer/dependency change requires a new authorized Freeze even when no new CR exists. Do not rewrite old records, distributed packages or overwrite prior output roots. Ordinary builds display Development; About/installer/Help derive version from the same authority.

Freeze source must be clean and committed. Record approval/source SHA/inputs hashes/included CRs/settings/verification; append build checksums under release/builds. Ledger-only commits are allowed after the source anchor; other changed source blocks builds. Production promotion requires complete actual automated and human verification plus separate user approval; never invent reviewer/evidence. Stable Windows mapping and future release increments follow ADR-001. Preserve historical 1.20.29-rc1 records/artifacts and user data/AppId.

## Localization maintenance — CR-072
New CosmicDesigner user text must use stable external language keys and validated composite arguments. Preserve internal IDs/DXF schema and numeric culture. Add English/Korean strings together, review translation/layout manually, update localization inventory and regression cases. User overrides remain outside installer ownership. Unit defaults follow CR-073 compatibility rules.

## Build report identity — user2026-10-07
Whenever delivering an executable/build, include its exact Help > About Build identifier alongside absolute path. Read AssemblyMetadata BuildIdentity from that artifact; do not infer from current source or reuse earlier values. Use existing development override to distinguish concurrent working builds. Freeze/customer version authority unchanged. AGENTS persistent rule applies.

## CR-093 — pre-Freeze installation testing (user2026-10-08)
User explicitly requires an installable integration-test package before manual installation/use review and Freeze approval. This supersedes the prior all-installers-require-Freeze interpretation for Development testing only. Build-IntegrationInstaller.ps1 creates a Development installer with internal DevelopmentVersion numeric mapping and unique source/build identity, exact source inputs/checksums, same AppId/install/settings and a separate IntegrationTest filename/output. It never allocates a customer version or Freeze. Frozen customer/production paths retain all existing gates. Complete manual review and requested fixes first, then request explicit Freeze approval of the reviewed source. Existing RC1/RC2 records retained; pending RC3 approval request superseded, no RC3 issued.
