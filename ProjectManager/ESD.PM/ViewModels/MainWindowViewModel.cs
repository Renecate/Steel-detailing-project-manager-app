using ESD.PM.Commands;
using ESD.PM.Models;
using ESD.PM.Views;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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

        private SharedSettings sharedSettings;
        #endregion

        #region Commands

        public DelegateCommand StructuralOpenCommand { get; set; }
        public DelegateCommand ArchOpenCommand { get; set; }
        public DelegateCommand MasterOpenCommand { get; set; }
        public DelegateCommand AddFavoriteProjectCommand { get; set; }
        public DelegateCommand CreateProjectCommand { get; set; }
        public DelegateCommand AddItemCommand { get; set; }
        public DelegateCommand UpdateCommand { get; set; }
        public DelegateCommand OpenProjectFolderCommand { get; set; }
        public DelegateCommand RemoveItemSelectionCommand { get; set; }
        public DelegateCommand OpenSettingsCommand { get; set; }

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            Folders = new ObservableCollection<FoldersViewModel>();
            HiddenFolders = new ObservableCollection<HiddenFoldersViewModel>();
            favoriteProjectsList = new ObservableCollection<ProjectsModel>();
            Folders.CollectionChanged += OnFoldersCollectionChanged;
            HiddenFolders.CollectionChanged += OnHiddenFoldersCollectionChanged;
            sharedSettings = ServerSettingsManager.LoadSettings();
            appSettings = SettingsManager.LoadSettings();

            CheckIfUserExists();
            CheckIfSharedSettingsAvailable();
            LoadProjectsAsync();

            FavoriteImageSourse = "/Views/Resourses/star.png";
            ProjectIsTrue = false;

            StructuralOpenCommand = new DelegateCommand(OnOpenStructural);
            ArchOpenCommand = new DelegateCommand(OnOpenArch);
            MasterOpenCommand = new DelegateCommand(OnOpenMaster);
            AddFavoriteProjectCommand = new DelegateCommand(AddFavoriteProject);
            CreateProjectCommand = new DelegateCommand(OnCreateProject);
            AddItemCommand = new DelegateCommand(OnAddItem);
            OpenProjectFolderCommand = new DelegateCommand(OnOpenProjectFolder);
            UpdateCommand = new DelegateCommand(OnUpdate);
            RemoveItemSelectionCommand = new DelegateCommand(OnRemoveItemSelection);
            OpenSettingsCommand = new DelegateCommand(OnOpenSettings);
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
                            var vm = new FoldersViewModel(folder, appSettings, sharedSettings);
                            appSettings.SavedFolders.Add(vm);
                        }
                    }
                    if (ItemsIsTrue)
                    {
                        foreach (var itemFolder in Directory.GetDirectories(_itemsPath))
                        {
                            foreach (var folder in Directory.GetDirectories(itemFolder))
                            {
                                var vm = new FoldersViewModel(folder, appSettings, sharedSettings);
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
            var templatesPath = "C:\\Dropbox\\technology\\Utilits\\ESD.PM";
            if (!(appSettings.ProjectTemplates.Any()))
            {
                if (Directory.Exists(templatesPath))
                {
                    foreach (var templatesFolder in Directory.GetDirectories(templatesPath))
                    {
                        if (templatesFolder.Contains("folder", StringComparison.OrdinalIgnoreCase) && (!(appSettings.StructureTemplates.Any())))
                        {
                            foreach (var template in Directory.GetDirectories(templatesFolder))
                            {
                                appSettings.StructureTemplates.Add(template);
                            }
                        }
                        else if (templatesFolder.Contains("project", StringComparison.OrdinalIgnoreCase) && (!(appSettings.ProjectTemplates.Any())))
                        {
                            foreach (var template in Directory.GetDirectories(templatesFolder))
                            {
                                appSettings.ProjectTemplates.Add(template);
                            }
                        }
                        else if (templatesFolder.Contains("rfi", StringComparison.OrdinalIgnoreCase) && (!(appSettings.RfiTemplates.Any())))
                        {
                            foreach (var template in Directory.GetFiles(templatesFolder))
                            {
                                appSettings.RfiTemplates.Add(template);
                            }
                        }
                        else if (templatesFolder.Contains("pdf", StringComparison.OrdinalIgnoreCase) && (!(appSettings.PdfTemplates.Any())))
                        {
                            foreach (var template in Directory.GetFiles(templatesFolder))
                            {
                                appSettings.PdfTemplates.Add(template);
                            }
                        }
                    }
                }
                SettingsManager.SaveSettings(appSettings);
            }
            var dialog = new CreateProjectDialog(Application.Current.MainWindow, appSettings);
            if (dialog.ShowDialog() == true)
            {
                LoadProjectsAsync();
            }
        }

         private void OnAddItem(object obj)
        {
            var dialog = new AddItemDialog(Application.Current.MainWindow, _itemsPath);
            if (dialog.ShowDialog() == true)
            {
                var selectedProject = _selectedProject;
                if (selectedProject != null)
                {
                    if (SelectedProject.Favorite)
                    {
                        if (ItemsIsTrue)
                        {
                            foreach (var itemFolder in Directory.GetDirectories(_itemsPath))
                            {
                                foreach (var folder in Directory.GetDirectories(itemFolder))
                                {
                                    var vm = new FoldersViewModel(folder, appSettings, sharedSettings);
                                    var isSaved = false;
                                    foreach (var saved in appSettings.SavedFolders)
                                    {
                                        if (saved.FullName == vm.FullName)
                                        {
                                            isSaved = true;
                                        }
                                    }
                                    if (!isSaved)
                                    {
                                        appSettings.SavedFolders.Add(vm);
                                    }
                                }
                            }
                        }
                    }
                    SettingsManager.SaveSettings(appSettings);
                    LoadProjectsAsync();
                }
            }
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
        private void OnRemoveItemSelection(object obj)
        {
            if (SelectedItem != null)
                GetFoldersOrItemsAsync();
        }
        private void OnOpenSettings(object obj)
        {
            var owner = Application.Current.MainWindow;
            var settingsDialog = new SettingsWindow(owner);
            if (settingsDialog.ShowDialog() == true)
            {
                appSettings = SettingsManager.LoadSettings();
                LoadProjectsAsync();
            }
        }
        #endregion

        #region Private Methods

        private static int ExtractNumber(string doc)
        {
            var match = Regex.Match(doc, @"Item (\d+)");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            return int.MinValue;
        }
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
                            if (folder.EndsWith("Items", StringComparison.OrdinalIgnoreCase))
                            {
                                ItemsIsTrue = true;
                                _itemsPath = folder;
                            }
                            else
                            {
                                var vm = new FoldersViewModel(folder, appSettings, sharedSettings);
                                Folders.Add(vm);
                                if (vm.HideFolderIsTrue == true)
                                {
                                    OnFolderPropertyChanged(vm, new PropertyChangedEventArgs(nameof(vm.HideFolderIsTrue)));
                                }
                            }
                        }
                        if (ItemsIsTrue)
                        {
                            foreach (var _item in Directory.GetDirectories(_itemsPath))
                            {
                                DisplayItemsNames.Add(new ProjectsModel(_item));
                            }
                            DisplayItemsNames = new ObservableCollection<ProjectsModel>(DisplayItemsNames.OrderBy(a => ExtractNumber(a.Name)));
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
                OnPropertyChanged(nameof(DisplayItemsNames));
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
                { AddProjectPath(); }
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
                        var vm = new FoldersViewModel(folder, appSettings, sharedSettings);
                        Folders.Add(vm);
                        if (vm.HideFolderIsTrue == true)
                        {
                            OnFolderPropertyChanged(vm, new PropertyChangedEventArgs(nameof(vm.HideFolderIsTrue)));
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
            if (e.PropertyName == nameof(FoldersViewModel.HideFolderIsTrue))
            {
                var folder = sender as FoldersViewModel;
                if (folder.HideFolderIsTrue)
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
                    var vm = (new FoldersViewModel(folder.FullName, appSettings, sharedSettings));
                    Folders.Add(vm);
                    vm.HideFolderIsTrue = false;
                    if (vm.FolderSettings != null)
                    {
                        var settingsPoint = appSettings.SavedFolders.IndexOf(vm.FolderSettings);
                        appSettings.SavedFolders[settingsPoint].HideFolderIsTrue = vm.HideFolderIsTrue;
                        SettingsManager.SaveSettings(appSettings);
                    }
                }
            }
        }
        private void AddProjectPath()
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
        private void CheckIfUserExists()
        {
            if (appSettings.User == null)
            {
                appSettings.User = Environment.UserName;
                SettingsManager.SaveSettings(appSettings);
            }
        }
        private void CheckIfSharedSettingsAvailable()
        {
            if (sharedSettings != null)
            {
                sharedSettings.Available = true;
                ServerSettingsManager.SaveSettings(sharedSettings);
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
