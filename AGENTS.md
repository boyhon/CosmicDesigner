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
