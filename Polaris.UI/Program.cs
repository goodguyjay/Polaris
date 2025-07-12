using System;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Polaris.Core.Services.Markdown;
using Polaris.UI.Services.Markdown;

namespace Polaris.UI;

internal sealed class Program
{
    public static IServiceProvider Services { get; private set; } = null!;
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var services = new ServiceCollection();

        services.AddSingleton<IMarkdownRendererService, MarkdownRendererService>();
        services.AddSingleton<IMarkdownParser, MarkdownParser>();
        
        Services = services.BuildServiceProvider();
        
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args, ShutdownMode.OnMainWindowClose);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}