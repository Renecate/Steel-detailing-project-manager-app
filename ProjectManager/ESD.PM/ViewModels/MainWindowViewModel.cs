using ESD.PM.Commands;
using ESD.PM.Models;
using ESD.PM.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;




namespace ESD.PM.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Public Properties
        public ObservableCollection<ProjectsModel> ProjectsNames { get; set; } = [];
        public ObservableCollection<ProjectsModel> FoldersNames { get; set; } = [];
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
                if (_selectedProject != null)
                    ProjectIsTrue = true;
                OnPropertyChanged(nameof(ProjectIsTrue));
                GetFoldersOrItems();
            }
        }

        public ProjectsModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {   
                _selectedItem = value;
                GetFoldersIfItemsExist();
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
        public DelegateCommand UpdateCommand { get; set; }
        public DelegateCommand OpenProjectFolderCommand { get; set; }

        #endregion

        #region Constructor

        public MainWindowViewModel()
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
            UpdateCommand = new DelegateCommand(OnUpdate);
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
                if (parts[^1].Contains("Architectural"))
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
                if (parts[^1].Contains("Master"))
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", item));
                }
            }

        }
        private void OnOpenProjectFolder(object obj)
        {
            Process.Start(new ProcessStartInfo("explorer.exe", SelectedProject.FullName));
        }
        private void AddProjectPath(object path)
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
        private void AddFavoriteProject(object obj)
        {
            var selectedProject = _selectedProject;
            if (selectedProject != null)
            {
                if (!appSettings.FavoriteProjects.Contains(selectedProject.FullName))
                {
                    appSettings.FavoriteProjects.Add(selectedProject.FullName);
                    ProjectsNames.Insert(0, selectedProject);
                    OnPropertyChanged(nameof(ProjectsNames));
                }
                else
                {
                    appSettings.FavoriteProjects.Remove(selectedProject.FullName);
                    ProjectsNames.Remove(selectedProject);
                    SelectedProject = null;
                    ProjectIsTrue = false;
                    OnPropertyChanged(nameof(ProjectIsTrue));
                    OnPropertyChanged(nameof(SelectedProject));
                    OnPropertyChanged(nameof(ProjectsNames));
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
            OnPropertyChanged(nameof(DisplayItemsNames));
            OnPropertyChanged(nameof(SelectedItem));
        }
        private void OnUpdate(object obj)
        {
            var _selectedProj = SelectedProject;
            var _selectedItem = SelectedItem;
            if (SelectedItem != null)
            {
                SelectedItem = null;
                OnPropertyChanged(nameof(SelectedItem));
                SelectedItem = _selectedItem;
                OnPropertyChanged(nameof(SelectedItem));
            }
            else
            {
                SelectedProject = null;
                OnPropertyChanged(nameof(SelectedProject));
                SelectedProject = _selectedProj;
                OnPropertyChanged(nameof(SelectedProject));
            }
        }
        private void RemoveProjectPath(object path)
        {
            appSettings.ProjectPaths.Clear();
            appSettings.FavoriteProjects.Clear();
            SettingsManager.SaveSettings(appSettings);
            LoadProjectsAsync();
        }

        #endregion

        #region Private Methods

        private static void CreateSubfolders(string basePath)
        {
            Directory.CreateDirectory(Path.Combine(basePath, "0 - Scope Of Work"));
            Directory.CreateDirectory(Path.Combine(basePath, "1 - Incoming"));
            Directory.CreateDirectory(Path.Combine(basePath, "2 - Outcoming"));
            Directory.CreateDirectory(Path.Combine(basePath, "3 - Checking"));
            Directory.CreateDirectory(Path.Combine(basePath, "4 - MiscFiles"));
        }
        private void GetFoldersOrItems()
        {
            MasterIsTrue = false;
            StructIsTrue = false;
            ArchIsTrue = false;
            ItemsIsTrue = false;
            FoldersNames.Clear();
            OnPropertyChanged(nameof(StructIsTrue));
            OnPropertyChanged(nameof(MasterIsTrue));
            OnPropertyChanged(nameof(ArchIsTrue));
            OnPropertyChanged(nameof(ItemsIsTrue));
            OnPropertyChanged(nameof(SelectedItem));

            var count = 0;
            DisplayItemsNames.Clear();

            if (SelectedProject == null)
                return;

            foreach (var folder in Directory.GetDirectories(_selectedProject.FullName))
            {
                if (folder.EndsWith("Items"))
                {
                    count++;
                    ItemsIsTrue = true;
                    OnPropertyChanged(nameof(ItemsIsTrue));
                    break;
                }
                else
                {
                    FoldersNames.Add(new ProjectsModel(folder));
                }
            }
            if (count != 0)
            {
                foreach (var folder in Directory.GetDirectories(_selectedProject.FullName))
                    if (folder.EndsWith("Items"))
                        foreach (var _item in Directory.GetDirectories(folder))
                        {
                            DisplayItemsNames.Add(new ProjectsModel(_item));
                        }
            }

            {
                string[] parts = null;
                foreach (var folder in Directory.GetFiles(_selectedProject.FullName))
                {
                    parts = folder.Split('\\');
                    if (parts[parts.Length - 1].Contains("Structural"))
                    {
                        StructIsTrue = true;
                        OnPropertyChanged(nameof(StructIsTrue));
                    }
                    if (parts[parts.Length - 1].Contains("Architectural"))
                    {
                        ArchIsTrue = true;
                        OnPropertyChanged(nameof(ArchIsTrue));
                    }
                    parts = folder.Split('\\');
                    if (parts[parts.Length - 1].Contains("Master"))
                    {
                        MasterIsTrue = true;
                        OnPropertyChanged(nameof(MasterIsTrue));
                    }
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
                OnPropertyChanged(nameof(ProjectsNames));
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
        private void GetFoldersIfItemsExist()
        {
            FoldersNames.Clear();
            foreach (var folder in Directory.GetDirectories(_selectedItem.FullName))
            {
                FoldersNames.Add(new ProjectsModel(folder));
            }
        }
        #endregion

        #region Public Methods
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

    }
}