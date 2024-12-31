using Avalonia.Controls;

namespace Polaris.UI.Services.Interfaces;

public interface IMarkdownRenderer
{
    Control RenderMarkdown(string markdownText);
}
