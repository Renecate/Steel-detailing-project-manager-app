using ESD.PM.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace ESD.PM.Models
{
    public class FoldersModel : ProjectsModel
    {
        #region Public Properties

        public ObservableCollection<ProjectsModel> FolderList { get; set; }
        public ObservableCollection<ProjectsModel> FilteredDocsList { get; set; }
        public ObservableCollection<ProjectsModel> TaggedDocsList { get; set; }
        public ObservableCollection<ProjectsModel> UntaggedDocsList {  get; set; }
        public ObservableCollection<TagsModel> Tags { get; set; }
        public bool ToggleViewCommandActive { get; set; }
        public ProjectsModel SelectedFolderName
        {
            get { return _selectedFolderName; }
            set
            {
                _selectedFolderName = value;
            }
        }

        #endregion

        #region Commands

        public DelegateCommand OpenCommand { get; set; }
        public DelegateCommand OpenFolderCommand { get; set; }
        public DelegateCommand ToggleViewCommand { get; set; }

        #endregion

        #region Private Properties

        private ProjectsModel _selectedFolderName;

        private bool _viewIsToggled;

        private string _location;

        private ObservableCollection<ProjectsModel> _localList {  get; set; }

        private ObservableCollection<ProjectsModel> _localListClearable { get; set; }

        #endregion

        #region Constructor

        public FoldersModel(string name) : base(name)
        {
            ToggleViewCommandActive = true;
            _viewIsToggled = false;
            _location = FullName;
            FolderList = new ObservableCollection<ProjectsModel>();
            Tags = new ObservableCollection<TagsModel>();
            FilteredDocsList = new ObservableCollection<ProjectsModel>();
            UntaggedDocsList = new ObservableCollection<ProjectsModel>();
            TaggedDocsList = new ObservableCollection<ProjectsModel>();
            _localList = new ObservableCollection<ProjectsModel>();
            _localListClearable = new ObservableCollection<ProjectsModel>();
            GetFolders();
            GetTags(_location);
            FilterFolders();

            if (Tags.Count > 0)
                ToggleViewCommandActive = false;

            OpenCommand = new DelegateCommand(OnOpen);
            ToggleViewCommand = new DelegateCommand(OnToggleView);
            OpenFolderCommand = new DelegateCommand(OnOpenFolder);
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
            return DateTime.Now;
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
            string[] files = { };
            string[] parts = { };
            TaggedDocsList.Clear();
            foreach (var tag in Tags)
            {
                foreach (var doc in _localList)
                {
                    if (tag.State is true)
                    {
                        files = doc.Name.Split('\\');
                        parts = files[files.Length - 1].Split(" - ");
                        foreach (var part in parts)
                            if (part.Equals(tag.Name))
                            {
                                TaggedDocsList.Add(doc);
                            }
                    }
                }
            }
            FilteredDocsList = new ObservableCollection<ProjectsModel>(UntaggedDocsList.Concat(TaggedDocsList));
            FilteredDocsList = new ObservableCollection<ProjectsModel>(FilteredDocsList.OrderBy(a => ExtrateDate(a.Name)));
            NotifyOfPropertyChange(() => FilteredDocsList);
        }

        private void GetFolders()
        {
            FolderList.Clear();
            foreach (var item in Directory.GetDirectories(FullName))
                FolderList.Add(new ProjectsModel(item));
        }

        private void GetTags(string location)
        {
            _localListClearable.Clear();
            string[] files = { };
            string[] parts = { };
            foreach (var item in Directory.GetDirectories(location))
                if (!_localList.Any(t => t.FullName == item))
                {
                    _localList.Add(new ProjectsModel(item));
                    _localListClearable.Add(new ProjectsModel(item));
                }
            foreach (var folder in _localListClearable)
            {
                parts = [];
                parts = folder.Name.Split("-");
                if (parts.Length > 1)
                {
                    string _tag = parts[1].Trim(' ');
                    if (_tag.Length == 2)
                    {
                        if (!Tags.Any(t => t.Name == _tag))
                        {
                            Tags.Add(new TagsModel(_tag));
                        }
                    }
                    else
                    {
                        UntaggedDocsList.Add(folder);
                    }
                }
                else
                {
                    UntaggedDocsList.Add(folder);
                }
            }
            FilteredDocsList = new ObservableCollection<ProjectsModel>(UntaggedDocsList.Concat(TaggedDocsList));
            FilteredDocsList = new ObservableCollection<ProjectsModel>(FilteredDocsList.OrderBy(a => ExtrateDate(a.Name)));
            NotifyOfPropertyChange(() => FilteredDocsList);
            NotifyOfPropertyChange(() => Tags);
            foreach (var item in Tags)
            {
                item.PropertyChanged += TagStateChanged;
            }
        }

        #endregion

        #region Commands Methods

        private void OnOpen(object obj)
        {
            Process.Start(new ProcessStartInfo("explorer.exe", _selectedFolderName.FullName));
        }

        private void OnToggleView (object obj) 
        {
            if (_viewIsToggled == true)
            {
                FilteredDocsList.Clear();
                Tags.Clear();
                UntaggedDocsList.Clear();
                TaggedDocsList.Clear();
                _localList.Clear();
                _viewIsToggled = false;
                _location = FullName;
                GetTags(_location);
                FilterFolders();
            }
            else if (_viewIsToggled == false)
            {
                FilteredDocsList.Clear();
                Tags.Clear();
                UntaggedDocsList.Clear();
                TaggedDocsList.Clear();
                _localList.Clear();
                _viewIsToggled = true;
                foreach (var folder in FolderList)
                {
                    _location = folder.FullName;
                    GetTags(_location);
                    FilterFolders();
                }
            }

        }

        private void OnOpenFolder(object obj)
        {
            Process.Start(new ProcessStartInfo("explorer.exe", FullName));
        }

        #endregion
    }
}
