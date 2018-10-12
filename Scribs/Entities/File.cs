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

        public override Task<bool> ExistsAsync() => CloudItem.ExistsAsync();

        public override Task CreateAsync() => CloudItem.CreateAsync(10000000);

        public override Task DeleteAsync() => CloudItem.DeleteAsync();

        public override Task CopyFromAsync(IFileSystemItem source) => CopyFromAsync(source as File);

        public async Task CopyFromAsync(File source) {
            await CreateAsync();
            await CloudItem.StartCopyAsync(source.CloudItem);
            await source.DeleteAsync();
        }
    }

    public class CloudFileFactory : IFileSystemFactory<CloudFile> {
        public CloudFile GetCloudReference(Directory dir, string relativePath) =>
            dir.CloudItem.GetFileReference(relativePath);
    }
}