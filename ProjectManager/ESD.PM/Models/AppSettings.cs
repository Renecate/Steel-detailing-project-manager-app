using System.Collections.ObjectModel;

namespace ESD.PM.Models
{
    public class AppSettings
    {
        public ObservableCollection<string> ProjectPaths { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<string> FavoriteProjects { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<FoldersViewModel> SavedFolders { get; set; } = new ObservableCollection<FoldersViewModel> { };

        public ObservableCollection<string> PdfTemplates { get; set; } = new ObservableCollection<string> { };

        public ObservableCollection<string> StructureTemplates { get; set; } = new ObservableCollection<string> { };

        public ObservableCollection<string> ProjectTemplates { get; set; } = new ObservableCollection<string> { };

        public ObservableCollection<string> RfiTemplates { get; set; } = new ObservableCollection<string> { };
    }
}
