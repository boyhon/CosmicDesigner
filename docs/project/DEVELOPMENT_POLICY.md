# Cosmic Designer 개발 — Development Policy

## 1. Purpose and scope

This document defines change management after the **2026-10-01 baseline**. The official management name is `Cosmic Designer 개발`. Existing technical and product names remain unchanged until an approved Change Request authorizes a rename.

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
