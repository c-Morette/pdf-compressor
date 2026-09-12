using System.Globalization;
using System.Threading;

namespace PdfCompressor.App.Localization;

internal static class AppCulture
{
    public static void Apply()
    {
        var currentCulture = CultureInfo.CurrentUICulture;
        var appCulture = currentCulture.TwoLetterISOLanguageName == "pt"
            ? CultureInfo.GetCultureInfo("pt-BR")
            : CultureInfo.GetCultureInfo("en-US");

        Thread.CurrentThread.CurrentUICulture = appCulture;
        CultureInfo.DefaultThreadCurrentUICulture = appCulture;
    }
}
