using Microsoft.WindowsAzure.Storage.File;
using System.Threading.Tasks;

namespace Scribs {

    public class Project : Directory {
        public Project(ScribsDbContext db, CloudFileDirectory cloudItem) : base(db, cloudItem) { }
        public Project(User user, string name) : base(user, "/" + FileSystem.SHARE_FILE + "/" + user.Name + "/" + name) { }

        public static Project GetFromDirectory(ScribsDbContext db, Directory directory) => new Project(db, directory.CloudItem);
        public Task<bool> CreateDirectoryAsync() => CloudItem.CreateIfNotExistsAsync();
    }
}