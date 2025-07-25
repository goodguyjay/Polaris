using Polaris.Core.Visitors;

namespace Polaris.Core.Document.Elements;

public sealed class CodeBlock : BlockElement
{
    public string? Language { get; set; }
    public string Code { get; set; } = string.Empty;

    public override T Accept<T>(IBlockElementVisitor<T> visitor) => visitor.VisitCodeBlock(this);
}
