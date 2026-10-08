# ADR-008 Separate converter and curve error certification
## Status
Accepted — user request 2026-10-08, CR-085. Human acceptance of GUI/CAD remains pending.
## Decision
Separate CosmicConvert WPF WinExe with no runtime reference to CosmicDesigner. Reuse framework data-only path parser, not integrated importer. Keep source Bezier control points/transformed elliptic parameters. Output physical mm LINE/ARC/CIRCLE in L; no material rectangle. Default .05mm error with numeric reserve.

Bezier chord deviation is bounded by max norm of control points minus chord control points at Bernstein index fractions. Elliptic chord interpolation is bounded by `(length(U)+length(V))*sweep²/8` from the second-derivative norm. Candidate circles are fitted through start/mid/end; recursive subdivision certifies same-parameter distance via triangle inequality through the circle chord and `radius*sweep²/8`. Bounds are conservative over all t, not sampled claims; failed fit falls back to subdivision. Exact circles/arcs under similarity use native primitives. No accumulated lossy simplification. Exact collinear lines and exact duplicate emitted entities can be removed without spending error budget.

SVG stroke uses centerline. Closed fill-none retains paths; nested nonintersecting fill handles winding transitions; touching/intersecting fill is explicitly unsupported to avoid an uncertified Boolean algorithm. Preview is an unoptimized cutting-path view, not painted SVG rendering. Resource limits fail explicitly. Atomic same-directory temporary write and readback precede replacement.
## Consequences
No new third-party dependency. Mixed ARC/LINE output preserves geometry but current CosmicDesigner general importer does not assemble those loops into one Cut; report actual grouping rather than changing Designer behavior. Independent engine and CLI tests plus required human GUI/CAD checks. Later full Boolean fill/CAD object assembly requires a separate approved change.
## References
CR-085; source CosmicConvert/Geometry.cs; ADR-007 retained for integrated import, not superseded.

## CR-086 amendment — 2026-10-08
Explicit user10mm margin request supersedes no-frame/no-origin-shift export: actual optimized LINE/ARC/CIRCLE bounds +10mm on all sides, frame origin0, rigid shape translation, unchanged scale/error. Reader/preview/source curves remain unchanged. No new ADR for this routine layout change.
CR-087 amendment: standalone converter snapshots actual VCuttingDxfSerializer nested DTO declarations; no runtime Designer dependency. Actual serializer Load/Save and hit/history/material integration guard the contract. Metadata uses signed unwrapped ARC angles; generic DXF uses CCW. Same layout drives both.
