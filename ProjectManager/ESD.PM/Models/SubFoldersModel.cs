using ESD.PM.Commands;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace ESD.PM.Models
{
    public class SubFoldersModel : FileModel
    {
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
        public bool FolderIsChecked { get; set; }
        public DelegateCommand OpenFileCommand { get; set; }

        private FileModel _selectedFileName;

        public SubFoldersModel(string name) : base(name)
        {
            FolderIsChecked = false;

            Name = new DirectoryInfo(name).Name;

            FullName = new DirectoryInfo(name).FullName;

            InsideFiles = new ObservableCollection<FileModel>();

            GetInsideFiles();

            OpenFileCommand = new DelegateCommand(OnOpenFile);
        }

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
    }
}
