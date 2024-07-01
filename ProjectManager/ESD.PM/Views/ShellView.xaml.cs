using ESD.PM.Models;
using ESD.PM.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ESD.PM.Views
{
    public partial class ShellView : Window
    {
        private const double WindowWidth = 800;
        private const double WindowHeight = 440;
        private const double Margin = 5;

        public ShellView()
        {
            InitializeComponent();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ExpandButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
        }

        private void HideButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ProjectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FoldersCanvas.Children.Clear();

            var selectedProject = ProjectComboBox.SelectedItem as ProjectsModel;
            if (selectedProject != null && ItemsComboBox.IsEnabled == false)
            {
                var folders = Directory.GetDirectories(selectedProject.FullName);
                CreateAndPlaceWindows(folders);
            }
        }

        private void ItemsComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            FoldersCanvas.Children.Clear();

            var selectedItem = ItemsComboBox.SelectedItem as ProjectsModel;
            if (selectedItem != null)
            {
                var folders = Directory.GetDirectories(selectedItem.FullName);
                CreateAndPlaceWindows(folders);
            }
        }

        private void CreateAndPlaceWindows(string[] folders)
        {
            double x = Margin;
            double y = Margin;

            foreach (var folder in folders)
            {
                var folderModel = new FoldersViewModel(folder);
                var folderView = new FolderView(folderModel)
                {
                    Width = WindowWidth,
                    Height = WindowHeight
                };

                var border = new Border
                {
                    Child = folderView,
                };

                ////Canvas.SetLeft(border, x);
                //Canvas.SetTop(border, y);

                FoldersCanvas.Children.Add(border);

                //x += WindowWidth + Margin;
                //if (x + WindowWidth > FoldersCanvas.ActualWidth)
                //{
                //    x = Margin;
                //    y += WindowHeight + Margin;
                //}
            }
        }
    }
}
