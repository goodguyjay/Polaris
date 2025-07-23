﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Polaris.Core.Document;
using Polaris.Core.Document.Elements;
using Polaris.Core.Document.InlineElements;
using Polaris.Core.Parsing;

namespace Polaris.UI.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    private PolarDocument? _polarDocument;

    [ObservableProperty]
    private string _documentText = string.Empty;

    [ObservableProperty]
    private bool _isSplashVisible = true;

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

        if (!File.Exists("example.polar"))
            return;

        using var fs = File.OpenRead("example.polar");
        _polarDocument = PolarDocumentParser.Load(fs);
        DocumentText = DocumentToPlainText(_polarDocument);
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
                    lines.Add(
                        string.Join(string.Empty, h.Inlines.OfType<TextRun>().Select(x => x.Text))
                    );
                    break;

                case Paragraph p:
                    lines.Add(
                        string.Join(string.Empty, p.Inlines.OfType<TextRun>().Select(x => x.Text))
                    );
                    break;

                case ListBlock l:
                    lines.AddRange(
                        l.Items.Select(item =>
                            string.Join("", item.Inlines.OfType<TextRun>().Select(x => x.Text))
                        )
                    );
                    break;

                case CodeBlock c:
                    lines.Add(c.Code);
                    break;
            }
        }

        return string.Join(Environment.NewLine, lines);
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
