using Polaris.Core.Visitors;

namespace Polaris.Core.Document.Elements;

// abstract base for all blocks
public abstract class BlockElement
{
    public string? Id { get; set; }
    public string? Style { get; set; }
    public abstract T Accept<T>(IBlockElementVisitor<T> visitor);
}
