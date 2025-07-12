using Polaris.Core.Document.InlineElements;

namespace Polaris.Core.Document.Elements;

// heading
public class Heading : BlockElement
{
    public int Level { get; set; }
    public List<InlineElement> Inlines { get; set; } = [];
}