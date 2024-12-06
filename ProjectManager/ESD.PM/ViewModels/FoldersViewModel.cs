using ESD.PM.Commands;
using ESD.PM.Settings;
using ESD.PM.ViewModels;
using ESD.PM.Views;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using Path = System.IO.Path;

namespace ESD.PM.Models
{
    public class FoldersViewModel : INotifyPropertyChanged
    {
        #region Public Properties

        [JsonProperty]
        public string FullName { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonIgnore]
        public ObservableCollection<FoldersModel> SubFolderList { get; set; }

        [JsonIgnore]
        public ObservableCollection<FoldersModel> FilteredDocsList { get; set; }

        [JsonIgnore]
        public ObservableCollection<FoldersModel> TaggedDocsList { get; set; }

        [JsonIgnore]
        public ObservableCollection<FoldersModel> UntaggedDocsList { get; set; }

        [JsonIgnore]
        public List<string> PathList { get; set; }

        [JsonProperty]
        public ObservableCollection<TagsModel> Tags { get; set; }

        [JsonIgnore]
        public bool ToggleViewCommandActive { get; set; }

        [JsonProperty]
        public bool HideFolderIsTrue
        {
            get { return _hideFolderIsTrue; }
            set
            {
                if (_hideFolderIsTrue != value)
                {
                    _hideFolderIsTrue = value;
                    OnPropertyChanged(nameof(HideFolderIsTrue));
                }
            }
        }

        [JsonProperty]
        public bool ViewIsToggled
        {
            get { return _viewIsToggled; }
            set
            {
                if (_viewIsToggled != value)
                {
                    _viewIsToggled = value;
                    OnPropertyChanged(nameof(ViewIsToggled));
                }
            }
        }

        [JsonIgnore]
        public FoldersModel SelectedFolderName
        {
            get { return _selectedFolderName; }
            set
            {
                _selectedFolderName = value;
                OnPropertyChanged(nameof(SelectedFolderName));
            }
        }

        [JsonIgnore]
        public FoldersViewModel FolderSettings { get; set; }

        [JsonProperty]
        public bool DateSortIsTrue { get; set; }

        [JsonProperty]
        public bool HideNumbersIsTrue
        {
            get { return _hideNumbersIsTrue; }
            set
            {
                if (_hideNumbersIsTrue != value)
                {
                    _hideNumbersIsTrue = value;
                    OnPropertyChanged(nameof(HideNumbersIsTrue));
                }
            }
        }

        [JsonIgnore]
        public string DateSortButtonSourse { get; set; } = "/Views/Resourses/sort_date.png";

        [JsonIgnore]
        public string HideNumbersButtonSourse { get; set; } = "/Views/Resourses/numbers_on.png";

        #endregion

        #region Commands

        [JsonIgnore]
        public DelegateCommand OpenCommand { get; set; }

        [JsonIgnore]
        public DelegateCommand OpenFolderCommand { get; set; }

        [JsonIgnore]
        public DelegateCommand ToggleViewCommand { get; set; }

        [JsonIgnore]
        public DelegateCommand CopyPathCommand { get; set; }

        [JsonIgnore]
        public DelegateCommand DateSortCommand { get; set; }

        [JsonIgnore]
        public DelegateCommand FileDropCommand { get; set; }

        [JsonIgnore]
        public DelegateCommand HideFolderCommand { get; set; }

        [JsonIgnore]
        public DelegateCommand RenameFolderCommand { get; set; }

        [JsonIgnore]
        public DelegateCommand CreateFolderCommand { get; set; }

        [JsonIgnore]
        public DelegateCommand HideNumbersCommand { get; set; }

        [JsonIgnore]
        public DelegateCommand FileEmptyDropCommand { get; set; }


        #endregion

        #region Private Properties

        private FoldersModel _selectedFolderName { get; set; }

        private bool _viewIsToggled;
        private bool _hideFolderIsTrue;
        private bool _hideNumbersIsTrue;

        private string _dynamicSearchText;

