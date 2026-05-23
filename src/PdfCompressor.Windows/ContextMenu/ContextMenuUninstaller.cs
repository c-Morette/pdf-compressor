using Microsoft.Win32;
using PdfCompressor.Core.Logging;

namespace PdfCompressor.Windows.ContextMenu;

public static class ContextMenuUninstaller
{
    public static void Uninstall()
    {
        // Remove a chave CompressPdf (e a subchave 'command') por completo.
        const string parentPath = @"Software\Classes\SystemFileAssociations\.pdf\shell";

        using var parentKey = Registry.CurrentUser.OpenSubKey(parentPath, writable: true);
        if (parentKey is null)
        {
            AppLogger.Info("Menu de contexto não estava instalado. Nada a remover.");
            return;
        }

        try
        {
            parentKey.DeleteSubKeyTree("CompressPdf", throwOnMissingSubKey: false);
            AppLogger.Info("Menu de contexto removido.");
        }
        catch (Exception ex)
        {
            AppLogger.Error("Falha ao remover o menu de contexto.", ex);
            throw;
        }
    }
}
