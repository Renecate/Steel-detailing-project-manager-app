using System.IO;

namespace ESD.PM.Models
{
    public class ProjectsModel : IComparable<ProjectsModel>
    {
        public string Name { get; }

        public string FullName { get; }

        public DateTime CreationDate { get; }

        public ProjectsModel(string name)
        {              
            Name = new DirectoryInfo(name).Name;

            FullName = new DirectoryInfo(name).FullName;

            CreationDate = (File.GetCreationTime(FullName));           
        }

        public int CompareTo(ProjectsModel? other)
        {
            return DateTime.Compare(this.CreationDate, other.CreationDate);
        }
    }
}