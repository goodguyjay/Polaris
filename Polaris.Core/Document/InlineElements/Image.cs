using Polaris.Core.Visitors;

namespace Polaris.Core.Document.InlineElements;

public sealed class Image : InlineElement
{
    public string Src { get; set; } = string.Empty;
    public string? OriginalPath { get; set; }
    public string Alt { get; set; } = string.Empty;
    public string? Title { get; set; }

    public override T Accept<T>(IInlineElementVisitor<T> visitor) => visitor.VisitImage(this);
}
