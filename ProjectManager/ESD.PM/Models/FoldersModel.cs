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
        private string _selectedFolderName;
        public ObservableCollection<ProjectsModel> FolderList { get; }
        public ObservableCollection<String> FilteredDocsList { get; set; }
        public ObservableCollection<String> TaggedDocsList { get; set; }
        public ObservableCollection<String> UntaggedDocsList {  get; set; }
        public ObservableCollection<TagsModel> Tags { get; }


        public DelegateCommand OpenCommand { get; set; }
        public DelegateCommand ClearCommand { get; set; }


        public string SelectedFolderName
        {
            get { return _selectedFolderName; }
            set
            {
                _selectedFolderName = value;
            }
        }

        public FoldersModel(string name) : base(name)
        {
            string[] files = { };
            string[] parts = { };
            FolderList = new ObservableCollection<ProjectsModel> { };
            Tags = new ObservableCollection<TagsModel>();
            FilteredDocsList = new ObservableCollection<string>();
            UntaggedDocsList = new ObservableCollection<string>();
            TaggedDocsList = new ObservableCollection<string>();
            foreach (var item in Directory.GetDirectories(FullName))
                FolderList.Add(new ProjectsModel(item));
            foreach (var item in Directory.GetFiles(FullName))
                FolderList.Add(new ProjectsModel(item));
            foreach (var folder in FolderList)
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
                        UntaggedDocsList.Add(folder.Name);
                }
                else
                    UntaggedDocsList.Add(folder.Name);
            }
            FilteredDocsList = new ObservableCollection<string>(UntaggedDocsList.Concat(TaggedDocsList));
            FilteredDocsList = new ObservableCollection<string>(FilteredDocsList.OrderBy(a => ExtrateDate(a)));
            NotifyOfPropertyChange(() => FilteredDocsList);
            OpenCommand = new DelegateCommand(OnOpen);

            foreach (var item in Tags)
            {
                item.PropertyChanged += TagStateChanged;
            }
        }

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

        private void OnOpen(object obj)
        {
            foreach (var folder in FolderList)
                if (folder.Name == _selectedFolderName)
                {
                    Process.Start(new ProcessStartInfo("explorer.exe", folder.FullName));
                }
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
                foreach (var doc in FolderList)
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
    }
}
