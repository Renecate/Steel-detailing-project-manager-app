using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ESD.PM.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ESD.PM.Commands;


namespace ESD.PM.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        #region Public Props

        public ObservableCollection<string> ProjectPath { get; set; }

        public ObservableCollection<FileModel> PdfTemplates { get; set; }

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

        #endregion

        #region Commands

        public DelegateCommand AddProjectPathCommand { get; set; }

        public DelegateCommand RemoveProjectPathCommand { get; set; }

        public DelegateCommand AddPdfTemplateCommand { get; set; }

        #endregion

        #region Constructor

        public SettingsViewModel()
        {
            appSettings = SettingsManager.LoadSettings();

            GetProjectPath();
            GetPdfTemplates();

            AddProjectPathCommand = new DelegateCommand(OnAddProjectPath);
            RemoveProjectPathCommand = new DelegateCommand(OnRemoveProjectPath);
            AddPdfTemplateCommand = new DelegateCommand(OnAddPdfTemplate);
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
                    else System.Windows.MessageBox.Show($"Folder '{selectedPath}' is not Projects folder");
                }
            }
        }

        private void OnRemoveProjectPath(object obj) 
        {
            appSettings.ProjectPaths.Remove(SelectedPath);
            SettingsManager.SaveSettings(appSettings);
            GetProjectPath();
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
