using Polaris.Core.Visitors;

namespace Polaris.Core.Document.InlineElements;

public abstract class InlineElement
{
    public abstract T Accept<T>(IInlineElementVisitor<T> visitor);
}
