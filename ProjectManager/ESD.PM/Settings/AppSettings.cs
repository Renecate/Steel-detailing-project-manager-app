using System.Collections.ObjectModel;
using ESD.PM.Models;

namespace ESD.PM.Settings
{
    public class AppSettings
    {
        public ObservableCollection<string> ProjectPaths { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> FavoriteProjects { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> PdfTemplates { get; set; } = new ObservableCollection<string> { };

        public ObservableCollection<string> StructureTemplates { get; set; } = new ObservableCollection<string> { };

        public ObservableCollection<string> ProjectTemplates { get; set; } = new ObservableCollection<string> { };

        public ObservableCollection<string> RfiTemplates { get; set; } = new ObservableCollection<string> { };

        public SessionModel User { get; set; }
    }
}
