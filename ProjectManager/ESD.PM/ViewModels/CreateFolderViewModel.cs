using ESD.PM.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public AppSettings AppSettings { get; set; }






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



        private List<string> _folderTags;

        private List<string> _tags;

        #endregion

        #region Commands

        #endregion

        #region Constructor

        public CreateFolderViewModel()
        {
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
        }

        #endregion

        #region Private Methods

        private void GetTemplates()
        {
            AvailableTemplates = new ObservableCollection<FileModel>();
            if (SelectedFolderType == FolderTypes[0])
            {
                FolderTagEnabled = false;
                SelectedFolderTag = "OR";

                foreach (var rfiTemplate in AppSettings.RfiTemplates)
                {
                    AvailableTemplates.Add(new FileModel(rfiTemplate));
                }
                if (AvailableTemplates.Count() > 1)
                {
                    TemplatesIsActive = true;
                    SelectedTemplate = AvailableTemplates[0];
                }
                else
                {
                    TemplatesIsActive = false;
                }
                SelectedTemplate = AvailableTemplates[0];
                GetRfiFolderName();
            }
            else if (SelectedFolderType == FolderTypes[1])
            {
                foreach (var structureTemplate in AppSettings.StructureTemplates)
                {
                    AvailableTemplates.Add(new FileModel(structureTemplate));
                    TemplatesIsActive = true;
                }
            }
            else if (SelectedFolderType == FolderTypes[2])
            {
                foreach (var pdfTemplate in AppSettings.PdfTemplates)
                {
                    AvailableTemplates.Add(new FileModel(pdfTemplate));
                    TemplatesIsActive = true;
                }
            }
            OnPropertyChanged(nameof(AvailableTemplates));
        }

        private void GetRfiFolderName()
        {

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
        #endregion

    }
}
