using System.Windows;
using System.Windows.Input;

namespace ESD.PM.Views
{
    public partial class CreateFolderDialog : Window
    {
        public string OrderNumber { get; set; }
        public string FolderTag { get; set; }
        public string Date { get; set; }
        public string FolderName { get; set; }
        public List<string> CreationPath { get; set; }
        public string SelectedPath
        {
            get { return _selectedPath; }
            set { _selectedPath = value; }
        }

        private string _selectedPath;

        public CreateFolderDialog(int initialOrderNumber, List<string> creationPath)
        {
            InitializeComponent();

            var windowPosition = Mouse.GetPosition(this);
            var screenPosition = this.PointToScreen(windowPosition);

            this.Top = screenPosition.Y;
            this.Left = screenPosition.X;

            OrderNumber = ($"({initialOrderNumber})");
            FolderTag = string.Empty;
            Date = DateTime.Now.Date.ToString("MM.dd.yyyy");
            FolderName = "New Folder";

            DataContext = this;
            CreationPath = creationPath;

            if (creationPath != null)
            {
                _selectedPath = creationPath[0];
            }

            if (_selectedPath != null)
            {
                SelectedPath = _selectedPath;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
