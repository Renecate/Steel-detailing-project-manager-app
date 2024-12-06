using ESD.PM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESD.PM.Settings
{
    public class FoldersSettings
    {
        public ObservableCollection<FoldersViewModel> SavedFolders { get; set; } = new ObservableCollection<FoldersViewModel> { };
    }
}
