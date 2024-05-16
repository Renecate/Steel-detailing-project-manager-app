using ESD.PM.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;

namespace ESD.PM.Models
{
    public class TagsModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private bool _state;
        public bool State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged(nameof(State));
            }
        }

        public TagsModel(string name)
        {
            Name = name;
        }
    }
}
