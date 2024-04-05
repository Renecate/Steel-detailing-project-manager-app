using Caliburn.Micro;
using ESD.PM.Commands;
using ESD.PM.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public DelegateCommand StructuralOpenCommand { get; set; }

        public DelegateCommand ArchOpenCommand { get; set; }

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
            StructuralOpenCommand = new DelegateCommand(OnOpenStructural);
            ArchOpenCommand = new DelegateCommand(OnOpenArch);
        }

        #endregion

        #region Commands Methods
        private void OnOpenStructural(object obj)
        {
            string[] parts = null;
            foreach (var item in Directory.GetFiles(SelectedProject.FullName))
            {
                parts = item.Split('\\');
                if (parts[parts.Length - 1].Contains("Structural"))
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", item));
                }
            }
        }
        private void OnOpenArch(object obj)
        {
            string[] parts = null;
            foreach (var item in Directory.GetFiles(SelectedProject.FullName))
            {
                parts = item.Split('\\');
                if (parts[parts.Length - 1].Contains("Architectural"))
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", item));
                }
            }

        }
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
            // Multi Item projects code below
            else
            {

                foreach (var item in Directory.GetDirectories(SelectedProject.FullName))
                    if (item.EndsWith("Items"))
                        foreach (var _item in Directory.GetDirectories(item))
                            DisplayItemsList.Add(new ProjectsModel(_item));
                }
        }

        // Multi Item projects code below
        private void Folders()
        {
            ItemsList.Clear();
            string[] parts = null; 
            if (SelectedItem != null)
            {
                foreach (var item in Directory.GetDirectories(SelectedItem.FullName))
                {
                    parts = item.Split('-');
                    if (parts[parts.Length - 1].Contains("Supplemental"))
                        foreach (var _item in Directory.GetDirectories(item))
                        {
                            ItemsList.Add(new ItemsModel(_item));
                        }
                    if (parts[parts.Length - 1].Contains("Incoming"))
                        foreach (var _item in Directory.GetDirectories(item))
                        {
                            ItemsList.Add(new ItemsModel(_item));
                        }
                    if (parts[parts.Length - 1].Contains("Outcoming"))
                        foreach (var _item in Directory.GetDirectories(item))
                        {
                            ItemsList.Add(new ItemsModel(_item));
                        }
                    else
                        ItemsList.Add(new ItemsModel(item));
                }

            }
        }
        #endregion
    }
}
