# CR-070

TC-070-001 — Execution Type: AUTO. ACTIVE. Isolated committed Git fixture, simulated approval only. Tests same Freeze reuse, changed/new inputs without new CR, dirty source, tampered record/properties, RC 9→10, RC→final→patch Windows ordering, promotion evidence completeness, immutable artifact checksums. Actual repository Freeze never issued. Implementation: versioning_tests.py and ReleaseVersionPolicy.

TC-070-002 — Execution Type: MANUAL. ACTIVE. Existing user setup; inspect Development About/Help; after separate authorized Freeze check installer/app/Help/version order. Expected consistent customer version and preserved settings/AppId. PENDING_MANUAL.

TC-065-001 change authorized by CR-070: hardcoded historical version superseded by selected Freeze/build.json; without candidate only build gate automated, actual new package NOT_RUN. ID and installer verification purpose retained.

2026-10-06: TC-070-001 AUTO PASS (4 isolated Python methods, including real frozen build/gates and regression integration). TC-070-002 MANUAL PENDING_MANUAL. New/affected/full AUTO 1/1, 6/6, 78/78 PASS; no actual new-policy Freeze/customer installer issued. See results/CR-070-2026-10-06.md.
