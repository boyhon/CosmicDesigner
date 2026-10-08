# CR-085 immutable regression cases

All cases ACTIVE, introduced 2026-10-08. Preconditions Windows/.NET10; run from repository root after building CosmicConvert.Verification. No existing expectations changed.

| ID | Execution Type | Purpose / procedure | Expected |
|---|---|---|---|
| TC-085-001 | AUTO | Run `--tests TC-085-001`; reader fixtures cover units/Y/viewBox/aspect/skew/nesting/use/primitives/relative paths, hidden geometry, malformed and unsupported/security data. | Accurate physical coordinates; invalid input fails code4; DTD/external references not executed. |
| TC-085-002 | AUTO | Run `--tests TC-085-002`; circle/native arc, nonuniform ellipse, cubic/quadratic fit, tiny corners/open paths, fill rules, independent sample diagnostics/readback/existing parser/cancellation. | Exact circular primitives; no erroneous ellipse CIRCLE; reported bound ≤.05mm; paths preserved; unsupported crossing fill rejected; DXF mm/L and geometry preserved. Samples are diagnostics, not certificates. |
| TC-085-003 | AUTO | Run `--tests TC-085-003`; spawn actual WinExe with multiple/blank/missing/unicode/dotted/absolute/relative args; overwrite then invalid input and exclusive-locked output, induced validation rejection. | codes2/3/4/5/6 correct, success0, existing output preserved on all errors, no main window observed, no temporary residue. Transient dialogs require human check. |
| TC-085-004 | AUTO | Run `--tests TC-085-004`; compare supplied Apple files, save separate output, check hashes/counts/cubic groups/closure/bounds/error and invoke existing parser/importer. | originals unchanged, 3 paths/45cubics/850baseline LINEs; reduced mixed LINE/ARC output, conservative bound ≤.05mm, readback supports entities; record actual importer grouping and no unmeasured UI speed claim. |
| TC-085-005 | MANUAL | Launch without args, Open Apple and invalid SVG, resize preview; Save/cancel/overwrite/locked output; Exit during processing then retry; run quiet success/failure from terminal with stderr capture, observe all windows; open DXF in external CAD and CosmicDesigner, compare units/closure/small details; open Help HTML. Record reviewer/time/build/screenshots. | Only Open/Save/Exit menus; no UI stall, original preview retained on Open failure, correct progress/state/atomicity/safe Exit; no quiet windows/dialogs; source/result agree within stated tolerance; importer limitations match manual. |

Human verification is PENDING_MANUAL; no automatic test establishes human PASS. Overall merge readiness remains pending.

CR-086 approved amendment: no-frame/no-origin-shift output superseded by10mm framing. TC-085-002 output count adds4; TC-085-004 compares translated shape-only bounds, not whole-frame bounds; originals/optimizer/error expectations unchanged. Historical reports retained. See CR-086/test-cases.md.
