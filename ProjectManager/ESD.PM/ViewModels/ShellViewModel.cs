using Caliburn.Micro;
using ESD.PM.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace ESD.PM.ViewModels
{
    public class ShellViewModel: Screen
    {
        #region Public Properties
        public ObservableCollection<ProjectsModel> ProjectsList { get; set; } = [];
        public ObservableCollection<ProjectsModel> ItemsList { get; set; } = [];
        #endregion

        #region Private Properties

        private ProjectsModel _selectedProject;

        #endregion

        #region Commands

        #endregion

        #region Constructor
        public ProjectsModel SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
                Items();
                NotifyOfPropertyChange(() => SelectedProject);
            }
        }

        public ShellViewModel()
        {
            Projects();
        }

        #endregion

        #region Commands Methods

        #endregion

        #region Private Methods

        #endregion


        #region Public Methods

        public void Projects()
        {
            ProjectsList.Clear();
            foreach (var project in Directory.GetDirectories("C:\\Dropbox\\Production\\Projects"))
            {
                ProjectsList.Add(new ProjectsModel(project));
            }
        }

        public void Items()
        {
            ItemsList.Clear();
            foreach(var item in Directory.GetDirectories(SelectedProject.FullName))
            {
                ItemsList.Add(new ProjectsModel(item));
            }
        }

        #endregion
    }
}
