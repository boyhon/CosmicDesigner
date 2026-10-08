# ADR-007 SVG geometry import
## Status
Accepted — user-approved CR-084, 2026-10-07.
## Decision
SVG is an import input, DXF remains save/interchange. Import into new document; root viewport is rectangular material and each closed geometric profile is a Cut, open figure a Slit, independent of painted color/stroke width. No heuristic largest-profile removal. Use SVG viewBox/aspect ratio/96dpi length rules and invert Y once into model coordinates. Preserve native circular primitives under similarity; general curves flatten nominal0.01mm in physical coordinates using WPF geometry. Normalize closed fill boundaries (SVG nonzero default/evenodd) to Compound even-odd loops so self-crossings do not retain redundant internal cutting paths. Explicitly report approximated curves and unsupported content. Material extent overflow rejects; no hidden clipping/resizing.

XmlReader prohibits DTD and external resolution; no SVG scripting/rendering/network, only geometry attributes. Local use IDs supported with recursion/complexity limits; external references ignored with warnings. Embedded CSS/text/images/filter/mask/clip/nested viewports unsupported with guidance; do not claim full SVG rendering fidelity. Numeric culture stays invariant regardless of UI language. Existing user settings/file metadata/units and Save As protection retained.
## Rationale / Consequences
Treating the largest outline as Outer can silently discard a logo/closed shape; retaining declared viewport keeps positions/scale predictable. Existing model has LINE/ARC/CIRCLE and Compound loops but no Bezier/native elliptic arc; sampled physical geometry stays editable/exportable without dependencies or new save schema. Human source comparison remains required for manufacturing. Future richer SVG styling/viewport support is a separate change; nominal tolerance is disclosed, not an exact-curve claim.
## References
[SVG2 paths](https://www.w3.org/TR/SVG2/paths.html), [SVG2 coordinates](https://www.w3.org/TR/SVG2/coords.html), [WPF path markup](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/path-markup-syntax).
