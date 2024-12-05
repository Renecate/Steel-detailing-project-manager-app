using System.Collections.ObjectModel;

namespace ESD.PM.Models
{
    public class SharedSettings
    {
        public ObservableCollection<string> CheckedFolders { get; set; } = new ObservableCollection<string>();

    }
}
