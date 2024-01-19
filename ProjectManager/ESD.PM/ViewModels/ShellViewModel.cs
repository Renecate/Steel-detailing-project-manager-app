using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.IO;

namespace ESD.PM.ViewModels
{
    public class ShellViewModel: Conductor<object>
    {
        #region Public Properties
        public ObservableCollection<ProjectsModel> ProjectsList { get; set; } = [];
        #endregion

        #region Private Properties


        #endregion

        #region Commands

        #endregion

        #region Constructor

        public ShellViewModel()
        {
            Projects();
        }

        #endregion

        #region Commands Methods

        #endregion

        #region Private Methods

        #endregion


        #region Public Methods

        public void Projects()
        {
            ProjectsList.Clear();
            foreach (var project in Directory.GetDirectories("C:\\Dropbox\\Production\\Projects"))
            {
                ProjectsList.Add(new ProjectsModel(project));
            }
        }

        #endregion
    }
}
