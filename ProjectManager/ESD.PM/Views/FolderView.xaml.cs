﻿using ESD.PM.Models;
using ESD.PM.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;

namespace ESD.PM.Views
{
    /// <summary>
    /// Логика взаимодействия для FolderRepresentation.xaml
    /// </summary>
    public partial class FolderView
    {
        public FoldersViewModel foldersViewModel { get; set; }
        public FolderView(FoldersViewModel folder)
        {
            InitializeComponent();
            foldersViewModel = folder;
            DataContext = foldersViewModel;
        }

        private void FileDropListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

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

                foldersViewModel.FileDropCommand.Execute(files);
            }
        }

        private void FilesListBoxSelection_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
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

