using ESD.PM.ViewModels;
using System.Windows;

namespace ESD.PM.Views
{

    public partial class SettingsWindow : Window
    {
        private SettingsViewModel settingsViewModel { get; set; }
        public SettingsWindow(Window owner)
        {
            Owner = owner;
            InitializeComponent();
            settingsViewModel = new SettingsViewModel();
            DataContext = settingsViewModel;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            settingsViewModel.SaveSettings();
        }
    }
}