        private ObservableCollection<FoldersModel> _iterationList { get; set; }
        private ObservableCollection<TagsModel> _tagsToRemove { get; set; }
        private FoldersSettings _foldersSettings { get; set; }

        private AppSettings _appSettings { get; set; }

        #endregion

        #region Constructor

        public FoldersViewModel(string fullName, AppSettings appSettings)
        {

            _foldersSettings = FoldersSettingsManager.LoadSettings();

            if (_foldersSettings != null)
            {
                foreach (var folder in _foldersSettings.SavedFolders)
                {
                    if (folder.FullName == fullName)
                    {
                        FolderSettings = folder;
                        break;
                    }
                }
            }

            if (_appSettings != null)
            {
                _appSettings = appSettings;
            }

            FullName = fullName;
            Name = new DirectoryInfo(fullName).Name;

            DateSortIsTrue = false;
            HideFolderIsTrue = false;
            HideNumbersIsTrue = false;
            _viewIsToggled = false;

            PathList = new List<string>();
            SubFolderList = new ObservableCollection<FoldersModel>();
            Tags = new ObservableCollection<TagsModel>();
            FilteredDocsList = new ObservableCollection<FoldersModel>();
            UntaggedDocsList = new ObservableCollection<FoldersModel>();
            TaggedDocsList = new ObservableCollection<FoldersModel>();
            _tagsToRemove = new ObservableCollection<TagsModel>();
            _iterationList = new ObservableCollection<FoldersModel>();

            ViewIsHiddenOrToggledCheck();
            GetSubFolders();
            ToggleViewCheck();

            OpenCommand = new DelegateCommand(OnOpen);
            ToggleViewCommand = new DelegateCommand(OnToggleView);
            OpenFolderCommand = new DelegateCommand(OnOpenFolder);
            CopyPathCommand = new DelegateCommand(OnCopyPath);
            DateSortCommand = new DelegateCommand(OnDateSort);
            FileDropCommand = new DelegateCommand(OnFileDrop);
            HideFolderCommand = new DelegateCommand(OnHideFolder);
            RenameFolderCommand = new DelegateCommand(OnRenameFolder);
            CreateFolderCommand = new DelegateCommand(OnCreateFolder);
            HideNumbersCommand = new DelegateCommand(OnHideNumbers);
            FileEmptyDropCommand = new DelegateCommand(OnFileEmptyDrop);

        }

        #endregion

        #region Private Methods
        private static DateTime ExtrateDate(string doc)
        {
            var parts = doc.Split('-');
            foreach (var part in parts)
            {
                if (DateTime.TryParseExact(part.Trim(' '), "MM.dd.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    return date;
                }
            }
            return Directory.GetCreationTime(doc);
        }

        private static int ExtractNumber(string doc)
        {
            var match = Regex.Match(doc, @"\((\d+)\)");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            return int.MinValue;
        }

        private void TagStateChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State")
            {
                FilterSubFolders();
                var changedTag = (TagsModel)sender;
                if (FolderSettings != null)
                {
                    var settingsIndex = _foldersSettings.SavedFolders.IndexOf(FolderSettings);
                    _foldersSettings.SavedFolders[settingsIndex].Tags = FolderSettings.Tags;
                    FoldersSettingsManager.SaveSettings(_foldersSettings);
                }
            }
        }

