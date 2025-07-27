using Polaris.Core.Visitors;

namespace Polaris.Core.Document.InlineElements;

public sealed class LineBreak : InlineElement
{
    public override T Accept<T>(IInlineElementVisitor<T> visitor) => visitor.VisitLineBreak(this);
}
