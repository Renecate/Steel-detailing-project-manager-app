using System.Collections.ObjectModel;
using ESD.PM.Models;

namespace ESD.PM.Settings
{
    public class SharedSettings
    {
        public ObservableCollection<SessionModel> Users { get; set; }

        public ObservableCollection<ProjectHistoryModel> ProjectHistory { get; set; }

        public SharedSettings()
        {
            Users = new ObservableCollection<SessionModel>();
            ProjectHistory = new ObservableCollection<ProjectHistoryModel>();
        }
    }
}
