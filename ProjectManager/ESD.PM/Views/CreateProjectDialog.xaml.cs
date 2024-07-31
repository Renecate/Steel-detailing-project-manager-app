using System.Collections.ObjectModel;
using System.Windows;

namespace ESD.PM.Views
{
    public partial class CreateProjectDialog : Window
    {
        public ObservableCollection<string> ProjectPaths { get; set; }
        public string SelectedProjectPath => ParentFolderComboBox.SelectedItem as string;
        public string ProjectName => ProjectNameTextBox.Text;
        public bool ItemsIncluded => ItemsIncludedCheckBox.IsChecked ?? false;
        public string[] Items => ItemsTextBox.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        public CreateProjectDialog(Window owner, ObservableCollection<string> projectPaths)
        {
            InitializeComponent();
            Owner = owner;
            ProjectPaths = projectPaths;
            DataContext = this;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
