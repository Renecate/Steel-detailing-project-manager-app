using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace ESD.PM.Models
{
    public class FileModel : INotifyPropertyChanged
    {
        private string _name;

        private string _fullName;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        public FileModel(string name)
        {
            Name = new DirectoryInfo(name).Name;

            FullName = new DirectoryInfo(name).FullName;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
