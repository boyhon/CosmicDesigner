#define AppName "DXFExplorer"
#define AppVersion "1.1.0"
#define AppPublisher "DXFExplorer"

[Setup]
AppId={{72D5CE59-6E07-47D0-88C6-ADE37D3368A1}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\DXFExplorer
DefaultGroupName=DXFExplorer
DisableProgramGroupPage=yes
OutputDir=output
OutputBaseFilename=DXFExplorerSetup
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\DXFExplorer.exe
CloseApplications=yes
RestartApplications=no
SetupLogging=yes
UsePreviousAppDir=yes
UsePreviousGroup=yes
VersionInfoVersion={#AppVersion}
SetupIconFile=..\cnc_vgroove_icon.ico

[Languages]
Name: "korean"; MessagesFile: "compiler:Languages\Korean.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "바탕 화면에 DXFExplorer 바로가기 만들기"; GroupDescription: "추가 바로가기:"; Flags: unchecked

[Files]
Source: "stage\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\DXFExplorer"; Filename: "{app}\DXFExplorer.exe"; WorkingDir: "{app}"
Name: "{group}\DXFViewer"; Filename: "{app}\DXFViewer.exe"; WorkingDir: "{app}"
Name: "{group}\DXFSimulator"; Filename: "{app}\DXFSimulator.exe"; WorkingDir: "{app}"
Name: "{group}\DXFDrawer"; Filename: "{app}\DXFDrawer.exe"; WorkingDir: "{app}"
Name: "{group}\DXFExplorer 제거"; Filename: "{uninstallexe}"
Name: "{autodesktop}\DXFExplorer"; Filename: "{app}\DXFExplorer.exe"; WorkingDir: "{app}"; Tasks: desktopicon

[Run]
Filename: "{app}\DXFExplorer.exe"; Description: "DXFExplorer 실행"; Flags: nowait postinstall skipifsilent
