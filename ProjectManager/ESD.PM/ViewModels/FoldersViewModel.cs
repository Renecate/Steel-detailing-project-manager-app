using ESD.PM.Commands;
using ESD.PM.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Path = System.IO.Path;

namespace ESD.PM.Models
{
    public class FoldersViewModel : ProjectsModel
    {
        #region Public Properties

        public ObservableCollection<ProjectsModel> FolderList { get; set; }
        public ObservableCollection<ProjectsModel> FilteredDocsList { get; set; }
        public ObservableCollection<ProjectsModel> TaggedDocsList { get; set; }
        public ObservableCollection<ProjectsModel> UntaggedDocsList { get; set; }
        public List<string> PathList { get; set; }
        public ObservableCollection<TagsModel> Tags { get; set; }
        public bool ToggleViewCommandActive { get; set; }
        public bool HideFolder
        {
            get => hideFolder;
            set
            {
                if (hideFolder != value)
                {
                    hideFolder = value;
                    OnPropertyChanged(nameof(HideFolder));
                }
            }
        }
        public ProjectsModel SelectedFolderName
        {
            get { return _selectedFolderName; }
            set
            {
                _selectedFolderName = value;
                OnPropertyChanged(nameof(SelectedFolderName));
            }
        }

        #endregion

        #region Commands

        public DelegateCommand OpenCommand { get; set; }
        public DelegateCommand OpenFolderCommand { get; set; }
        public DelegateCommand ToggleViewCommand { get; set; }
        public DelegateCommand CopyPathCommand { get; set; }
        public DelegateCommand DateSortCommand { get; set; }
        public DelegateCommand FileDropCommand { get; set; }
        public DelegateCommand HideFolderCommand { get; set; }
        public DelegateCommand RenameFolderCommand { get; set; }
        public DelegateCommand CreateFolderCommand {  get; set; }
        #endregion

        #region Private Properties

        private ProjectsModel _selectedFolderName { get; set; }
        private bool _viewIsToggled;
        private bool hideFolder;
        private ObservableCollection<ProjectsModel> _iterationList { get; set; }
        private ObservableCollection<TagsModel> _tagsToRemove { get; set; }

        #endregion

        #region Constructor

        public FoldersViewModel(string name) : base(name)
        {
            hideFolder = false;
            _viewIsToggled = false;
            PathList = new List<string>();
            FolderList = new ObservableCollection<ProjectsModel>();
            Tags = new ObservableCollection<TagsModel>();
            FilteredDocsList = new ObservableCollection<ProjectsModel>();
            UntaggedDocsList = new ObservableCollection<ProjectsModel>();
            TaggedDocsList = new ObservableCollection<ProjectsModel>();
            _tagsToRemove = new ObservableCollection<TagsModel>();
            _iterationList = new ObservableCollection<ProjectsModel>();

            GetFolders();
            ProcessLocalList();
            FilterFolders();
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
                bool newState = changedTag.State;
                string tagName = changedTag.Name;
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

            FilteredDocsList = new ObservableCollection<ProjectsModel>(UntaggedDocsList.Concat(TaggedDocsList));
            if (_filterdDocsList.Count == 0)
                _filterdDocsList = FilteredDocsList;
            if (_filterdDocsList.Count > 0)
            {
                if (ExtrateDate(_filterdDocsList.First().Name) < ExtrateDate(_filterdDocsList.Last().Name))
                {
                    FilteredDocsList = new ObservableCollection<ProjectsModel>(UntaggedDocsList.Concat(TaggedDocsList).OrderBy(a => ExtrateDate(a.Name)));
                }
                else
                {
                    FilteredDocsList = new ObservableCollection<ProjectsModel>(UntaggedDocsList.Concat(TaggedDocsList).OrderByDescending(a => ExtrateDate(a.Name)));
                }
            }
            OnPropertyChanged(nameof(FilteredDocsList));
        }

        private void ToggleViewCheck()
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

        private void GetFolders()
        {
            PathList.Clear();
            if (_viewIsToggled == false)
            {
                FolderList.Clear();
                PathList.Add(FullName);
                foreach (var item in Directory.GetDirectories(FullName))
                    FolderList.Add(new ProjectsModel(item));
                _iterationList = new ObservableCollection<ProjectsModel>(FolderList);
            }
            if (_viewIsToggled == true) 
            {
                FolderList.Clear();
                foreach (var folder in _iterationList)
                {
                    PathList.Add(folder.FullName);
                    foreach (var insideFolder in Directory.GetDirectories(folder.FullName))
                    {
                        FolderList.Add(new ProjectsModel(insideFolder));
                    }
                }
            }
        }

