using Microsoft.Extensions.Logging;
using Polaris.Core.Document;
using Polaris.Core.Document.Elements;
using Polaris.Core.Document.InlineElements;
using Polaris.Core.Export;
using Polaris.Pdf.Model;

namespace Polaris.Pdf.Builders;

/// <summary>
/// transforms polar document ast into pdf intermediate representation
/// flattens nested inline formatting and accumulates styles
/// </summary>
public sealed class PdfIrBuilder(ILogger<PdfIrBuilder> logger)
{
    public PdfDocument Build(PolarDocument document)
    {
        logger.LogInformation("building pdf ir from polar document...");

        var blocks = document.Blocks.Select(TransformBlock).ToList();

        return new PdfDocument { Blocks = blocks };
    }

    private PdfBlock TransformBlock(BlockElement block)
    {
        return block switch
        {
            Heading h => new PdfHeading(h.Level, FlattenInlines(h.Inlines, PdfTextStyle.Default)),
            Paragraph p => new PdfParagraph(FlattenInlines(p.Inlines, PdfTextStyle.Default)),
            CodeBlock cb => new PdfCodeBlock(cb.Language, cb.Code),
            ListBlock lb => new PdfList(
                lb.Type,
                lb.Items.Select(item => new PdfListItem(
                        FlattenInlines(item.Inlines, PdfTextStyle.Default)
                    ))
                    .ToList()
            ),
            HorizontalRule => new PdfHorizontalRule(),
            Blank b => new PdfBlank(b.Count),
            _ => throw new NotImplementedException(
                $"block type {block.GetType().Name} not implemented."
            ),
        };
    }

    /// <summary>
    /// flatten nested inline elements into a list of styled spans
    /// accumulates styles from parent formatting elements
    /// </summary>
    private List<PdfInlineElement> FlattenInlines(
        IReadOnlyList<InlineElement> inlines,
        PdfTextStyle currentStyle
    )
    {
        var result = new List<PdfInlineElement>();

        foreach (var inlineElement in inlines)
        {
            switch (inlineElement)
            {
                case TextRun tr:
                    result.Add(new PdfTextSpan(tr.Text, currentStyle));
                    break;

                case Strong s:
                    var boldStyle = currentStyle.With(bold: true);
                    result.AddRange(FlattenInlines(s.Children, boldStyle));
                    break;

                case Emphasis e:
                    var italicStyle = currentStyle.With(italic: true);
                    result.AddRange(FlattenInlines(e.Children, italicStyle));
                    break;

                case InlineCode ic:
                    var codeStyle = currentStyle.With(
                        fontFamily: "Courier New",
                        backgroundColor: "#F5F5F5",
                        fontSizeMultiplier: 0.9f
                    );
                    result.Add(new PdfTextSpan(ic.Code, codeStyle));
                    break;

                case Link l:
                    var linkStyle = currentStyle.With(
                        hyperlinkUrl: l.Href,
                        color: "#0000EE",
                        underline: true
                    );
                    result.AddRange(FlattenInlines(l.Children, linkStyle));
                    break;

                case Image img:
                    result.Add(new PdfImage(img.Src, img.Alt, img.Title));
                    break;

                case LineBreak:
                    result.Add(new PdfLineBreak());
                    break;

                default:
                    logger.LogWarning(
                        "unknown inline element type: {Type}",
                        inlineElement.GetType().Name
                    );
                    break;
            }
        }

        return result;
    }
}
