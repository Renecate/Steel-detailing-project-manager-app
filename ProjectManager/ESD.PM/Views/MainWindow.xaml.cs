using ESD.PM.ViewModels;
using System.Windows;

namespace ESD.PM.Views
{
    public partial class MainWindow : Window
    {

        public MainWindowViewModel mainWindowViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            mainWindowViewModel = new MainWindowViewModel();
            DataContext = mainWindowViewModel;
        }
    }
}
