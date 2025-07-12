using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Polaris.Core.Services.Markdown;
using Polaris.UI.Services.Markdown;
using Polaris.UI.ViewModels;
using Polaris.UI.Views;

namespace Polaris.UI;

public partial class App : Application
{
    // fear not, this is a necessary evil
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var serviceCollection = new ServiceCollection();

            RegisterServices(serviceCollection);
            // RegisterViewModels(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();
            
            var mainWindow = new MainWindow();
            
            var parser = ServiceProvider.GetRequiredService<IMarkdownParser>();
            var renderer = ServiceProvider.GetRequiredService<IMarkdownRendererService>();

            DisableAvaloniaDataAnnotationValidation();
            
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(parser, renderer, mainWindow),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove = BindingPlugins
            .DataValidators.OfType<DataAnnotationsValidationPlugin>()
            .ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IMarkdownParser, MarkdownParser>();
        services.AddSingleton<IMarkdownRendererService, MarkdownRendererService>();
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddSingleton<MainWindowViewModel>();
    }
}
