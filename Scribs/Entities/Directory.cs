using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.File;

namespace Scribs {

    public class Directory : FileSystemItem<CloudFileDirectory, CloudFileDirectoryFactory> {
        public Directory(ScribsDbContext db, CloudFileDirectory cloudItem) : base(db, cloudItem) { }

        public Directory(User user, string path) : base(user, path) { }

        public IEnumerable<File> Files =>
            ListFilesAndDirectories().Select(o => o as CloudFile).Where(o => o != null)
            .Select(o => new File(db, o));

        public IEnumerable<Directory> Directories =>
            ListFilesAndDirectories().Select(o => o as CloudFileDirectory).Where(o => o != null && !o.Name.StartsWith("."))
            .Select(o => new Directory(db, o));

        public Directory GetDirectory(string name) => new Directory(db, CloudItem.GetDirectoryReference(name));

        public File GetFile(string name) => new File(db, CloudItem.GetFileReference(name));

        public IEnumerable<IListFileItem> ListFilesAndDirectories() => CloudItem.ListFilesAndDirectoriesSegmentedAsync(null).GetAwaiter().GetResult().Results;

        public Task<bool> CreateIfNotExistsAsync() => CloudItem.CreateIfNotExistsAsync();

        public override Task<bool> ExistsAsync() => CloudItem.ExistsAsync();

        public override Task CreateAsync() => CloudItem.CreateIfNotExistsAsync();

        public override Task DeleteAsync() => CloudItem.DeleteAsync();

        public override Task CopyFromAsync(IFileSystemItem source) => CopyFromAsync(source as Directory);

        public async Task CopyFromAsync(Directory source) {
            await CreateAsync();
            foreach (var sourceFile in source.Files) {
                var file = new File(User, Path + "/" + sourceFile.Name);
                await file.CopyFromAsync(sourceFile);
            }
            foreach (var sourceSubDir in source.Directories) {
                var subDir = new Directory(User, Path + "/" + sourceSubDir.Name);
                await subDir.CopyFromAsync(sourceSubDir);
            }
            await source.DeleteAsync();
        }

        public override IDictionary<string, string> Metadata => CloudItem.Metadata;
    }

    public class CloudFileDirectoryFactory : IFileSystemFactory<CloudFileDirectory> {
        public CloudFileDirectory GetCloudReference(Directory dir, string relativePath) =>
            dir.CloudItem.GetDirectoryReference(relativePath);
    }
}