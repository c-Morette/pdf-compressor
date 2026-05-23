#Requires -Version 5.1
<#
.SYNOPSIS
    Baixa e instala o Ghostscript em vendor\ghostscript\ para build do instalador.
.DESCRIPTION
    Deve ser executado uma vez após clonar o repositório.
    Requer acesso à internet para baixar o Ghostscript do GitHub (Artifex Software).
#>

$ErrorActionPreference = 'Stop'

$GS_VERSION     = "10.07.1"
$GS_TAG         = "gs10071"
$GS_EXE         = "gs10071w64.exe"
$GS_URL         = "https://github.com/ArtifexSoftware/ghostpdl-downloads/releases/download/$GS_TAG/$GS_EXE"
$VENDOR_DIR     = Join-Path $PSScriptRoot "..\vendor\ghostscript"
$VENDOR_DIR     = [IO.Path]::GetFullPath($VENDOR_DIR)
$TEMP_INSTALLER = Join-Path $env:TEMP $GS_EXE

# ── Verificar se já está instalado ────────────────────────────────────────────
$gsExe = Join-Path $VENDOR_DIR "bin\gswin64c.exe"
if (Test-Path $gsExe) {
    $version = & $gsExe --version 2>$null
    if ($version -eq $GS_VERSION) {
        Write-Host "Ghostscript $GS_VERSION já está em vendor\ghostscript\. Nada a fazer." -ForegroundColor Green
        exit 0
    }
    Write-Host "Versão diferente encontrada ($version). Re-instalando..." -ForegroundColor Yellow
}

# ── Download ───────────────────────────────────────────────────────────────────
Write-Host "Baixando Ghostscript $GS_VERSION (~62 MB)..."
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Invoke-WebRequest -Uri $GS_URL -OutFile $TEMP_INSTALLER -UseBasicParsing
Write-Host "Download concluído: $('{0:N1} MB' -f ((Get-Item $TEMP_INSTALLER).Length / 1MB))"

# ── Instalação silenciosa em vendor\ghostscript\ ───────────────────────────────
New-Item -ItemType Directory -Force $VENDOR_DIR | Out-Null
Write-Host "Instalando em: $VENDOR_DIR"
$proc = Start-Process -FilePath $TEMP_INSTALLER -ArgumentList "/S /D=$VENDOR_DIR" -Wait -PassThru
if ($proc.ExitCode -ne 0) {
    throw "Falha na instalação do Ghostscript (exit code $($proc.ExitCode))."
}

# ── Verificar resultado ────────────────────────────────────────────────────────
if (-not (Test-Path $gsExe)) {
    throw "gswin64c.exe não encontrado após instalação. Verifique $VENDOR_DIR"
}
$installedVersion = & $gsExe --version 2>$null
Write-Host "Ghostscript $installedVersion instalado com sucesso em vendor\ghostscript\" -ForegroundColor Green

# ── Limpeza ────────────────────────────────────────────────────────────────────
Remove-Item $TEMP_INSTALLER -Force
Write-Host "Pronto. Agora execute: .\build\publish-single.ps1"
