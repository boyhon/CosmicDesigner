# Repository Instructions

## Project Identity

- The official project-management name is **Cosmic Designer 개발**.
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
