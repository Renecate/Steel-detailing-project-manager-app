using ESD.PM.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;

namespace ESD.PM.Views
{
    public partial class FolderView
    {
        public FolderView()
        {
            InitializeComponent();
        }

        private void FileDropListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                var viewModel = DataContext as FoldersViewModel;
                if (viewModel != null)
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        var textBlock = sender as TextBlock;
                        if (textBlock != null)
                        {
                            var listBoxItem = FindParent<ListBoxItem>(textBlock);
                            if (listBoxItem != null)
                            {
                                listBoxItem.IsSelected = true;
                            }
                        }
                    }
                    e.Handled = true;
                    viewModel.FileDropCommand.Execute(files);
                }
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }
    }
}

