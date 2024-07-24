using System.Windows;

namespace ESD.PM.Views
{
    public partial class CreateFolderDialog : Window
    {
        public string OrderNumber { get; set; }
        public string FolderTag { get; set; }
        public string Date { get; set; }
        public string FolderName { get; set; }

        public CreateFolderDialog(int initialOrderNumber)
        {
            InitializeComponent();

            OrderNumber = ($"({initialOrderNumber})");
            FolderTag = string.Empty;
            Date = DateTime.Now.Date.ToString("MM.dd.yyyy");
            FolderName = "New Folder";

            DataContext = this;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
