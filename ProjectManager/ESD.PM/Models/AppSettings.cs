using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESD.PM.Models
{
    public class AppSettings
    {
        public ObservableCollection<string> ProjectPaths { get; set; } = new ObservableCollection<string>();

        public List<string> FavoriteProject { get; set; } = new List<string>();
    }
}
