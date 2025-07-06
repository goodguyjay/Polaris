using Avalonia.Controls;
using Markdig.Syntax;

namespace Polaris.UI.Services.Interfaces;

public interface IMarkdownRendererService
{
    Control RenderMarkdown(MarkdownDocument doc);
}
