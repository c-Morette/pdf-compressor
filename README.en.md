# PDF Compressor

[🇧🇷 Português](README.md) | 🇺🇸 English

Lightweight utility to **compress PDF files directly from the Windows Explorer context menu**, without opening any windows. Select a PDF, right-click and choose **Compress PDF** — the compressed file appears in the same folder in seconds.

![Windows 7 SP1+](https://img.shields.io/badge/Windows-7%20SP1%20%7C%2010%20%7C%2011-0078D4?logo=windows)
![.NET Framework 4.7.2](https://img.shields.io/badge/.NET%20Framework-4.7.2-512BD4?logo=dotnet)
![Ghostscript 10.07.1](https://img.shields.io/badge/Ghostscript-10.07.1-green)
![License MIT](https://img.shields.io/badge/License-MIT-yellow)

---

## Features

- Adds the **Compress PDF** option to the context menu for `.pdf` files
- Compresses in the background, **with no visible window or console**
- Saves the result in the **same folder** with the `_compressed` suffix
- **Never overwrites** the original file
- Discards the generated file if it is not smaller than the original
- Discreet notification upon completion: success with % reduction, no reduction, or error
- Ghostscript **already bundled** in the installer — zero external dependencies

### Example

| Original file              | Generated file                          |
|----------------------------|-----------------------------------------|
| `C:\Docs\Contract.pdf`     | `C:\Docs\Contract_compressed.pdf`       |

If `Contract_compressed.pdf` already exists, the app generates `Contract_compressed_1.pdf`, `Contract_compressed_2.pdf`, and so on.

---

## Installation

1. Download the installer from the [Releases](https://github.com/c-Morette/pdf-compressor/releases/latest) page: `PDFCompress-Setup-x.x.x.exe`
2. Run the installer (requires administrator privileges to install in `Program Files`)
3. Done — the context menu will now be available in Explorer

**System Requirements:**
- Windows 7 SP1 x64, Windows 10 x64, or Windows 11 x64
- [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472) or higher
  - Pre-installed on Windows 10/11. For Windows 7 SP1, install via Windows Update or the link above.

**Uninstall:** Control Panel → Programs → PDF Compressor → Uninstall.
The context menu is removed automatically.

---

## How to use

1. Right-click any `.pdf` file in Explorer
2. Choose **Compress PDF**
3. Wait for the notification with the result

Logs are stored at:
```
%LOCALAPPDATA%\PdfCompressor\logs\app.log
```

---

## Language

The application automatically detects the Windows UI language:
- **Portuguese**: used when the system language is `pt-*` (e.g., `pt-BR`, `pt-PT`).
- **English**: used for all other languages as the default fallback.

This setting controls runtime texts, including system notifications, error messages, and the context menu label (**Compress PDF** or **Comprimir PDF**).

> [!NOTE]
> **Context menu limitation:** The Explorer context menu label is written to the Windows Registry at the time of installation. Changing the Windows display language afterwards does not automatically update an existing entry. To re-register the menu in the current system language, run:
> ```cmd
> PdfCompressor.exe --install-context-menu
> ```

---

## Building the project

### Prerequisites

- [.NET SDK 6+](https://dotnet.microsoft.com/download) (any recent version)
- [Inno Setup 6](https://jrsoftware.org/isdl.php) (to generate the installer)
- Ghostscript extracted to `vendor\ghostscript\` (see below)

### 1. Clone and build

```powershell
git clone https://github.com/c-Morette/pdf-compressor.git
cd pdf-compressor

# Publish app binaries
.\build\publish-single.ps1
# Output: publish\net472\
```

### 2. Prepare Ghostscript

Ghostscript is not versioned in the repository (third-party binaries, ~60 MB).
A setup script downloads and installs it automatically:

```powershell
.\scripts\setup-vendor.ps1
```

The script downloads Ghostscript 10.07.1 from GitHub (Artifex Software) and extracts it to
`vendor\ghostscript\`. It only needs to be run once — subsequent runs will detect
the installation and do nothing.

<details>
<summary>Manual installation (without script)</summary>

Download the Windows x64 installer at [ghostscript.com/releases](https://www.ghostscript.com/releases/gsdnld.html) and run:

```powershell
.\gs10071w64.exe /S /D=C:\path\to\repo\vendor\ghostscript
```

Expected directory structure in `vendor\ghostscript\`:
```
bin\gswin64c.exe
bin\gsdll64.dll
lib\
Resource\
iccprofiles\
```
</details>

### 3. Generate installer

Open `installer\PdfCompressor.iss` in Inno Setup Compiler and click **Build**, or via command line:

```powershell
& "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe" installer\PdfCompressor.iss
# Output: installer\output\PDFCompress-Setup-1.0.0.exe
```

---

## Project structure

```
pdf-compressor/
├── src/
│   ├── PdfCompressor.App/          # Entry point (WinExe, .NET FX 4.7.2)
│   │   ├── Assets/app.ico          # Application icon
│   │   └── Localization/           # Localized resources and strings
│   ├── PdfCompressor.Core/         # Compression and validation logic
│   └── PdfCompressor.Windows/      # Context menu and notifications (P/Invoke)
├── build/
│   ├── publish-single.ps1          # Publish script (PowerShell)
│   └── publish-single.cmd          # Publish script (CMD)
├── installer/
│   └── PdfCompressor.iss           # Inno Setup script
└── vendor/
    └── ghostscript/                # Extracted Ghostscript (not versioned)
```

---

## Compression presets

The app uses Ghostscript's `ebook` preset. Other values supported internally:

| Preset    | Quality   | Reduction |
|-----------|-----------|-----------|
| `screen`  | Low       | Maximum   |
| `ebook`   | Medium    | Good (default) |
| `printer` | High      | Lower     |

---

## License

This project's source code is licensed under the **MIT** license — see the [LICENSE](LICENSE) file.

The installer bundles **[Ghostscript](https://www.ghostscript.com/)**, distributed under the
[GNU AGPL v3](https://www.gnu.org/licenses/agpl-3.0.html) by Artifex Software.
Ghostscript source code is available at
[github.com/ArtifexSoftware/ghostpdl](https://github.com/ArtifexSoftware/ghostpdl).

---

## Contributing

Contributions are welcome! To report bugs or suggest enhancements, open an
[issue](https://github.com/c-Morette/pdf-compressor/issues). To submit code, fork the repository, create a branch, and open a Pull Request.
