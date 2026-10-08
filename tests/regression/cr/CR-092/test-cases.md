# TC-092-001
Execution Type: AUTO. Status: ACTIVE.
Purpose: reference WinExe publication without duplicate Help version output.
Procedure: versioning_tests.py::test_referenced_apps_publish_one_help_version creates isolated Viewer/Simulator projects, references Viewer from Simulator, invokes actual dotnet publish and reads output Help version.js. TC092001 shares full ReleaseVersionPolicy runner.
Expected: successful publish, Development text exactly correct, duplicate-output gate retained. Full policy suite remains PASS.
Actual six-project self-contained publication and stage hashes recorded separately before replacement Freeze.
