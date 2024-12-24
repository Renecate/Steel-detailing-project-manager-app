using System.IO;

namespace ESD.PM.Models
{
    public class ProjectsModel : FileModel
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