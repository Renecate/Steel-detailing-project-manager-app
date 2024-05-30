using System.Collections.ObjectModel;

namespace ESD.PM.Models
{
    public class AppSettings
    {
        public ObservableCollection<string> ProjectPaths { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> FavoriteProjects { get; set; } = new ObservableCollection<string>();
    }
}
