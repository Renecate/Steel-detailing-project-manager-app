using ESD.PM.Settings;
using ESD.PM.ViewModels;
using System.Windows;

namespace ESD.PM.Views
{
    public partial class CreateFolderDialog : Window
    {
        public string SelectedTag
        {
            get { return _selectedTag; }
            set
            {
                if (_selectedTag != value)
                {
                    SelectedTagChanged();
                    _selectedTag = value;
                }
            }
        }


        private string _selectedTag;

        private CreateFolderViewModel _createFolderViewModel;

        public CreateFolderDialog(Window owner, int initialOrderNumber, string rfiNumber, List<string> creationPath, List<string> tags)
        {

            InitializeComponent();
            _createFolderViewModel = new CreateFolderViewModel();
            DataContext = _createFolderViewModel;
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            _createFolderViewModel.OrderNumber = initialOrderNumber.ToString();
            _createFolderViewModel.CreationPath = creationPath;
            _createFolderViewModel.FolderTags = tags;
            _createFolderViewModel.RfiNumber = rfiNumber;
            _createFolderViewModel.GetTags();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            _createFolderViewModel.Create();
        }

        private void SelectedTagChanged()
        {
            _createFolderViewModel.SelectedFolderTag = SelectedTag;
        }
    }
}
