using ESD.PM.Commands;
using ESD.PM.Models;
using ESD.PM.Views;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Application = System.Windows.Application;


namespace ESD.PM.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Public Properties
        public ObservableCollection<ProjectsModel> ProjectsNames { get; set; } = [];
        public ObservableCollection<FoldersViewModel> Folders
        {
            get => folders;
            set
            {
                folders = value;
                OnPropertyChanged(nameof(Folders));
            }
        }
        public ObservableCollection<HiddenFoldersViewModel> HiddenFolders
        {
            get => hiddenFolders;
            set
            {
                hiddenFolders = value;
                OnPropertyChanged(nameof(HiddenFolders));
            }
        }
        public string FavoriteImageSourse { get; set; } = string.Empty;
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
                CheckIfFavorite();
                GetFoldersOrItemsAsync();
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

        private string _itemsPath;

        private AppSettings appSettings;

        private ObservableCollection<FoldersViewModel> folders;

        private ObservableCollection<HiddenFoldersViewModel> hiddenFolders;

        private ObservableCollection<ProjectsModel> favoriteProjectsList;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
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
        public DelegateCommand RemoveItemSelectionCommand { get; set; }

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            Folders = new ObservableCollection<FoldersViewModel>();
            HiddenFolders = new ObservableCollection<HiddenFoldersViewModel>();
            favoriteProjectsList = new ObservableCollection<ProjectsModel>();
            Folders.CollectionChanged += OnFoldersCollectionChanged;
            HiddenFolders.CollectionChanged += OnHiddenFoldersCollectionChanged;
            appSettings = SettingsManager.LoadSettings();

            FavoriteImageSourse = "/Views/Resourses/star.png";
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
            RemoveItemSelectionCommand = new DelegateCommand(OnRemoveItemSelection);
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
                    if (File.Exists(item))
                    {
                        Process.Start(new ProcessStartInfo("explorer.exe", item));
                    }
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
                    if (File.Exists(item))
                    {
                        Process.Start(new ProcessStartInfo("explorer.exe", item));
                    }
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
                    if (File.Exists(item))
                    {
                        Process.Start(new ProcessStartInfo("explorer.exe", item));
                    }
                }
            }

        }
        private void OnOpenProjectFolder(object obj)
        {
            if (Directory.Exists(SelectedProject.FullName))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", SelectedProject.FullName));
            }
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
                    foreach (var folder in Directory.GetDirectories(selectedProject.FullName))
                    {
                        if (folder.EndsWith("Items"))
                        {

                        }
                        else
                        {
                            var vm = new FoldersViewModel(folder, appSettings);
                            appSettings.SavedFolders.Add(vm);
                        }
                    }
                    if (ItemsIsTrue)
                    {
                        foreach (var itemFolder in Directory.GetDirectories(_itemsPath))
                        {
                            foreach (var folder in Directory.GetDirectories(itemFolder))
                            {
                                var vm = new FoldersViewModel(folder, appSettings);
                                appSettings.SavedFolders.Add(vm);
                            }
                        }
                    }
                    selectedProject.Favorite = true;
                    var index = ProjectsNames.IndexOf(selectedProject);
                    if (index != 0)
                    {
                        ProjectsNames.Move(index, 0);
                    }
                    OnPropertyChanged(nameof(ProjectsNames));
                }
                else
                {
                    selectedProject.Favorite = false;
                    favoriteProjectsList.Remove(selectedProject);
                    appSettings.FavoriteProjects.Remove(selectedProject.FullName);
                    var foldersToRemove = appSettings.SavedFolders
                                      .Where(f => f.FullName.StartsWith(selectedProject.FullName))
                                      .ToList();
                    foreach (var folder in foldersToRemove)
                    {
                        appSettings.SavedFolders.Remove(folder);
                    }

                    ProjectIsTrue = false;
                    LoadProjectsAsync();
                }
                SettingsManager.SaveSettings(appSettings);
                OnUpdate(this);
            }
            CheckIfFavorite();
        }
        private void OnCreateProject(object obj)
        {
            var dialog = new CreateProjectDialog(Application.Current.MainWindow, appSettings.ProjectPaths);
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
                    string itemsPath = Path.Combine(projectPath, "1 - Items");
                    Directory.CreateDirectory(itemsPath);
                    Directory.CreateDirectory(Path.Combine(projectPath, "0 - Scope Of Work"));
                    Directory.CreateDirectory(Path.Combine(projectPath, "2 - Personal Master Sets"));
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
            appSettings.SavedFolders.Clear();
            SettingsManager.SaveSettings(appSettings);
            LoadProjectsAsync();
        }
        private void OnRemoveItemSelection(object obj)
        {
            if (SelectedItem != null)
                GetFoldersOrItemsAsync();
        }
        #endregion

        #region Private Methods

        private void CheckIfFavorite()
        {
            if (_selectedProject != null)
            {
                if (_selectedProject.Favorite == true)
                {
                    FavoriteImageSourse = "/Views/Resourses/star_gold.png";
                    OnPropertyChanged(nameof(FavoriteImageSourse));
                }
                else
                {
                    FavoriteImageSourse = "/Views/Resourses/star.png";
                    OnPropertyChanged(nameof(FavoriteImageSourse));
                }
            }
            else
            {
                FavoriteImageSourse = "/Views/Resourses/star.png";
                OnPropertyChanged(nameof(FavoriteImageSourse));
            }
        }
        private static void CreateSubfolders(string basePath)
        {
            Directory.CreateDirectory(Path.Combine(basePath, "0 - Scope Of Work"));
            Directory.CreateDirectory(Path.Combine(basePath, "1 - Incoming"));
            Directory.CreateDirectory(Path.Combine(basePath, "2 - Outcoming"));
            Directory.CreateDirectory(Path.Combine(basePath, "3 - Checking"));
            Directory.CreateDirectory(Path.Combine(basePath, "4 - MiscFiles"));
        }
        private async Task GetFoldersOrItemsAsync()
        {
            var _instantlySelected = _selectedProject;

            await Task.Delay(50);

            if (_instantlySelected == _selectedProject)
            {
                MasterIsTrue = false;
                StructIsTrue = false;
                ArchIsTrue = false;
                ItemsIsTrue = false;
                Folders.Clear();
                HiddenFolders.Clear();

                var count = 0;
                DisplayItemsNames.Clear();

                if (SelectedProject != null)
                {
                    if (Directory.Exists(SelectedProject.FullName))
                    {
                        foreach (var folder in Directory.GetDirectories(_selectedProject.FullName))
                        {
                            if (folder.EndsWith("Items"))
                            {
                                count++;
                                ItemsIsTrue = true;
                                _itemsPath = folder;
                            }
                            else
                            {
                                var vm = new FoldersViewModel(folder, appSettings);
                                Folders.Add(vm);
                                if (vm.HideFolder == true)
                                {
                                    OnFolderPropertyChanged(vm, new PropertyChangedEventArgs(nameof(vm.HideFolder)));
                                }
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
                                }
                                if (parts[parts.Length - 1].Contains("Architectural"))
                                {
                                    ArchIsTrue = true;
                                }
                                parts = folder.Split('\\');
                                if (parts[parts.Length - 1].Contains("Master"))
                                {
                                    MasterIsTrue = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        LoadProjectsAsync();
                    }
                }
                OnPropertyChanged(nameof(StructIsTrue));
                OnPropertyChanged(nameof(MasterIsTrue));
                OnPropertyChanged(nameof(ArchIsTrue));
                OnPropertyChanged(nameof(ItemsIsTrue));
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
        private async Task LoadProjectsAsync()
        {
            try
            {
                favoriteProjectsList.Clear();
                ProjectsNames.Clear();

                var favoriteProjectsSet = new HashSet<string>(appSettings.FavoriteProjects);
                var tasks = new List<Task>();

                if (!appSettings.ProjectPaths.Any())
                { AddProjectPath(this); }
                else
                {
                    foreach (var path in appSettings.ProjectPaths)
                    {
                        if (Directory.Exists(path))
                        {
                            tasks.Add(Task.Run(async () =>
                            {
                                var projects = Directory.GetDirectories(path);
                                foreach (var project in projects)
                                {
                                    var projectToAdd = new ProjectsModel(project);
                                    projectToAdd.Favorite = favoriteProjectsSet.Contains(project);

                                    await Application.Current.Dispatcher.InvokeAsync(() =>
                                    {
                                        if (projectToAdd.Favorite)
                                        {
                                            favoriteProjectsList.Add(projectToAdd);
                                        }
                                        else
                                        {
                                            ProjectsNames.Add(projectToAdd);
                                        }
                                    });
                                }
                            }));
                        }
                    }
                    await Task.WhenAll(tasks);

                    var sortedProjects = ProjectsNames.OrderBy(x => x.Name).ToList();

                    var combinedProjects = favoriteProjectsList.Concat(sortedProjects).ToList();


                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        SelectedProject = combinedProjects[0];
                        ProjectsNames = new ObservableCollection<ProjectsModel>(combinedProjects);
                        OnPropertyChanged(nameof(ProjectsNames));
                        OnPropertyChanged(nameof(SelectedProject));
                        var mainWindow = Application.Current.MainWindow;
                        mainWindow.Activate();
                        mainWindow.Focus();
                    });
                }
            }
            catch (Exception ex)
            {

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
            HiddenFolders.Clear();
            Folders.Clear();
            if (_selectedItem != null)
            {
                if ((Directory.Exists(SelectedItem.FullName)))
                {
                    foreach (var folder in Directory.GetDirectories(_selectedItem.FullName))
                    {
                        var vm = new FoldersViewModel(folder, appSettings);
                        Folders.Add(vm);
                        if (vm.HideFolder == true)
                        {
                            OnFolderPropertyChanged(vm, new PropertyChangedEventArgs(nameof(vm.HideFolder)));
                        }
                    }
                }
                else
                {
                    SelectedItem = null;
                    LoadProjectsAsync();
                }
            }
        }
        private void OnFoldersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (FoldersViewModel folder in e.NewItems)
                {
                    folder.PropertyChanged += OnFolderPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (FoldersViewModel folder in e.OldItems)
                {
                    folder.PropertyChanged -= OnFolderPropertyChanged;
                }
            }
        }
        private void OnHiddenFoldersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (HiddenFoldersViewModel folder in e.NewItems)
                {
                    folder.PropertyChanged += OnFolderPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (HiddenFoldersViewModel folder in e.OldItems)
                {
                    folder.PropertyChanged -= OnFolderPropertyChanged;
                }
            }
        }
        private void OnFolderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FoldersViewModel.HideFolder))
            {
                var folder = sender as FoldersViewModel;
                if (folder.HideFolder)
                {
                    HiddenFolders.Add(new HiddenFoldersViewModel(folder.FullName));
                    Folders.Remove(folder);
                }
            }
            if (e.PropertyName == nameof(HiddenFoldersViewModel.ShowFolder))
            {
                var folder = sender as HiddenFoldersViewModel;
                if (folder.ShowFolder)
                {
                    HiddenFolders.Remove(folder);
                    var vm = (new FoldersViewModel(folder.FullName, appSettings));
                    Folders.Add(vm);
                    vm.HideFolder = false;
                    if (vm.FolderSettings != null)
                    {
                        var settingsPoint = appSettings.SavedFolders.IndexOf(vm.FolderSettings);
                        appSettings.SavedFolders[settingsPoint].HideFolder = vm.HideFolder;
                        SettingsManager.SaveSettings(appSettings);
                    }
                }
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
