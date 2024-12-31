using Markdig;
using Polaris.Core.Services.Interfaces;

namespace Polaris.Core.Services.Implementations;

public sealed class MarkdownParser : IMarkdownParser
{
    public string Parse(string markdownText)
    {
        return string.IsNullOrWhiteSpace(markdownText)
            ? string.Empty
            : Markdown.ToHtml(markdownText);
    }
}
