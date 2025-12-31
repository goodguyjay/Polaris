using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polaris.Core.Parsing;
using Polaris.Pdf;
using Polaris.UI.ViewModels;
using Polaris.UI.Views;

namespace Polaris.UI;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // setup DI container
        var services = new ServiceCollection();

        // logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Information);
#if DEBUG
            builder.SetMinimumLevel(LogLevel.Debug);
#endif
        });

        // register services
        services.AddSingleton<IPolarSyntaxParser, PolarSyntaxParser>();
        services.AddTransient<IPdfGenerator, PdfGenerator>();
        services.AddTransient<MainWindowViewModel>();

        _serviceProvider = services.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();

            DisableAvaloniaDataAnnotationValidation();

            // resolve ViewModel from DI
            var viewModel = _serviceProvider!.GetRequiredService<MainWindowViewModel>();
            viewModel.SetWindow(mainWindow);
            mainWindow.DataContext = viewModel;

            desktop.MainWindow = mainWindow;
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
}
