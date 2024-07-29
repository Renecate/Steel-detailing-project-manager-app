using ESD.PM.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Application = System.Windows.Application;
using DataFormats = System.Windows.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using ListBox = System.Windows.Controls.ListBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace ESD.PM.Views
{
    public static class UIElementExtensions
    {
        public static T FindAncestor<T>(this DependencyObject child) where T : DependencyObject
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
                return FindAncestor<T>(parentObject);
            }
        }
    }

    public partial class FolderView
    {
        private Popup currentPopup;
        public FolderView()
        {
            InitializeComponent();
        }

        private async void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            var textBlock = sender as TextBlock;
            var popup = textBlock?.FindName("popup") as Popup;
            if (popup != null)
            {
                currentPopup = popup;
                await Task.Delay(400);

                var windowPosition = Mouse.GetPosition(this);
                var screenPosition = this.PointToScreen(windowPosition);
                popup.HorizontalOffset = screenPosition.X + 6;
                popup.VerticalOffset = screenPosition.Y + 1;

                if (currentPopup == popup)
                {
                    popup.IsOpen = true;
                }
            }                                                           
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            var textBlock = sender as TextBlock;
            var popup = textBlock?.FindName("popup") as Popup;
            if (popup != null)
            {
                popup.IsOpen = false;
                currentPopup = null;
            }
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

