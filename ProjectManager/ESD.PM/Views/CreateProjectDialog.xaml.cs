using ESD.PM.Models;
using ESD.PM.ViewModels;
using System.Windows;

namespace ESD.PM.Views
{
    public partial class CreateProjectDialog : Window
    {

        private CreateProjectViewModel _createProjectViewModel;

        public CreateProjectDialog(Window owner, AppSettings appSettings)
        {
            InitializeComponent();
            _createProjectViewModel = new CreateProjectViewModel();
            DataContext = _createProjectViewModel;
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _createProjectViewModel.appSettings = appSettings;
            _createProjectViewModel.GetPathAndTemplates();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _createProjectViewModel.CreateProject();
        }
    }
}
