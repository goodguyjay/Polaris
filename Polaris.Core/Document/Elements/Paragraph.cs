using Polaris.Core.Document.InlineElements;

namespace Polaris.Core.Document.Elements;

public sealed class Paragraph : BlockElement
{
    public List<InlineElement> Inlines { get; set; } = [];
}