namespace Polaris.Core.Document.InlineElements;

public sealed class Emphasis : InlineElement
{
    public List<InlineElement> Children { get; set; } = [];
}