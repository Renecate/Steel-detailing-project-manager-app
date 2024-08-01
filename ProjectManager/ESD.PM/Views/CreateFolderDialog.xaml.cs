using ESD.PM.Models;
using ESD.PM.ViewModels;
using System.Windows;

namespace ESD.PM.Views
{
    public partial class CreateFolderDialog : Window
    {


        private string _selectedPath;

        private CreateFolderViewModel _createFolderViewModel;

        public CreateFolderDialog(Window owner, int initialOrderNumber, List<string> creationPath, List<string> tags, AppSettings appSettings)
        {

            InitializeComponent();
            _createFolderViewModel = new CreateFolderViewModel();
            DataContext = _createFolderViewModel;
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _createFolderViewModel.OrderNumber = initialOrderNumber.ToString();
            _createFolderViewModel.CreationPath = creationPath;
            _createFolderViewModel.FolderTags = tags;
            _createFolderViewModel.AppSettings = appSettings;
            _createFolderViewModel.GetTags();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
