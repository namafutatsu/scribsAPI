using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.File;

namespace Scribs {

    public class File : FileSystemItem<CloudFile, CloudFileFactory> {
        public File(ScribsDbContext db, CloudFile cloudItem) : base(db, cloudItem) {
        }

        public File(User user, string path) : base(user, path) { }

        public Task<string> DownloadTextAsync() => CloudItem.DownloadTextAsync();

        public override Task<bool> ExistsAsync() => CloudItem.ExistsAsync();

        public override Task CreateAsync() => CloudItem.CreateAsync(0);

        public override Task DeleteAsync() => CloudItem.DeleteAsync();

        public override Task CopyFromAsync(IFileSystemItem source) => CopyFromAsync(source as File);

        public async Task CopyFromAsync(File source) {
            await CreateAsync();
            await CloudItem.StartCopyAsync(source.CloudItem);
            await source.DeleteAsync();
        }

        public override IDictionary<string, string> Metadata => CloudItem.Metadata;

        public override void SetMetadata() => CloudItem.SetMetadata();

        public override void FetchAttributes() => CloudItem.FetchAttributes();
    }

    public class CloudFileFactory : IFileSystemFactory<CloudFile> {
        public CloudFile GetCloudReference(Directory dir, string relativePath) =>
            dir.CloudItem.GetFileReference(relativePath);
    }
}