using Microsoft.Extensions.Logging;
using Polaris.Core.Document;
using Polaris.Core.Export;
using Polaris.Pdf.Builders;
using Polaris.Pdf.Renderers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Polaris.Pdf;

public sealed class PdfGenerator : IPdfGenerator
{
    private readonly ILogger<PdfGenerator> _logger;
    private readonly ILogger<PdfIrBuilder> _builderLogger;
    private readonly ILogger<PdfRenderer> _rendererLogger;

    public PdfGenerator(
        ILogger<PdfGenerator> logger,
        ILogger<PdfIrBuilder> builderLogger,
        ILogger<PdfRenderer> rendererLogger
    )
    {
        _logger = logger;
        _builderLogger = builderLogger;
        _rendererLogger = rendererLogger;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] Generate(PolarDocument document, PdfOptions options)
    {
        _logger.LogInformation("starting pdf generation...");

        var builder = new PdfIrBuilder(_builderLogger);
        var pdfDocument = builder.Build(document);

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                var template = options.Template;

                // cfg
                page.Size(PageSizes.A4);
                page.Margin(template.Margins.Top, Unit.Centimetre);
                page.MarginLeft(template.Margins.Left, Unit.Centimetre);
                page.MarginRight(template.Margins.Right, Unit.Centimetre);
                page.MarginBottom(template.Margins.Bottom, Unit.Centimetre);

                page.DefaultTextStyle(style =>
                    style
                        .FontFamily(template.FontFamily)
                        .FontSize(template.FontSize)
                        .LineHeight(template.LineHeight)
                        .FontColor(template.TextColor)
                );

                // metadata
                if (options.Title != null)
                    page.Header().Text(options.Title).FontSize(10).Italic();

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });

                // content
                page.Content()
                    .Column(column =>
                    {
                        var renderer = new PdfRenderer(template, _rendererLogger);
                        renderer.Render(pdfDocument, column);
                    });
            });
        });

        var bytes = pdf.GeneratePdf();
        _logger.LogInformation("PDF generated successfully, size: {Size} bytes", bytes.Length);

        return bytes;
    }
}
