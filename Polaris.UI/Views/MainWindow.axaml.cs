using Avalonia.Controls;
using Polaris.UI.ViewModels;

namespace Polaris.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel(this);
    }
}
