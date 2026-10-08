# CR-086 accumulated cases
2026-10-08, ACTIVE.

- TC-086-001 — Execution Type AUTO. Preconditions Windows/.NET10 converter build. Run `CosmicConvert.Verification --tests TC-086-001`. Circle30×30 frame, negative/open/zero-height geometry, cubic/major arc bounds: every optimized-shape extrema has10mm clearance to four rectangle edges; four closed LINEs; rigid translation preserves radii/sweeps/bounds; actual quiet subprocess and DXF readback retain mm/frame. Source viewport does not determine frame size.
- TC-086-002 — Execution Type MANUAL. Open Apple, Save/cancel/overwrite, inspect status; open result in CAD/CosmicDesigner and measure four margins, frame sizes, unchanged shape proportions; compare quiet and GUI output, capture reviewer/time/build/screenshots. Expected10mm margins and correct external material frame. PENDING_MANUAL.

Approved output expectation changes: CR-086 supersedes CR-085 no-frame/no-origin-shift policy only. TC-085-002 saved entity count adds4frame LINEs; optimizer shape expectations retained. TC-085-004 compares shape bounds after removing the rigid offset and frame; baseline/original/hash/curve/closure expectations retained. Historical CR-085 results are unchanged. TC-085-003 atomic/quiet behavior retained.
