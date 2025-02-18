using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESD.PM.Models
{
    public class CheckHistoryModel
    {
        public bool IsChecked { get; set; }

        public string Path { get; set; }

        public CheckHistoryModel(string path, bool isChecked)
        {
            Path = path;
            IsChecked = isChecked;
        }
    }
}
