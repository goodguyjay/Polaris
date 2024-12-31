using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Polaris.Core.Services.Interfaces;
using Polaris.UI.Services.Interfaces;

namespace Polaris.UI.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    private readonly IMarkdownRenderer _markdownRenderer;

    [ObservableProperty]
    private string _markdownText = string.Empty;

    [ObservableProperty]
    private Control _markdownPreview = new TextBlock { Text = "Preview will appear here." };

    public MainWindowViewModel(IMarkdownRenderer markdownRenderer)
    {
        _markdownRenderer = markdownRenderer;

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MarkdownText))
            {
                MarkdownPreview = _markdownRenderer.RenderMarkdown(MarkdownText);
            }
        };
    }
}
