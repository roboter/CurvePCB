using Avalonia.Controls;
using BasicMvvmSample.ViewModels;

namespace BasicMvvmSample.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
