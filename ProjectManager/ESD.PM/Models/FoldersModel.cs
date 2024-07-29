using ESD.PM.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ESD.PM.Models
{
    public class FoldersModel : FileModel
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

        public DelegateCommand OpenFileCommand { get; set; }

        private FileModel _selectedFileName;

        public FoldersModel(string name) : base(name)
        {
            Name = new DirectoryInfo(name).Name;

            FullName = new DirectoryInfo(name).FullName;

            InsideFiles = new ObservableCollection<FileModel>();

            GetInsideFiles();

            OpenFileCommand = new DelegateCommand(OnOpenFile);
        }

        private void GetInsideFiles()
        {
            foreach (var file in Directory.GetDirectories(FullName))
            {
                InsideFiles.Add(new FileModel(file));
            }
            foreach (var file in Directory.GetFiles(FullName))
            {
                InsideFiles.Add(new FileModel(file));
            }
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
