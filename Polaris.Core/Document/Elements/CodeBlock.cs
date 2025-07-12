namespace Polaris.Core.Document.Elements;

public sealed class CodeBlock : BlockElement
{
    public string? Language { get; set; }
    public string Code { get; set; } = string.Empty;
}