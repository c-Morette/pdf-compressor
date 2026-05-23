using System.Diagnostics;
using PdfCompressor.Core.Logging;

namespace PdfCompressor.Core.Compression;

public sealed class PdfCompressionService
{
    private static readonly string[] AllowedPresets = { "screen", "ebook", "printer" };

    public CompressionResult Compress(string inputPath, PdfCompressionOptions options)
    {
        var result = new CompressionResult { InputPath = inputPath };

        try
        {
            var originalSize = new FileInfo(inputPath).Length;
            result.OriginalSizeBytes = originalSize;

            var ghostscript = GhostscriptLocator.Find();
            if (ghostscript is null)
            {
                result.Success = false;
                result.ErrorMessage = "Ghostscript não foi encontrado. Instale o Ghostscript para comprimir PDFs.";
                AppLogger.Error(result.ErrorMessage);
                return result;
            }

            var outputPath = BuildOutputPath(inputPath);
            var preset = NormalizePreset(options.QualityPreset);

            var exitCode = RunGhostscript(ghostscript, preset, inputPath, outputPath);
            if (exitCode != 0 || !File.Exists(outputPath))
            {
                SafeDelete(outputPath);
                result.Success = false;
                result.ErrorMessage = $"Ghostscript terminou com código {exitCode}.";
                AppLogger.Error(result.ErrorMessage);
                return result;
            }

            var compressedSize = new FileInfo(outputPath).Length;
            result.CompressedSizeBytes = compressedSize;

            if (compressedSize >= originalSize)
            {
                // O arquivo gerado não é menor: descartar e não manter nada.
                SafeDelete(outputPath);
                result.Success = true;
                result.OutputPath = string.Empty;
                result.ReductionPercent = 0;
                AppLogger.Info($"Sem redução para '{inputPath}'. Arquivo gerado descartado.");
                return result;
            }

            result.Success = true;
            result.OutputPath = outputPath;
            result.ReductionPercent = Math.Round(
                (1.0 - (double)compressedSize / originalSize) * 100.0, 2);

            AppLogger.Info(
                $"Comprimido '{inputPath}' -> '{outputPath}' | " +
                $"original={originalSize} bytes, comprimido={compressedSize} bytes, " +
                $"redução={result.ReductionPercent}%");

            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            AppLogger.Error($"Falha ao comprimir '{inputPath}'.", ex);
            return result;
        }
    }

    private static string NormalizePreset(string? preset)
    {
        if (!string.IsNullOrWhiteSpace(preset))
        {
            var lowered = preset.Trim().ToLowerInvariant();
            if (Array.IndexOf(AllowedPresets, lowered) >= 0)
                return lowered;
        }
        return "ebook";
    }

    private static int RunGhostscript(string ghostscript, string preset, string inputPath, string outputPath)
    {
        var psi = new ProcessStartInfo
        {
            FileName = ghostscript,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            Arguments = BuildArguments(preset, inputPath, outputPath),
        };

        using var process = Process.Start(psi);
        if (process is null)
            return -1;

        process.StandardOutput.ReadToEnd();
        process.StandardError.ReadToEnd();
        process.WaitForExit();
        return process.ExitCode;
    }

    private static string BuildArguments(string preset, string inputPath, string outputPath)
    {
        var args = new[]
        {
            "-sDEVICE=pdfwrite",
            "-dCompatibilityLevel=1.4",
            $"-dPDFSETTINGS=/{preset}",
            "-dNOPAUSE",
            "-dQUIET",
            "-dBATCH",
            $"-sOutputFile={outputPath}",
            inputPath,
        };
        return string.Join(" ", Array.ConvertAll(args, QuoteIfNeeded));
    }

    private static string QuoteIfNeeded(string arg)
    {
        if (!arg.Contains(' ') && !arg.Contains('"'))
            return arg;
        return "\"" + arg.Replace("\"", "\\\"") + "\"";
    }

    public static string BuildOutputPath(string inputPath)
    {
        var directory = Path.GetDirectoryName(inputPath) ?? string.Empty;
        var stem = Path.GetFileNameWithoutExtension(inputPath);
        var extension = Path.GetExtension(inputPath);

        var candidate = Path.Combine(directory, $"{stem}_compressed{extension}");
        if (!File.Exists(candidate))
            return candidate;

        for (var index = 1; ; index++)
        {
            candidate = Path.Combine(directory, $"{stem}_compressed_{index}{extension}");
            if (!File.Exists(candidate))
                return candidate;
        }
    }

    private static void SafeDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // Ignorar falhas de limpeza.
        }
    }
}
