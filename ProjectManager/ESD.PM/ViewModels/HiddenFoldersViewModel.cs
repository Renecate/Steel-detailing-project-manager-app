using ESD.PM.Commands;
using ESD.PM.Models;

namespace ESD.PM.ViewModels
{
    public class HiddenFoldersViewModel : ProjectsModel
    {
        public DelegateCommand ShowFolderCommand { get; set; }
        public bool ShowFolder
        {
            get => showFolder;
            set
            {
                if (showFolder != value)
                {
                    showFolder = value;
                    OnPropertyChanged(nameof(ShowFolder));
                }
            }
        }

        private bool showFolder;

        public HiddenFoldersViewModel(string name) : base(name)
        {
            showFolder = false;
            ShowFolderCommand = new DelegateCommand(OnShowFolder);
        }

        private void OnShowFolder(object obj)
        {
            ShowFolder = true;
        }
    }
}
