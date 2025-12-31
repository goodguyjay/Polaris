namespace Polaris.Core.Export;

public static class PdfTemplates
{
    public static PdfTemplateConfig Government =>
        new()
        {
            Margins = new MarginsConfig(2.5f, 2.5f, 2.5f, 2.5f), // cm
            FontFamily = "Arial",
            FontSize = 12,
            LineHeight = 1.5f,
        };

    public static PdfTemplateConfig Generic =>
        new()
        {
            Margins = new MarginsConfig(2.0f, 2.0f, 2.0f, 2.0f), // cm
            FontFamily = "Calibri",
            FontSize = 11,
            LineHeight = 1.15f,
        };
}
