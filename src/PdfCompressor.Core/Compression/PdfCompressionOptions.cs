namespace PdfCompressor.Core.Compression;

public sealed class PdfCompressionOptions
{
    public string QualityPreset { get; set; } = "ebook";
    public bool Overwrite { get; set; } = false;
}
