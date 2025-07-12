using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Polaris.Core.Services.Markdown;
using Polaris.UI.Services.Markdown;
using Polaris.UI.ViewModels;

namespace Polaris.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var parser = Program.Services.GetRequiredService<IMarkdownParser>();
        var rendererService = Program.Services.GetRequiredService<IMarkdownRendererService>();

        DataContext = new MainWindowViewModel(parser, rendererService, this);
    }
}
