using Microsoft.Extensions.Logging;
using Polaris.Core.Document.Elements;
using Polaris.Core.Export;
using Polaris.Pdf.Model;
using QuestPDF.Fluent;

namespace Polaris.Pdf.Renderers;

/// <summary>
/// renders pdf intermediate representation to questpdf
/// applies template styling and handles layout
/// </summary>
public sealed class PdfRenderer(PdfTemplateConfig template, ILogger<PdfRenderer> logger)
{
    public void Render(PdfDocument document, ColumnDescriptor column)
    {
        logger.LogInformation("rendering pdf ir to pdf...");

        foreach (var block in document.Blocks)
        {
            RenderBlock(block, column);
        }
    }

    private void RenderBlock(PdfBlock block, ColumnDescriptor column)
    {
        switch (block)
        {
            case PdfHeading heading:
                RenderHeading(heading, column);
                break;

            case PdfParagraph paragraph:
                RenderParagraph(paragraph, column);
                break;

            case PdfCodeBlock codeBlock:
                RenderCodeBlock(codeBlock, column);
                break;

            case PdfList list:
                RenderList(list, column);
                break;

            case PdfHorizontalRule:
                RenderHorizontalRule(column);
                break;

            case PdfBlank blank:
                RenderBlank(blank, column);
                break;

            default:
                logger.LogWarning(
                    "unknown block type {BlockType}, skipping...",
                    block.GetType().Name
                );
                break;
        }
    }

    private void RenderHeading(PdfHeading heading, ColumnDescriptor column)
    {
        var sizeMultiplier = heading.Level switch
        {
            1 => template.Heading1Size,
            2 => template.Heading2Size,
            3 => template.Heading3Size,
            4 => template.Heading4Size,
            5 => template.Heading5Size,
            6 => template.Heading6Size,
            _ => 1.0f,
        };

        var fontSize = template.FontSize * sizeMultiplier;

        column
            .Item()
            .PaddingBottom(8)
            .Text(text =>
            {
                text.DefaultTextStyle(style => style.FontSize(fontSize).Bold());
                RenderInlines(heading.Elements, text);
            });
    }

    private void RenderParagraph(PdfParagraph paragraph, ColumnDescriptor column)
    {
        column
            .Item()
            .PaddingBottom(8)
            .Text(text =>
            {
                RenderInlines(paragraph.Elements, text);
            });
    }

    private void RenderCodeBlock(PdfCodeBlock codeBlock, ColumnDescriptor column)
    {
        column
            .Item()
            .PaddingBottom(8)
            .Background("#F5F5F5")
            .Padding(10)
            .Text(codeBlock.Code)
            .FontFamily("Courier New")
            .FontSize(template.FontSize * 0.9f);
    }

    private void RenderList(PdfList list, ColumnDescriptor column)
    {
        column
            .Item()
            .PaddingBottom(8)
            .Column(listColumn =>
            {
                for (var i = 0; i < list.Items.Count; i++)
                {
                    var item = list.Items[i];
                    var marker = list.Type == ListType.Bullet ? "•" : $"{i + 1}.";

                    listColumn
                        .Item()
                        .Row(row =>
                        {
                            // marker column (fixed width)
                            row.ConstantItem(30).Text(marker).FontSize(template.FontSize);

                            // content column (flexible)
                            row.RelativeItem()
                                .Text(text =>
                                {
                                    RenderInlines(item.Elements, text);
                                });
                        });
                }
            });
    }

    private void RenderHorizontalRule(ColumnDescriptor column)
    {
        column.Item().PaddingVertical(8).LineHorizontal(1).LineColor("#CCCCCC");
    }

    private void RenderBlank(PdfBlank blank, ColumnDescriptor column)
    {
        var height = template.FontSize * template.LineHeight * blank.Count;
        column.Item().Height(height);
    }

    private void RenderInlines(List<PdfInlineElement> elements, TextDescriptor text)
    {
        foreach (var element in elements)
        {
            switch (element)
            {
                case PdfTextSpan span:
                    RenderTextSpan(span, text);
                    break;

                case PdfLineBreak:
                    text.Line(string.Empty);
                    break;

                case PdfImage image:
                    RenderImage(image, text);
                    break;

                default:
                    logger.LogWarning(
                        "unknown inline element type: {Type}",
                        element.GetType().Name
                    );
                    break;
            }
        }
    }

    private void RenderTextSpan(PdfTextSpan span, TextDescriptor text)
    {
        var style = span.Style;

        if (style.HyperlinkUrl != null)
        {
            var textSpan = text.Hyperlink(span.Text, style.HyperlinkUrl);
            ApplySpanStyle(textSpan, style);
        }
        else
        {
            var textSpan = text.Span(span.Text);
            ApplySpanStyle(textSpan, style);
        }
    }

    private void ApplySpanStyle(TextSpanDescriptor textSpan, PdfTextStyle style)
    {
        if (style.Bold)
            textSpan.Bold();

        if (style.Italic)
            textSpan.Italic();

        if (style.FontFamily != null)
            textSpan.FontFamily(style.FontFamily);

        if (style.FontSizeMultiplier.HasValue)
            textSpan.FontSize(template.FontSize * style.FontSizeMultiplier.Value);

        if (style.Color != null)
            textSpan.FontColor(style.Color);

        if (style.BackgroundColor != null)
            textSpan.BackgroundColor(style.BackgroundColor);

        if (style.Underline)
            textSpan.Underline();
    }

    private void RenderImage(PdfImage image, TextDescriptor text)
    {
        // for now, just renders placeholder text
        // proper image handling requires questpdf's Image() method which doesn't work inline
        text.Span($"[Image: {image.Alt}]").Italic().FontColor("#888888");

        logger.LogWarning("image rendering not yet fully implemented: {Alt}", image.Alt);
    }
}
