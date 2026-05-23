using Microsoft.Win32;
using PdfCompressor.Core.Logging;

namespace PdfCompressor.Windows.ContextMenu;

public static class ContextMenuInstaller
{
    // HKEY_CURRENT_USER não exige privilégios de administrador.
    internal const string ShellKeyPath =
        @"Software\Classes\SystemFileAssociations\.pdf\shell\CompressPdf";

    private const string MenuText = "Comprimir PDF";

    public static void Install(string executablePath)
    {
        if (string.IsNullOrWhiteSpace(executablePath) || !File.Exists(executablePath))
            throw new FileNotFoundException("Executável não encontrado para registrar o menu.", executablePath);

        using var shellKey = Registry.CurrentUser.CreateSubKey(ShellKeyPath, writable: true)
            ?? throw new InvalidOperationException("Não foi possível criar a chave de registro.");

        shellKey.SetValue(null, MenuText, RegistryValueKind.String);
        shellKey.SetValue("Icon", $"\"{executablePath}\"", RegistryValueKind.String);

        using var commandKey = shellKey.CreateSubKey("command", writable: true)
            ?? throw new InvalidOperationException("Não foi possível criar a chave 'command'.");

        var command = $"\"{executablePath}\" \"%1\"";
        commandKey.SetValue(null, command, RegistryValueKind.String);

        AppLogger.Info($"Menu de contexto instalado. Comando: {command}");
    }
}
