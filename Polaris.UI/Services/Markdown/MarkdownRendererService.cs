using Avalonia.Controls;
using Markdig.Syntax;
using Polaris.UI.Rendering;

namespace Polaris.UI.Services.Markdown;

public class MarkdownRendererService : IMarkdownRendererService
{
    public Control RenderMarkdown(MarkdownDocument doc)
    {
        var visitor = new MarkdownUiVisitor();

        return MarkdownRenderer.RenderDocument(doc, visitor);
    }
}
