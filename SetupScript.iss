; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "RsMapper"
#define MyAppVersion "0.1.0.0"
#define MyAppPublisher "GreenJames"
#define MyAppURL "https://github.com/GreenJamesDev/RsMapper"
#define MyAppExeName "RsMapper.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{F9B445BA-7024-4605-8394-ACF31DBD31B9}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=C:\Users\James\source\repos\RsMapper\RsMapper\bin\Release\Licenses\RsMapper-License.txt
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputDir=C:\Users\James\Desktop
OutputBaseFilename=RsMapperInstall-0.1.0.0
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "C:\Users\James\source\repos\RsMapper\RsMapper\bin\Release\RsMapper.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\James\source\repos\RsMapper\RsMapper\bin\Release\Imgs\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\James\source\repos\RsMapper\RsMapper\bin\Release\Licenses\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "C:\Users\James\source\repos\RsMapper\RsMapper\bin\Release\Components.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\James\source\repos\RsMapper\RsMapper\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\James\source\repos\RsMapper\RsMapper\bin\Release\Newtonsoft.Json.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\James\source\repos\RsMapper\RsMapper\bin\Release\Octokit.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\James\source\repos\RsMapper\RsMapper\bin\Release\Octokit.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\James\source\repos\RsMapper\RsMapper\bin\Release\RsMapper.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\James\source\repos\RsMapper\RsMapper\bin\Release\RsMapper.pdb"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