        private void FilterSubFolders()
        {
            string[] files = [];
            string[] parts = [];
            var _filterdDocsList = FilteredDocsList;
            _tagsToRemove.Clear();
            TaggedDocsList.Clear();
            foreach (var tag in Tags)
            {
                var count = 0;
                foreach (var doc in SubFolderList)
                {
                    if (tag.State is true)
                    {
                        files = doc.Name.Split('\\');
                        parts = files[^1].Split("-");
                        foreach (var part in parts)
                        {
                            var partTrimmed = part.Trim();
                            if (partTrimmed.Equals(tag.Name))
                            {
                                TaggedDocsList.Add(doc);
                                count++;
                                break;
                            }
                        }
                    }
                }
                if (count == 0 && tag.State is true)
                {
                    _tagsToRemove.Add(tag);
                }
            }

            foreach (var tag in _tagsToRemove)
            {
                Tags.Remove(tag);
            }

            FilteredDocsList = new ObservableCollection<FoldersModel>(UntaggedDocsList.Concat(TaggedDocsList));

            if (_filterdDocsList.Count == 0)
                _filterdDocsList = FilteredDocsList;

            if (!DateSortIsTrue)
            {
                FilteredDocsList = new ObservableCollection<FoldersModel>(UntaggedDocsList.Concat(TaggedDocsList)
                    .OrderBy(a => ExtrateDate(a.Name))
                    .ThenBy(a => ExtractNumber(a.Name)));
            }
            else
            {
                FilteredDocsList = new ObservableCollection<FoldersModel>(UntaggedDocsList.Concat(TaggedDocsList)
                    .OrderByDescending(a => ExtrateDate(a.Name))
                    .ThenByDescending(a => ExtractNumber(a.Name)));
            }

            if (_dynamicSearchText != null)
            {
                if (_dynamicSearchText != null)
                {
                    foreach (var folder in SubFolderList)
                    {
                        if (!(folder.Name.Contains(_dynamicSearchText, StringComparison.OrdinalIgnoreCase)))
                        {
                            FilteredDocsList.Remove(folder);
                        }
                    }
                }
            }

            if (_hideNumbersIsTrue == true)
            {
                foreach (var folder in FilteredDocsList)
                {
                    var nameParts = folder.Name.Split('-');
                    if (nameParts.Length > 1)
                    {
                        var numberPart = nameParts[0]
                            .Replace("(", "")
                            .Replace(")", "")
                            .Trim();

                        if (int.TryParse(numberPart, out _))
                        {
                            folder.Name = folder.Name.Substring(nameParts[0].Length + 1).TrimStart('-');
                        }
                    }
                }
            }


            OnPropertyChanged(nameof(FilteredDocsList));
        }

        private void ToggleViewCheck()
        {
            if (ViewIsToggled)
            {
                ToggleViewCommandActive = true;
            }
            else
            {
                if (Tags.Count > 0)
                {
                    ToggleViewCommandActive = false;
                    OnPropertyChanged(nameof(ToggleViewCommandActive));
                }

                else if (Tags.Count == 0 && SubFolderList.Count() == 0)
                {
                    ToggleViewCommandActive = false;
                    OnPropertyChanged(nameof(ToggleViewCommandActive));
                }
                else
                {
                    ToggleViewCommandActive = true;
                    OnPropertyChanged(nameof(ToggleViewCommandActive));
                }
            }
        }

        private void GetSubFolders()
        {
            PathList.Clear();
            _iterationList = new ObservableCollection<FoldersModel>();
            if (Directory.Exists(FullName))
            {
                foreach (var item in Directory.GetDirectories(FullName))
                {
                    _iterationList.Add(new FoldersModel(item));
                }
            }
            if (_viewIsToggled == false)
            {
                SubFolderList.Clear();
                PathList.Add(FullName);
                SubFolderList = new ObservableCollection<FoldersModel>(_iterationList);

            }
            if (_viewIsToggled == true)
            {
                SubFolderList.Clear();
                foreach (var folder in _iterationList)
                {
                    PathList.Add(folder.FullName);
                    foreach (var insideFolder in Directory.GetDirectories(folder.FullName))
                    {
                        SubFolderList.Add(new FoldersModel(insideFolder));
                    }
                }
            }
            ProcessLocalList();
            FilterSubFolders();
        }

        private void ProcessLocalList()
        {
            UntaggedDocsList.Clear();
            if (FolderSettings != null)
            {
                Tags = FolderSettings.Tags;
            }
            foreach (var folder in SubFolderList)
            {
                var parts = folder.Name.Split("-");
                if (parts.Length > 1)
                {
                    string tag = parts[1].Trim(' ');
                    if (tag.Length == 2)
                    {
                        AddTagIfNotExist(tag);
                    }
                    else
                    {
                        if (!UntaggedDocsList.Any(t => t.FullName == folder.FullName))
                        {
                            UntaggedDocsList.Add(folder);
                        }
                    }
                }
                else
                {
                    if (!UntaggedDocsList.Any(t => t.FullName == folder.FullName))
                    {
                        UntaggedDocsList.Add(folder);
                    }
                }
            }
            UpdateFilteredDocsList();
        }

