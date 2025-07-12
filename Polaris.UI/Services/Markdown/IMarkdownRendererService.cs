using Avalonia.Controls;
using Markdig.Syntax;

namespace Polaris.UI.Services.Markdown;

public interface IMarkdownRendererService
{
    Control RenderMarkdown(MarkdownDocument doc);
}
