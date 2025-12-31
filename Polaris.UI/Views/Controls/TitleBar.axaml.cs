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

    public static readonly StyledProperty<ICommand> SaveFileCommandProperty =
        AvaloniaProperty.Register<TitleBar, ICommand>(nameof(SaveFileCommand));

    public static readonly StyledProperty<ICommand> ExportFileCommandProperty =
        AvaloniaProperty.Register<TitleBar, ICommand>(nameof(ExportFileCommand));

    public ICommand OpenFileCommand
    {
        get => GetValue(OpenFileCommandProperty);
        set => SetValue(OpenFileCommandProperty, value);
    }

    public ICommand SaveFileCommand
    {
        get => GetValue(SaveFileCommandProperty);
        set => SetValue(SaveFileCommandProperty, value);
    }

    public ICommand ExportFileCommand
    {
        get => GetValue(ExportFileCommandProperty);
        set => SetValue(ExportFileCommandProperty, value);
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
