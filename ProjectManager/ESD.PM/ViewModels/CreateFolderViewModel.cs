using ESD.PM.Models;
using ESD.PM.Settings;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;


namespace ESD.PM.ViewModels
{
    public class CreateFolderViewModel : INotifyPropertyChanged
    {
        #region Public Props

        public List<string> FolderTypes { get; set; }

        public string SelectedFolderType
        {
            get { return _selectedFolderType; }
            set
            {
                if (_selectedFolderType != value)
                {
                    _selectedFolderType = value;
                    GetTemplates();
                    OnPropertyChanged(nameof(SelectedFolderType));
                }
            }
        }

        public ObservableCollection<FileModel> AvailableTemplates { get; set; }

        public bool TemplatesIsActive
        {
            get { return _templatesIsActive; }
            set
            {
                if (_templatesIsActive != value)
                {
                    _templatesIsActive = value;
                    OnPropertyChanged(nameof(TemplatesIsActive));
                }
            }
        }

        public FileModel SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                if (_selectedTemplate != value)
                {
                    _selectedTemplate = value;
                    GetInsideFiles();
                    OnPropertyChanged(nameof(SelectedTemplate));
                }
            }
        }

        public string OrderNumber
        {
            get { return _orderNumber; }
            set
            {
                if (_orderNumber != value)
                {
                    _orderNumber = value;
                    OnPropertyChanged(nameof(OrderNumber));
                }
            }
        }

        public List<string> FolderTags
        {
            get { return _folderTags; }
            set
            {
                if (_folderTags != value)
                {
                    _folderTags = value;
                    OnPropertyChanged(nameof(FolderTags));
                }
            }
        }

        public string SelectedFolderTag
        {
            get { return _selectedFolderTag; }
            set
            {
                if (_selectedFolderTag != value)
                {
                    _selectedFolderTag = value;
                    OnPropertyChanged(nameof(SelectedFolderTag));
                }
            }
        }

        public bool FolderTagEnabled
        {
            get { return _folderTagEnabled; }
            set
            {
                if (_folderTagEnabled != value)
                {
                    _folderTagEnabled = value;
                    OnPropertyChanged(nameof(FolderTagEnabled));
                }
            }
        }

        public string FolderName
        {
            get { return _folderName; }
            set
            {
                if (_folderName != value)
                {
                    _folderName = value;
                    GetInsideFiles();
                    OnPropertyChanged(nameof(FolderName));
                }
            }
        }

        public string Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
        }

        public List<string> CreationPath { get; set; }

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

        public bool CreationPathEnabled
        {
            get { return _creationPathEnabled; }
            set
            {
                if (_creationPathEnabled != value)
                {
                    _creationPathEnabled = value;
                    OnPropertyChanged(nameof(CreationPathEnabled));
                }
            }
        }

        public string ProjectName { get; set; }

        public AppSettings AppSettings { get; set; }

        public string RfiNumber { get; set; }

        public ObservableCollection<string> InsideFiles { get; set; }

        public bool EiwRfiIsTrue
        {
            get { return _eiwRfiIsTrue; }
            set
            {
                if (_eiwRfiIsTrue != value)
                {
                    _eiwRfiIsTrue = value;
                    SetRfiName();
                    OnPropertyChanged(nameof(EiwRfiIsTrue));
                }
            }
        }

        public bool IsRfiSelected {  get; set; }

        #endregion

        #region PrivateProps

        private string _selectedFolderType;

        private bool _templatesIsActive;

        private FileModel _selectedTemplate;

        private string _orderNumber;

        private string _selectedFolderTag;

        private bool _folderTagEnabled;

        private string _folderName;

        private string _date;

        private string _selectedPath;

        private bool _creationPathEnabled;

        private string _pdfName;

        private List<string> _folderTags;

        private List<string> _tags;

        private bool _eiwRfiIsTrue;

        #endregion

        #region Commands

        #endregion

        #region Constructor

        public CreateFolderViewModel()
        {
            AppSettings = new AppSettings();
            AppSettings = AppSettingsManager.LoadSettings();
            FolderTypes = new List<string>()
            {
                "RFI",
                "Sub-Folders",
                "Other"
            };

            _tags = new List<string>()
            {
                "CD", "DC", "FA", "FC", "FM", "FF", "OD", "OR", "RR"
            };

            FolderName = "New Folder";
            _templatesIsActive = false;
            _folderTagEnabled = true;
            _creationPathEnabled = false;

            Date = DateTime.Now.Date.ToString("MM.dd.yyyy");
            IsRfiSelected = false;
        }

        #endregion

        #region Private Methods

        private void GetTemplates()
        {
            ProjectName = string.Empty;
            var parts = CreationPath[0].Split("\\");
            var nextIndex = 0;
            foreach (var part in parts)
            {
                nextIndex++;
                if (part.Contains("project", StringComparison.OrdinalIgnoreCase))
                {
                    if (nextIndex < parts.Length - 1)
                    {
                        ProjectName = parts[nextIndex];
                        break;
                    }
                }
            }
            AvailableTemplates = new ObservableCollection<FileModel>();
            if (SelectedFolderType == FolderTypes[0])
            {
                FolderTagEnabled = false;
                SelectedFolderTag = "OR";

                foreach (var rfiTemplate in AppSettings.RfiTemplates)
                {
                    AvailableTemplates.Add(new FileModel(rfiTemplate));
                }
                if (AvailableTemplates.Any())
                {
                    if (AvailableTemplates.Count() > 1)
                    {
                        TemplatesIsActive = true;
                    }
                    else
                    {
                        TemplatesIsActive = false;
                    }
                    SelectedTemplate = AvailableTemplates[0];
                }
                _pdfName = "RFI " + RfiNumber;
                FolderName = _pdfName;
            }
            else if (SelectedFolderType == FolderTypes[1])
            {
                foreach (var structureTemplate in AppSettings.StructureTemplates)
                {
                    FolderTagEnabled = true;
                    AvailableTemplates.Add(new FileModel(structureTemplate));
                    if (AvailableTemplates.Any())
                    {
                        TemplatesIsActive = true;
                        SelectedTemplate = AvailableTemplates[0];
                    }
                    FolderName = "New Folder";
                }
            }
            else if (SelectedFolderType == FolderTypes[2])
            {
                foreach (var pdfTemplate in AppSettings.PdfTemplates)
                {
                    FolderTagEnabled = true;
                    AvailableTemplates.Add(new FileModel(pdfTemplate));
                    if (AvailableTemplates.Any())
                    {
                        TemplatesIsActive = true;
                    }
                    TemplatesIsActive = true;
                    FolderName = "New Folder";
                }
            }
            OnPropertyChanged(nameof(AvailableTemplates));
        }
        private void GetInsideFiles()
        {
            InsideFiles = new ObservableCollection<string>();
            if (SelectedFolderType == FolderTypes[0])
            {
                if (SelectedTemplate != null)
                {
                    InsideFiles.Add(_pdfName + ".pdf");
                    IsRfiSelected = true;
                }
                if (SelectedTemplate == null)
                {
                    InsideFiles.Clear();
                    System.Windows.MessageBox.Show($"Templates are not available");
                }
            }
            if (SelectedFolderType == FolderTypes[1])
            {
                IsRfiSelected = false;
                EiwRfiIsTrue = false;
                if (SelectedTemplate != null)
                {
                    if (Directory.Exists(SelectedTemplate.FullName))
                    {
                        foreach (var path in Directory.GetDirectories(SelectedTemplate.FullName))
                        {
                            var directory = new DirectoryInfo(path).Name;
                            InsideFiles.Add(directory);
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show($"Template '{SelectedTemplate.Name}' no longer exists");
                    }
                }
                if (SelectedTemplate == null)
                {
                    InsideFiles.Clear();
                    System.Windows.MessageBox.Show($"Templates are not available");
                }
            }
            if (SelectedFolderType == FolderTypes[2])
            {
                IsRfiSelected = false;
                EiwRfiIsTrue = false;
                if (SelectedTemplate == null)
                {
                    InsideFiles.Clear();
                }
                else
                {
                    _pdfName = Date + " - " + FolderName + ".pdf";
                    InsideFiles.Add(_pdfName);
                }
            }
            OnPropertyChanged(nameof(InsideFiles));
            OnPropertyChanged(nameof(IsRfiSelected));
        }

        private void SetRfiName()
        {
            if (EiwRfiIsTrue)
            {
                InsideFiles.Clear();
                InsideFiles.Add(ProjectName + " " + _pdfName + ".pdf");
            }
            else
            {
                InsideFiles.Clear();
                InsideFiles.Add(_pdfName + ".pdf");
            }

        }

        #endregion

        #region Command Methods

        #endregion

        #region Public Methods

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void GetTags()
        {
            if (FolderTags != null)
            {
                foreach (var tag in _tags)
                {
                    if (!FolderTags.Any(t => t == tag))
                    {
                        FolderTags.Add(tag);
                    }
                }
                FolderTags.Sort();
            }
            if (CreationPath.Count > 1)
            {
                CreationPathEnabled = true;
                SelectedPath = CreationPath[0];
            }
            else
            {
                SelectedPath = CreationPath[0];
            }
            SelectedFolderType = FolderTypes[2];
            OnPropertyChanged(nameof(FolderTags));
            OnPropertyChanged(nameof(CreationPath));
        }

        public void Create()
        {
            var newFolderName = string.Empty;
            if (SelectedFolderTag != null)
            {
                if (SelectedFolderTag.Length > 2)
                {
                    SelectedFolderTag = SelectedFolderTag.Substring(0, 2);
                }
                else if (SelectedFolderTag.Length == 1)
                {
                    SelectedFolderTag = SelectedFolderTag + SelectedFolderTag;
                }
            }
            var collection = new List<string>()
            {
                    $"({OrderNumber})",
                SelectedFolderTag,
                Date,
                FolderName
            };


            collection = new List<string>(collection.Where(n => !string.IsNullOrEmpty(n)));

            foreach (var part in collection)
            {
                if (!(collection.IndexOf(part) == collection.Count - 1))
                {
                    newFolderName += part + " - ";
                }
                else
                {
                    newFolderName += part;
                }
            }

            var path = SelectedPath + "\\" + newFolderName;
            char[] invalidChars = Path.GetInvalidFileNameChars();

            if (Directory.Exists(path) != true)
            {
                if (newFolderName.Any(ch => invalidChars.Contains(ch)))
                {
                    System.Windows.MessageBox.Show("The folder name contains invalid characters. Please use a valid name.");
                }
                else
                {
                    Directory.CreateDirectory(path);
                }
            }
            else
                System.Windows.MessageBox.Show($"Folder '{newFolderName}' already exists");

            foreach (var file in InsideFiles)
            {
                if (SelectedTemplate.FullName.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
                {
                    var pdfGenerator = new PdfGenerator();
                    if (File.Exists(SelectedTemplate.FullName))
                    {
                        pdfGenerator.CreatePdfFromTemplate(SelectedTemplate.FullName, path + "\\" + file);
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = "explorer.exe";
                        startInfo.Arguments = path + "\\" + file;
                        Process.Start(startInfo);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show($"Template '{SelectedTemplate.Name}' no longer exists");
                    }
                }
                else
                {
                    Directory.CreateDirectory(path + "\\" + file);
                }
            }
        }
        #endregion

    }
}
