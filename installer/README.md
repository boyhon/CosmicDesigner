# DXFExplorer Windows 설치 프로그램

네 WPF 프로젝트를 Release/win-x64/self-contained로 게시하고, 파일 충돌을 검증해 하나의
스테이지로 병합한 뒤 Inno Setup으로 단일 설치 파일을 생성합니다.

    powershell -ExecutionPolicy Bypass -File .\installer\Build-Installer.ps1

ISCC가 PATH나 기본 설치 위치에 없다면 `-IsccPath C:\path\to\ISCC.exe`를 지정합니다.
결과물은 `installer\output\DXFExplorerSetup.exe`, 기본 설치 위치는 표준 `{autopf}`를 사용한
`C:\Program Files\DXFExplorer`입니다.

설치 항목은 DXFExplorer, DXFViewer, DXFSimulator, DXFDrawer 및 self-contained .NET Windows Desktop
종속 파일입니다. 대표 Help HTML 5개와 오프라인 `help` 디렉터리도 함께 설치하고 제거합니다.
시작 메뉴에는 네 앱과 제거 항목을 등록하며, 선택형 바탕 화면 바로가기는
DXFExplorer 하나만 만듭니다. 사용자 설정은 `%LOCALAPPDATA%\DXFExplorer\settings.json`에
저장됩니다. 기존 AppId를 유지하므로 이전 설치를 같은 위치에서 업그레이드할 수 있습니다.
