using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;

namespace Scribs {

    public static class FileSystem {

        public const string SHARE_FILE = "scribs";
        private static Directory rootDir;
        public static Directory GetRootDir(ScribsDbContext db) {
            if (rootDir == null) {
                var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                var fileClient = storageAccount.CreateCloudFileClient();
                var share = fileClient.GetShareReference(SHARE_FILE);
                if (!share.Exists())
                    throw new System.Exception("This file share does not exist");
                rootDir = new Directory(db, share.GetRootDirectoryReference());
            }
            return rootDir;
        }

        public static E GetItem<E>(this IFileSystemFactory<E> factory, User user, string url)
            where E : class, IListFileItem {
            var path = new Path(user.db, url);
            if (path.Share != SHARE_FILE || path.UserName != user.Name)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            var relativePath = path.Relative;
            return factory.GetCloudReference(user.Directory, relativePath);
        }
    }

    public interface IFileSystemItem {
        string Key { get; set; }
        int Index { get; set; }
        Task<bool> ExistsAsync();
        Task CreateAsync();
        Task DeleteAsync();
        Task CopyFromAsync(IFileSystemItem source);
        IDictionary<string, string> Metadata { get; }
        void SetMetadata();
        void FetchAttributes();
        bool Fetched { get; set; }
    }

    public abstract class FileSystemItem<E, F>: IFileSystemItem where F : IFileSystemFactory<E>, new() where E : class, IListFileItem {
        public FileSystemItem(User user, string path) {
            db = user.db;
            CloudItem = CloudFactory.GetItem(user, path);
            Path = new Path(db, path);
        }
        public FileSystemItem(ScribsDbContext db, E cloudItem) {
            this.db = db;
            CloudItem = cloudItem;
            Path = new Path(db, cloudItem.Uri.LocalPath);
        }
        private static F cloudFactory;
        public static F CloudFactory {
            get {
                if (cloudFactory == null)
                    cloudFactory = new F();
                return cloudFactory;
            }
        }
        private User user;
        public User User {
            get {
                if (user == null)
                    user = Path.User;
                return user;
            }
        }
        public E CloudItem { get; }
        public string Name => Path.Last;
        public Path Path { get; set; }
        public string Key {
            get {
                return (this).GetMetadata(MetadataUtils.Key);
            }
            set {
                (this).SetMetadata(MetadataUtils.Key, value);
            }
        }
        public int Index {
            get {
                return this.GetMetadata(MetadataUtils.Index);
            }
            set {
                this.SetMetadata(MetadataUtils.Index, value);
            }
        }
        public abstract Task<bool> ExistsAsync();
        public abstract Task CreateAsync();
        public abstract Task DeleteAsync();
        public abstract Task CopyFromAsync(IFileSystemItem source);
        public abstract IDictionary<string, string> Metadata { get; }
        public abstract void SetMetadata();
        public bool Fetched { get; set; } = false;
        public abstract void FetchAttributes();

        public ScribsDbContext db;
    }

    public interface IFileSystemFactory<E> where E : class, IListFileItem {
        E GetCloudReference(Directory dir, string relativePath);
    }
}