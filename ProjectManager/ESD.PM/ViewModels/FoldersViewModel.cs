using ESD.PM.Commands;
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
        public ObservableCollection<FoldersModel> FolderList { get; set; }

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

        #endregion

        #region Private Properties

        private FoldersModel _selectedFolderName { get; set; }

        private bool _viewIsToggled;
        private bool _hideFolderIsTrue;
        private bool _hideNumbersIsTrue;

        private string _dynamicSearchText;

        private ObservableCollection<FoldersModel> _iterationList { get; set; }
        private ObservableCollection<TagsModel> _tagsToRemove { get; set; }
        private AppSettings _appSettings { get; set; }

        #endregion

        #region Constructor

        public FoldersViewModel(string fullName, AppSettings appSettings)
        {

            if (appSettings != null)
            {
                foreach (var folder in appSettings.SavedFolders)
                {
                    if (folder.FullName == fullName)
                    {
                        FolderSettings = folder;
                        break;
                    }
                }
                _appSettings = appSettings;
            }

            FullName = fullName;
            Name = new DirectoryInfo(fullName).Name;

            DateSortIsTrue = false;
            HideFolderIsTrue = false;
            HideNumbersIsTrue = false;
            _viewIsToggled = false;

            PathList = new List<string>();
            FolderList = new ObservableCollection<FoldersModel>();
            Tags = new ObservableCollection<TagsModel>();
            FilteredDocsList = new ObservableCollection<FoldersModel>();
            UntaggedDocsList = new ObservableCollection<FoldersModel>();
            TaggedDocsList = new ObservableCollection<FoldersModel>();
            _tagsToRemove = new ObservableCollection<TagsModel>();
            _iterationList = new ObservableCollection<FoldersModel>();

            ViewIsHiddenOrToggledCheck();
            GetFolders();
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
            return File.GetCreationTime(doc);
        }

        private void TagStateChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State")
            {
                FilterFolders();
                var changedTag = (TagsModel)sender;
                if (FolderSettings != null)
                {
                    var settingsIndex = _appSettings.SavedFolders.IndexOf(FolderSettings);
                    _appSettings.SavedFolders[settingsIndex].Tags = FolderSettings.Tags;
                    SettingsManager.SaveSettings(_appSettings);
                }
            }
        }

        private void FilterFolders()
        {
            string[] files = [];
            string[] parts = [];
            var _filterdDocsList = FilteredDocsList;
            _tagsToRemove.Clear();
            TaggedDocsList.Clear();
            foreach (var tag in Tags)
            {
                var count = 0;
                foreach (var doc in FolderList)
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
                FilteredDocsList = new ObservableCollection<FoldersModel>(UntaggedDocsList.Concat(TaggedDocsList).OrderBy(a => ExtrateDate(a.Name)));
            }
            else
            {
                FilteredDocsList = new ObservableCollection<FoldersModel>(UntaggedDocsList.Concat(TaggedDocsList).OrderByDescending(a => ExtrateDate(a.Name)));
            }

            if (_dynamicSearchText != null)
            {
                if (_dynamicSearchText != null)
                {
                    foreach (var folder in FolderList)
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

                else if (Tags.Count == 0 && FolderList.Count() == 0)
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

        private void GetFolders()
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
                FolderList.Clear();
                PathList.Add(FullName);
                FolderList = new ObservableCollection<FoldersModel>(_iterationList);

            }
            if (_viewIsToggled == true)
            {
                FolderList.Clear();
                foreach (var folder in _iterationList)
                {
                    PathList.Add(folder.FullName);
                    foreach (var insideFolder in Directory.GetDirectories(folder.FullName))
                    {
                        FolderList.Add(new FoldersModel(insideFolder));
                    }
                }
            }
            ProcessLocalList();
            FilterFolders();
        }

        private void ProcessLocalList()
        {
            UntaggedDocsList.Clear();
            if (FolderSettings != null)
            {
                Tags = FolderSettings.Tags;
            }
            foreach (var folder in FolderList)
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
                    var settingsIndex = _appSettings.SavedFolders.IndexOf(FolderSettings);
                    _appSettings.SavedFolders[settingsIndex].Tags = FolderSettings.Tags;
                    SettingsManager.SaveSettings(_appSettings);
                }
            }
        }

        private void UpdateFilteredDocsList()
        {

            if (!DateSortIsTrue)
            {
                FilteredDocsList = new ObservableCollection<FoldersModel>(UntaggedDocsList.Concat(TaggedDocsList).OrderBy(a => ExtrateDate(a.Name)));
                OnPropertyChanged(nameof(FilteredDocsList));
            }
            else
            {
                FilteredDocsList = new ObservableCollection<FoldersModel>(UntaggedDocsList.Concat(TaggedDocsList).OrderByDescending(a => ExtrateDate(a.Name)));
                OnPropertyChanged(nameof(FilteredDocsList));
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

            var numbers = localList
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
                    var settingsIndex = _appSettings.SavedFolders.IndexOf(FolderSettings);
                    _appSettings.SavedFolders[settingsIndex].ViewIsToggled = _viewIsToggled;
                    SettingsManager.SaveSettings(_appSettings);
                }
                GetFolders();
                ProcessLocalList();
                FilterFolders();
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
                    var settingsIndex = _appSettings.SavedFolders.IndexOf(FolderSettings);
                    _appSettings.SavedFolders[settingsIndex].ViewIsToggled = _viewIsToggled;
                    SettingsManager.SaveSettings(_appSettings);
                }
                GetFolders();
                ProcessLocalList();
                FilterFolders();
            }

        }

        private void OnOpenFolder(object obj)
        {
            Process.Start(new ProcessStartInfo("explorer.exe", FullName));
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
                    var settingsIndex = _appSettings.SavedFolders.IndexOf(FolderSettings);
                    _appSettings.SavedFolders[settingsIndex].DateSortIsTrue = DateSortIsTrue;
                    SettingsManager.SaveSettings(_appSettings);
                }
            }
            else
            {
                DateSortIsTrue = true;
                DateSortButtonSourse = "/Views/Resourses/sort_date_dark.png";
                if (FolderSettings != null)
                {
                    var settingsIndex = _appSettings.SavedFolders.IndexOf(FolderSettings);
                    _appSettings.SavedFolders[settingsIndex].DateSortIsTrue = DateSortIsTrue;
                    SettingsManager.SaveSettings(_appSettings);
                }
            }
            OnPropertyChanged(nameof(DateSortButtonSourse));
            GetFolders();
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
                GetFolders();
            }
        }

        private void OnHideFolder(object obj)
        {
            HideFolderIsTrue = true;
            if (FolderSettings != null)
            {
                var settingsPoint = _appSettings.SavedFolders.IndexOf(FolderSettings);
                _appSettings.SavedFolders[settingsPoint].HideFolderIsTrue = HideFolderIsTrue;
                SettingsManager.SaveSettings(_appSettings);
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
                            GetFolders();
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
                int orderNumber = GetOrderNumber();
                var pathList = PathList;
                var tags = new List<string>();
                if (Tags != null)
                {
                    foreach (var tag in Tags)
                    {
                        tags.Add(tag.Name);
                    }
                }
                var dialog = new CreateFolderDialog(Application.Current.MainWindow, orderNumber, pathList, tags, _appSettings);
                dialog.ShowDialog();
                //var localTags = string.Empty;
                //if (Tags != null)
                //{
                //    foreach (var tag in Tags)
                //    {
                //        if (!dialog.FolderTag.Any(t => t == tag.Name))
                //        {
                //            dialog.FolderTag.Add(tag.Name);
                //        }
                //    }
                //}
                //dialog.FolderTag.Sort();
                //if (dialog.ShowDialog() == true)
                //{
                //    if (dialog.SelectedTag != null && dialog.SelectedTag.Length > 1) 
                //    {
                //        dialog.SelectedTag = dialog.SelectedTag.Substring(0, 2);
                //    }

                //    var collection = new List<string>()
                //    {
                //        dialog.OrderNumber,
                //        dialog.SelectedTag,
                //        dialog.Date,
                //        dialog.FolderName
                //    };
                //    var newFolderName = string.Empty;
                //    collection = new List<string>(collection.Where(n => !string.IsNullOrEmpty(n)));
                //    var count = 0;
                //    foreach (var item in collection)
                //    {
                //        count++;
                //        if (count != collection.Count)
                //        {
                //            newFolderName += item + " - ";
                //        }
                //        else
                //        {
                //            newFolderName += item;
                //        }
                //    }
                //    var path = dialog.SelectedPath + "\\" + newFolderName;
                //    if (Directory.Exists(path) != true)
                //    {
                //        Directory.CreateDirectory(path);
                //        GetFolders();
                //        if (dialog.RfiState)
                //        {
                //            var pdfGenerator = new PdfGenerator();
                //            pdfGenerator.CreatePdfFromTemplate("C:\\Dropbox\\technology\\Standarts\\BlueBeam Templates\\ESD RC Form.pdf", path + "\\" + newFolderName + ".pdf");
                //        }
                //    }
                //    else
                //        System.Windows.MessageBox.Show($"Folder '{newFolderName}' already exists");
                //}
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
                    var settingsIndex = _appSettings.SavedFolders.IndexOf(FolderSettings);
                    _appSettings.SavedFolders[settingsIndex].HideNumbersIsTrue = HideNumbersIsTrue;
                    SettingsManager.SaveSettings(_appSettings);
                }
            }
            else
            {
                HideNumbersIsTrue = true;
                HideNumbersButtonSourse = "/Views/Resourses/numbers_off.png";
                if (FolderSettings != null)
                {
                    var settingsIndex = _appSettings.SavedFolders.IndexOf(FolderSettings);
                    _appSettings.SavedFolders[settingsIndex].HideNumbersIsTrue = HideNumbersIsTrue;
                    SettingsManager.SaveSettings(_appSettings);
                }
            }
            OnPropertyChanged(nameof(HideNumbersButtonSourse));
            GetFolders();
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
            GetFolders();
        }
        #endregion
    }
}
