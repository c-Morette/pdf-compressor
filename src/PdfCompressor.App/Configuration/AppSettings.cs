namespace PdfCompressor.App.Configuration;

internal sealed class AppSettings
{
    public const string LanguageAuto = "auto";
    public const string LanguagePtBr = "pt-BR";
    public const string LanguageEn = "en";

    public const string PresetScreen = "screen";
    public const string PresetEbook = "ebook";
    public const string PresetPrinter = "printer";

    public static readonly string[] AllowedLanguages = { LanguageAuto, LanguagePtBr, LanguageEn };
    public static readonly string[] AllowedPresets = { PresetScreen, PresetEbook, PresetPrinter };

    public string Language { get; set; } = LanguageAuto;
    public string QualityPreset { get; set; } = PresetEbook;

    public static bool IsValidLanguage(string? value) =>
        value is not null && Array.IndexOf(AllowedLanguages, value) >= 0;

    public static bool IsValidPreset(string? value) =>
        value is not null && Array.IndexOf(AllowedPresets, value) >= 0;
}
