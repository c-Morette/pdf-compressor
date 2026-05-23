#Requires -Version 5.1
$ErrorActionPreference = 'Stop'

$projectPath = Join-Path $PSScriptRoot '..\src\PdfCompressor.App\PdfCompressor.App.csproj'
$outputPath  = Join-Path $PSScriptRoot '..\publish\net472'

dotnet publish $projectPath -c Release -o $outputPath

Write-Host "Publicado em: $outputPath" -ForegroundColor Green
