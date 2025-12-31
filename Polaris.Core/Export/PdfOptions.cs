namespace Polaris.Core.Export;

public record PdfOptions
{
    public required PdfTemplateConfig Template { get; init; }
    public string? Title { get; init; }
    public string? Author { get; init; }
}
