using System.IO;

namespace ESD.PM.Models
{
    public class ProjectsModel
    {
        public string Name { get; }

        public string FullName { get; }

        public ProjectsModel(string name)
        {
            Name = new DirectoryInfo(name).Name;

            FullName = new DirectoryInfo(name).FullName;
        }
    }
}