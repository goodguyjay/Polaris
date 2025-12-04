using Polaris.Core.Visitors;

namespace Polaris.Core.Document.Elements;

public sealed class Blank : BlockElement
{
    public int Count { get; set; } = 1;

    public override T Accept<T>(IBlockElementVisitor<T> visitor) => visitor.VisitBlank(this);
}
