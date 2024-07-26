using ESD.PM.ViewModels;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;

namespace ESD.PM.Views
{
    public partial class MainWindow : Window
    {

        public MainWindowViewModel mainWindowViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            mainWindowViewModel = new MainWindowViewModel();
            DataContext = mainWindowViewModel;
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
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
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
        }

        private async void Window_OnMouseMove(object sender, MouseEventArgs e)
        {
            await Task.Delay(40);
            if (e.LeftButton == MouseButtonState.Pressed && WindowState == WindowState.Normal)
                DragMove();
            else if (e.LeftButton == MouseButtonState.Pressed && WindowState != WindowState.Normal)
            {
                var mousePosition = e.GetPosition(this);
                var screenMousePosition = PointToScreen(mousePosition);
                WindowState = WindowState.Normal;
                Left = screenMousePosition.X - 450;
                Top = screenMousePosition.Y - 25;
                DragMove();
            }
        }
    }
}
