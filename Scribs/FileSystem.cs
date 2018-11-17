using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;

namespace Scribs {

    public static class FileSystem {

        //public const string SHARE_FILE = "scribs";
        public const string STORAGE = @"D:\Scribs-Storage";

        //private static Directory rootDir;
        //public static Directory GetRootDir(ScribsDbContext db) {
        //    if (rootDir == null) {
        //        var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        //        var fileClient = storageAccount.CreateCloudFileClient();
        //        var share = fileClient.GetShareReference(SHARE_FILE);
        //        if (!share.Exists())
        //            throw new System.Exception("This file share does not exist");
        //        rootDir = new Directory(db, share.GetRootDirectoryReference());
        //    }
        //    return rootDir;
        //}

        //public static E GetItem<E>(this IFileSystemFactory<E> factory, User user, string url)
        //    where E : class, IListFileItem {
        //    var path = new Path(user.db, url);
        //    if (path.Share != SHARE_FILE || path.UserName != user.Name)
        //        throw new HttpResponseException(HttpStatusCode.Unauthorized);
        //    var relativePath = path.Relative;
        //    return factory.GetCloudReference(user.Directory, relativePath);
        //}
    }

    public abstract class FileSystemItem {
        public Project Project { get; protected set; }
        protected XElement node;
        public string Url => new List<string> { Name }.Concat(node?.Ancestors().Select(o => (string)o.Attribute("name"))).Reverse().Aggregate(System.IO.Path.Combine);
        public ScribsDbContext Db { get; set; }
        public string Name {
            get {
                return (string)node.Attribute("name");
            }
            set {
                node.SetAttributeValue("name", value);
            }
        }
        public string Key {
            get {
                return (string)node.Attribute("key");
            }
            set {
                node.SetAttributeValue("key", value);
            }
        }
        public int Index {
            get {
                return (int)node.Attribute("index");
            }
            set {
                node.SetAttributeValue("index", value);
            }
        }
        protected string Path => System.IO.Path.Combine(Project.User.Path, Url);
        public FileSystemItem(ScribsDbContext db, Project project, XElement node) {
            Db = db;
            Project = project;
            this.node = node;
        }
        public abstract bool Exists();
        public abstract void Create();
        public abstract void Delete();
        public abstract void Move(FileSystemItem source);
    }
}