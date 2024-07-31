using System.Windows;

namespace ESD.PM.Views
{
    public partial class CreateFolderDialog : Window
    {
        public string SelectedTag { get; set; }
        public string OrderNumber { get; set; }
        public List<string> FolderTag { get; set; }
        public string Date { get; set; }
        public string FolderName { get; set; }
        public bool RfiState { get; set; }
        public List<string> CreationPath { get; set; }
        public string SelectedPath
        {
            get { return _selectedPath; }
            set { _selectedPath = value; }
        }

        private string _selectedPath;

        public CreateFolderDialog(Window owner, int initialOrderNumber, List<string> creationPath)
        {
            RfiState = false;

            InitializeComponent();

            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            OrderNumber = ($"({initialOrderNumber})");
            FolderTag = new List<string>()
            {
                "CD", "DC", "FA", "FC", "FM", "FF", "OD", "OR", "RR"
            };
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