        private void ProcessLocalList()
        {
            UntaggedDocsList.Clear();
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
            }
        }

        private void UpdateFilteredDocsList()
        {
            if (FilteredDocsList.Count > 1)
            {
                if (ExtrateDate(FilteredDocsList.First().Name) < ExtrateDate(FilteredDocsList.Last().Name))
                {

                    FilteredDocsList = new ObservableCollection<ProjectsModel>(UntaggedDocsList.Concat(TaggedDocsList).OrderBy(a => ExtrateDate(a.Name)));
                    OnPropertyChanged(nameof(FilteredDocsList));
                }
                else
                {
                    FilteredDocsList = new ObservableCollection<ProjectsModel>(UntaggedDocsList.Concat(TaggedDocsList).OrderByDescending(a => ExtrateDate(a.Name)));
                    OnPropertyChanged(nameof(FilteredDocsList));
                }
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

            var numbers = FolderList
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
        #endregion

        #region Commands Methods

        private void OnOpen(object obj)
        {
            if (_selectedFolderName != null)
                Process.Start(new ProcessStartInfo("explorer.exe", _selectedFolderName.FullName));
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
            if (FilteredDocsList.Count > 1)
            {
                if (ExtrateDate(FilteredDocsList.First().Name) < ExtrateDate(FilteredDocsList.Last().Name))
                {
                    FilteredDocsList = new ObservableCollection<ProjectsModel>(UntaggedDocsList.Concat(TaggedDocsList).OrderByDescending(a => ExtrateDate(a.Name)));
                    OnPropertyChanged(nameof(FilteredDocsList));
                }
                else
                {
                    FilteredDocsList = new ObservableCollection<ProjectsModel>(UntaggedDocsList.Concat(TaggedDocsList).OrderBy(a => ExtrateDate(a.Name)));
                    OnPropertyChanged(nameof(FilteredDocsList));
                }
            }
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
            }
        }

        private void OnHideFolder(object obj)
        {
            HideFolder = true;
        }

        private void OnRenameFolder(object obj)
        {
            if (_selectedFolderName != null)
            {
                var dialog = new RenameDialog(_selectedFolderName.Name);
                if (dialog.ShowDialog() == true)
                {
                    var rootPath = _selectedFolderName.FullName.Replace(_selectedFolderName.Name, "").TrimEnd('\\');
                    var newFolderName = dialog.NewFolderName;
                    if (Directory.Exists(rootPath + "\\" + newFolderName) != true)
                    {
                        Directory.Move(_selectedFolderName.FullName, rootPath + "\\" + newFolderName);
                        _selectedFolderName.Name = newFolderName;
                        _selectedFolderName.FullName = rootPath + "\\" + newFolderName;
                        ProcessLocalList();
                        FilterFolders();
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
                var dialog = new CreateFolderDialog(orderNumber, pathList);
                if (dialog.ShowDialog() == true)
                {
                    var collection = new List<string>()
                    {
                        dialog.OrderNumber,
                        dialog.FolderTag,
                        dialog.Date,
                        dialog.FolderName
                    };
                    var newFolderName = string.Empty;
                    collection = new List<string>(collection.Where(n => !string.IsNullOrEmpty(n)));
                    var count = 0;
                    foreach (var item in collection) 
                    { 
                        count++;
                        if (count != collection.Count)
                        {
                            newFolderName += item + " - ";
                        }
                        else
                        {
                            newFolderName += item;
                        }
                    }
                    var path = dialog.SelectedPath + "\\" + newFolderName;
                    if (Directory.Exists(path) != true)
                    {
                        Directory.CreateDirectory(path);
                        GetFolders();
                        ProcessLocalList();
                        FilterFolders();
                    }
                    else
                        System.Windows.MessageBox.Show($"Folder '{newFolderName}' already exists");
                }
            }
            if (_viewIsToggled == false)
            {
                ToggleViewCheck();
            }
        }
        #endregion
    }
}
