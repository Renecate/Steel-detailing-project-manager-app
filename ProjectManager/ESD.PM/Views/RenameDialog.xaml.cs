using System.Windows;


namespace ESD.PM.Views
{
    /// <summary>
    /// Логика взаимодействия для RenameDialog.xaml
    /// </summary>
    public partial class RenameDialog : Window
    {
        public string NewFolderName { get; private set; }

        public RenameDialog(string filename)
        {
            InitializeComponent();
            FolderNameTextBox.Text = filename;
            FolderNameTextBox.SelectAll();
            FolderNameTextBox.Focus();

        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            NewFolderName = FolderNameTextBox.Text;
            DialogResult = true;
        }
    }
}
