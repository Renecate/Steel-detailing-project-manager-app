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
        private ObservableCollection<ProjectsModel> ProjectsList { get; set; } = [];
        public ObservableCollection<ItemsModel> ItemsList { get; set; } = [];
        private ObservableCollection<ProjectsModel> DisplayItemsList { get; set; } = [];
        public ObservableCollection<String> ProjectsNames { get; set; } = [ ];
        public ObservableCollection<String> DisplayItemsNames { get; set; } = [ ];
        public ProjectsModel SelectedProject { get; set; }
        public ProjectsModel SelectedItem { get; set; }
        public bool ArchIsTrue { get; set; }
        public bool StructIsTrue { get; set; }
        public bool MasterIsTrue { get; set; }


        public string SelectedProjectName
        {
            get { return _selectedProjectName; }
            set
            {
                _selectedProjectName = value;
                Items();
                NotifyOfPropertyChange(() => SelectedProjectName);
            }
        }

        public string SelectedItemName
        {
            get { return _selectedItemName; }
            set
            {
                _selectedItemName = value;
                Folders();
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }
        #endregion

        #region Private Properties

        private string _selectedProjectName;

        private string _selectedItemName;

        private ProjectsModel _selectedFolder;

        #endregion

        #region Commands

        public DelegateCommand StructuralOpenCommand { get; set; }

        public DelegateCommand ArchOpenCommand { get; set; }

        public DelegateCommand MasterOpenCommand { get; set; }

        #endregion

        #region Constructor

        public ShellViewModel()
        {
            Projects();
            StructuralOpenCommand = new DelegateCommand(OnOpenStructural);
            ArchOpenCommand = new DelegateCommand(OnOpenArch);
            MasterOpenCommand = new DelegateCommand(OnOpenMaster);
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

        private void OnOpenMaster(object obj)
        {
            string[] parts = null;
            foreach (var item in Directory.GetFiles(SelectedProject.FullName))
            {
                parts = item.Split('\\');
                if (parts[parts.Length - 1].Contains("Master"))
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
            foreach (var project in Directory.GetDirectories("C:\\Dropbox\\Projects"))
            {
                ProjectsList.Add(new ProjectsModel(project));
            }
            foreach (var project in ProjectsList)
            {
                ProjectsNames.Add(project.Name);
            }
            ProjectsNames = new ObservableCollection<string>(ProjectsNames.OrderBy(x => x));
            NotifyOfPropertyChange(() => ProjectsNames);
        }

        public void Items()
        {
            MasterIsTrue = false;
            StructIsTrue = false;
            ArchIsTrue = false;
            NotifyOfPropertyChange(() => StructIsTrue);
            NotifyOfPropertyChange(() => MasterIsTrue);
            NotifyOfPropertyChange(() => ArchIsTrue);
            var count = 0;
            DisplayItemsList.Clear();
            ItemsList.Clear();
            foreach (var project in ProjectsList)
                if (project.Name == _selectedProjectName)
                    SelectedProject = project;
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
                        {
                            DisplayItemsList.Add(new ProjectsModel(_item));
                        }
                foreach (var item in DisplayItemsList)
                {
                    DisplayItemsNames.Add(item.Name);
                }
                }
            {
                string[] parts = null;
                foreach (var item in Directory.GetFiles(SelectedProject.FullName))
                {
                    parts = item.Split('\\');
                    if (parts[parts.Length - 1].Contains("Structural"))
                    {
                        StructIsTrue = true;
                        NotifyOfPropertyChange(() => StructIsTrue);
                    }
                }
                foreach (var item in Directory.GetFiles(SelectedProject.FullName))
                {
                    parts = item.Split('\\');
                    if (parts[parts.Length - 1].Contains("Architectural"))
                    {
                        ArchIsTrue = true;
                        NotifyOfPropertyChange(() => ArchIsTrue);
                    }
                }
                foreach (var item in Directory.GetFiles(SelectedProject.FullName))
                {
                    parts = item.Split('\\');
                    if (parts[parts.Length - 1].Contains("Master"))
                    {
                        MasterIsTrue = true;
                        NotifyOfPropertyChange(() => MasterIsTrue);
                    }
                }
            }
        }

        private void Folders()
        {
            ItemsList.Clear();
            foreach (var item in DisplayItemsList)
                if (item.Name == _selectedItemName)
                    SelectedItem = item;
            if (SelectedItem != null)
            {
                foreach (var item in Directory.GetDirectories(SelectedItem.FullName))
                {
                    ItemsList.Add(new ItemsModel(item));
                    NotifyOfPropertyChange(() => ItemsList);
                }
            }
        }
        #endregion
    }
}
