using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ESD.PM.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ESD.PM.Commands;
using System.IO;
using System.Net.Http.Headers;


namespace ESD.PM.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        #region Public Props

        public ObservableCollection<string> ProjectPath { get; set; }

        public ObservableCollection<FileModel> PdfTemplates { get; set; }

        public ObservableCollection<FileModel> FolderStructureTemplates { get; set; }

        public ObservableCollection<FileModel> ProjectTemplates { get; set; }

        public FileModel SelectedProjectTemplate
        {
            get { return _selectedProjectTemplate; }
            set
            {
                if (_selectedProjectTemplate != value)
                {
                    _selectedProjectTemplate = value;
                    OnPropertyChanged(nameof(SelectedProjectTemplate));
                }
            }
        }

        public FileModel SelectedFolderStructureTemplate
        {
            get { return _selectedFolderStructureTemplate;  }
            set
            {
                if (_selectedFolderStructureTemplate != value) 
                {
                    _selectedFolderStructureTemplate = value;
                    OnPropertyChanged(nameof(SelectedFolderStructureTemplate));
                }
            }
        }

        public FileModel SelectedPdfTemplate
        {
            get { return _selectedPdfTemplate; }
            set
            {
                if (_selectedPdfTemplate != value)
                {
                    _selectedPdfTemplate = value;
                    OnPropertyChanged(nameof(SelectedPath));
                }
            }
        }


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

        #endregion

        #region Private Props

        private AppSettings appSettings { get; set; }

        private string _selectedPath;

        private FileModel _selectedPdfTemplate;

        private FileModel _selectedProjectTemplate;

        private FileModel _selectedFolderStructureTemplate;

        #endregion

        #region Commands

        public DelegateCommand AddProjectPathCommand { get; set; }

        public DelegateCommand RemoveProjectPathCommand { get; set; }

        public DelegateCommand AddPdfTemplateCommand { get; set; }

        public DelegateCommand RemovePdfTemplateCommand { get; set; }

        public DelegateCommand AddFolderStructureTemplateCommand { get; set; }

        public DelegateCommand RemoveFolderStructureTemplateCommand { get; set; }

        public DelegateCommand AddProjectTemplateCommand { get; set; }

        public DelegateCommand RemoveProjectTemplateCommand { get; set; }
        #endregion

        #region Constructor

        public SettingsViewModel()
        {
            appSettings = SettingsManager.LoadSettings();

            GetProjectPath();
            GetPdfTemplates();
            GetFolderStructureTemplates();
            GetProjectTemplates();

            AddProjectPathCommand = new DelegateCommand(OnAddProjectPath);
            RemoveProjectPathCommand = new DelegateCommand(OnRemoveProjectPath);
            AddPdfTemplateCommand = new DelegateCommand(OnAddPdfTemplate);
            RemovePdfTemplateCommand = new DelegateCommand(OnRemovePdfTemplate);
            AddFolderStructureTemplateCommand = new DelegateCommand(OnnAddFolderStructureTemplate);
            RemoveFolderStructureTemplateCommand = new DelegateCommand(OnRemoveFolderStructureTemplate);
            AddProjectTemplateCommand = new DelegateCommand(OnAddProjectTemplate);
            RemoveProjectTemplateCommand = new DelegateCommand(OnRemoveProjectTemplate);

        }

        #endregion

        #region Commands methods

        private void OnAddProjectPath(object obj)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "PLEASE CHOOSE 'PROJECT' FOLDER";
                dialog.ShowNewFolderButton = false;

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    string selectedPath = dialog.SelectedPath;

                    if (!appSettings.ProjectPaths.Contains(selectedPath) && selectedPath.EndsWith("Projects"))
                    {
                        appSettings.ProjectPaths.Add(selectedPath);
                        SettingsManager.SaveSettings(appSettings);
                        GetProjectPath();
                    }
                    else if (appSettings.ProjectPaths.Contains(selectedPath))
                    {
                        System.Windows.MessageBox.Show($"Folder '{selectedPath}' already exists");
                    }
                    else System.Windows.MessageBox.Show($"Folder '{selectedPath}' is not Projects folder");

                }
            }
        }

        private void OnRemoveProjectPath(object obj) 
        {
            if (SelectedPath != null) 
            {
                appSettings.ProjectPaths.Remove(SelectedPath);
                SettingsManager.SaveSettings(appSettings);
                GetProjectPath();
            }
        }

        private void OnAddPdfTemplate(object obj)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "PLEASE CHOOSE PDF TEMPLATES";
                dialog.Filter = "PDF Files (*.pdf)|*.pdf";
                dialog.Multiselect = true; 

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    foreach (var selectedPath in dialog.FileNames)
                    {
                        if (!appSettings.PdfTemplates.Contains(selectedPath))
                        {
                            appSettings.PdfTemplates.Add(selectedPath);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show($"File '{selectedPath}' is already in the list of PDF templates");
                        }
                    }

                    SettingsManager.SaveSettings(appSettings);
                    GetPdfTemplates();

                }
                else if (result == DialogResult.Cancel)
                {
                    System.Windows.MessageBox.Show("No file selected");
                }
            }
        }

        private void OnRemovePdfTemplate(object obj)
        {
            if (SelectedPdfTemplate != null) 
            {
                appSettings.PdfTemplates.Remove(SelectedPdfTemplate.FullName);
                SettingsManager.SaveSettings(appSettings);
                GetPdfTemplates();
            }
        }

        private void OnnAddFolderStructureTemplate(object obj) 
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "PLEASE CHOOSE FOLDER STRUCTURE TEMPLATES FOLDER";
                dialog.ShowNewFolderButton = false;

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    string selectedPath = dialog.SelectedPath;

                    foreach (var template in Directory.GetDirectories(selectedPath)) 
                    {
                        if (!appSettings.StructureTemplates.Contains(template) && template.EndsWith("Template", StringComparison.OrdinalIgnoreCase))
                        {
                            appSettings.StructureTemplates.Add(template);
                            SettingsManager.SaveSettings(appSettings);
                            GetFolderStructureTemplates();
                        }
                        else if (appSettings.StructureTemplates.Contains(template))
                        {
                            System.Windows.MessageBox.Show($"Folder '{template}' already exists");
                        }
                        else System.Windows.MessageBox.Show($"Folder '{template}' is not template");
                    }
                }
            }
        }

        private void OnRemoveFolderStructureTemplate(object obj) 
        {
            if (SelectedFolderStructureTemplate != null)
            {
                appSettings.StructureTemplates.Remove(SelectedFolderStructureTemplate.FullName);
                SettingsManager.SaveSettings(appSettings);
                GetFolderStructureTemplates();
            }
        }

        private void OnAddProjectTemplate(object obj)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "PLEASE CHOOSE PROJECT TEMPLATES FOLDER";
                dialog.ShowNewFolderButton = false;

                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    string selectedPath = dialog.SelectedPath;

                    foreach (var template in Directory.GetDirectories(selectedPath))
                    {
                        if (!appSettings.ProjectTemplates.Contains(template) && template.EndsWith("Template", StringComparison.OrdinalIgnoreCase))
                        {
                            appSettings.ProjectTemplates.Add(template);
                            SettingsManager.SaveSettings(appSettings);
                            GetProjectTemplates();
                        }
                        else if (appSettings.ProjectTemplates.Contains(template))
                        {
                            System.Windows.MessageBox.Show($"Folder '{template}' already exists");
                        }
                        else System.Windows.MessageBox.Show($"Folder '{template}' is not template");
                    }
                }
            }
        }

        private void OnRemoveProjectTemplate(object obj)
        {
            if (SelectedProjectTemplate != null) 
            {
                appSettings.ProjectTemplates.Remove(SelectedProjectTemplate.FullName);
                SettingsManager.SaveSettings(appSettings);
                GetProjectTemplates();
            }
        }
        #endregion

        #region Private Methods

        private void GetProjectPath()
        {
            ProjectPath = new ObservableCollection<string>(appSettings.ProjectPaths);
            OnPropertyChanged (nameof(ProjectPath));
        }

        private void GetPdfTemplates()
        {
            PdfTemplates = new ObservableCollection<FileModel>();
            foreach (var pdfTemplatePath in appSettings.PdfTemplates) 
            {
                PdfTemplates.Add(new FileModel(pdfTemplatePath));
            }
            OnPropertyChanged(nameof(PdfTemplates));
        }

        private void GetFolderStructureTemplates()
        {
            FolderStructureTemplates = new ObservableCollection<FileModel>();
            foreach (var folderStructureTemplate in appSettings.StructureTemplates)
            {
                FolderStructureTemplates.Add(new FileModel(folderStructureTemplate));
            }
            OnPropertyChanged(nameof(FolderStructureTemplates));
        }

        private void GetProjectTemplates()
        {
            ProjectTemplates = new ObservableCollection<FileModel>();
            foreach (var folderStructureTemplate in appSettings.ProjectTemplates)
            {
                ProjectTemplates.Add(new FileModel(folderStructureTemplate));
            }
            OnPropertyChanged(nameof(ProjectTemplates));
        }

        #endregion

        #region Public Methods
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}
