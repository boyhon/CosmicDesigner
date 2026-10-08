# TC-091-001
Execution Type: AUTO. Status: ACTIVE.
Purpose: WPF generated-project exclusion with real-source integrity retained.
Preconditions: Python3/Git/.NET, isolated repository fixture.
Procedure: VCutting.Verification --tests TC-091-001; versioning_tests.py::test_wpf_generated_project_is_not_source plus existing policy suite.
Expected: generated Viewer_123_wpftmp.csproj ignored, Actual.csproj rejected; existing Freeze mismatch/rebuild/version/approval checks remain PASS.
Implementation: tests/regression/versioning_tests.py, shared ReleaseVersionPolicy runner. Actual RC1 failure and replacement build tracked under release/builds.
