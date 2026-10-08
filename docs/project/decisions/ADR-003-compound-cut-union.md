# ADR-003 Compound Cut union geometry
## Status
Accepted — CR-077 user-approved internal union workflow, 2026-10-07.
## Context
Overlapping holes require a single editable/manufacturing contour, preserving circular arcs and any material islands rather than polygonizing circles or erasing topology.
## Decision
Split analytic LINE/ARC segments at intersections; retain only the union boundary, deduplicate coincident pieces, order closed loops. CIRCLE inputs normalize to two exact semicircular ARC segments. Selected Cut identity is retained as Shape=Compound. Existing Geometry array stores successive closed loops; endpoint closure determines loop boundaries. Within one Cut, loops use even-odd fill; across independent Cuts, filled holes are unioned. DXF writes existing LINE/ARC entries and metadata/history preserve their order; no synthetic joining/cutting lines or new file schema required. Current application reads older single-loop data. Older application versions may not interpret new compound multi-loop semantics correctly.
Compound movement preserves geometry. Curve-containing Compound resize is uniform to retain true circular arcs; LINE-only Compound supports affine resize. Reject invalid/nonmanifold geometry and point-only contacts safely. Existing Outer boundary union/subtraction workflow stays distinct.
## Alternatives
WPF polygon-only union loses circular precision. Multiple primitive Cuts retain redundant seams and do not satisfy one-object behavior. New geometry container/file format is unnecessary: DXF LINE/ARC and ordered loops already represent the requested shape.
## Consequences
Flat masking and 3D clipping use per-Cut loop parity and union across Cuts; preserve material islands. Tests must cover topology, signed ARC direction, compound edits, IDs, history, unit conversion and DXF. Ellipse keeps its currently sampled geometry; native ELLIPSE remains separate. Human UI acceptance remains required.
