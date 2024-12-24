using ESD.PM.Models;
using ESD.PM.Settings;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace ESD.PM.ViewModels
{
    public class CreateProjectViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> Paths { get; set; }

        public string SelectedPath
        {
            get { return _selectedPath; }
            set
            {
                if (_selectedPath != value)
                {
                    _selectedPath = value;
                    OnPropertyChanged(nameof(SelectedPath));
                }
            }
        }

        public ObservableCollection<FileModel> ProjectsTemplates { get; set; }

        public FileModel SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                if (_selectedTemplate != value)
                {
                    _selectedTemplate = value;
                    OnPropertyChanged(nameof(SelectedTemplate));
                }
            }
        }

        public string ProjectName { get; set; }

        public AppSettings appSettings;


        private FileModel _selectedTemplate;

        private string _selectedPath;


        public CreateProjectViewModel()
        {

        }

        public void GetPathAndTemplates()
        {
            Paths = new ObservableCollection<string>();
            ProjectsTemplates = new ObservableCollection<FileModel>();
            foreach (var path in appSettings.ProjectPaths)
            {
                Paths.Add(path);
            }
            foreach (var template in appSettings.ProjectTemplates)
            {
                ProjectsTemplates.Add(new FileModel(template));
            }
            SelectedTemplate = ProjectsTemplates[0];
            SelectedPath = Paths[0];
            OnPropertyChanged(nameof(Paths));
            OnPropertyChanged(nameof(ProjectsTemplates));
        }

        public void CreateProject()
        {
            var path = SelectedPath + "\\" + ProjectName;
            char[] invalidChars = Path.GetInvalidFileNameChars();

            if (!Directory.Exists(path))
            {
                if (ProjectName.Any(ch => invalidChars.Contains(ch)))
                {
                    System.Windows.MessageBox.Show("The folder name contains invalid characters. Please use a valid name.");
                }
                else
                {
                    Directory.CreateDirectory(path);
                    if (Directory.Exists(SelectedTemplate.FullName))
                    {
                        foreach (var directory in Directory.GetDirectories(SelectedTemplate.FullName))
                        {
                            var subFolders = new DirectoryInfo(directory).Name;
                            var foldersPath = path + "\\" + subFolders;
                            Directory.CreateDirectory(foldersPath);
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show($"Template '{SelectedTemplate.Name}' no longer exists");
                    }
                }
            }
            else
            {
                System.Windows.MessageBox.Show($"Project '{ProjectName}' already exists");
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
