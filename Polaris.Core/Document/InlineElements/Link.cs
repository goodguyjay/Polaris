namespace Polaris.Core.Document.InlineElements;

public sealed class Link : InlineElement
{
    public string Href { get; set; } = string.Empty;
    public string? Title { get; set; }
    public List<InlineElement> Children { get; set; } = [];
}