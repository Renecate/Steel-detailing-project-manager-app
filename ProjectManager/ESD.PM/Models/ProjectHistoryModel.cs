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
        public string Name { get; set; }

        public ObservableCollection<HistoryModel> History { get; set; }

        public ProjectHistoryModel(string name)
        {
            Name = name;
        }
    }
}
