using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ESD.PM.Models
{
    public class FileNameModel : INotifyPropertyChanged
    {
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string _name;

        public FileNameModel(string name)
        {
            Name = name;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
