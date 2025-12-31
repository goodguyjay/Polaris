namespace Polaris.Core.Export;

public record PdfTemplateConfig
{
    public required MarginsConfig Margins { get; init; }
    public required string FontFamily { get; init; }
    public required float FontSize { get; init; }
    public required float LineHeight { get; init; }

    // colors in hex
    public string TextColor { get; init; } = "#000000";
    public string BackgroundColor { get; init; } = "#FFFFFF";

    // heading sizes
    public float Heading1Size { get; init; } = 2.0f;
    public float Heading2Size { get; init; } = 1.5f;
    public float Heading3Size { get; init; } = 1.17f;
    public float Heading4Size { get; init; } = 1.0f;
    public float Heading5Size { get; init; } = 0.83f;
    public float Heading6Size { get; init; } = 0.67f;
}

public record MarginsConfig(float Top, float Right, float Bottom, float Left);
