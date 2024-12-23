using ESD.PM.Views;
using ESD.PM.Settings;
using ESD.PM.Models;
using System.Windows;

namespace ESD.PM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private AppSettings _appSettings;
        private SharedSettings _sharedSettings;
        private string _userName;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _appSettings = AppSettingsManager.LoadSettings();

            CheckIfUserExists();
            CreateSession();

            var mainWindow = new MainWindow();

            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            KillSession();
        }

        private void CheckIfUserExists()
        {
            if (_appSettings.User == null)
            {
                _userName = Environment.UserName;
            }
        }

        private void CreateSession()
        {
            if (_appSettings.User == null)
            {
                _appSettings.User = new SessionModel (_userName);
                AppSettingsManager.SaveSettings(_appSettings);
            }
            _sharedSettings = ServerSettingsManager.LoadSettings();
            _sharedSettings.Users.Add(_appSettings.User);
            ServerSettingsManager.SaveSettings(_sharedSettings);
        }

        private void KillSession()
        {
            var id = _appSettings.User.Id;
            _sharedSettings = ServerSettingsManager.LoadSettings();
            foreach (var user in _sharedSettings.Users)
            {
                if (user.Id == id)
                {
                    _sharedSettings.Users.Remove(user);
                    break;
                }
            }
            ServerSettingsManager.SaveSettings(_sharedSettings);
        }
    }
}
