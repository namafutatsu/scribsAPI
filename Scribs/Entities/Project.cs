using Microsoft.WindowsAzure.Storage.File;

namespace Scribs {

    public class Project : Directory {
        public Project(CloudFileDirectory cloudItem) : base(cloudItem) { }
        public Project(User user, string path) : base(user, path) { }

        public static Project GetFromDirectory(Directory directory) => new Project(directory.CloudItem);
    }
}