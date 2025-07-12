using Markdig.Syntax;

namespace Polaris.Core.Services.Markdown;

[Obsolete("Will change to XML parsing, only kept for compatibility with existing markdown files.")]
public interface IMarkdownParser
{
    MarkdownDocument Parse(string markdownText);
}
