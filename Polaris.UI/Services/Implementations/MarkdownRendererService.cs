using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Markdig.Syntax;
using Polaris.UI.Rendering;
using Polaris.UI.Services.Interfaces;

namespace Polaris.UI.Services.Implementations;

public class MarkdownRendererService : IMarkdownRendererService
{
    public Control RenderMarkdown(MarkdownDocument doc)
    {
        var visitor = new MarkdownUiVisitor();

        return MarkdownRenderer.RenderDocument(doc, visitor);
    }
}
