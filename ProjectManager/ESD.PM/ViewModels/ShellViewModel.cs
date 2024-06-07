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
        public ObservableCollection<ProjectsModel> ProjectsNames { get; set; } = [];
        public ObservableCollection<ProjectsModel> DisplayItemsNames { get; set; } = [];
        public bool ArchIsTrue { get; set; }
        public bool StructIsTrue { get; set; }
        public bool MasterIsTrue { get; set; }
        public bool ItemsIsTrue { get; set; }
        public bool ProjectIsTrue { get; set; }


        public ProjectsModel SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
                ProjectIsTrue = true;
                NotifyOfPropertyChange(() => ProjectIsTrue);
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
        public DelegateCommand AddProjectPathCommand { get; set; }
        public DelegateCommand RemoveProjectPathCommand { get; set; }
        public DelegateCommand AddFavoriteProjectCommand { get; set; }
        public DelegateCommand CreateProjectCommand { get; set; }
        public DelegateCommand AddItemCommand { get; set; }

        public DelegateCommand OpenProjectFolderCommand { get; set; }

        #endregion

        #region Constructor

        public ShellViewModel()
        {
            appSettings = SettingsManager.LoadSettings();
            ProjectIsTrue = false;
            LoadProjectsAsync();
            StructuralOpenCommand = new DelegateCommand(OnOpenStructural);
            ArchOpenCommand = new DelegateCommand(OnOpenArch);
            MasterOpenCommand = new DelegateCommand(OnOpenMaster);
            AddProjectPathCommand = new DelegateCommand(AddProjectPath);
            RemoveProjectPathCommand = new DelegateCommand(RemoveProjectPath);
            AddFavoriteProjectCommand = new DelegateCommand(AddFavoriteProject);
            CreateProjectCommand = new DelegateCommand(OnCreateProject);
            AddItemCommand = new DelegateCommand(OnAddItem);
            OpenProjectFolderCommand = new DelegateCommand(OnOpenProjectFolder);
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
        private void OnOpenProjectFolder(object obj)
        {
            Process.Start(new ProcessStartInfo("explorer.exe", SelectedProject.FullName));
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

                    if (!appSettings.ProjectPaths.Contains(selectedPath) && selectedPath.EndsWith("Projects"))
                    {
                        appSettings.ProjectPaths.Add(selectedPath);
                        SettingsManager.SaveSettings(appSettings);
                        LoadProjectsAsync();
                    }
                    else System.Windows.MessageBox.Show($"Folder '{selectedPath}' is not Projects folder");
                }
            }
        }
        public void RemoveProjectPath(object path)
        {
            appSettings.ProjectPaths.Clear();
            appSettings.FavoriteProjects.Clear();
            SettingsManager.SaveSettings(appSettings);
            LoadProjectsAsync();
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
                    ProjectIsTrue = false;
                    NotifyOfPropertyChange(() => ProjectIsTrue);
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
                LoadProjectsAsync();
            }
        }
        private void OnAddItem(object obj)
        {
            string itemsPath = Path.Combine(SelectedProject.FullName, "Items");
            ObservableCollection<string> existingItemNames = new ObservableCollection<string>();

            if (Directory.Exists(itemsPath))
            {
                foreach (var directory in Directory.GetDirectories(itemsPath))
                {
                    existingItemNames.Add(Path.GetFileName(directory));
                }
            }

            var dialog = new AddItemDialog(existingItemNames);
            if (dialog.ShowDialog() == true)
            {
                var itemNames = dialog.ItemNames.Where(item => !string.IsNullOrWhiteSpace(item.Name)).Select(item => item.Name).ToList();
                CreateItemsFolders(itemNames);
                DisplayItemsNames.Clear();
                foreach (var item in Directory.GetDirectories(itemsPath))
                    DisplayItemsNames.Add(new ProjectsModel(item));
                DisplayItemsNames = new ObservableCollection<ProjectsModel>(DisplayItemsNames.OrderBy(item => item.Name));
            }
            NotifyOfPropertyChange(() => DisplayItemsNames);
            NotifyOfPropertyChange(() => SelectedItem);
        }
        #endregion

        #region Private Methods

        private void CreateSubfolders(string basePath)
        {
            Directory.CreateDirectory(Path.Combine(basePath, "0 - Scope Of Work"));
            Directory.CreateDirectory(Path.Combine(basePath, "1 - Incoming"));
            Directory.CreateDirectory(Path.Combine(basePath, "2 - Outcoming"));
            Directory.CreateDirectory(Path.Combine(basePath, "3 - Checking"));
            Directory.CreateDirectory(Path.Combine(basePath, "4 - MiscFiles"));
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
                            DisplayItemsNames.Add(new ProjectsModel(_item));
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
            foreach (var item in DisplayItemsNames)
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

        private async Task LoadProjectsAsync()
        {
            try
            {
                ProjectsNames.Clear();
                foreach (var path in appSettings.ProjectPaths)
                {
                    if (Directory.Exists(path))
                    {
                        var projects = await Task.Run(() => Directory.GetDirectories(path));
                        foreach (var project in projects)
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
            catch (Exception ex)
            {
                // Логирование ошибки или уведомление пользователя
            }
        }
        private void CreateItemsFolders(List<string> itemNames)
        {
            string itemsPath = Path.Combine(SelectedProject.FullName, "Items");
            if (!Directory.Exists(itemsPath))
            {
                Directory.CreateDirectory(itemsPath);
            }

            foreach (var itemName in itemNames)
            {
                string itemPath = Path.Combine(itemsPath, itemName);
                if (!Directory.Exists(itemPath))
                {
                    Directory.CreateDirectory(itemPath);
                    CreateSubfolders(itemPath);
                }
            }
        }
        #endregion

        #region Public Methods

        #endregion
        
    }
}