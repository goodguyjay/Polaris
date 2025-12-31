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
using Polaris.Core.Export;
using Polaris.Core.Parsing;
using Polaris.Pdf;
using Polaris.UI.DocumentRendering;
using Image = Polaris.Core.Document.InlineElements.Image;

namespace Polaris.UI.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    private PolarDocument? _polarDocument;

    private string? _currentDocumentDirectory;

    [ObservableProperty]
    private string _documentText = string.Empty;

    [ObservableProperty]
    private Control _polarPreview = new TextBlock { Text = "Preview will appear here." };

    [ObservableProperty]
    private bool _isSplashVisible = true;

    private readonly IPolarSyntaxParser _parser;

    private readonly IPdfGenerator _pdfGenerator;

    private Window? _mainWindow;

    public MainWindowViewModel(IPolarSyntaxParser parser, IPdfGenerator pdfGenerator)
    {
        _parser = parser;
        _pdfGenerator = pdfGenerator;

        IsSplashVisible = true;
#if !DEBUG
        _ = Task.Run(async () =>
        {
            await Task.Delay(2500); // Simulate loading time
            Avalonia.Threading.Dispatcher.UIThread.Post(() => IsSplashVisible = false);
        });
#endif

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(DocumentText))
                return;

            _polarDocument = _parser.Parse(DocumentText, _currentDocumentDirectory);
            UpdatePolarPreview();
        };

        if (!File.Exists("example.polar"))
            return;

        _currentDocumentDirectory = Path.GetDirectoryName(Path.GetFullPath("example.polar"));

        using var fs = File.OpenRead("example.polar");
        _polarDocument = PolarDocumentParser.Load(fs);
        DocumentText = DocumentToPlainText(_polarDocument);
        PolarPreview = RenderPolarPreview(_polarDocument);
    }

    public void SetWindow(Window window)
    {
        _mainWindow = window;
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
            Image img =>
                $"![{img.Alt}]({img.OriginalPath ?? img.Src}{(img.Title != null ? $" \"{img.Title}\"" : "")})", // Prefer original path if available
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
            doc = _parser.Parse(DocumentText, _currentDocumentDirectory);
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
        var files = await _mainWindow!.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Open Polaris Document",
                AllowMultiple = false,
                FileTypeFilter = [new FilePickerFileType("Polaris") { Patterns = ["*.polar"] }],
            }
        );

        if (files is { Count: > 0 })
        {
            var file = files[0];
            _currentDocumentDirectory = Path.GetDirectoryName(file.Path.LocalPath);

            await using var stream = await files[0].OpenReadAsync();
            _polarDocument = PolarDocumentParser.Load(stream);
            DocumentText = DocumentToPlainText(_polarDocument);
            PolarPreview = RenderPolarPreview(_polarDocument);
        }
    }

    [RelayCommand]
    public async Task SavePolarFileAsync()
    {
        var files = await _mainWindow!.StorageProvider.SaveFilePickerAsync(
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

    [RelayCommand]
    public async Task ExportPolarPdfAsync()
    {
        if (_polarDocument is null)
        {
            // add dialogs boxes when ready
            return;
        }

        var file = await _mainWindow!.StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions
            {
                Title = "Export to PDF",
                SuggestedFileName = "Untitled.pdf",
                FileTypeChoices = [new FilePickerFileType("PDF") { Patterns = ["*.pdf"] }],
            }
        );

        if (file is not null)
        {
            var options = new PdfOptions
            {
                Template = CreateDefaultTemplate(), // temporary
                Title = _polarDocument.Metadata.Title ?? "Untitled Document",
            };

            var pdfBytes = _pdfGenerator.Generate(_polarDocument, options);

            await using var stream = await file.OpenWriteAsync();
            await stream.WriteAsync(pdfBytes);
        }
    }

    // super temporary
    private static PdfTemplateConfig CreateDefaultTemplate()
    {
        return new PdfTemplateConfig
        {
            FontFamily = "Arial",
            FontSize = 12,
            LineHeight = 1.5f,
            TextColor = "#000000",
            Margins = new MarginsConfig(2.0f, 2.0f, 2.0f, 2.0f),
            Heading1Size = 2.0f,
            Heading2Size = 1.5f,
            Heading3Size = 1.25f,
            Heading4Size = 1.1f,
            Heading5Size = 1.0f,
            Heading6Size = 0.9f,
        };
    }
}
