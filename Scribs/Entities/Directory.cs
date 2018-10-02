using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.File;

namespace Scribs {

    public class Directory : FileSystemItem<CloudFileDirectory, CloudFileDirectoryFactory> {
        public Directory(CloudFileDirectory cloudItem) : base(cloudItem) {
        }

        public Directory(User user, string path) : base(user, path) {
        }

        public IEnumerable<File> Files =>
            CloudItem.ListFilesAndDirectories().Select(o => o as CloudFile).Where(o => o != null)
            .Select(o => new File(o));

        public IEnumerable<Directory> Directories =>
            CloudItem.ListFilesAndDirectories().Select(o => o as CloudFileDirectory).Where(o => o != null && !o.Name.StartsWith("."))
            .Select(o => new Directory(o));

        public Directory GetDirectory(string name) {
            return new Directory(CloudItem.GetDirectoryReference(name));
        }

        public bool Exists() => CloudItem.Exists();

        public void CreateIfNotExistsAsync() => CreateIfNotExistsAsync();
    }

    public class CloudFileDirectoryFactory : IFileSystemFactory<CloudFileDirectory> {
        public CloudFileDirectory GetCloudReference(Directory dir, string relativePath) {
            var reference = dir.CloudItem.GetDirectoryReference(relativePath);
            if (!reference.ExistsAsync().Result)
                throw new Exception("Directory not found");
            return reference;
        }
    }
}