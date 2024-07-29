using System.Windows;
using System.Windows.Input;


namespace ESD.PM.Views
{

    public partial class RenameDialog : Window
    {
        public string NewFolderName { get; private set; }

        public RenameDialog(string filename)
        {
            InitializeComponent();
            var windowPosition = Mouse.GetPosition(this);
            var screenPosition = this.PointToScreen(windowPosition);

            this.Top = screenPosition.Y;
            this.Left = screenPosition.X;
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
