using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Markdig.Renderers;
using Polaris.Core.Services.Interfaces;
using Polaris.UI.Services.Interfaces;

namespace Polaris.UI.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    private readonly IFileService _fileService;
    private readonly IMarkdownParser _markdownParser;
    private readonly IMarkdownRendererService _markdownRenderer;
    private readonly Window _mainWindow;
    
    [ObservableProperty]
    private string _markdownText = string.Empty;

    [ObservableProperty]
    private Control _markdownPreview = new TextBlock { Text = "Preview will appear here." };
    
    public MainWindowViewModel(IFileService fileService, IMarkdownParser parser, IMarkdownRendererService rendererService, Window mainWindow)
    {
        _fileService = fileService;
        _markdownParser = parser;
        _markdownRenderer = rendererService;
        _mainWindow = mainWindow;

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(MarkdownText)) return;
            
            var ast = _markdownParser.Parse(MarkdownText);
            MarkdownPreview = _markdownRenderer.RenderMarkdown(ast);
        };
    }

    [RelayCommand]
    public async Task OpenFileAsync()
    {
        var storageProvider = _mainWindow.StorageProvider;
        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Markdown File",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Markdown") { Patterns = ["*.md"] },
                new FilePickerFileType("Text") { Patterns = ["*.txt"] }
            ]
        });

        if (files is { Count: > 0 })
        {
            var file = files[0];
            await using var stream = await file.OpenReadAsync();
            using var reader = new StreamReader(stream);
            MarkdownText = await reader.ReadToEndAsync();
        }
    }
}
