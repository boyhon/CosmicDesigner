#define AppName "CosmicDesigner"
#include "ReleaseVersion.iss"
#define AppPublisher "CosmicDesigner"
#ifndef StageRoot
#define StageRoot "stage"
#endif

[Setup]
AppId={{72D5CE59-6E07-47D0-88C6-ADE37D3368A1}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\CosmicDesigner
DefaultGroupName=CosmicDesigner
DisableProgramGroupPage=yes
OutputDir=output
OutputBaseFilename={#SetupBaseFilename}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\CosmicDesigner.exe
CloseApplications=yes
RestartApplications=no
SetupLogging=yes
UsePreviousAppDir=yes
UsePreviousGroup=yes
VersionInfoVersion={#AppNumericVersion}
VersionInfoProductVersion={#AppNumericVersion}
VersionInfoProductTextVersion={#AppVersion}
VersionInfoTextVersion={#AppVersion}
SetupIconFile=..\cnc_vgroove_icon.ico

[Languages]
Name: "korean"; MessagesFile: "compiler:Languages\Korean.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "바탕 화면에 CosmicDesigner 바로가기 만들기"; GroupDescription: "추가 바로가기:"; Flags: unchecked

[Files]
Source: "{#StageRoot}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\DXFExplorer"; Filename: "{app}\DXFExplorer.exe"; WorkingDir: "{app}"
Name: "{group}\DXFViewer"; Filename: "{app}\DXFViewer.exe"; WorkingDir: "{app}"
Name: "{group}\DXFSimulator"; Filename: "{app}\DXFSimulator.exe"; WorkingDir: "{app}"
Name: "{group}\DXFDrawer"; Filename: "{app}\DXFDrawer.exe"; WorkingDir: "{app}"
Name: "{group}\CosmicDesigner"; Filename: "{app}\CosmicDesigner.exe"; WorkingDir: "{app}"
Name: "{group}\CosmicDesigner 제거"; Filename: "{uninstallexe}"
Name: "{autodesktop}\CosmicDesigner"; Filename: "{app}\CosmicDesigner.exe"; WorkingDir: "{app}"; Tasks: desktopicon

[Run]
Filename: "{app}\CosmicDesigner.exe"; Description: "CosmicDesigner 실행"; Flags: nowait postinstall skipifsilent
