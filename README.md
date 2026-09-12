# PDF Compressor

🇧🇷 Português | [🇺🇸 English](README.en.md)

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
- **Janela de configurações** ao abrir o executável diretamente (permite escolher idioma, preset de compressão e gerenciar o menu de contexto)
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
O menu de contexto é removido automaticamente e a pasta de configurações (`%LOCALAPPDATA%\PdfCompressor`) é apagada.

---

## Como usar

### Comprimir um PDF
1. Clique com o botão direito em qualquer arquivo `.pdf` no Explorer
2. Escolha **Comprimir PDF**
3. Aguarde a notificação com o resultado

O fluxo de compressão pelo menu de contexto roda silenciosamente em segundo plano, sem abrir nenhuma janela.

Os logs ficam em:
```
%LOCALAPPDATA%\PdfCompressor\logs\app.log
```

### Janela de configurações
Ao executar o `PdfCompressor.exe` sem argumentos (por exemplo, pelo Explorer em `C:\Program Files\PDFCompress\PdfCompressor.exe`), uma janela de configurações é exibida:

- **Idioma**: **Automático (Windows)**, **Português (Brasil)** ou **English**.
  - No modo **Automático** (padrão): sistemas com idioma `pt-*` usam Português; os demais adotam English.
  - Trocar o idioma manualmente atualiza as notificações e o rótulo do menu do Explorer (regravado imediatamente se o menu estiver instalado).
- **Preset de compressão**: define o preset utilizado nas próximas compressões (`screen`, `ebook` ou `printer`). Padrão: `ebook`.
- **Status do menu de contexto**: exibe o estado atual (`Menu de contexto: instalado` ou `não instalado`), com botões para **Instalar / Atualizar** e **Remover**.
- **Fechar**: fecha a janela.

As configurações ficam salvas em:
```
%LOCALAPPDATA%\PdfCompressor\settings.json
```
No formato:
```json
{"language":"auto","qualityPreset":"ebook"}
```
Se o arquivo estiver ausente ou inválido, o aplicativo restaura automaticamente os padrões. A pasta inteira é apagada na desinstalação.

---

## Idioma

O aplicativo detecta automaticamente o idioma da interface do Windows ou respeita a seleção feita na janela de configurações:
- **Automático (Windows)**: seleciona **Português** quando o idioma do sistema for `pt-*` (ex.: `pt-BR`, `pt-PT`), e **Inglês** para todos os demais idiomas como padrão (*fallback*).
- **Português (Brasil)**: fixa o idioma em português.
- **English**: fixa o idioma em inglês.

Essa configuração define o idioma dos textos em tempo de execução, incluindo notificações, mensagens de erro e o rótulo do menu de contexto (**Comprimir PDF** ou **Compress PDF**).

> [!NOTE]
> O rótulo do menu do Explorer é gravado no Registro no momento da instalação. Ao trocar o idioma do Windows ou alterar a opção na janela de configurações, o rótulo do menu pode ser atualizado diretamente pela janela (ao trocar o idioma ou clicar no botão **Instalar / Atualizar**). Também é possível regravar via linha de comando:
> ```cmd
> PdfCompressor.exe --install-context-menu
> ```

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
│   │   ├── Assets/app.ico          # Ícone do aplicativo
│   │   ├── Configuration/          # Gerenciamento de configurações (settings.json)
│   │   ├── Localization/           # Recursos e strings localizadas
│   │   └── UI/
│   │       └── SettingsForm.cs     # Janela de configurações
│   ├── PdfCompressor.Core/         # Lógica de compressão e validação
│   └── PdfCompressor.Windows/      # Menu de contexto e notificações (P/Invoke)
│       └── ContextMenu/
│           └── ContextMenuStatus.cs # Verificação de status do menu de contexto
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

O aplicativo permite escolher o nível de compressão através da janela de configurações. O preset padrão é o `ebook`. Os valores disponíveis são:

| Preset    | Qualidade | Redução | Descrição |
|-----------|-----------|---------|-----------|
| `screen`  | Baixa     | Máxima  | Arquivo menor — qualidade baixa, maior compressão |
| `ebook`   | Média     | Boa     | Equilibrado — boa qualidade e compressão balanceada (padrão) |
| `printer` | Alta      | Menor   | Qualidade maior — qualidade de impressão, menor compressão |

O preset selecionado é salvo nas configurações e utilizado nas próximas compressões feitas pelo menu de contexto.

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
