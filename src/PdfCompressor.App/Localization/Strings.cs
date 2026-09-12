using System.Resources;

namespace PdfCompressor.App.Localization;

internal static class Strings
{
    private static readonly ResourceManager ResourceManager =
        new("PdfCompressor.App.Localization.Strings", typeof(Strings).Assembly);

    public static string App_Title => GetString(nameof(App_Title));
    public static string Notification_Success => GetString(nameof(Notification_Success));
    public static string Notification_NotReduced => GetString(nameof(Notification_NotReduced));
    public static string Notification_Error => GetString(nameof(Notification_Error));
    public static string ContextMenu_CompressPdf => GetString(nameof(ContextMenu_CompressPdf));
    public static string Error_UnknownOption => GetString(nameof(Error_UnknownOption));
    public static string Settings_LanguageLabel => GetString(nameof(Settings_LanguageLabel));
    public static string Language_Auto => GetString(nameof(Language_Auto));
    public static string Language_PtBr => GetString(nameof(Language_PtBr));
    public static string Language_En => GetString(nameof(Language_En));
    public static string Settings_QualityLabel => GetString(nameof(Settings_QualityLabel));
    public static string Preset_Screen_Name => GetString(nameof(Preset_Screen_Name));
    public static string Preset_Screen_Description => GetString(nameof(Preset_Screen_Description));
    public static string Preset_Ebook_Name => GetString(nameof(Preset_Ebook_Name));
    public static string Preset_Ebook_Description => GetString(nameof(Preset_Ebook_Description));
    public static string Preset_Printer_Name => GetString(nameof(Preset_Printer_Name));
    public static string Preset_Printer_Description => GetString(nameof(Preset_Printer_Description));
    public static string Settings_ContextMenuInstalled => GetString(nameof(Settings_ContextMenuInstalled));
    public static string Settings_ContextMenuNotInstalled => GetString(nameof(Settings_ContextMenuNotInstalled));
    public static string Settings_InstallOrUpdate => GetString(nameof(Settings_InstallOrUpdate));
    public static string Settings_Remove => GetString(nameof(Settings_Remove));
    public static string Settings_Close => GetString(nameof(Settings_Close));
    public static string Settings_ContextMenuError => GetString(nameof(Settings_ContextMenuError));

    private static string GetString(string name) =>
        ResourceManager.GetString(name) ?? name;
}
