using Polaris.Core.Visitors;

namespace Polaris.Core.Document.InlineElements;

public sealed class Emphasis : InlineElement
{
    public List<InlineElement> Children { get; set; } = [];

    public override T Accept<T>(IInlineElementVisitor<T> visitor) => visitor.VisitEmphasis(this);
}
