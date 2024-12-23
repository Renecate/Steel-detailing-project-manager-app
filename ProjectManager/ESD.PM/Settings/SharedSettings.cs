using System.Collections.ObjectModel;
using ESD.PM.Models;

namespace ESD.PM.Settings
{
    public class SharedSettings
    {
        public ObservableCollection<SessionModel> Users { get; set; }

        public ObservableCollection<ProjectHistoryModel> History { get; set; }

        public SharedSettings()
        {
            Users = new ObservableCollection<SessionModel>();
            History = new ObservableCollection<ProjectHistoryModel>();
        }
    }
}
