@echo off
setlocal

dotnet publish ..\src\PdfCompressor.App\PdfCompressor.App.csproj ^
  -c Release ^
  -o ..\publish\net472

echo.
echo Publicado em: %~dp0..\publish\net472
endlocal
