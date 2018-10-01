using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Scribs {

    public static class FileSystem {

        const string SHARE_FILE = "scribs";

        public enum Type {
            Directory,
            File
        }

        public static string[] GetSegments(string path) {
            return path.Split('/').Skip(1).ToArray();
        }

        public static bool IsAuthoziedPath(User user, string path) {
            var segments = GetSegments(path);
            return segments[0] == SHARE_FILE && segments[1] == user.Name;
        }

        public static string GetRelativePath(string path) {
            return GetSegments(path).Skip(2).Aggregate((a, b) => a + "/" + b);
        }


        public static IListFileItem GetFSItem(User user, string path, Type type) {
            if (!IsAuthoziedPath(user, path))
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            var userDir = GetUserDir(user.Name);
            if (userDir.Exists()) {
                var relativePath = GetRelativePath(path);
                if (type == Type.Directory)
                    return userDir.GetDirectoryReference(relativePath);
                else
                    return userDir.GetFileReference(relativePath);
            }
            return null;
        }

        public static CloudFileDirectory GetDirectory(User user, string path) {
            return (CloudFileDirectory)GetFSItem(user, path, Type.Directory);
        }

        public static CloudFile GetFile(User user, string path) {
            return (CloudFile)GetFSItem(user, path, Type.File);
        }

        public static string GetFileContent(User user, string path) {
            var file = GetFile(user, path);
            return file.DownloadTextAsync().Result;
        }

        private static CloudFileDirectory GetRootDir() {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var fileClient = storageAccount.CreateCloudFileClient();
            var share = fileClient.GetShareReference(SHARE_FILE);
            if (!share.Exists())
                throw new System.Exception("This file share does not exist");
            return share.GetRootDirectoryReference();
        }

        private static CloudFileDirectory GetUserDir(string username) {
            var rootDir = GetRootDir();
            return rootDir.GetDirectoryReference(username);
        }

        public static void CreateUserDir(User user) {
            var userDir = GetUserDir(user.Name);
            userDir.CreateIfNotExistsAsync();
        }
    }
}