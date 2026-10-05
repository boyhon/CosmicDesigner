# CR-059
## TC-059-001
Execution Type: AUTO. Status: ACTIVE.
Precondition: Release verification.
Procedure: --tests TC-059-001.
Expected: horizontal/vertical labels at midpoints; nested 20 line 30 DIP farther than 10; independent 10 stays on inner lane; input order independent.
Implementation: Program.cs::SectionDimensionPlacement.
## TC-059-002
Execution Type: MANUAL. Status: ACTIVE.
Procedure: open supplied Bent example; inspect top/bottom 10/20 labels and vertical 20/50; rotate, Zoom; edit dimensions.
Expected: centered along lines; larger overlapping dimension farther out; values and lines stay synchronized, editing works.
Result: PENDING_MANUAL.
