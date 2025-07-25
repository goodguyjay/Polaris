using Polaris.Core.Document.InlineElements;
using Polaris.Core.Visitors;

namespace Polaris.Core.Document.Elements;

// heading
public class Heading : BlockElement
{
    public int Level { get; set; }
    public List<InlineElement> Inlines { get; set; } = [];

    public override T Accept<T>(IBlockElementVisitor<T> visitor) => visitor.VisitHeading(this);
}
