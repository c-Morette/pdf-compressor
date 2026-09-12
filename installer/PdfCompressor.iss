; ============================================================
;  PDF Compressor — Inno Setup Script
;  Compatível com: Windows 7 SP1 x64 / Windows 10 x64 / Windows 11 x64
;
;  Dependências bundled:
;    - Ghostscript (AGPL v3) em vendor\ghostscript\
;      Baixe o Ghostscript para Windows em https://www.ghostscript.com/releases/gsdnld.html
;      Extraia os conteúdos de modo que fiquem:
;        vendor\ghostscript\bin\gswin64c.exe
;        vendor\ghostscript\bin\gsdll64.dll
;        vendor\ghostscript\lib\*.ps
;        vendor\ghostscript\Resource\...
;        vendor\ghostscript\iccprofiles\...
;
;  Pré-requisito de instalação: .NET Framework 4.7.2+
;    (incluído no Win 10/11; para Win 7 SP1 instalar via Windows Update)
; ============================================================

#define AppName       "PDF Compressor"
#define AppVersion    "1.0.1"
#define AppPublisher  "PDFCompress"
#define AppExeName    "PdfCompressor.exe"
#define AppId         "E2B2CD85-F409-425E-8E10-8B9509A2F8A3"

; Caminhos relativos ao script .iss
#define BuildDir      "..\publish\net472"
#define GhostscriptDir "..\vendor\ghostscript"

[Setup]
AppId={{{#AppId}}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
AppCopyright=Copyright (C) {#AppPublisher}

; Diretório padrão de instalação
DefaultDirName={autopf}\PDFCompress
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes

; Saída do instalador
OutputDir=output
OutputBaseFilename=PDFCompress-Setup-{#AppVersion}

; Compressão máxima (LZMA2)
Compression=lzma2/ultra64
SolidCompression=yes

; Visual
WizardStyle=modern
SetupIconFile=..\src\PdfCompressor.App\Assets\app.ico

; Somente x64
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

; Windows 7 SP1 mínimo (NT 6.1 build 7601)
MinVersion=6.1.7601

; Não criar atalho em Start Menu Group (app sem janela própria)
CreateUninstallRegKey=yes
UninstallDisplayName={#AppName}
UninstallDisplayIcon={app}\{#AppExeName}

; Solicitar elevação para poder escrever em Program Files
PrivilegesRequired=admin

[Languages]
; O primeiro da lista é o padrão quando o idioma do Windows não casa com nenhum
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "ptbr";    MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

; ── Mensagens customizadas ──────────────────────────────────
[CustomMessages]
ptbr.DotNetMissing=Este programa requer o .NET Framework 4.7.2 ou superior.%n%nPor favor, instale o .NET Framework antes de continuar.%n%nDisponível gratuitamente em:%nhttps://dotnet.microsoft.com/download/dotnet-framework/net472
english.DotNetMissing=This application requires .NET Framework 4.7.2 or higher.%n%nPlease install .NET Framework before continuing.%n%nFree download at:%nhttps://dotnet.microsoft.com/download/dotnet-framework/net472

ptbr.ContextMenuSuccess=Menu de contexto registrado com sucesso.
english.ContextMenuSuccess=Context menu registered successfully.

ptbr.ContextMenuStatus=Registrando menu de contexto...
english.ContextMenuStatus=Registering context menu...

; ── Arquivos ───────────────────────────────────────────────
[Files]
; Binários do aplicativo
Source: "{#BuildDir}\{#AppExeName}";            DestDir: "{app}"; Flags: ignoreversion
Source: "{#BuildDir}\{#AppExeName}.config";     DestDir: "{app}"; Flags: ignoreversion
Source: "{#BuildDir}\PdfCompressor.Core.dll";   DestDir: "{app}"; Flags: ignoreversion
Source: "{#BuildDir}\PdfCompressor.Windows.dll"; DestDir: "{app}"; Flags: ignoreversion
; Recursos localizados (pt-BR) — sem isto o app cai para inglês em Windows PT
Source: "{#BuildDir}\pt-BR\PdfCompressor.resources.dll"; DestDir: "{app}\pt-BR"; Flags: ignoreversion

; Ghostscript bundled — copiado para {app}\gs\
Source: "{#GhostscriptDir}\bin\*";          DestDir: "{app}\gs\bin";          Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#GhostscriptDir}\lib\*";          DestDir: "{app}\gs\lib";          Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#GhostscriptDir}\Resource\*";     DestDir: "{app}\gs\Resource";     Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#GhostscriptDir}\iccprofiles\*";  DestDir: "{app}\gs\iccprofiles";  Flags: ignoreversion recursesubdirs createallsubdirs

; ── Executar após instalar ─────────────────────────────────
[Run]
; Registra o menu de contexto para o usuário atual (HKCU — sem elevação necessária)
Filename: "{app}\{#AppExeName}"; Parameters: "--install-context-menu"; Flags: runhidden; StatusMsg: "{cm:ContextMenuStatus}"

; ── Executar antes de desinstalar ─────────────────────────
[UninstallRun]
; Remove o menu de contexto antes de apagar os arquivos
Filename: "{app}\{#AppExeName}"; Parameters: "--uninstall-context-menu"; Flags: runhidden

; ── Limpeza extra na desinstalação ────────────────────────
[UninstallDelete]
; Remove a pasta de logs deixada pelo app em AppData
Type: filesandordirs; Name: "{localappdata}\PdfCompressor"

; ── Código Pascal ─────────────────────────────────────────
[Code]

// .NET Framework 4.7.2 corresponde ao valor Release >= 461808
// Referência: https://learn.microsoft.com/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed
const
  NET_REQUIRED_RELEASE = 461808;
  NET_REG_KEY = 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full';

function IsDotNetSufficient(): Boolean;
var
  releaseValue: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM, NET_REG_KEY, 'Release', releaseValue)
            and (releaseValue >= NET_REQUIRED_RELEASE);
end;

// Bloqueia a instalação se .NET Framework não atender ao requisito mínimo
function InitializeSetup(): Boolean;
begin
  if not IsDotNetSufficient() then
  begin
    MsgBox(CustomMessage('DotNetMissing'), mbCriticalError, MB_OK);
    Result := False;
  end
  else
    Result := True;
end;

// Exibe a versão instalada do .NET no painel de informações do wizard
function GetDotNetVersion(Param: String): String;
var
  releaseValue: Cardinal;
begin
  if RegQueryDWordValue(HKLM, NET_REG_KEY, 'Release', releaseValue) then
  begin
    if releaseValue >= 528040 then Result := '4.8'
    else if releaseValue >= 461808 then Result := '4.7.2'
    else if releaseValue >= 461308 then Result := '4.7.1'
    else Result := '< 4.7.1';
  end
  else
    Result := 'Não encontrado';
end;
