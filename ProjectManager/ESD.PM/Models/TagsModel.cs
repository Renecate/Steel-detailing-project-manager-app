using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESD.PM.Models
{
    public class TagsModel
    {
        public string Name { get; }

        public TagsModel(string name) 
        {
            Name = name;
        }
    }
}