        private void AddTagIfNotExist(string tag)
        {
            if (!Tags.Any(t => t.Name == tag))
            {
                Tags.Add(new TagsModel(tag));
                if (FolderSettings != null)
                {
                    var settingsIndex = _foldersSettings.SavedFolders.IndexOf(FolderSettings);
                    _foldersSettings.SavedFolders[settingsIndex].Tags = FolderSettings.Tags;
                    FoldersSettingsManager.SaveSettings(_foldersSettings);
                }
            }
        }

        private void UpdateFilteredDocsList()
        {

            if (!DateSortIsTrue)
            {
                FilteredDocsList = new ObservableCollection<FoldersModel>(UntaggedDocsList.Concat(TaggedDocsList)
                    .OrderBy(a => ExtrateDate(a.Name))
                    .ThenBy(a => ExtractNumber(a.Name)));
            }
            else
            {
                FilteredDocsList = new ObservableCollection<FoldersModel>(UntaggedDocsList.Concat(TaggedDocsList)
                    .OrderByDescending(a => ExtrateDate(a.Name))
                    .ThenByDescending(a => ExtractNumber(a.Name)));
            }

            OnPropertyChanged(nameof(FilteredDocsList));
            OnPropertyChanged(nameof(Tags));
            foreach (var item in Tags)
            {
                item.PropertyChanged += TagStateChanged;
            }
        }

        private int GetOrderNumber()
        {
            Regex regex = new Regex(@"\((\d+)\)");

            var numbers = GetOriginalList()
            .Select(proj => regex.Match(proj.Name))
            .Where(match => match.Success)
            .Select(match => int.Parse(match.Groups[1].Value))
            .ToList();

            if (numbers.Count == 0)
            {
                return 1;
            }

            int maxNumber = numbers.Max();
            return maxNumber + 1;
        }

        private string GetNextRfiNumber()
        {
            Regex regex = new Regex(@"RFI\s(\d+(\.\d+)?)");

            var numbers = GetOriginalList()
                .Select(proj => regex.Match(proj.Name))
                .Where(match => match.Success)
                .Select(match => match.Groups[1].Value)
                .ToList();

            if (numbers.Count == 0)
            {
                return "1";
            }

            numbers.Sort(CompareRfiNumbers);

            string maxNumber = numbers.Last();
            string nextNumber = IncrementRfiNumber(maxNumber);
            return $"{nextNumber}";
        }

        private int CompareRfiNumbers(string rfi1, string rfi2)
        {
            string[] parts1 = rfi1.Split('.');
            string[] parts2 = rfi2.Split('.');

            int integerPart1 = int.Parse(parts1[0]);
            int integerPart2 = int.Parse(parts2[0]);

            int result = integerPart1.CompareTo(integerPart2);
            if (result != 0)
            {
                return result;
            }

            if (parts1.Length == 1 && parts2.Length == 1)
            {
                return 0;
            }
            if (parts1.Length == 1)
            {
                return -1;
            }
            if (parts2.Length == 1)
            {
                return 1;
            }

            int decimalPart1 = int.Parse(parts1[1]);
            int decimalPart2 = int.Parse(parts2[1]);

            return decimalPart1.CompareTo(decimalPart2);
        }

        private string IncrementRfiNumber(string currentNumber)
        {
            if (!currentNumber.Contains('.'))
            {
                int integerPart = int.Parse(currentNumber);
                return (integerPart + 1).ToString();
            }
            else
            {
                string[] parts = currentNumber.Split('.');
                int integerPart = int.Parse(parts[0]);
                int decimalPart = int.Parse(parts[1]);
                decimalPart++;
                return $"{integerPart}.{decimalPart}";
            }
        }

