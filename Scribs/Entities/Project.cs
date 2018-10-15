using Microsoft.WindowsAzure.Storage.File;
using System.Threading.Tasks;

namespace Scribs {

    public class Project : Directory {
        public Project(ScribsDbContext db, CloudFileDirectory cloudItem) : base(db, cloudItem) { }
        public Project(User user, string name) : base(user, "/" + FileSystem.SHARE_FILE + "/" + user.Name + "/" + name) { }

        public enum Types {
            Novel = 0,
            ShortStory = 1
        }

        public string Description {
            get {
                return this.GetMetadata(MetadataUtils.Description);
            }
            set {
                this.SetMetadata(MetadataUtils.Description, value);
            }
        }

        public string Structure {
            get {
                return this.GetMetadata(MetadataUtils.Template);
            }
            set {
                this.SetMetadata(MetadataUtils.Template, value);
            }
        }

        public Types Type {
            get {
                return this.GetMetadata(MetadataUtils.Type);
            }
            set {
                this.SetMetadata(MetadataUtils.Type, value);
            }
        }

        public static Project GetFromDirectory(ScribsDbContext db, Directory directory) => new Project(db, directory.CloudItem);
        public Task<bool> CreateDirectoryAsync() => CloudItem.CreateIfNotExistsAsync();
    }
}