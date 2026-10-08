# VCutting installer — current CR-070 procedure

Customer version is allocated only at an explicitly authorized Freeze. No current new-policy Freeze has been issued. See [Version procedure](../docs/project/releases/VERSIONING.md). Ordinary development builds cannot create an approved customer installer.

```powershell
./installer/Build-Installer.ps1 -FreezeRecord release/freezes/<approved-version>.json -BuildNumber <unique-build-id> -OutputRoot artifacts/release/<unique-build-id> -PythonPath <python-executable> -IsccPath <ISCC.exe>
```

The build consumes the recorded version and source/settings, generates ReleaseVersion.iss and MSBuild Freeze.props, validates source before resets/each publish/compilation and appends artifact SHA256 evidence. It does not allocate/increment RC numbers. Existing package output roots and recorded build IDs cannot be reused. Installed app/product, VCuttingSetup.exe name, AppId, default VCutting folder and existing user settings/upgrade paths remain unchanged. Static installer/ReleaseVersion.iss intentionally blocks direct unapproved builds.

Use Verify-Installer.ps1 -Path <installer> -Definitions <generated-ReleaseVersion.iss> for numeric/text metadata checks. Historical notes below describe earlier workflows; this section supersedes their commands and hardcoded version authority.

## Historical installer notes (before CR-070)

# VCutting Windows 설치 프로그램

**CR-068 / CR-069 적용:** 새 설치 기본 폴더는 `C:\Program Files\VCutting`입니다. 이전 Freeze 산출물은 보존하고 검증 패키지는 별도 경로에 빌드합니다.

다섯 WPF 프로젝트를 Release/win-x64/self-contained로 게시하고, 파일 충돌을 검증해 하나의
스테이지로 병합한 뒤 Inno Setup으로 단일 설치 파일을 생성합니다.

    powershell -ExecutionPolicy Bypass -File .\installer\Build-Installer.ps1

ISCC가 PATH나 기본 설치 위치에 없다면 `-IsccPath C:\path\to\ISCC.exe`를 지정합니다.
결과물은 `installer\output\VCuttingSetup.exe`, 기본 설치 위치는 표준 `{autopf}`를 사용한
`C:\Program Files\VCutting`입니다.

설치 항목은 VCutting.Explorer, VCutting.Viewer, VCutting.Simulator, VCutting.Drawer, VCutting 및 self-contained .NET Windows Desktop
종속 파일입니다. 대표 Help HTML 6개와 오프라인 `help` 디렉터리도 함께 설치하고 제거합니다.
VCutting 공식 사용자 원본은 `help-content/help/index.html`이며 설치 후 `help/index.html`입니다.
시작 메뉴에는 다섯 앱과 제거 항목을 등록하며, 선택형 바탕 화면 바로가기는
VCutting 하나만 만듭니다. 사용자 설정은 `%LOCALAPPDATA%\DXFExplorer\settings.json`에
저장됩니다. 기존 AppId를 유지하므로 이전 설치를 같은 위치에서 업그레이드할 수 있습니다.

VCutting 설정은 별도로 `%LOCALAPPDATA%\CosmicDesigner\settings.json`에 저장됩니다.
CR-064의 실제 새/사용자 지정 경로 설치, 업그레이드 및 제거 검증은 TC-064-003에서 사람 확인이 필요합니다.

## Release Candidate Freeze
`Build-Installer.ps1 -OutputRoot "$PWD/artifacts/release/1.20.29-rc1" -IsccPath <ISCC.exe>`는 기존 출력과 분리된 publish/stage/output을 사용한다. OutputRoot는 저장소 내부 경로여야 하며 재귀 초기화 전에 최종 절대 경로를 검사한다. 기본 실행은 기존 installer 경로를 사용한다. stdlib Python `installer/Verify-Freeze.py`로 관리 대상/비밀 패턴/개인 경로/프로젝트 참조·리소스를 검사한다. RC 소스 식별과 재현 절차는 docs/project/releases/RC-1.20.29-rc1.md 참조.

`ReleaseVersion.iss`가 설치 버전 설정 원본이다. AppVersion/ProductVersion: 1.20.29-rc1; Windows FileVersion: 1.20.29.0. Build-Installer.ps1은 생성 EXE를 Verify-Installer.ps1으로 검사한다. 새 Freeze 승인 시 해당 정의를 갱신한다.

CR-066 당시에는 CosmicDesigner 이름을 적용했다. CR-069에서 현재 VCutting 이름으로 대체했다. 기존 AppId/UsePreviousAppDir는 유지하므로 업데이트는 이전 설치 경로를 사용한다. 보조 앱도 VCutting 접두어로 변경했다.

CR-067: SetupIconFile=compiler:SetupClassicIcon.ico; Inno Setup 제공 클래식 설치 아이콘 사용.

## CR-069 current naming
VCutting.sln / VCutting, VCutting.Explorer, VCutting.Viewer, VCutting.Simulator, VCutting.Drawer. Installer source VCutting.iss; output VCuttingSetup.exe. CR-068 default folder is now implemented as C:\Program Files\VCutting; upgrades reuse their prior folder and group. Existing release artifacts remain historical. Build: `./installer/Build-Installer.ps1 -OutputRoot artifacts/release/cr069`. No new Freeze approval is implied.

## CR-090 package update — 2026-10-08
The current package publishes six programs, including standalone CosmicConvert.exe and its Start Menu shortcut. Main design executable is CosmicDesigner.exe; VCutting remains the package/installer name. User explicitly authorized 1.0.0-rc.1 Freeze; pending human verification does not imply production approval.
