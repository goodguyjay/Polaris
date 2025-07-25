using Polaris.Core.Visitors;

namespace Polaris.Core.Document.Elements;

public sealed class ListBlock : BlockElement
{
    public ListType Type { get; set; }
    public List<ListItem> Items { get; set; } = [];

    public override T Accept<T>(IBlockElementVisitor<T> visitor) => visitor.VisitListBlock(this);
}