        private ObservableCollection<FoldersModel> GetOriginalList()
        {
            var localIterationList = new ObservableCollection<FoldersModel>();
            var localList = new ObservableCollection<FoldersModel>();
            if (Directory.Exists(FullName))
            {
                foreach (var item in Directory.GetDirectories(FullName))
                {
                    localIterationList.Add(new FoldersModel(item));
                }
            }
            if (_viewIsToggled == false)
            {
                localList = new ObservableCollection<FoldersModel>(localIterationList);
            }
            if (_viewIsToggled == true)
            {

                foreach (var folder in localIterationList)
                {
                    foreach (var insideFolder in Directory.GetDirectories(folder.FullName))
                    {
                        localList.Add(new FoldersModel(insideFolder));
                    }
                }
            }

            return localList;
        }

        private void ViewIsHiddenOrToggledCheck()
        {
            if (FolderSettings != null)
            {
                if (FolderSettings.HideFolderIsTrue)
                {
                    OnHideFolder(this);
                }
                if (FolderSettings.ViewIsToggled)
                {
                    _viewIsToggled = FolderSettings.ViewIsToggled;
                }
                if (FolderSettings.DateSortIsTrue)
                {
                    DateSortIsTrue = FolderSettings.DateSortIsTrue;
                    if (DateSortIsTrue)
                    {
                        DateSortButtonSourse = "/Views/Resourses/sort_date_dark.png";
                    }
                    else
                    {
                        DateSortButtonSourse = "/Views/Resourses/sort_date.png";
                    }
                }
                if (FolderSettings.HideNumbersIsTrue)
                {
                    _hideNumbersIsTrue = FolderSettings.HideNumbersIsTrue;
                    if (_hideNumbersIsTrue)
                    {
                        HideNumbersButtonSourse = "/Views/Resourses/numbers_off.png";
                    }
                    else
                    {
                        HideNumbersButtonSourse = "/Views/Resourses/numbers_on.png";
                    }
                }
            }
            OnPropertyChanged(nameof(DateSortButtonSourse));
            OnPropertyChanged(nameof(HideNumbersButtonSourse));
        }


        #endregion

        #region Commands Methods

        private void OnOpen(object obj)
        {
            if (_selectedFolderName != null)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "explorer.exe";
                startInfo.Arguments = "\"" + _selectedFolderName.FullName + "\"";
                Process.Start(startInfo);
            }
        }

        private void OnToggleView(object obj)
        {
            if (_viewIsToggled == true)
            {
                FilteredDocsList.Clear();
                Tags.Clear();
                UntaggedDocsList.Clear();
                TaggedDocsList.Clear();
                _viewIsToggled = false;
                if (FolderSettings != null)
                {
                    var settingsIndex = _foldersSettings.SavedFolders.IndexOf(FolderSettings);
                    _foldersSettings.SavedFolders[settingsIndex].ViewIsToggled = _viewIsToggled;
                    FoldersSettingsManager.SaveSettings(_foldersSettings);
                }
                GetSubFolders();
                ProcessLocalList();
                FilterSubFolders();
            }
            else if (_viewIsToggled == false)
            {
                FilteredDocsList.Clear();
                Tags.Clear();
                UntaggedDocsList.Clear();
                TaggedDocsList.Clear();
                _viewIsToggled = true;
                if (FolderSettings != null)
                {
                    var settingsIndex = _foldersSettings.SavedFolders.IndexOf(FolderSettings);
                    _foldersSettings.SavedFolders[settingsIndex].ViewIsToggled = _viewIsToggled;
                    FoldersSettingsManager.SaveSettings(_foldersSettings);
                }
                GetSubFolders();
                ProcessLocalList();
                FilterSubFolders();
            }

        }

