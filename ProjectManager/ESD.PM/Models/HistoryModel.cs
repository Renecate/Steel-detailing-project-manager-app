using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESD.PM.Models
{
    public class HistoryModel
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public bool IsChecked { get; set; }

        public string Comment { get; set; }

        public HistoryModel(int id)
        {
            Id = id;
        }
    }
}
