using Polaris.Core.Visitors;

namespace Polaris.Core.Document.InlineElements;

public sealed class InlineCode : InlineElement
{
    public string Code { get; set; } = string.Empty;

    public override T Accept<T>(IInlineElementVisitor<T> visitor) => visitor.VisitInlineCode(this);
}
