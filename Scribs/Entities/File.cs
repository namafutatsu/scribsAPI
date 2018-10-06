using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.File;

namespace Scribs {

    public class File : FileSystemItem<CloudFile, CloudFileFactory> {
        public File(ScribsDbContext db, CloudFile cloudItem) : base(db, cloudItem) {
        }

        public File(User user, string path) : base(user, path) { }

        public Task<string> DownloadTextAsync() => CloudItem.DownloadTextAsync();

        public override string Key {
            get {
                if (CloudItem.Metadata.ContainsKey("Key"))
                    return CloudItem.Metadata["Key"];
                string key = Guid.NewGuid().ToString();
                CloudItem.Metadata.Add("Key", key);
                return key;
            }
            set {
                if (CloudItem.Metadata.ContainsKey("Key"))
                    CloudItem.Metadata["Key"] = value;
                else
                    CloudItem.Metadata.Add("Key", value);
            }
        }

        public override int Index {
            get {
                if (CloudItem.Metadata.ContainsKey("Index"))
                    return int.Parse(CloudItem.Metadata["Index"]);
                CloudItem.Metadata.Add("Index", "0");
                return 0;
            }
            set {
                if (CloudItem.Metadata.ContainsKey("Index"))
                    CloudItem.Metadata["Index"] = value.ToString();
                else
                    CloudItem.Metadata.Add("Index", value.ToString());
            }
        }

        public override bool Exists() => CloudItem.Exists();

        public override void Create() => CloudItem.Create(10000000);

        public override void Delete() => CloudItem.Delete();

        public override void CopyFrom(IFileSystemItem source) => CopyFrom(source as File);

        public void CopyFrom(File source) {
            Create();
            CloudItem.StartCopy(source.CloudItem);
            source.Delete();
        }
    }

    public class CloudFileFactory : IFileSystemFactory<CloudFile> {
        public CloudFile GetCloudReference(Directory dir, string relativePath) =>
            dir.CloudItem.GetFileReference(relativePath);
    }
}