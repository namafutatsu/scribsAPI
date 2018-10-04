using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Scribs {

    public static class FileSystem {

        public const string SHARE_FILE = "scribs";
        private static Directory rootDir;
        public static Directory RootDir {
            get {
                if (rootDir == null) {
                    var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                    var fileClient = storageAccount.CreateCloudFileClient();
                    var share = fileClient.GetShareReference(SHARE_FILE);
                    if (!share.Exists())
                        throw new System.Exception("This file share does not exist");
                    rootDir = new Directory(share.GetRootDirectoryReference());
                }
                return rootDir;
            }
        }

        public static E GetItem<E>(this IFileSystemFactory<E> factory, User user, string path)
            where E : class, IListFileItem {
            var segments = path.Split('/').Skip(path.StartsWith("/") ? 1 : 0).ToArray();
            if (segments[0] != SHARE_FILE || segments[1] != user.Name)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            if (user.Directory.Exists()) {
                var relativePath = segments.Skip(2).Aggregate((a, b) => a + "/" + b);
                return factory.GetCloudReference(user.Directory, relativePath);
            }
            return null;
        }
    }

    public interface IFileSystemItem {
        string Key { get; set; }
        int Index { get; set; }
        bool Exists();
        void Create();
        void Delete();
        void Copy(string path);
    }

    public abstract class FileSystemItem<E, F>: IFileSystemItem where F : IFileSystemFactory<E>, new() where E : class, IListFileItem {
        public FileSystemItem(User user, string path) {
            CloudItem = CloudFactory.GetItem(user, path);
        }
        public FileSystemItem(E cloudItem) {
            CloudItem = cloudItem;
        }
        private static F cloudFactory;
        public static F CloudFactory {
            get {
                if (cloudFactory == null)
                    cloudFactory = new F();
                return cloudFactory;
            }
        }
        public E CloudItem { get; }
        public Uri Uri => CloudItem?.Uri;
        public string Path => Uri.AbsolutePath;
        public string Name => CloudItem?.Uri.Segments.Last();
        public abstract string Key { get; set; }
        public abstract int Index { get; set; }
        public abstract bool Exists();
        public abstract void Create();
        public abstract void Delete();
        public abstract void Copy(string path);
    }

    public interface IFileSystemFactory<E> where E : class, IListFileItem {
        E GetCloudReference(Directory dir, string relativePath);
    }
}