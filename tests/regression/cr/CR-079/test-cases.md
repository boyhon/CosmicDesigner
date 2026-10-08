# CR-079 accumulated cases
All cases ACTIVE. Preconditions: Windows .NET10 Development build, source CR-079.

## TC-079-001
Execution Type: AUTO. Procedure: --tests TC-079-001.
Expected: {5/2},{6/2},{7/2},{7/3},{8/2},{8/3},{512/255} have N equal circumradius vertices and equal skip-edge lengths; gcd loop count. Invalid N/K/finite/radius/bounds rejected without dirty mutation. Simple fillet/Outer merge not applied to intersecting primitive.
Implementation: VCutting.Verification/RegularStarTests.cs::Geometry.

## TC-079-002
Execution Type: AUTO. Procedure: --tests TC-079-002.
Expected: actual Flat creation/preview/ESC event and per-edge whole-object hit; invalid Step retains preview. Parameter editing/history/create/delete/movement/unit scaling preserve one object and cycle topology. DXF metadata roundtrip and standard parser/plain import preserve all N segments, closed LWPOLYLINE per cycle. en/ko keys present.
Implementation: VCutting.Verification/RegularStarTests.cs::Integration.

## TC-079-003
Execution Type: AUTO. Procedure: --tests TC-079-003.
Expected: analytical parity and precise Flat mask agree across sample grid for all variants outside the 0.02 screen-unit WPF boundary tolerance band (exact line geometry checked independently); 3D triangle centroids avoid cut areas; internal union with containing circle creates one Compound and removes central material island.
Implementation: VCutting.Verification/RegularStarTests.cs::Material.

## TC-079-004
Execution Type: MANUAL. Procedure: launch CosmicDesigner.exe; English and 한국어; Hole star icon, N/Step and center→first vertex. Try listed variants, live N/K changes, invalid inputs, right-click/ESC/tool/document switch, select/move/delete/property edit/Undo/Redo. Check narrow-window labels and keyboard workflow, Flat and 3D crossing lines/material islands; save/open in CosmicDesigner and external CAD (closed cycles without synthetic bridges). Compare official holes#regular-star instructions. Capture actual screenshots.
Expected: natural/localized uncut controls, preview exactly matches creation, one logical object and stable parameters, no unintended functions changed; external CAD geometry matches all crossings. Human reviewer/date/evidence required.
Result: PENDING_MANUAL; no human PASS recorded.

## User-approved supersession — 2026-10-07
TC-079-003/004 are SUPERSEDED by TC-079-005/006. Historical parity expectations above remain unchanged; user explicitly requires the entire star interior to be removed.

## TC-079-005
Execution Type: AUTO. Status: ACTIVE. Preconditions: Windows .NET10 build.
Procedure: --tests TC-079-005.
Expected: {5/2},{6/2},{7/3},{8/2},{8/3} center and full nonzero interior removed. Flat mask matches winding away from0.02 screen-unit boundary band; all retained3D triangle centroids outside the solid cut. Union with other cuts includes center and yields valid Compound. Existing ring/island behavior remains TC-077-001. Saved stars rederive same geometry/solid semantics without data migration; parameters/history/cycles remain TC-079-002.
Implementation: VCutting.Verification/RegularStarTests.cs::SolidMaterial.

## TC-079-006
Execution Type: MANUAL. Status: ACTIVE. Preconditions: CosmicDesigner solid-star build.
Procedure: repeat TC-079-004 control/preview/selection/DXF checks, replacing island expectations with all interior white/empty. Verify screenshot example with large star, center selection, Flat/3D at different zooms, save/open existing star; compound ring still has its material island. Review Korean/English help. Capture evidence.
Expected: no residual inverted polygon; crossing outline retained; UI/3D visually correct. Result: PENDING_MANUAL, no human PASS.

## Profile-only supersession — 2026-10-07
TC-079-002/006 SUPERSEDED by007/009, approved removal of crossing path expectations; original specs/code retained.
## TC-079-007
Execution Type: AUTO. Status ACTIVE. Windows .NET10. Procedure --tests TC-079-007.
Expected: actual Flat preview/create/select/ESC; one logical parametric star; properties/history/units/delete preserved. DXF one closed exterior LWPOLYLINE with2N vertices, plain import same2N profile edges, metadata roundtrip. No multiple-cycle crossing export. Implementation RegularStarTests::ProfileIntegration.
## TC-079-008
Execution Type: AUTO. Status ACTIVE. Windows .NET10. Procedure --tests TC-079-008.
Expected: N5..24 all validK profiles each single closed2N-edge loop; each edge midpoint has filled side versus material side in original winding area (no interior cutting line). Grid matches raw solid-star area; profile total length less than original paths. Implementation RegularStarTests::ProfileBoundary.
## TC-079-009
Execution Type: MANUAL. Status ACTIVE. Launch new profile build. Draw5/2,6/2,7/2,7/3,8/2,8/3 at rotations; preview/Flat/3D only external silhouette, entire center empty. Select/edit/move/delete/Undo/Redo; reopen old saved star. Inspect DXF in external CAD/CAM: one closed profile, no interior laser paths, contiguous sequence. Korean/English/narrow layout/help review and screenshots. Human evidence required. PENDING_MANUAL; no human PASS.
