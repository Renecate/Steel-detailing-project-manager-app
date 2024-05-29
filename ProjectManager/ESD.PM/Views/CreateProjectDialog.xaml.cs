using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ESD.PM.Views
{
    /// <summary>
    /// Логика взаимодействия для CreateProjectDialog.xaml
    /// </summary>
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
