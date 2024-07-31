using System.Windows;


namespace ESD.PM.Views
{

    public partial class RenameDialog : Window
    {
        public string NewFolderName { get; private set; }

        public RenameDialog(Window owner, string filename)
        {
            Owner = owner;
            InitializeComponent();
            FolderNameTextBox.Text = filename;
            FolderNameTextBox.SelectAll();
            FolderNameTextBox.Focus();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            NewFolderName = FolderNameTextBox.Text;
            DialogResult = true;
        }
    }
}
