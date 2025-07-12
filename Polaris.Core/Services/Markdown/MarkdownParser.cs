using Markdig;
using Markdig.Syntax;

namespace Polaris.Core.Services.Markdown;

[Obsolete("Will change to XML parsing, only kept for compatibility with existing markdown files.")]
public sealed class MarkdownParser : IMarkdownParser
{
    public MarkdownDocument Parse(string markdownText)
    {
        var pipeline = new MarkdownPipelineBuilder().UseSoftlineBreakAsHardlineBreak().Build();
        return Markdig.Markdown.Parse(markdownText, pipeline);
    }
}
