# CosmicDesigner Windows 설치 프로그램

다섯 WPF 프로젝트를 Release/win-x64/self-contained로 게시하고, 파일 충돌을 검증해 하나의
스테이지로 병합한 뒤 Inno Setup으로 단일 설치 파일을 생성합니다.

    powershell -ExecutionPolicy Bypass -File .\installer\Build-Installer.ps1

ISCC가 PATH나 기본 설치 위치에 없다면 `-IsccPath C:\path\to\ISCC.exe`를 지정합니다.
결과물은 `installer\output\CosmicDesignerSetup.exe`, 기본 설치 위치는 표준 `{autopf}`를 사용한
`C:\Program Files\CosmicDesigner`입니다.

설치 항목은 DXFExplorer, DXFViewer, DXFSimulator, DXFDrawer, CosmicDesigner 및 self-contained .NET Windows Desktop
종속 파일입니다. 대표 Help HTML 6개와 오프라인 `help` 디렉터리도 함께 설치하고 제거합니다.
CosmicDesigner 공식 사용자 원본은 `help-content/help/index.html`이며 설치 후 `help/index.html`입니다.
시작 메뉴에는 다섯 앱과 제거 항목을 등록하며, 선택형 바탕 화면 바로가기는
CosmicDesigner 하나만 만듭니다. 사용자 설정은 `%LOCALAPPDATA%\DXFExplorer\settings.json`에
저장됩니다. 기존 AppId를 유지하므로 이전 설치를 같은 위치에서 업그레이드할 수 있습니다.

CosmicDesigner 설정은 별도로 `%LOCALAPPDATA%\CosmicDesigner\settings.json`에 저장됩니다.
CR-064의 실제 새/사용자 지정 경로 설치, 업그레이드 및 제거 검증은 TC-064-003에서 사람 확인이 필요합니다.

## Release Candidate Freeze
`Build-Installer.ps1 -OutputRoot "$PWD/artifacts/release/1.20.29-rc1" -IsccPath <ISCC.exe>`는 기존 출력과 분리된 publish/stage/output을 사용한다. OutputRoot는 저장소 내부 경로여야 하며 재귀 초기화 전에 최종 절대 경로를 검사한다. 기본 실행은 기존 installer 경로를 사용한다. stdlib Python `installer/Verify-Freeze.py`로 관리 대상/비밀 패턴/개인 경로/프로젝트 참조·리소스를 검사한다. RC 소스 식별과 재현 절차는 docs/project/releases/RC-1.20.29-rc1.md 참조.

`ReleaseVersion.iss`가 설치 버전 설정 원본이다. AppVersion/ProductVersion: 1.20.29-rc1; Windows FileVersion: 1.20.29.0. Build-Installer.ps1은 생성 EXE를 Verify-Installer.ps1으로 검사한다. 새 Freeze 승인 시 해당 정의를 갱신한다.

CR-066: 설치 화면/설치 목록/새 기본 폴더/시작 메뉴 그룹은 CosmicDesigner. 바탕 화면 및 완료 후 실행은 CosmicDesigner.exe. 기존 AppId/UsePreviousAppDir는 유지하므로 업데이트는 이전 설치 경로를 사용한다. 포함된 다른 앱의 이름은 유지한다.
