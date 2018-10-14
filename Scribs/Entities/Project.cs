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
                if (CloudItem.Metadata.ContainsKey("Description"))
                    return CloudItem.Metadata["Description"];
                string description = "";
                CloudItem.Metadata.Add("Description", description);
                return description;
            }
            set {
                if (CloudItem.Metadata.ContainsKey("Description"))
                    CloudItem.Metadata["Description"] = value;
                else
                    CloudItem.Metadata.Add("Description", value);
            }
        }

        public string Template {
            get {
                if (CloudItem.Metadata.ContainsKey("Template"))
                    return CloudItem.Metadata["Template"];
                string template = "";
                CloudItem.Metadata.Add("Template", template);
                return template;
            }
            set {
                if (CloudItem.Metadata.ContainsKey("Template"))
                    CloudItem.Metadata["Template"] = value;
                else
                    CloudItem.Metadata.Add("Template", value);
            }
        }

        public Types Type {
            get {
                if (CloudItem.Metadata.ContainsKey("Type"))
                    return (Types)int.Parse(CloudItem.Metadata["Type"]);
                CloudItem.Metadata.Add("Type", ((int)Types.Novel).ToString());
                return Types.Novel;
            }
            set {
                if (CloudItem.Metadata.ContainsKey("Type"))
                    CloudItem.Metadata["Type"] = ((int)value).ToString();
                else
                    CloudItem.Metadata.Add("Type", ((int)value).ToString());
            }
        }

        public static Project GetFromDirectory(ScribsDbContext db, Directory directory) => new Project(db, directory.CloudItem);
        public Task<bool> CreateDirectoryAsync() => CloudItem.CreateIfNotExistsAsync();
    }
}