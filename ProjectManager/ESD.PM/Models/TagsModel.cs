using ESD.PM.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESD.PM.Models
{
    public class TagsModel
    {
        public DelegateCommand StateCommand { get; set; }

        public string Name { get; }
        public bool State { get; }

        public TagsModel(string name) 
        {
            Name = name;

            StateCommand = new DelegateCommand(StateChange); 
        }

        private void StateChange(object obj)
        {

        }
    }

}
