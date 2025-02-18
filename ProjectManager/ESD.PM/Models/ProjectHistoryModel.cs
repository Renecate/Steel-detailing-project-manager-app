using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESD.PM.Models
{
    public class ProjectHistoryModel
    {
        public string Folder { get; set; }

        public ObservableCollection<CheckHistoryModel> CheckHistory { get; set; }

        public ProjectHistoryModel(string folder)
        {
            Folder = folder;

            CheckHistory = new ObservableCollection<CheckHistoryModel> { };
        }
    }
}
