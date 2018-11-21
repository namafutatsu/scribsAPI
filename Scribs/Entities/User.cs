using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scribs {

    public partial class User: Entity<User> {
        public string Path => System.IO.Path.Combine(FileSystemItem.STORAGE, Name);

        public void CreateDirectory() {
            if (!System.IO.Directory.Exists(Path))
                System.IO.Directory.CreateDirectory(Path);
        }

        public IEnumerable<Project> GetProjects() => System.IO.Directory.GetDirectories(Path).Select(o => new Project(this, System.IO.Path.GetFileName(o)));

        public Project GetProject(string name) => new Project(this, name);
    }
}