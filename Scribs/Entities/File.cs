using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.File;

namespace Scribs {

    public class File : FileSystemItem<CloudFile, CloudFileFactory> {
        public File(CloudFile cloudItem) : base(cloudItem) {
        }

        public File(User user, string path) : base(user, path) {
        }

        public Task<string> DownloadTextAsync() {
            return CloudItem.DownloadTextAsync();
        }
    }

    public class CloudFileFactory : IFileSystemFactory<CloudFile> {
        public CloudFile GetCloudReference(Directory dir, string relativePath) {
            var reference = dir.CloudItem.GetFileReference(relativePath);
            if (!reference.ExistsAsync().Result)
                throw new Exception("File not found");
            return reference;
        }
    }
}