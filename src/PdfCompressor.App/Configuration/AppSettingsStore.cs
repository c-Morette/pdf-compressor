using System.Text;
using System.Web.Script.Serialization;
using PdfCompressor.Core.Logging;

namespace PdfCompressor.App.Configuration;

internal static class AppSettingsStore
{
    // Mesma pasta usada pelo AppLogger (%LOCALAPPDATA%\PdfCompressor).
    private static string SettingsDirectory =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PdfCompressor");

    private static string SettingsFile => Path.Combine(SettingsDirectory, "settings.json");

    public static AppSettings Load()
    {
        var settings = new AppSettings();

        if (!File.Exists(SettingsFile))
            return settings;

        try
        {
            var json = File.ReadAllText(SettingsFile, Encoding.UTF8);
            var data = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(json);

            if (data is null)
            {
                AppLogger.Error("settings.json vazio ou inválido. Usando padrões.");
                return settings;
            }

            if (data.TryGetValue("language", out var language) && AppSettings.IsValidLanguage(language as string))
                settings.Language = (string)language;
            else
                AppLogger.Error("settings.json: 'language' ausente ou inválido. Usando 'auto'.");

            if (data.TryGetValue("qualityPreset", out var preset) && AppSettings.IsValidPreset(preset as string))
                settings.QualityPreset = (string)preset;
            else
                AppLogger.Error("settings.json: 'qualityPreset' ausente ou inválido. Usando 'ebook'.");
        }
        catch (Exception ex)
        {
            AppLogger.Error("Falha ao ler settings.json. Usando padrões.", ex);
        }

        return settings;
    }

    public static void Save(AppSettings settings)
    {
        var data = new Dictionary<string, object>
        {
            ["language"] = settings.Language,
            ["qualityPreset"] = settings.QualityPreset,
        };
        var json = new JavaScriptSerializer().Serialize(data);

        Directory.CreateDirectory(SettingsDirectory);

        // Escrita atômica: grava em temporário e substitui o destino.
        var tempFile = SettingsFile + ".tmp";
        File.WriteAllText(tempFile, json, Encoding.UTF8);

        if (File.Exists(SettingsFile))
            File.Replace(tempFile, SettingsFile, null);
        else
            File.Move(tempFile, SettingsFile);

        AppLogger.Info($"Configurações salvas: {json}");
    }
}
