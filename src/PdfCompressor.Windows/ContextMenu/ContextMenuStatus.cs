using Microsoft.Win32;
using PdfCompressor.Core.Logging;

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

        try
        {
            using var commandKey = Registry.CurrentUser.OpenSubKey(ContextMenuInstaller.ShellKeyPath + @"\command");
            if (commandKey?.GetValue(null) is not string command || string.IsNullOrWhiteSpace(command))
                return false;

            var expected = $"\"{executablePath}\" \"%1\"";
            return string.Equals(command, expected, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            // Registro inacessível/corrompido: tratar como não instalado em vez de derrubar a janela.
            AppLogger.Error("Falha ao consultar o menu de contexto no Registro.", ex);
            return false;
        }
    }
}
