using Polaris.Core.Document.InlineElements;

namespace Polaris.Core.Document.Elements;

public sealed class ListItem
{
    public List<InlineElement> Inlines { get; set; } = [];
}