using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public Task<bool> CreateDirectoryAsync() => Directory.CreateIfNotExistsAsync();

        public IEnumerable<Project> GetProjects() => Directory.Directories.Select(o => Project.GetFromDirectory(db, o));

        public Project GetProject(string name) => Project.GetFromDirectory(db,  Directory.GetDirectory(name));
    }
}