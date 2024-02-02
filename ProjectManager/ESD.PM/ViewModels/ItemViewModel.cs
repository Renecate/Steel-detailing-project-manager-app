using Caliburn.Micro;
using ESD.PM.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace ESD.PM.ViewModels
{
    public class ItemViewModel : Screen
    {
        public ObservableCollection<ProjectsModel> DocsList { get; set; } = [];

        private ProjectsModel _selectedFolder;

        public ProjectsModel SelectedFolder
        {
            get { return _selectedFolder; }
            set
            {
                _selectedFolder = value;
                NotifyOfPropertyChange(() => SelectedFolder);
            }
        }
        private void Docs()
        {
            DocsList.Clear();
            foreach (var item in Directory.GetDirectories(SelectedFolder.FullName))
                DocsList.Add(new ProjectsModel(item));
        }

    }
}
