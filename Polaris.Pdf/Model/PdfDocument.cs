using Polaris.Core.Document.Elements;

namespace Polaris.Pdf.Model;

/// <summary>
/// intermediate representation of a document optimized for PDF rendering
/// </summary>
public sealed class PdfDocument
{
    public List<PdfBlock> Blocks { get; init; } = [];
}

// ========================================
// block-level elements
// ========================================

public abstract record PdfBlock;

public sealed record PdfHeading(int Level, List<PdfInlineElement> Elements) : PdfBlock;

public sealed record PdfParagraph(List<PdfInlineElement> Elements) : PdfBlock;

public sealed record PdfCodeBlock(string? Language, string Code) : PdfBlock;

public sealed record PdfList(ListType Type, List<PdfListItem> Items) : PdfBlock;

public sealed record PdfListItem(List<PdfInlineElement> Elements) : PdfBlock;

public sealed record PdfHorizontalRule : PdfBlock;

public sealed record PdfBlank(int Count) : PdfBlock;

// ========================================
// inline elements - flattened with styles
// ========================================

public abstract record PdfInlineElement;

/// <summary>
/// text span with accumulated styles from all parent formatting elements
/// </summary>
public sealed record PdfTextSpan(string Text, PdfTextStyle Style) : PdfInlineElement;

public sealed record PdfLineBreak : PdfInlineElement;

public sealed record PdfImage(string Src, string Alt, string? Title) : PdfInlineElement;

// ========================================
// style information
// ========================================

public sealed record PdfTextStyle
{
    public bool Bold { get; init; }
    public bool Italic { get; init; }
    public string? FontFamily { get; init; }
    public float? FontSizeMultiplier { get; init; }
    public string? Color { get; init; }
    public string? BackgroundColor { get; init; }
    public string? HyperlinkUrl { get; init; }
    public bool Underline { get; init; }

    public static readonly PdfTextStyle Default = new();

    /// <summary>
    /// create a new style with modifications applied
    /// </summary>
    /// <returns></returns>
    public PdfTextStyle With(
        bool? bold = null,
        bool? italic = null,
        string? fontFamily = null,
        float? fontSizeMultiplier = null,
        string? color = null,
        string? backgroundColor = null,
        string? hyperlinkUrl = null,
        bool? underline = null
    )
    {
        return new PdfTextStyle
        {
            Bold = bold ?? Bold,
            Italic = italic ?? Italic,
            FontFamily = fontFamily ?? FontFamily,
            FontSizeMultiplier = fontSizeMultiplier ?? FontSizeMultiplier,
            Color = color ?? Color,
            BackgroundColor = backgroundColor ?? BackgroundColor,
            HyperlinkUrl = hyperlinkUrl ?? HyperlinkUrl,
            Underline = underline ?? Underline,
        };
    }
}
