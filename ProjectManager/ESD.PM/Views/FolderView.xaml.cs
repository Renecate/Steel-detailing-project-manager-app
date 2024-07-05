using ESD.PM.Models;
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

namespace ESD.PM.Views
{
    /// <summary>
    /// Логика взаимодействия для FolderRepresentation.xaml
    /// </summary>
    public partial class FolderView
    {
        public FolderView(FoldersViewModel folder)
        {
            InitializeComponent();
            DataContext = folder;
        }

        private void ListBox_Drop(object sender, System.Windows.DragEventArgs e)
        {

        }
    }
}

