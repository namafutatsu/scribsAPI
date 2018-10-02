using Microsoft.WindowsAzure.Storage.File;

namespace Scribs {

    public class Project : Directory {
        public Project(CloudFileDirectory cloudItem) : base(cloudItem) { }
        public Project(User user, string name) : base(user, "/" + FileSystem.SHARE_FILE + "/" + user.Name + "/" + name) { }

        public static Project GetFromDirectory(Directory directory) => new Project(directory.CloudItem);
        public void CreateDirectory() => CloudItem.CreateIfNotExistsAsync();
    }
}