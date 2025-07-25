using Polaris.Core.Visitors;

namespace Polaris.Core.Document.Elements;

public sealed class HorizontalRule : BlockElement
{
    // marker element: no content yet
    public override T Accept<T>(IBlockElementVisitor<T> visitor) =>
        visitor.VisitHorizontalRule(this);
}
