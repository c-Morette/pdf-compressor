namespace PdfCompressor.Core.Compression;

public sealed class CompressionResult
{
    public bool Success { get; set; }
    public string InputPath { get; set; } = string.Empty;
    public string OutputPath { get; set; } = string.Empty;
    public long OriginalSizeBytes { get; set; }
    public long CompressedSizeBytes { get; set; }
    public double ReductionPercent { get; set; }
    public string? ErrorMessage { get; set; }
}
