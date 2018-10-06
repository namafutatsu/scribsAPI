using System.Collections.Generic;
using System.Linq;

namespace Scribs {

    public partial class User: Entity<User> {

        private Directory directory;
        public Directory Directory {
            get {
                if (directory == null) {
                    directory = FileSystem.GetRootDir(db).GetDirectory(Name);
                }
                return directory;
            }
        }

        public void CreateDirectory() => Directory.CreateIfNotExistsAsync();

        public IEnumerable<Project> GetProjects() => Directory.Directories.Select(o => Project.GetFromDirectory(db, o));

        public Project GetProject(string name) => Project.GetFromDirectory(db,  Directory.GetDirectory(name));
    }
}