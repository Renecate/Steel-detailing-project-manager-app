﻿using ESD.PM.Views;
using System.Windows;

namespace ESD.PM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var mainWindow = new MainWindow();

            mainWindow.Show();
        }
    }
}
