using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.File;

namespace Scribs {

    public class Directory : FileSystemItem<CloudFileDirectory, CloudFileDirectoryFactory> {
        public Directory(CloudFileDirectory cloudItem) : base(cloudItem) { }

        public Directory(User user, string path) : base(user, path) { }

        public IEnumerable<File> Files =>
            CloudItem.ListFilesAndDirectories().Select(o => o as CloudFile).Where(o => o != null)
            .Select(o => new File(o));

        public IEnumerable<Directory> Directories =>
            CloudItem.ListFilesAndDirectories().Select(o => o as CloudFileDirectory).Where(o => o != null && !o.Name.StartsWith("."))
            .Select(o => new Directory(o));

        public Directory GetDirectory(string name) {
            return new Directory(CloudItem.GetDirectoryReference(name));
        }

        public override bool Exists() => CloudItem.Exists();

        public void CreateIfNotExistsAsync() => CloudItem.CreateIfNotExistsAsync();

        public IEnumerable<IListFileItem> ListFilesAndDirectories() => CloudItem.ListFilesAndDirectories();

        public override void Create() => CloudItem.CreateIfNotExistsAsync();

        public override void Delete() => CloudItem.Delete();

        public override void CopyFrom(IFileSystemItem source) => CopyFrom(source as Directory);

        public void CopyFrom(Directory source) {
            Create();
            foreach (var sourceFile in source.Files) {
                var file = new File(User, Path + "/" + sourceFile.Name);
                file.CopyFrom(sourceFile);
            }
            foreach (var sourceSubDir in source.Directories) {
                var subDir = new Directory(User, Path + "/" + sourceSubDir.Name);
                subDir.CopyFrom(sourceSubDir);
            }
            source.Delete();
        }

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
    }

    public class CloudFileDirectoryFactory : IFileSystemFactory<CloudFileDirectory> {
        public CloudFileDirectory GetCloudReference(Directory dir, string relativePath) =>
            dir.CloudItem.GetDirectoryReference(relativePath);
    }
}