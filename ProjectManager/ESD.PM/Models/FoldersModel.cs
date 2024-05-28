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
        public ObservableCollection<String> FilteredDocsList { get; set; }
        public ObservableCollection<String> TaggedDocsList { get; set; }
        public ObservableCollection<String> UntaggedDocsList {  get; set; }
        public ObservableCollection<TagsModel> Tags { get; set; }
        public string SelectedFolderName
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
        public DelegateCommand ClearCommand { get; set; }
        public DelegateCommand ToggleViewCommand { get; set; }

        #endregion

        #region Private Properties

        private string _selectedFolderName;

        private bool _viewIsToggled;

        private string _location;

        private ObservableCollection<ProjectsModel> _localList {  get; set; }

        private ObservableCollection<ProjectsModel> _localListClearable { get; set; }

        #endregion

        #region Constructor

        public FoldersModel(string name) : base(name)
        {

            _viewIsToggled = false;
            _location = FullName;
            FolderList = new ObservableCollection<ProjectsModel>();
            Tags = new ObservableCollection<TagsModel>();
            FilteredDocsList = new ObservableCollection<string>();
            UntaggedDocsList = new ObservableCollection<string>();
            TaggedDocsList = new ObservableCollection<string>();
            _localList = new ObservableCollection<ProjectsModel>();
            _localListClearable = new ObservableCollection<ProjectsModel>();
            GetFolders();
            GetTags(_location);
            OpenCommand = new DelegateCommand(OnOpen);
            ToggleViewCommand = new DelegateCommand(OnToggleView);
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
                            if (part.Contains(tag.Name))
                            {
                                TaggedDocsList.Add(doc.Name);
                            }
                    }
                }
            }
            FilteredDocsList = new ObservableCollection<string>(UntaggedDocsList.Concat(TaggedDocsList));
            FilteredDocsList = new ObservableCollection<string>(FilteredDocsList.OrderBy(a => ExtrateDate(a)));
            NotifyOfPropertyChange(() => FilteredDocsList);
        }

        private void GetFolders()
        {
            FolderList.Clear();
            foreach (var item in Directory.GetDirectories(FullName))
                FolderList.Add(new ProjectsModel(item));
            foreach (var item in Directory.GetFiles(FullName))
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
            foreach (var item in Directory.GetFiles(location))
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
                        UntaggedDocsList.Add(folder.Name);
                    }
                }
                else
                {
                    UntaggedDocsList.Add(folder.Name);
                }
            }
            FilteredDocsList = new ObservableCollection<string>(UntaggedDocsList.Concat(TaggedDocsList));
            FilteredDocsList = new ObservableCollection<string>(FilteredDocsList.OrderBy(a => ExtrateDate(a)));
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
            foreach (var folder in _localList)
                if (folder.Name == _selectedFolderName)
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", folder.FullName));
                }
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
                }
            }
        }

        #endregion
    }
}
