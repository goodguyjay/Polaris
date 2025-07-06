using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Polaris.Core.Services.Interfaces;
using Polaris.UI.Services.Interfaces;

namespace Polaris.UI.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _markdownText = string.Empty;

    [ObservableProperty]
    private Control _markdownPreview = new TextBlock { Text = "Preview will appear here." };

    public MainWindowViewModel(IMarkdownParser parser, IMarkdownRendererService rendererService)
    {
        var mdParser = parser;
        var mdRenderer = rendererService;

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(MarkdownText)) return;
            
            var ast = mdParser.Parse(MarkdownText);
            MarkdownPreview = mdRenderer.RenderMarkdown(ast);
        };
    }
}
