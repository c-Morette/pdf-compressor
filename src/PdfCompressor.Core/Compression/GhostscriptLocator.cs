namespace PdfCompressor.Core.Compression;

internal static class GhostscriptLocator
{
    private static readonly string[] ExecutableNames = { "gswin64c.exe", "gswin32c.exe", "gs.exe" };

    public static string? Find()
    {
        return FindBundled() ?? FindOnPath() ?? FindInProgramFiles();
    }

    // Procura o Ghostscript na subpasta "gs\bin" relativa ao executável (bundled pelo instalador).
    private static string? FindBundled()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var binDir = Path.Combine(baseDir, "gs", "bin");

        foreach (var name in ExecutableNames)
        {
            var candidate = Path.Combine(binDir, name);
            if (File.Exists(candidate))
                return candidate;
        }

        return null;
    }

    private static string? FindOnPath()
    {
        var pathVariable = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(pathVariable))
            return null;

        foreach (var directory in pathVariable.Split(Path.PathSeparator))
        {
            if (string.IsNullOrWhiteSpace(directory))
                continue;

            foreach (var name in ExecutableNames)
            {
                try
                {
                    var candidate = Path.Combine(directory, name);
                    if (File.Exists(candidate))
                        return candidate;
                }
                catch
                {
                    // Ignorar entradas de PATH inválidas.
                }
            }
        }

        return null;
    }

    private static string? FindInProgramFiles()
    {
        var roots = new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
        };

        foreach (var root in roots)
        {
            if (string.IsNullOrEmpty(root))
                continue;

            var gsRoot = Path.Combine(root, "gs");
            if (!Directory.Exists(gsRoot))
                continue;

            // Procurar a versão mais recente (ordenação descendente por nome de pasta).
            foreach (var versionDir in Directory.GetDirectories(gsRoot).OrderByDescending(d => d))
            {
                var binDir = Path.Combine(versionDir, "bin");
                foreach (var name in ExecutableNames)
                {
                    var candidate = Path.Combine(binDir, name);
                    if (File.Exists(candidate))
                        return candidate;
                }
            }
        }

        return null;
    }
}
