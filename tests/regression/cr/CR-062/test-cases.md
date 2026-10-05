# CR-062 Test Cases
## TC-062-001
Execution Type: AUTO / ACTIVE
Purpose: All material intervals and void-aware rendering.
Preconditions: .NET 10 Windows WPF; no source design is modified.
Procedure: --tests TC-062-001
Inputs: H notch intervals [0,190], [210,300]; continuous bridge X=150; W transpose; ARC notch; no scan intersections; bends at 100/200/250.
Expected: all intervals shown, 20-unit void retained, two dimension editors, white pixels in gap/material pixels in upper piece, no gap hit, both valid bend hits; Flat/Bent render succeeds. Bend at 200 excluded. Lower fragment edit preserves upper fragment; Undo restores outline.
Implementation: CosmicDesigner.Verification/Program.cs::DisconnectedSections.
## TC-062-002
Execution Type: MANUAL / ACTIVE
Purpose: User drawing visual acceptance.
Procedure: Open supplied drawing; move H from bridge to right/left wings; compare all material spans and void spacing in Flat/Bent. Repeat W with transposed geometry. Check dimensions on each span; edit/Undo; Zoom and rotate Bent view. Click material to create a bend, click void to confirm no bend. Move section completely outside material to verify empty profile.
Expected: actual intersections shown, no solid strip or dimensions spanning the void; no loss of an upper/lower fragment.
Result: NOT_RUN / PENDING_MANUAL.
