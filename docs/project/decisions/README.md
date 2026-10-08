# Architecture Decision Record Index

Use ADRs only for significant, long-lived structural decisions. Routine small changes belong in their CR without a separate ADR.

## Next available number

`ADR-003`

## Index

| ADR | Decision | Status | Related CR |
|---|---|---|---|
| [ADR-001](ADR-001-release-version-authority.md) | Customer version authority / immutable Freeze / Windows mapping | Accepted | CR-070 |

## File naming

```text
ADR-001-short-title.md
```

## ADR template

```markdown
# ADR-NNN — Decision title

## Status
Proposed | Accepted | Superseded | Rejected

## Context

## Decision

## Alternatives Considered

## Consequences

## Compatibility and Migration

## Related Change Request
```

Do not infer or reconstruct undocumented historical decisions as ADRs.

| [ADR-002](ADR-002-external-localization.md) | External UTF-8 catalogs / user overrides / safe fallback | Accepted | CR-072 |
| [ADR-003](ADR-003-compound-cut-union.md) | Analytic compound Cut union / closed loops / islands | Accepted | CR-077 |

| [ADR-004](ADR-004-regular-polygon-parameters.md) | RegularPolygon parameter authority / LWPOLYLINE | Accepted | CR-078 |

| [ADR-005](ADR-005-regular-star-cycles.md) | RegularStar skip cycles / crossing and material parity | Accepted | CR-079 |

| [ADR-006](ADR-006-editable-arc-rectangle-angles.md) | Signed ARC reconnection / Rectangle local rotation | Accepted | CR-081/082 |

- [ADR-007](ADR-007-svg-import-geometry.md) — SVG viewport/profile/physical curve approximation and safe data-only import.

- [ADR-008](ADR-008-cosmicconvert-curve-certification.md): separate converter and certified LINE/ARC approximation (CR-085).
