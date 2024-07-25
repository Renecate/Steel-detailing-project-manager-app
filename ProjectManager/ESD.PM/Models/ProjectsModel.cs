using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace ESD.PM.Models
{
    public class ProjectsModel : FoldersModel
    {

        private bool _favorite;

        public bool Favorite
        {
            get { return _favorite; }
            set
            {
                _favorite = value;
                OnPropertyChanged(nameof(Favorite));
            }
        }

        public ProjectsModel(string name) : base(name)
        {
            Name = new DirectoryInfo(name).Name;

            FullName = new DirectoryInfo(name).FullName;

            Favorite = false;
        }
    }
}