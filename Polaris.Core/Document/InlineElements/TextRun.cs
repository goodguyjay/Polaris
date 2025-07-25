using Polaris.Core.Visitors;

namespace Polaris.Core.Document.InlineElements;

public sealed class TextRun : InlineElement
{
    public string Text { get; set; } = string.Empty;

    public override T Accept<T>(IInlineElementVisitor<T> visitor) => visitor.VisitTextRun(this);
}
