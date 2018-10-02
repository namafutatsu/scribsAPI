using System.Collections.Generic;
using System.Linq;

namespace Scribs {

    public partial class User {

        private Directory directory;
        public Directory Directory {
            get {
                if (directory == null) {
                    directory = FileSystem.RootDir.GetDirectory(Name);
                }
                return directory;
            }
        }

        public void CreateDirectory() => Directory.CreateIfNotExistsAsync();

        public IEnumerable<Project> GetProjects() => Directory.Directories.Select(o => Project.GetFromDirectory(o));

        public Project GetProject(string name) => Project.GetFromDirectory(Directory.GetDirectory(name));
    }
}