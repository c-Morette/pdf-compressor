using System.Diagnostics;
using System.Globalization;
using PdfCompressor.Core.Compression;
using PdfCompressor.Core.Logging;
using PdfCompressor.Core.Validation;
using PdfCompressor.App.Localization;
using PdfCompressor.Windows.ContextMenu;
using PdfCompressor.Windows.Notifications;

namespace PdfCompressor.App;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        AppCulture.Apply();

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
                    if (firstArg.StartsWith("--", StringComparison.Ordinal))
                        return HandleUnknownOption(firstArg);

                    return CompressFile(firstArg);
            }
        }
        catch (Exception ex)
        {
            AppLogger.Error("Erro não tratado.", ex);
            WindowsNotifier.ShowError(Strings.App_Title, Strings.Notification_Error);
            return 1;
        }
    }

    private static int InstallContextMenu()
    {
        var executablePath = GetExecutablePath();
        ContextMenuInstaller.Install(executablePath, Strings.ContextMenu_CompressPdf);
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
            WindowsNotifier.ShowError(Strings.App_Title, Strings.Notification_Error);
            return 1;
        }

        var service = new PdfCompressionService();
        var options = new PdfCompressionOptions { QualityPreset = "ebook" };
        var result = service.Compress(path, options);

        if (!result.Success)
        {
            WindowsNotifier.ShowError(Strings.App_Title, Strings.Notification_Error);
            return 1;
        }

        if (string.IsNullOrEmpty(result.OutputPath))
        {
            WindowsNotifier.ShowInformation(Strings.App_Title, Strings.Notification_NotReduced);
            return 0;
        }

        var reductionPercent = result.ReductionPercent.ToString("0.##", CultureInfo.CurrentUICulture);
        var message = string.Format(
            CultureInfo.CurrentUICulture,
            Strings.Notification_Success,
            reductionPercent);
        WindowsNotifier.ShowInformation(Strings.App_Title, message);
        return 0;
    }

    private static int HandleUnknownOption(string option)
    {
        AppLogger.Error($"Opção desconhecida: '{option}'.");
        var message = string.Format(CultureInfo.CurrentUICulture, Strings.Error_UnknownOption, option);
        WindowsNotifier.ShowError(Strings.App_Title, message);
        return 1;
    }

    private static string GetExecutablePath()
    {
        return Process.GetCurrentProcess().MainModule?.FileName
            ?? throw new InvalidOperationException("Não foi possível determinar o caminho do executável.");
    }
}
