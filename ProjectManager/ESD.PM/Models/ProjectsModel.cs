using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace ESD.PM.Models
{
    public class ProjectsModel : INotifyPropertyChanged
    {
        public string Name { get; }

        public string FullName { get; }

        public ProjectsModel(string name)
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