using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Polaris.UI.Views.Controls;

public sealed partial class TitleBar : UserControl
{
    public TitleBar()
    {
        InitializeComponent();
    }

    private void MinimizeWindow(object? sender, RoutedEventArgs e)
    {
        if (VisualRoot is Window hostWindow)
            hostWindow.WindowState = WindowState.Minimized;
    }

    private void MaximizeWindow(object? sender, RoutedEventArgs e)
    {
        if (VisualRoot is Window hostWindow)
        {
            hostWindow.WindowState =
                hostWindow.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
        }
    }

    private void CloseWindow(object? sender, RoutedEventArgs e)
    {
        if (VisualRoot is Window hostWindow)
            hostWindow.Close();
    }
}
