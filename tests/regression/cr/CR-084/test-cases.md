# CR-084 SVG import cases

## TC-084-001
Execution Type AUTO; Status ACTIVE; Windows .NET10. Procedure --tests TC-084-001.
Expected declared mm/cm/in/pt/pc/px sizes, viewBox offsets/default meet/none, Y inversion, group transform order/rotation/matrix/local use, document-unit physical size, primitives/open/closed counts and native Circle. SvgImportTests.Coordinates.

## TC-084-002
Execution Type AUTO; Status ACTIVE; Windows .NET10. Procedure --tests TC-084-002.
Expected absolute/relative line/Bezier/smooth/quadratic/elliptic arc paths, multi-subpath/winding/islands/self-crossing exterior only, nominal physical curve approximation, DXF/history/clipboard/3D. SvgImportTests.Paths.

## TC-084-003
Execution Type AUTO; Status ACTIVE; Windows .NET10. Procedure --tests TC-084-003.
Expected hidden/unsupported warnings, no script/network/DTD, malformed/path/bounds/negative/singular/cycle/duplicateID/viewport rejection and invariant numeric culture/source unchanged. SvgImportTests.Validation.

## TC-084-004
Execution Type MANUAL; Status ACTIVE; PENDING_MANUAL.
Procedure actual File Import SVG pick/confirm/cancel with modified current document, size/unit/English-Korean dialogs; compare representative Inkscape/Illustrator export to source (viewBox/groups/curves/islands), edit/Flat/3D/copy/Undo/DXF/CAD. Verify warning and Save As prevents source overwrite. Review actual screenshots and curve approximation before manufacture. Reviewer/date/evidence required; no human PASS recorded.
