using Polaris.Core.Visitors;

namespace Polaris.Core.Document.Elements;

public sealed class Blank : BlockElement
{
    public override T Accept<T>(IBlockElementVisitor<T> visitor) => visitor.VisitBlank(this);
}
