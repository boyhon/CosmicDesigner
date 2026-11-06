# CR-094 regression cases

## TC-094-001 — 부분 중첩 U-S
Execution Type: AUTO. Status: ACTIVE. User source mapping: TC-SUB-001.
Precondition: .NET 10 Windows verification build. Procedure: --tests TC-094-001.
Inputs/expected: see immutable assertions in CutDifferenceTests.Run(1); selected is cutter, other connected cuts are targets, analytic geometry and InnerContours persist.

## TC-094-002 — 완전 포함 Hole
Execution Type: AUTO. Status: ACTIVE. User source mapping: TC-SUB-002.
Precondition: .NET 10 Windows verification build. Procedure: --tests TC-094-002.
Inputs/expected: see immutable assertions in CutDifferenceTests.Run(2); selected is cutter, other connected cuts are targets, analytic geometry and InnerContours persist.

## TC-094-003 — 빈 결과
Execution Type: AUTO. Status: ACTIVE. User source mapping: TC-SUB-003.
Precondition: .NET 10 Windows verification build. Procedure: --tests TC-094-003.
Inputs/expected: see immutable assertions in CutDifferenceTests.Run(3); selected is cutter, other connected cuts are targets, analytic geometry and InnerContours persist.

## TC-094-004 — 세 객체
Execution Type: AUTO. Status: ACTIVE. User source mapping: TC-SUB-004.
Precondition: .NET 10 Windows verification build. Procedure: --tests TC-094-004.
Inputs/expected: see immutable assertions in CutDifferenceTests.Run(4); selected is cutter, other connected cuts are targets, analytic geometry and InnerContours persist.

## TC-094-005 — 두 Island
Execution Type: AUTO. Status: ACTIVE. User source mapping: TC-SUB-005.
Precondition: .NET 10 Windows verification build. Procedure: --tests TC-094-005.
Inputs/expected: see immutable assertions in CutDifferenceTests.Run(5); selected is cutter, other connected cuts are targets, analytic geometry and InnerContours persist.

## TC-094-006 — 겹침 없음
Execution Type: AUTO. Status: ACTIVE. User source mapping: TC-SUB-006.
Precondition: .NET 10 Windows verification build. Procedure: --tests TC-094-006.
Inputs/expected: see immutable assertions in CutDifferenceTests.Run(6); selected is cutter, other connected cuts are targets, analytic geometry and InnerContours persist.

## TC-094-007 — 공유 변
Execution Type: AUTO. Status: ACTIVE. User source mapping: TC-SUB-007.
Precondition: .NET 10 Windows verification build. Procedure: --tests TC-094-007.
Inputs/expected: see immutable assertions in CutDifferenceTests.Run(7); selected is cutter, other connected cuts are targets, analytic geometry and InnerContours persist.

## TC-094-008 — 점 접촉
Execution Type: AUTO. Status: ACTIVE. User source mapping: TC-SUB-008.
Precondition: .NET 10 Windows verification build. Procedure: --tests TC-094-008.
Inputs/expected: see immutable assertions in CutDifferenceTests.Run(8); selected is cutter, other connected cuts are targets, analytic geometry and InnerContours persist.

## TC-094-009 — 연쇄 그룹
Execution Type: AUTO. Status: ACTIVE. User source mapping: TC-SUB-009.
Precondition: .NET 10 Windows verification build. Procedure: --tests TC-094-009.
Inputs/expected: see immutable assertions in CutDifferenceTests.Run(9); selected is cutter, other connected cuts are targets, analytic geometry and InnerContours persist.

## TC-094-010 — Save/Reload/DXF
Execution Type: AUTO. Status: ACTIVE. User source mapping: TC-SUB-010.
Precondition: .NET 10 Windows verification build. Procedure: --tests TC-094-010.
Inputs/expected: see immutable assertions in CutDifferenceTests.Run(10); selected is cutter, other connected cuts are targets, analytic geometry and InnerContours persist.

## TC-094-011 — Undo/Redo 선택 및 순서
Execution Type: AUTO. Status: ACTIVE. User source mapping: TC-SUB-011.
Precondition: .NET 10 Windows verification build. Procedure: --tests TC-094-011.
Inputs/expected: see immutable assertions in CutDifferenceTests.Run(11); selected is cutter, other connected cuts are targets, analytic geometry and InnerContours persist.

## TC-094-012 — 기존 Merge 회귀
Execution Type: AUTO. Status: ACTIVE. User source mapping: TC-SUB-012.
Precondition: .NET 10 Windows verification build. Procedure: --tests TC-094-012.
Inputs/expected: see immutable assertions in CutDifferenceTests.Run(12); selected is cutter, other connected cuts are targets, analytic geometry and InnerContours persist.

## TC-094-013 — 정확한 원호
Execution Type: AUTO. Status: ACTIVE. User source mapping: TC-SUB-013 (additional analytic ARC coverage).
Precondition: .NET 10 Windows verification build. Procedure: --tests TC-094-013.
Inputs/expected: see immutable assertions in CutDifferenceTests.Run(13); selected is cutter, other connected cuts are targets, analytic geometry and InnerContours persist.

## TC-094-014
Execution Type: MANUAL. Status: ACTIVE. Result: PENDING_MANUAL.
Select overlapping cuts; English/Korean right-click menu must show Merge then Subtract. Test partial overlap, nested Hole, split islands, empty deletion, chain, edge and vertex touch; inspect Flat/3D, first-result selection and cleared empty properties. Undo once must restore originals/order/selection; Redo exact. Save/reopen/save DXF and open in external CAD: only resulting analytic paths, no source reappearance or synthetic bridge. Review manual sections and language/layout.
