using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Polaris.Core.Document.Elements;
using Polaris.Core.Document.InlineElements;
using Polaris.Core.Parsing;
using Polaris.Core.Services.Markdown;
using Polaris.UI.Services.Markdown;

namespace Polaris.UI.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    private readonly IMarkdownParser _markdownParser;
    private readonly IMarkdownRendererService _markdownRenderer;
    private readonly Window _mainWindow;
    
    [ObservableProperty]
    private string _markdownText = string.Empty;

    [ObservableProperty]
    private Control _markdownPreview = new TextBlock { Text = "Preview will appear here." };
    
    public MainWindowViewModel(IMarkdownParser parser, IMarkdownRendererService rendererService, Window mainWindow)
    {
        _markdownParser = parser;
        _markdownRenderer = rendererService;
        _mainWindow = mainWindow;

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(MarkdownText)) return;
            
            var ast = _markdownParser.Parse(MarkdownText);
            MarkdownPreview = _markdownRenderer.RenderMarkdown(ast);
        };

        using var fs = File.OpenRead("example.polar");
        var doc = PolarDocumentParser.Load(fs);

        Console.WriteLine($"Title: {doc.Metadata.Title}");
        Console.WriteLine($"First heading: {doc.Blocks.OfType<Heading>().FirstOrDefault()?.Inlines.OfType<TextRun>().FirstOrDefault()?.Text}");
    }

    [RelayCommand]
    private async Task OpenFileAsync()
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

    [RelayCommand]
    private async Task SaveFileAsync()
    {
        var storageProvider = _mainWindow.StorageProvider;
        var files = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Markdown File",
            SuggestedFileName = "Untitled.md",
            FileTypeChoices =
            [
                new FilePickerFileType("Markdown") { Patterns = ["*.md"] },
                new FilePickerFileType("Text") { Patterns = ["*.txt"] }
            ]
        });

        if (files is not null)
        {
            await using var stream = await files.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(MarkdownText);
        }
    }
}
