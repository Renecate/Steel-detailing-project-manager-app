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

        public CreateProjectDialog(ObservableCollection<string> projectPaths)
        {
            InitializeComponent();
            ProjectPaths = projectPaths;
            DataContext = this;
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
