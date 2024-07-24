using System;
using System.Collections.Generic;
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
using MouseEventArgs = System.Windows.Input.MouseEventArgs;


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
