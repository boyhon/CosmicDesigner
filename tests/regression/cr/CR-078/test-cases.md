# CR-078 accumulated cases
Original user request: docs/project/change-requests/assets/cr078-regular-polygon-request.txt. TC-RP-001..010 map in order to TC-078-001..010.

## TC-078-001
Execution Type: AUTO. Status: ACTIVE.
Preconditions: Windows .NET10 Development build.
Procedure: --tests TC-078-001.
Expected: N3 radius10 count3 and circumradius equal.
Implementation: VCutting.Verification/RegularPolygonTests.cs::Triangle.

## TC-078-002
Execution Type: AUTO. Status: ACTIVE.
Preconditions: Windows .NET10 Development build.
Procedure: --tests TC-078-002.
Expected: N4 radius10 rotation0 vertices (10,0)/(0,10)/(-10,0)/(0,-10).
Implementation: VCutting.Verification/RegularPolygonTests.cs::Square.

## TC-078-003
Execution Type: AUTO. Status: ACTIVE.
Preconditions: Windows .NET10 Development build.
Procedure: --tests TC-078-003.
Expected: N6 equal edge lengths.
Implementation: VCutting.Verification/RegularPolygonTests.cs::EqualEdges.

## TC-078-004
Execution Type: AUTO. Status: ACTIVE.
Preconditions: Windows .NET10 Development build.
Procedure: --tests TC-078-004.
Expected: All vertices equal circumradius for3..512 and rotations.
Implementation: VCutting.Verification/RegularPolygonTests.cs::RadiusDistance.

## TC-078-005
Execution Type: AUTO. Status: ACTIVE.
Preconditions: Windows .NET10 Development build.
Procedure: --tests TC-078-005.
Expected: Sides6→8 regenerates all vertices, other parameters fixed.
Implementation: VCutting.Verification/RegularPolygonTests.cs::ChangeSides.

## TC-078-006
Execution Type: AUTO. Status: ACTIVE.
Preconditions: Windows .NET10 Development build.
Procedure: --tests TC-078-006.
Expected: Radius20→40, center/sides/angle fixed.
Implementation: VCutting.Verification/RegularPolygonTests.cs::ChangeRadius.

## TC-078-007
Execution Type: AUTO. Status: ACTIVE.
Preconditions: Windows .NET10 Development build.
Procedure: --tests TC-078-007.
Expected: N4 radius50 rotation0→45 correctly CCW rotates.
Implementation: VCutting.Verification/RegularPolygonTests.cs::ChangeRotation.

## TC-078-008
Execution Type: AUTO. Status: ACTIVE.
Preconditions: Windows .NET10 Development build.
Procedure: --tests TC-078-008.
Expected: Center offset (0,0)→(100,50), body move parameters fixed.
Implementation: VCutting.Verification/RegularPolygonTests.cs::ChangeCenter.

## TC-078-009
Execution Type: AUTO. Status: ACTIVE.
Preconditions: Windows .NET10 Development build.
Procedure: --tests TC-078-009.
Expected: One closed LWPOLYLINE N vertices/layer L/subclasses; JSON restores parameters; generic plain import stays contour.
Implementation: VCutting.Verification/RegularPolygonTests.cs::Export.

## TC-078-010
Execution Type: AUTO. Status: ACTIVE.
Preconditions: Windows .NET10 Development build.
Procedure: --tests TC-078-010.
Expected: Invalid radius/sides/finite/bounds rejected without dirty state; no-op unchanged, angle normalized.
Implementation: VCutting.Verification/RegularPolygonTests.cs::Invalid.

## TC-078-011
Execution Type: AUTO. Status: ACTIVE.
Preconditions: Windows .NET10 Development build.
Procedure: --tests TC-078-011.
Expected: Two-click state/shared preview and live sides/cancel, whole Cut/Inner/delete, create/edit/delete Undo/Redo, units, DXF, Flat/3D, internal/boundary merge and fillet→Compound, localized strings.
Implementation: VCutting.Verification/RegularPolygonTests.cs::Integration.

## TC-078-012
Execution Type: MANUAL. Status: ACTIVE. Result: PENDING_MANUAL.
Preconditions: latest CosmicDesigner.exe, English/한국어, 100%/150% DPI; AutoCAD available for interchange review.
Procedure/Expected: inspect polygon icon/tooltip/Sides6 input. Click center then move without button pressed; preview reflects radius/CCW direction, change Sides during preview then return and finalize first vertex. ESC/right-click and other tools cancel without object/history. Invalid/fractional Sides, zero radius and outside-material placement give guidance and no mutation. Edit Center/Radius/Sides/Rotation, each other parameter preserved and one Undo; edge/body selects whole polygon, body drag moves center, no individual edges or deformation handles. Delete whole, Undo/Redo, Zoom/Pan, mm/cm, save/reopen, Flat/3D, internal/outer contour merge. AutoCAD opens exported shape as one closed LWPOLYLINE on L without repeated last vertex; compare orientation and dimensions. Actual app startup smoke is AUTO only, not this visual acceptance.
Reviewer/time/evidence: pending; never infer human PASS.
