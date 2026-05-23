using System.Diagnostics;
using PdfCompressor.Core.Compression;
using PdfCompressor.Core.Logging;
using PdfCompressor.Core.Validation;
using PdfCompressor.Windows.ContextMenu;
using PdfCompressor.Windows.Notifications;

namespace PdfCompressor.App;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                AppLogger.Info("Nenhum argumento fornecido. Encerrando silenciosamente.");
                return 0;
            }

            var firstArg = args[0];

            switch (firstArg.ToLowerInvariant())
            {
                case "--install-context-menu":
                    return InstallContextMenu();

                case "--uninstall-context-menu":
                    return UninstallContextMenu();

                default:
                    return CompressFile(firstArg);
            }
        }
        catch (Exception ex)
        {
            AppLogger.Error("Erro não tratado.", ex);
            WindowsNotifier.Error();
            return 1;
        }
    }

    private static int InstallContextMenu()
    {
        var executablePath = GetExecutablePath();
        ContextMenuInstaller.Install(executablePath);
        return 0;
    }

    private static int UninstallContextMenu()
    {
        ContextMenuUninstaller.Uninstall();
        return 0;
    }

    private static int CompressFile(string path)
    {
        var validation = PdfValidator.Validate(path);
        if (!validation.IsValid)
        {
            AppLogger.Error($"Validação falhou para '{path}': {validation.ErrorMessage}");
            WindowsNotifier.Error();
            return 1;
        }

        var service = new PdfCompressionService();
        var options = new PdfCompressionOptions { QualityPreset = "ebook" };
        var result = service.Compress(path, options);

        if (!result.Success)
        {
            WindowsNotifier.Error();
            return 1;
        }

        if (string.IsNullOrEmpty(result.OutputPath))
        {
            WindowsNotifier.NotReduced();
            return 0;
        }

        WindowsNotifier.Success(result.ReductionPercent);
        return 0;
    }

    private static string GetExecutablePath()
    {
        return Process.GetCurrentProcess().MainModule?.FileName
            ?? throw new InvalidOperationException("Não foi possível determinar o caminho do executável.");
    }
}
