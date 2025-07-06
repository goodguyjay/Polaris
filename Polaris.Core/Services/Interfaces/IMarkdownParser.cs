using Markdig.Syntax;

namespace Polaris.Core.Services.Interfaces;

public interface IMarkdownParser
{
    MarkdownDocument Parse(string markdownText);
}
