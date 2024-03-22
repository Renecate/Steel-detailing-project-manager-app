using Caliburn.Micro;
using ESD.PM.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ESD.PM.Models
{
    public class ItemsModel : ProjectsModel
    {
        private ProjectsModel _selectedDoc;
        public List<ProjectsModel> DocsList { get; } 

        public DelegateCommand OpenCommand { get; set; }

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
            DocsList = new List<ProjectsModel> { };
            foreach (var item in Directory.GetDirectories(FullName))
            {
                DocsList.Add(new ProjectsModel(item));
            }
            foreach (var item in Directory.GetFiles(FullName))
            {
                DocsList.Add(new ProjectsModel(item));
            }
            OpenCommand = new DelegateCommand(OnOpen);
            DocsList.Sort();
        }

        private void OnOpen(object obj)
        {
            Process.Start(new ProcessStartInfo("explorer.exe", _selectedDoc.FullName));
        }
    }
}
