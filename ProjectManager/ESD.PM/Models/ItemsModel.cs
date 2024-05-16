using Caliburn.Micro;
using ESD.PM.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace ESD.PM.Models
{
    public class ItemsModel : ProjectsModel
    {
        private ProjectsModel _selectedDoc;
        public ObservableCollection<ProjectsModel> DocsList { get; }
        public ObservableCollection<ProjectsModel> FilteredDocsList { get; }
        public ObservableCollection<TagsModel> Tags { get; }


        public DelegateCommand OpenCommand { get; set; }
        public DelegateCommand ClearCommand { get; set; }


        public ProjectsModel SelectedDoc
        {
            get { return _selectedDoc; }
            set
            {
                _selectedDoc = value;
            }
        }

        public ItemsModel(string name) : base(name)
        {
            string[] files = { };
            string[] parts = { };
            DocsList = new ObservableCollection<ProjectsModel> { };
            Tags = new ObservableCollection<TagsModel>();
            FilteredDocsList = new ObservableCollection<ProjectsModel>();
            foreach (var item in Directory.GetDirectories(FullName))
            {
                DocsList.Add(new ProjectsModel(item));
                files = item.Split('\\');
                parts = files[files.Length - 1].Split(" - ");
                if (parts.Length > 1)
                    if (parts[1].Length == 2)
                    {
                        string _tag = parts[1];
                        if (!Tags.Any(t => t.Name == _tag))
                        {
                            Tags.Add(new TagsModel(_tag));
                        }
                    }
                    else if (parts[1].Length > 2)
                    {
                        FilteredDocsList.Add(new ProjectsModel(item));
                    }
            }

            OpenCommand = new DelegateCommand(OnOpen);

            foreach (var item in Tags)
            {
                item.PropertyChanged += TagStateChanged;
            }
        }

        private void OnOpen(object obj)
        {
            Process.Start(new ProcessStartInfo("explorer.exe", _selectedDoc.FullName));
        }
        private void TagStateChanged(object sender, PropertyChangedEventArgs e)
        {
            string[] files = { };
            string[] parts = { };
            FilteredDocsList.Clear();
            if (e.PropertyName == "State")
            {
                foreach (var doc in DocsList)
                {
                    foreach (var tag in Tags)
                    {
                        if (tag.State is true)
                        {
                            files = doc.Name.Split('\\');
                            parts = files[files.Length - 1].Split(" - ");
                            if (parts[1].Contains(tag.Name))
                            {
                                FilteredDocsList.Add((ProjectsModel)doc);
                            }
                        }
                    }
                }
                var changedTag = (TagsModel)sender;
                bool newState = changedTag.State;
                string tagName = changedTag.Name;
            }
        }
    }
}
