using Caliburn.Micro;
using ESD.PM.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
        private Collection<string> _tags { get; }

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
            Tags = new ObservableCollection<TagsModel> { };
            _tags = new Collection<string> { };
            foreach (var item in Directory.GetDirectories(FullName))
            {
                DocsList.Add(new ProjectsModel(item));
                files = item.Split('\\');
                parts = files[files.Length - 1].Split(" - ");
                if (parts.Length > 1)
                    if (parts[1].Length == 2)
                    {
                        string _tag = parts[1];
                        if (_tags.Contains(_tag))
                        {
                            continue;
                        }
                        else
                        {
                            _tags.Add(_tag);
                        }
                    }
            }
            foreach (var item in Directory.GetFiles(FullName))
            {
                DocsList.Add(new ProjectsModel(item));
            }
            foreach (var tag in _tags)
                Tags.Add(new TagsModel(tag));
            OpenCommand = new DelegateCommand(OnOpen);
            FilteredDocsList = DocsList;
        }

        private void OnOpen(object obj)
        {
            Process.Start(new ProcessStartInfo("explorer.exe", _selectedDoc.FullName));
        }
    }
}
