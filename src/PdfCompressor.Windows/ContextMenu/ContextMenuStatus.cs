using Microsoft.Win32;

namespace PdfCompressor.Windows.ContextMenu;

public static class ContextMenuStatus
{
    /// <summary>
    /// Instalado somente quando a chave existe E o comando registrado aponta para o executável atual.
    /// </summary>
    public static bool IsInstalled(string executablePath)
    {
        if (string.IsNullOrWhiteSpace(executablePath))
            return false;

        using var commandKey = Registry.CurrentUser.OpenSubKey(ContextMenuInstaller.ShellKeyPath + @"\command");
        if (commandKey?.GetValue(null) is not string command || string.IsNullOrWhiteSpace(command))
            return false;

        var expected = $"\"{executablePath}\" \"%1\"";
        return string.Equals(command, expected, StringComparison.OrdinalIgnoreCase);
    }
}
