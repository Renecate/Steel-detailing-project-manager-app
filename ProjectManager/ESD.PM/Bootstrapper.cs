using Caliburn.Micro;
using ESD.PM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ESD.PM
{
  public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            LogManager.GetLog = type => new DebugLog(type);
            Initialize();

        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync(typeof(ShellViewModel));
        }
    }
}
