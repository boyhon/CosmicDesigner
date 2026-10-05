# CR-056 Test Cases
## TC-056-001
Execution Type: AUTO. Status: ACTIVE.
Precondition: Windows/.NET verification build.
Procedure: --tests TC-056-001.
Expected: loaded rectangle H 300→100, W→150, Undo/Redo, subsequent DXF and Flat mask match current W/H; custom contour retained.
Implementation: CosmicDesigner.Verification/Program.cs::SectionRectangleResize.
## TC-056-002
Execution Type: MANUAL. Status: ACTIVE.
Precondition: open saved basic 300×300 material.
Procedure: H Section non-Bent height 300→100; compare Flat/3D; Undo/Redo; W edit; save/open and repeat.
Expected: Flat becomes a 300×100 rectangle; selectors stay within material; all views reflect dimensions; Undo/Redo restores geometry.
Result: PENDING_MANUAL.
