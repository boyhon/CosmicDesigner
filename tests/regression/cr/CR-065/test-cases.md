# CR-065 Regression Cases

## TC-065-001
Status: ACTIVE. Execution Type: AUTO.
Purpose: 실제 배포 파일 식별/버전 검증.
Preconditions: Inno Setup 컴파일 완료.
Procedure: installer/Verify-Installer.ps1 -Path <CosmicDesignerSetup.exe>.
Expected: 정확한 이름 CosmicDesignerSetup.exe, 숫자형 FileVersion 1.20.29.0, ProductVersion 1.20.29-rc1. 불일치 시 실패.
Implementation: installer/Verify-Installer.ps1. 앱 harness와 별도로 실행하며 Build-Installer.ps1이 컴파일 후 실행한다.

## TC-065-002
Status: ACTIVE. Execution Type: MANUAL.
Purpose: 설치 표시 및 lifecycle 확인.
Preconditions: 독립 Windows 설치/업데이트 테스트 환경.
Procedure: 설치 파일을 열고 설치 버전과 Windows 설치 목록 버전을 확인한다. 설치/업데이트/실행/도움말/제거는 TC-064-003 절차를 따른다.
Expected: 표시 버전 1.20.29-rc1, 설치 정상 및 사용자 설정 보존.
Result: PENDING_MANUAL. 확인자/일시/근거가 있어야 PASS 가능.

AUTO runner: VCutting.Verification/Program.cs::InstallerRelease; --tests TC-065-001. COSMIC_INSTALLER_PATH overrides the default compiled artifact path. Windows numeric version parts and displayed strings are checked separately.

CR-069 supersedes prior product/output branding expectations; CR-068 supersedes the installer default folder. TC IDs/purpose and other assertions remain unchanged. Historical reports preserve their original results.

CR-070 approval supersedes hardcoded historical version expectations. AUTO checks immutable Freeze gates in development; selected approved installer uses generated build.json for numeric/text metadata comparison. No selected Freeze means actual new installer compile NOT_RUN. Historical results preserved.
