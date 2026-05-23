# PDF Compressor

Utilitário leve para **comprimir arquivos PDF diretamente pelo menu de contexto do Windows Explorer**, sem abrir nenhuma janela. Selecione um PDF, clique com o botão direito e escolha **Comprimir PDF** — o arquivo comprimido aparece na mesma pasta em segundos.

![Windows 7 SP1+](https://img.shields.io/badge/Windows-7%20SP1%20%7C%2010%20%7C%2011-0078D4?logo=windows)
![.NET Framework 4.7.2](https://img.shields.io/badge/.NET%20Framework-4.7.2-512BD4?logo=dotnet)
![Ghostscript 10.07.1](https://img.shields.io/badge/Ghostscript-10.07.1-green)
![License MIT](https://img.shields.io/badge/License-MIT-yellow)

---

## Funcionalidades

- Adiciona a opção **Comprimir PDF** ao menu de contexto de arquivos `.pdf`
- Comprime em segundo plano, **sem nenhuma janela ou console visível**
- Salva o resultado na **mesma pasta** com o sufixo `_compressed`
- **Nunca sobrescreve** o arquivo original
- Descarta o arquivo gerado se ele não for menor que o original
- Notificação discreta ao final: sucesso com % de redução, sem redução, ou erro
- Ghostscript **já incluso** no instalador — zero dependências externas

### Exemplo

| Arquivo original           | Arquivo gerado                          |
|----------------------------|-----------------------------------------|
| `C:\Docs\Contrato.pdf`     | `C:\Docs\Contrato_compressed.pdf`       |

Se `Contrato_compressed.pdf` já existir, o app gera `Contrato_compressed_1.pdf`, `Contrato_compressed_2.pdf`, e assim por diante.

---

## Instalação

1. Baixe o instalador na página de [Releases](https://github.com/c-Morette/pdf-compressor/releases/latest): `PDFCompress-Setup-x.x.x.exe`
2. Execute o instalador (requer privilégio de administrador para instalar em `Program Files`)
3. Pronto — o menu de contexto já estará disponível no Explorer

**Requisitos do sistema:**
- Windows 7 SP1 x64, Windows 10 x64 ou Windows 11 x64
- [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472) ou superior
  - Já incluído no Windows 10/11. Para Windows 7 SP1, instale via Windows Update ou pelo link acima.

**Desinstalar:** Painel de Controle → Programas → PDF Compressor → Desinstalar.
O menu de contexto é removido automaticamente.

---

## Como usar

1. Clique com o botão direito em qualquer arquivo `.pdf` no Explorer
2. Escolha **Comprimir PDF**
3. Aguarde a notificação com o resultado

Os logs ficam em:
```
%LOCALAPPDATA%\PdfCompressor\logs\app.log
```

---

## Compilando o projeto

### Pré-requisitos

- [.NET SDK 6+](https://dotnet.microsoft.com/download) (qualquer versão recente)
- [Inno Setup 6](https://jrsoftware.org/isdl.php) (para gerar o instalador)
- Ghostscript extraído em `vendor\ghostscript\` (veja abaixo)

### 1. Clonar e compilar

```powershell
git clone https://github.com/c-Morette/pdf-compressor.git
cd pdf-compressor

# Publicar os binários do app
.\build\publish-single.ps1
# Saída: publish\net472\
```

### 2. Preparar o Ghostscript

O Ghostscript não é versionado no repositório (binários de terceiros, ~60 MB).
Um script de setup faz o download e a instalação automaticamente:

```powershell
.\scripts\setup-vendor.ps1
```

O script baixa o Ghostscript 10.07.1 do GitHub (Artifex Software) e instala em
`vendor\ghostscript\`. Só é necessário rodar uma vez — nas próximas vezes ele detecta
que já está instalado e não faz nada.

<details>
<summary>Instalar manualmente (sem o script)</summary>

Baixe o instalador Windows x64 em [ghostscript.com/releases](https://www.ghostscript.com/releases/gsdnld.html) e execute:

```powershell
.\gs10071w64.exe /S /D=C:\caminho\do\repo\vendor\ghostscript
```

Estrutura esperada em `vendor\ghostscript\`:
```
bin\gswin64c.exe
bin\gsdll64.dll
lib\
Resource\
iccprofiles\
```
</details>

### 3. Gerar o instalador

Abra `installer\PdfCompressor.iss` no Inno Setup Compiler e clique em **Build**, ou via linha de comando:

```powershell
& "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe" installer\PdfCompressor.iss
# Saída: installer\output\PDFCompress-Setup-1.0.0.exe
```

---

## Estrutura do projeto

```
pdf-compressor/
├── src/
│   ├── PdfCompressor.App/          # Ponto de entrada (WinExe, .NET FX 4.7.2)
│   ├── PdfCompressor.Core/         # Lógica de compressão e validação
│   └── PdfCompressor.Windows/      # Menu de contexto e notificações (P/Invoke)
├── build/
│   ├── publish-single.ps1          # Script de publicação (PowerShell)
│   └── publish-single.cmd          # Script de publicação (CMD)
├── installer/
│   └── PdfCompressor.iss           # Script Inno Setup
└── vendor/
    └── ghostscript/                # Ghostscript extraído (não versionado)
```

---

## Presets de compressão

O app usa o preset `ebook` do Ghostscript. Outros valores suportados internamente:

| Preset    | Qualidade | Redução |
|-----------|-----------|---------|
| `screen`  | Baixa     | Máxima  |
| `ebook`   | Média     | Boa (padrão) |
| `printer` | Alta      | Menor   |

---

## Licença

O código-fonte deste projeto é distribuído sob a licença **MIT** — veja o arquivo [LICENSE](LICENSE).

O instalador inclui o **[Ghostscript](https://www.ghostscript.com/)**, distribuído sob a
[GNU AGPL v3](https://www.gnu.org/licenses/agpl-3.0.html) pela Artifex Software.
O código-fonte do Ghostscript está disponível em
[github.com/ArtifexSoftware/ghostpdl](https://github.com/ArtifexSoftware/ghostpdl).

---

## Contribuindo

Contribuições são bem-vindas! Para reportar bugs ou sugerir melhorias, abra uma
[issue](https://github.com/c-Morette/pdf-compressor/issues). Para enviar código, faça um fork, crie um branch e abra um Pull Request.
