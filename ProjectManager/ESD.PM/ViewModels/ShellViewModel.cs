using Caliburn.Micro;
using ESD.PM.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;



namespace ESD.PM.ViewModels
{
    public class ShellViewModel : Screen
    {
        #region Public Properties
        public ObservableCollection<ProjectsModel> ProjectsList { get; set; } = [];
        public ObservableCollection<ItemsModel> ItemsList { get; set; } = [];
        public ObservableCollection<ProjectsModel> DisplayItemsList { get; set; } = [];

        #endregion

        #region Private Properties

        private ProjectsModel _selectedProject;

        private ProjectsModel _selectedItem;

        private ProjectsModel _selectedFolder;

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

        public ProjectsModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                Folders();
                NotifyOfPropertyChange(() => SelectedItem);
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
            var count = 0;
            DisplayItemsList.Clear();
            ItemsList.Clear();
            foreach(var item in Directory.GetDirectories(SelectedProject.FullName))
            {
                if (item.EndsWith("Items"))
                    count ++;
            }
            if (count == 0)
            {
                foreach (var item in Directory.GetDirectories(SelectedProject.FullName))
                {
                    ItemsList.Add(new ItemsModel(item));
                }
            }
            else
            {

                foreach (var item in Directory.GetDirectories(SelectedProject.FullName))
                    if (item.EndsWith("Items"))
                        foreach (var _item in Directory.GetDirectories(item))
                            DisplayItemsList.Add(new ProjectsModel(_item));
                }
        }

        private void Folders()
        {
            ItemsList.Clear();
            if (SelectedItem != null)
            {
                foreach (var item in Directory.GetDirectories(SelectedItem.FullName))
                {
                    ItemsList.Add(new ItemsModel(item));
                }
            }
        }
        #endregion
    }
}
