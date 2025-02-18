using ESD.PM.Commands;
using ESD.PM.Settings;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace ESD.PM.Models
{
    public class SubFoldersModel : FileModel
    {
        #region Public Properties

        public FileModel SelectedFileName
        {
            get { return _selectedFileName; }
            set
            {
                _selectedFileName = value;
                OnPropertyChanged(nameof(SelectedFileName));
            }
        }
        public ObservableCollection<FileModel> InsideFiles { get; set; }
        public string SubFolderIsChecked { get; set; }
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    ChangeColour();
                }
            }
        }

        #endregion

        #region Commands

        public DelegateCommand OpenFileCommand { get; set; }

        #endregion

        #region Private Properties

        private FileModel _selectedFileName;

        private string _projectName;

        private bool _settingsIsTrue;
        private bool _isChecked; 

        private CheckHistoryModel _folderHistory;
        private ProjectHistoryModel _projectHistory;
        private SharedSettings _sharedSettings;

        #endregion

        #region Constructor

        public SubFoldersModel(string name, string _projectName, SharedSettings sharedSettings) : base(name)
        {
            SubFolderIsChecked = "Black";

            Name = new DirectoryInfo(name).Name;

            FullName = new DirectoryInfo(name).FullName;

            _sharedSettings = sharedSettings;

            if (sharedSettings != null)
            {
                foreach (var projectHistory in _sharedSettings.ProjectHistory)
                {
                    if (FullName.Contains(projectHistory.Folder))
                    {
                        _projectHistory = projectHistory;
                        foreach (var folderHisory in _projectHistory.CheckHistory)
                        {
                            if (folderHisory.Path.Equals(Name))
                            {
                                _isChecked = folderHisory.IsChecked;
                                ChangeColour();
                                break;
                            }
                        }
                    }
                }
            }

            InsideFiles = new ObservableCollection<FileModel>();

            GetInsideFiles();

            OpenFileCommand = new DelegateCommand(OnOpenFile);
        }

        #endregion

        #region Private Methods

        private async Task GetInsideFiles()
        {
            var insideFiles = new ConcurrentBag<FileModel>();

            await Task.Run(() =>
            {
                if (Directory.Exists(FullName))
                {
                    Parallel.ForEach(Directory.GetDirectories(FullName), (directory) =>
                    {
                        insideFiles.Add(new FileModel(directory));
                    });

                    Parallel.ForEach(Directory.GetFiles(FullName), (file) =>
                    {
                        insideFiles.Add(new FileModel(file));
                    });
                }
            });

            InsideFiles = new ObservableCollection<FileModel>(insideFiles);
            OnPropertyChanged(nameof(InsideFiles));
        }

        private void ChangeColour()
        {
            if (_isChecked)
            {
                SubFolderIsChecked = "Green";
            }
            else
            {
                SubFolderIsChecked = "Black";
            }
            OnPropertyChanged(nameof(SubFolderIsChecked));
        }

        #endregion

        #region Commands Methods

        private void OnOpenFile(object obj)
        {
            if (_selectedFileName != null)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "explorer.exe";
                startInfo.Arguments = "\"" + _selectedFileName.FullName + "\"";
                Process.Start(startInfo);
            }
        }
        #endregion
    }
}
