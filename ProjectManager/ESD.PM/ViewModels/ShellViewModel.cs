using ESD.PM.Commands;
using ESD.PM.Models;
using ESD.PM.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;




namespace ESD.PM.ViewModels
{
    public class ShellViewModel : Caliburn.Micro.Screen
    {
        #region Public Properties
        public ObservableCollection<FoldersModel> FoldersList { get; set; } = [];
        private ObservableCollection<ProjectsModel> DisplayItemsList { get; set; } = [];
        public ObservableCollection<ProjectsModel> ProjectsNames { get; set; } = [ ];
        public ObservableCollection<ProjectsModel> DisplayItemsNames { get; set; } = [ ];
        public bool ArchIsTrue { get; set; }
        public bool StructIsTrue { get; set; }
        public bool MasterIsTrue { get; set; }
        public bool ItemsIsTrue { get; set; }


        public ProjectsModel SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
                Items();
            }
        }

        public ProjectsModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                Folders();
            }
        }
        #endregion

        #region Private Properties

        private ProjectsModel _selectedProject;

        private ProjectsModel _selectedItem;

        private ProjectsModel _selectedFolder;

        private string _selectedProjectPath;

        private AppSettings appSettings;

        #endregion

        #region Commands

        public DelegateCommand StructuralOpenCommand { get; set; }

        public DelegateCommand ArchOpenCommand { get; set; }

        public DelegateCommand MasterOpenCommand { get; set; }

        public DelegateCommand AddProjectPathCommand {  get; set; }

        public DelegateCommand RemoveProjectPathCommand { get; set; }

        public DelegateCommand AddFavoriteProjectCommand { get; set; }

        public DelegateCommand CreateProjectCommand { get; set; }

        #endregion

        #region Constructor

        public ShellViewModel()
        {
            appSettings = SettingsManager.LoadSettings();
            LoadProjects();
            StructuralOpenCommand = new DelegateCommand(OnOpenStructural);
            ArchOpenCommand = new DelegateCommand(OnOpenArch);
            MasterOpenCommand = new DelegateCommand(OnOpenMaster);
            AddProjectPathCommand = new DelegateCommand(AddProjectPath);
            RemoveProjectPathCommand = new DelegateCommand(RemoveProjectPath);
            AddFavoriteProjectCommand = new DelegateCommand(AddFavoriteProject);
            CreateProjectCommand = new DelegateCommand(OnCreateProject);
        }

        #endregion

        #region Commands Methods
        private void OnOpenStructural(object obj)
        {
            string[] parts = null;
            foreach (var item in Directory.GetFiles(_selectedProject.FullName))
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
            foreach (var item in Directory.GetFiles(_selectedProject.FullName))
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
            foreach (var item in Directory.GetFiles(_selectedProject.FullName))
            {
                parts = item.Split('\\');
                if (parts[parts.Length - 1].Contains("Master"))
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", item));
                }
            }

        }
        public void AddProjectPath(object path)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Выберите папку с проектами";
                dialog.ShowNewFolderButton = false;

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    string selectedPath = dialog.SelectedPath;

                    if (!appSettings.ProjectPaths.Contains(selectedPath))
                    {
                        appSettings.ProjectPaths.Add(selectedPath);
                        SettingsManager.SaveSettings(appSettings);
                        LoadProjects();
                    }
                }
            }
        }

        public void RemoveProjectPath(object path)
        {
                appSettings.ProjectPaths.Clear();
                appSettings.FavoriteProjects.Clear();
                SettingsManager.SaveSettings(appSettings);
                LoadProjects();
        }

        private void AddFavoriteProject(object obj)
        {
            var selectedProject = _selectedProject;
            if (selectedProject != null)
            {
                if (!appSettings.FavoriteProjects.Contains(selectedProject.FullName))
                {
                    appSettings.FavoriteProjects.Add(selectedProject.FullName);
                    ProjectsNames.Insert(0, selectedProject);
                    NotifyOfPropertyChange(() => ProjectsNames);
                }
                else
                {
                    appSettings.FavoriteProjects.Remove(selectedProject.FullName);                 
                    ProjectsNames.Remove(selectedProject);
                    SelectedProject = null;
                    NotifyOfPropertyChange(() => SelectedProject);
                    NotifyOfPropertyChange(() => ProjectsNames);
                }
                SettingsManager.SaveSettings(appSettings);
            }
        }

        private void OnCreateProject(object obj)
        {
            var dialog = new CreateProjectDialog(appSettings.ProjectPaths);
            if (dialog.ShowDialog() == true)
            {
                string selectedPath = dialog.SelectedProjectPath;
                if (string.IsNullOrEmpty(selectedPath))
                {
                    System.Windows.MessageBox.Show("Please select a parent folder.");
                    return;
                }

                string projectPath = Path.Combine(selectedPath, dialog.ProjectName);
                Directory.CreateDirectory(projectPath);

                if (dialog.ItemsIncluded)
                {
                    string itemsPath = Path.Combine(projectPath, "Items");
                    Directory.CreateDirectory(itemsPath);

                    foreach (var item in dialog.Items)
                    {
                        string itemPath = Path.Combine(itemsPath, item.Trim());
                        Directory.CreateDirectory(itemPath);
                        CreateSubfolders(itemPath);
                    }
                }
                else
                {
                    CreateSubfolders(projectPath);
                }
                LoadProjects();
            }
        }

        #endregion

        #region Private Methods

        private void CreateSubfolders(string basePath)
        {
            Directory.CreateDirectory(Path.Combine(basePath, "1 - Incoming"));
            Directory.CreateDirectory(Path.Combine(basePath, "2 - Outcoming"));
            Directory.CreateDirectory(Path.Combine(basePath, "3 - Misc Files"));
        }

        private void Items()
        {
            MasterIsTrue = false;
            StructIsTrue = false;
            ArchIsTrue = false;
            ItemsIsTrue = false;
            SelectedItem = null;

            NotifyOfPropertyChange(() => StructIsTrue);
            NotifyOfPropertyChange(() => MasterIsTrue);
            NotifyOfPropertyChange(() => ArchIsTrue);
            NotifyOfPropertyChange(() => ItemsIsTrue);
            NotifyOfPropertyChange(() => SelectedItem);

            var count = 0;
            DisplayItemsList.Clear();
            FoldersList.Clear();
            DisplayItemsNames.Clear();

            if (SelectedProject == null)
                return;

            foreach (var item in Directory.GetDirectories(_selectedProject.FullName))
            {
                if (item.EndsWith("Items"))
                {
                    count++;
                    ItemsIsTrue = true;
                    NotifyOfPropertyChange(() => ItemsIsTrue);
                    break;
                }
            }
            if (count == 0)
            {
                foreach (var folder in Directory.GetDirectories(_selectedProject.FullName))
                {
                    FoldersList.Add(new FoldersModel(folder));
                }
            }
            else
            {
                foreach (var item in Directory.GetDirectories(_selectedProject.FullName))
                    if (item.EndsWith("Items"))
                        foreach (var _item in Directory.GetDirectories(item))
                        {
                            DisplayItemsList.Add(new ProjectsModel(_item));
                        }
                foreach (var item in DisplayItemsList)
                {
                    DisplayItemsNames.Add(item);
                }
            }

            {
                string[] parts = null;
                foreach (var item in Directory.GetFiles(_selectedProject.FullName))
                {
                    parts = item.Split('\\');
                    if (parts[parts.Length - 1].Contains("Structural"))
                    {
                        StructIsTrue = true;
                        NotifyOfPropertyChange(() => StructIsTrue);
                    }
                    if (parts[parts.Length - 1].Contains("Architectural"))
                    {
                        ArchIsTrue = true;
                        NotifyOfPropertyChange(() => ArchIsTrue);
                    }
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
            FoldersList.Clear();
            foreach (var item in DisplayItemsList)
                if (item == _selectedItem)
                    _selectedItem = item;
            if (_selectedItem != null)
            {
                foreach (var folder in Directory.GetDirectories(_selectedItem.FullName))
                {
                    FoldersList.Add(new FoldersModel(folder));
                }
            }
        }

        private void LoadProjects()
        {
            ProjectsNames.Clear();

            foreach (var path in appSettings.ProjectPaths)
            {
                if (Directory.Exists(path))
                {
                    foreach (var project in Directory.GetDirectories(path))
                    {
                        ProjectsNames.Add(new ProjectsModel(project));
                    }
                }
            }
            ProjectsNames = new ObservableCollection<ProjectsModel>(ProjectsNames.OrderBy(x => x.Name));
            NotifyOfPropertyChange(() => ProjectsNames);

            foreach (var project in appSettings.FavoriteProjects)
            {           
                ProjectsNames.Insert(0, new ProjectsModel(project));
            }
        }

        #endregion

        #region Public Methods

        #endregion
    }
}
