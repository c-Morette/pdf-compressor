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

    private static string GetString(string name) =>
        ResourceManager.GetString(name) ?? name;
}
