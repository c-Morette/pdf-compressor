namespace PdfCompressor.Core.Validation;

public sealed class ValidationResult
{
    public bool IsValid { get; init; }
    public string? ErrorMessage { get; init; }

    public static ValidationResult Ok() => new() { IsValid = true };
    public static ValidationResult Fail(string message) => new() { IsValid = false, ErrorMessage = message };
}

public static class PdfValidator
{
    public static ValidationResult Validate(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return ValidationResult.Fail("O caminho do arquivo está vazio.");

        if (Directory.Exists(path))
            return ValidationResult.Fail("O caminho aponta para uma pasta, não um arquivo.");

        if (!File.Exists(path))
            return ValidationResult.Fail("O arquivo não existe.");

        if (!string.Equals(Path.GetExtension(path), ".pdf", StringComparison.OrdinalIgnoreCase))
            return ValidationResult.Fail("O arquivo não é um PDF.");

        try
        {
            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        catch (Exception ex)
        {
            return ValidationResult.Fail($"O arquivo não está acessível para leitura: {ex.Message}");
        }

        return ValidationResult.Ok();
    }
}
