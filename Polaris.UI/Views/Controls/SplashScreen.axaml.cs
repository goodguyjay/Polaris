using Avalonia.Controls;
using Avalonia.Interactivity;
using Polaris.UI.ViewModels;

namespace Polaris.UI.Views.Controls;

public sealed partial class SplashScreen : UserControl
{
    public SplashScreen()
    {
        InitializeComponent();

#if DEBUG
        ContinueButton.IsVisible = true;
#endif
    }

    private void OnContinueClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
            vm.DismissSplash();
    }
}
