using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Polaris.Core.Document;
using Polaris.Core.Document.Elements;
using Polaris.Core.Document.InlineElements;
using Polaris.Core.Parsing;
using Polaris.UI.DocumentRendering;

namespace Polaris.UI.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    private PolarDocument? _polarDocument;

    [ObservableProperty]
    private string _documentText = string.Empty;

    [ObservableProperty]
    private Control _polarPreview = new TextBlock { Text = "Preview will appear here." };

    [ObservableProperty]
    private bool _isSplashVisible = true;

    private readonly PolarSyntaxParser _parser = new();

    private readonly Window _mainWindow;

    public MainWindowViewModel(Window mainWindow)
    {
        IsSplashVisible = true;
#if !DEBUG
        _ = Task.Run(async () =>
        {
            await Task.Delay(2500); // Simulate loading time
            Avalonia.Threading.Dispatcher.UIThread.Post(() => IsSplashVisible = false);
        });
#endif
        _mainWindow = mainWindow;

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(DocumentText))
                return;

            _polarDocument = _parser.Parse(DocumentText);
            UpdatePolarPreview();
        };

        if (!File.Exists("example.polar"))
            return;

        using var fs = File.OpenRead("example.polar");
        _polarDocument = PolarDocumentParser.Load(fs);
        DocumentText = DocumentToPlainText(_polarDocument);
        PolarPreview = RenderPolarPreview(_polarDocument);
    }

    public void DismissSplash() => IsSplashVisible = false;

    private static string DocumentToPlainText(PolarDocument doc)
    {
        var lines = new List<string>();
        foreach (var block in doc.Blocks)
        {
            switch (block)
            {
                case Heading h:
                    var prefix = new string('#', h.Level);
                    var headingText = string.Join(string.Empty, h.Inlines.Select(InlineToText));
                    lines.Add($"{prefix} {headingText}");
                    break;

                case Paragraph p:
                    lines.Add(string.Join(string.Empty, p.Inlines.Select(InlineToText)));
                    break;

                case ListBlock l:
                    lines.AddRange(
                        l.Items.Select(
                            (item, index) =>
                            {
                                var itemText = string.Join(
                                    string.Empty,
                                    item.Inlines.Select(InlineToText)
                                );
                                var itemPrefix =
                                    l.Type == ListType.Bullet ? "- " : $"{index + 1}. ";
                                return $"{itemPrefix}{itemText}";
                            }
                        )
                    );
                    break;

                case CodeBlock c:
                    lines.Add("```" + (c.Language ?? ""));
                    lines.Add(c.Code);
                    lines.Add("```");
                    break;

                case HorizontalRule:
                    lines.Add("---");
                    break;

                case Blank b:
                    for (var i = 0; i < b.Count; i++)
                        lines.Add(string.Empty);
                    break;
            }
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static string InlineToText(InlineElement inline)
    {
        return inline switch
        {
            TextRun t => t.Text,
            Strong s => string.Join(string.Empty, s.Children.Select(InlineToText)),
            Emphasis e => string.Join(string.Empty, e.Children.Select(InlineToText)),
            InlineCode c => $"`{c.Code}`",
            Link l =>
                $"[{string.Join("", l.Children.Select(InlineToText))}]({l.Href}{(l.Title != null ? $" \"{l.Title}\"" : "")})",
            LineBreak => "\n",
            _ => string.Empty,
        };
    }

    private static StackPanel RenderPolarPreview(PolarDocument doc)
    {
        var renderer = new UiVisitor();
        var rootPanel = new StackPanel { Orientation = Orientation.Vertical };
        foreach (var block in doc.Blocks)
            rootPanel.Children.Add(block.Accept(renderer));
        return rootPanel;
    }

    private void UpdatePolarPreview()
    {
        PolarDocument doc;

        try
        {
            doc = _parser.Parse(DocumentText);
        }
        catch
        {
            PolarPreview = new TextBlock { Text = "Parsing error!", Foreground = Brushes.Red };
            return;
        }

        PolarPreview = RenderPolarPreview(doc);
    }

    [RelayCommand]
    public async Task OpenPolarFileAsync()
    {
        var files = await _mainWindow.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Open Polaris Document",
                AllowMultiple = false,
                FileTypeFilter = [new FilePickerFileType("Polaris") { Patterns = ["*.polar"] }],
            }
        );

        if (files is { Count: > 0 })
        {
            await using var stream = await files[0].OpenReadAsync();
            _polarDocument = PolarDocumentParser.Load(stream);
            DocumentText = DocumentToPlainText(_polarDocument);
            PolarPreview = RenderPolarPreview(_polarDocument);
        }
    }

    [RelayCommand]
    public async Task SavePolarFileAsync()
    {
        var files = await _mainWindow.StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions
            {
                Title = "Save Polaris File",
                SuggestedFileName = "Untitled.polar",
                FileTypeChoices = [new FilePickerFileType("Polaris") { Patterns = ["*.polar"] }],
            }
        );

        if (files is not null && _polarDocument is not null)
        {
            await using var stream = await files.OpenWriteAsync();
            PolarXmlWriter.Save(_polarDocument, stream);
        }
    }
}
