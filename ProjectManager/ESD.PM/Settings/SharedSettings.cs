using System.Collections.ObjectModel;

namespace ESD.PM.Settings
{
    public class SharedSettings
    {
        public ObservableCollection<string> CheckedFolders { get; set; } = new ObservableCollection<string>();

        public bool Available { get; set; }
    }
}
