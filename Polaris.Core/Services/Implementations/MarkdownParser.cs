using Markdig;
using Markdig.Syntax;
using Polaris.Core.Services.Interfaces;

namespace Polaris.Core.Services.Implementations;

public sealed class MarkdownParser : IMarkdownParser
{
    public MarkdownDocument Parse(string markdownText)
    {
        var pipeline = new MarkdownPipelineBuilder().Build();
        return Markdown.Parse(markdownText, pipeline);
    }
}