        private void OnOpenFolder(object obj)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "explorer.exe";
            startInfo.Arguments = "\"" + FullName + "\"";
            Process.Start(startInfo);
        }

        private void OnCopyPath(object obj)
        {
            if (SelectedFolderName != null)
                Clipboard.SetText(SelectedFolderName.FullName);
        }

        private void OnDateSort(object obj)
        {
            if (DateSortIsTrue)
            {
                DateSortIsTrue = false;
                DateSortButtonSourse = "/Views/Resourses/sort_date.png";
                if (FolderSettings != null)
                {
                    var settingsIndex = _foldersSettings.SavedFolders.IndexOf(FolderSettings);
                    _foldersSettings.SavedFolders[settingsIndex].DateSortIsTrue = DateSortIsTrue;
                    FoldersSettingsManager.SaveSettings(_foldersSettings);
                }
            }
            else
            {
                DateSortIsTrue = true;
                DateSortButtonSourse = "/Views/Resourses/sort_date_dark.png";
                if (FolderSettings != null)
                {
                    var settingsIndex = _foldersSettings.SavedFolders.IndexOf(FolderSettings);
                    _foldersSettings.SavedFolders[settingsIndex].DateSortIsTrue = DateSortIsTrue;
                    FoldersSettingsManager.SaveSettings(_foldersSettings);
                }
            }
            OnPropertyChanged(nameof(DateSortButtonSourse));
            GetSubFolders();
        }

        private void OnFileDrop(object obj)
        {
            var pathCollection = obj as string[];
            if (pathCollection != null)
            {
                foreach (string path in pathCollection)
                {
                    var destination = SelectedFolderName.FullName + "\\" + Path.GetFileName(path);
                    if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                    {
                        Directory.Move(path, destination);
                    }
                    else if (Path.GetFileName(path) != null)
                    {
                        File.Move(path, destination);
                    }
                }
                GetSubFolders();
            }
        }

        private void OnFileEmptyDrop(object obj)
        {
            var pathCollection = obj as string[];
            if (pathCollection != null)
            {
                int orderNumber = GetOrderNumber();
                string rfiNumber = GetNextRfiNumber();
                var pathList = PathList;
                var tags = new List<string>();
                var dialog = new CreateFolderDialog(Application.Current.MainWindow, orderNumber, rfiNumber, pathList, tags);
                var existingDirectories = new List<string>();

                foreach (var existingDirectory in SubFolderList)
                {
                    existingDirectories.Add(existingDirectory.FullName);
                }

                var directoryPath = string.Empty;

                if (dialog.ShowDialog() == true)
                {
                    GetSubFolders();
                    foreach (var newDirectory in SubFolderList)
                    {
                        if (!existingDirectories.Contains(newDirectory.FullName))
                        {
                            directoryPath = newDirectory.FullName;
                            break;
                        }
                    }
                    foreach (string path in pathCollection)
                    {
                        var destination = directoryPath + "\\" + Path.GetFileName(path);
                        if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                        {
                            Directory.Move(path, destination);
                        }
                        else if (Path.GetFileName(path) != null)
                        {
                            File.Move(path, destination);
                        }
                    }
                }
                GetSubFolders();
                Application.Current.MainWindow.Focus();
            }
        }

        private void OnHideFolder(object obj)
        {
            HideFolderIsTrue = true;
            if (FolderSettings != null)
            {
                var settingsPoint = _foldersSettings.SavedFolders.IndexOf(FolderSettings);
                _foldersSettings.SavedFolders[settingsPoint].HideFolderIsTrue = HideFolderIsTrue;
                FoldersSettingsManager.SaveSettings(_foldersSettings);
            }
        }

        private void OnRenameFolder(object obj)
        {
            if (_selectedFolderName != null)
            {
                var name = new DirectoryInfo(_selectedFolderName.FullName).Name;
                var dialog = new RenameDialog(Application.Current.MainWindow, name);
                if (dialog.ShowDialog() == true)

                {
                    var rootPath = _selectedFolderName.FullName.Replace(name, "").TrimEnd('\\');
                    var newFolderName = dialog.NewFolderName;
                    if (Directory.Exists(rootPath + "\\" + newFolderName) != true)
                    {
                        try
                        {
                            Directory.Move(_selectedFolderName.FullName, rootPath + "\\" + newFolderName);
                            _selectedFolderName.Name = newFolderName;
                            _selectedFolderName.FullName = rootPath + "\\" + newFolderName;
                            GetSubFolders();
                        }
                        catch (Exception ex)
                        {
                            if (ex is System.IO.IOException)
                            {
                                System.Windows.MessageBox.Show("Access denied. Folder is locked by another program");
                            }
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show($"Folder '{newFolderName}' already exists");
                    }
                }
            }
            if (_viewIsToggled == false)
            {
                ToggleViewCheck();
            }
        }

        private void OnCreateFolder(object obj)
        {
            if (Directory.Exists(FullName) == true)
            {
                var templatesPath = "C:\\Dropbox\\technology\\Utilits\\ESD.PM";
                if (!(_appSettings.StructureTemplates.Any()) || !(_appSettings.PdfTemplates.Any()))
                {
                    if (Directory.Exists(templatesPath))
                    {
                        foreach (var templatesFolder in Directory.GetDirectories(templatesPath))
                        {
                            if (templatesFolder.Contains("folder", StringComparison.OrdinalIgnoreCase) && (!(_appSettings.StructureTemplates.Any())))
                            {
                                foreach (var template in Directory.GetDirectories(templatesFolder))
                                {
                                    _appSettings.StructureTemplates.Add(template);
                                }
                            }
                            else if (templatesFolder.Contains("project", StringComparison.OrdinalIgnoreCase) && (!(_appSettings.ProjectTemplates.Any())))
                            {
                                foreach (var template in Directory.GetDirectories(templatesFolder))
                                {
                                    _appSettings.ProjectTemplates.Add(template);
                                }
                            }
                            else if (templatesFolder.Contains("rfi", StringComparison.OrdinalIgnoreCase) && (!(_appSettings.RfiTemplates.Any())))
                            {
                                foreach (var template in Directory.GetFiles(templatesFolder))
                                {
                                    _appSettings.RfiTemplates.Add(template);
                                }
                            }
                            else if (templatesFolder.Contains("pdf", StringComparison.OrdinalIgnoreCase) && (!(_appSettings.PdfTemplates.Any())))
                            {
                                foreach (var template in Directory.GetFiles(templatesFolder))
                                {
                                    _appSettings.PdfTemplates.Add(template);
                                }
                            }
                        }
                    }
                    AppSettingsManager.SaveSettings(_appSettings);
                }
                int orderNumber = GetOrderNumber();
                string rfiNumber = GetNextRfiNumber();
                var pathList = PathList;
                var tags = new List<string>();
                if (Tags != null)
                {
                    foreach (var tag in Tags)
                    {
                        tags.Add(tag.Name);
                    }
                }
                var dialog = new CreateFolderDialog(Application.Current.MainWindow, orderNumber, rfiNumber, pathList, tags);
                if (dialog.ShowDialog() == true)
                {
                    GetSubFolders();
                }
            }
            if (_viewIsToggled == false)
            {
                ToggleViewCheck();
            }
        }

        private void OnHideNumbers(object obj)
        {
            if (HideNumbersIsTrue)
            {
                HideNumbersIsTrue = false;
                HideNumbersButtonSourse = "/Views/Resourses/numbers_on.png";
                if (FolderSettings != null)
                {
                    var settingsIndex = _foldersSettings.SavedFolders.IndexOf(FolderSettings);
                    _foldersSettings.SavedFolders[settingsIndex].HideNumbersIsTrue = HideNumbersIsTrue;
                    FoldersSettingsManager.SaveSettings(_foldersSettings);
                }
            }
            else
            {
                HideNumbersIsTrue = true;
                HideNumbersButtonSourse = "/Views/Resourses/numbers_off.png";
                if (FolderSettings != null)
                {
                    var settingsIndex = _foldersSettings.SavedFolders.IndexOf(FolderSettings);
                    _foldersSettings.SavedFolders[settingsIndex].HideNumbersIsTrue = HideNumbersIsTrue;
                    FoldersSettingsManager.SaveSettings(_foldersSettings);
                }
            }
            OnPropertyChanged(nameof(HideNumbersButtonSourse));
            GetSubFolders();
        }
        #endregion

        #region Public Methods
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void DynamicSearch(string text)
        {
            _dynamicSearchText = text;
            GetSubFolders();
        }
        #endregion
    }
}
