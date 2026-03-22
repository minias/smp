[Setup]
AppName=SMP
AppVersion=1.0.0

; ✅ 64bit 기준 Program Files 사용
DefaultDirName={autopf}\SMP

DefaultGroupName=SMP
OutputDir=Output
OutputBaseFilename=SMP_Setup

; ✅ 권한 상승 (Program Files 설치를 위해 필요)
PrivilegesRequired=admin

Compression=lzma
SolidCompression=yes

; ✅ 설치 아키텍처 명시
;ArchitecturesAllowed=x64
;ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Files]
; ✅ publish 폴더 전체 복사
Source: "SMP.App\bin\Release\net10.0-windows\win-x64\publish\*"; \
DestDir: "{app}"; \
Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\SMP"; Filename: "{app}\SMP.exe"
Name: "{group}\Uninstall SMP"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\SMP.exe"; \
Description: "Run SMP"; \
Flags: nowait postinstall skipifsilent

[Code]
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usUninstall then
  begin
    if MsgBox('사용자 데이터를 삭제하시겠습니까?', mbConfirmation, MB_YESNO) = IDYES then
    begin
      DelTree(ExpandConstant('{userappdata}\SMP'), True, True, True);
    end;
  end;
end;