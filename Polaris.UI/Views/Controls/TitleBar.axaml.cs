using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Polaris.UI.Views.Controls;

public sealed partial class TitleBar : UserControl
{
    public TitleBar()
    {
        InitializeComponent();
    }
    
    public static readonly StyledProperty<ICommand> OpenFileCommandProperty =
        AvaloniaProperty.Register<TitleBar, ICommand>(nameof(OpenFileCommand));

    public ICommand OpenFileCommand
    {
        get => GetValue(OpenFileCommandProperty);
        set => SetValue(OpenFileCommandProperty, value);
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
