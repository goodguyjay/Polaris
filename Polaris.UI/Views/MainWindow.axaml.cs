using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Polaris.Core.Services.Interfaces;
using Polaris.UI.Services.Interfaces;
using Polaris.UI.ViewModels;

namespace Polaris.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var fileService = Program.Services.GetRequiredService<IFileService>();
        var parser = Program.Services.GetRequiredService<IMarkdownParser>();
        var rendererService = Program.Services.GetRequiredService<IMarkdownRendererService>();

        DataContext = new MainWindowViewModel(fileService, parser, rendererService, this);
    }
}
