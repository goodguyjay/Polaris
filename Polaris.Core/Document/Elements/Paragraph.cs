using Polaris.Core.Document.InlineElements;
using Polaris.Core.Visitors;

namespace Polaris.Core.Document.Elements;

public sealed class Paragraph : BlockElement
{
    public List<InlineElement> Inlines { get; set; } = [];

    public override T Accept<T>(IBlockElementVisitor<T> visitor) => visitor.VisitParagraph(this);
}
