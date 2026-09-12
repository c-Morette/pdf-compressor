using System.Globalization;
using System.Threading;
using PdfCompressor.App.Configuration;

namespace PdfCompressor.App.Localization;

internal static class AppCulture
{
    // Cultura do Windows capturada antes de qualquer override, para "auto" continuar correto após trocas.
    private static readonly CultureInfo WindowsCulture = CultureInfo.CurrentUICulture;

    public static void Apply(string language)
    {
        var appCulture = language switch
        {
            AppSettings.LanguagePtBr => CultureInfo.GetCultureInfo("pt-BR"),
            AppSettings.LanguageEn => CultureInfo.GetCultureInfo("en-US"),
            _ => WindowsCulture.TwoLetterISOLanguageName == "pt"
                ? CultureInfo.GetCultureInfo("pt-BR")
                : CultureInfo.GetCultureInfo("en-US"),
        };

        Thread.CurrentThread.CurrentUICulture = appCulture;
        CultureInfo.DefaultThreadCurrentUICulture = appCulture;
    }
}
