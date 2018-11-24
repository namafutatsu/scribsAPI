using System;
using System.Linq;
using System.Xml.Linq;

namespace Scribs {

    public abstract class FileSystemItem {
        public const string STORAGE = @"D:\Scribs-Storage";
        public Project Project { get; protected set; }
        public XElement Node { get; protected set; }
        public Directory Parent => Project.GetDirectory((string)Node.Ancestors().FirstOrDefault().Attribute("key"));
        public virtual string Url => Node?.AncestorsAndSelf().Select(o => (string)o.Attribute("name")).Reverse().Aggregate(System.IO.Path.Combine);
        public ScribsDbContext Db { get; set; }
        public virtual string Name {
            get => (string)Node.Attribute("name");
            set => Node.SetAttributeValue("name", value);
        }
        public string Key {
            get => (string)Node.Attribute("key");
            set => Node.SetAttributeValue("key", value);
        }
        public int Index {
            get => (int)Node.Attribute("index");
            set => Node.SetAttributeValue("index", value);
        }
        public DateTime Time {
            get => Node.Attribute("time") == null ? DateTime.MinValue : (DateTime)Node.Attribute("time");
            set => Node.SetAttributeValue("time", value);
        }
        protected string Path => System.IO.Path.Combine(Project.User.Path, Url);
        public FileSystemItem(ScribsDbContext db, Project project, XElement node) {
            Db = db;
            Project = project;
            Node = node;
        }
        public abstract bool ExistsItem();
        public abstract void CreateItem();
        public abstract void DeleteItem();
        public abstract void MoveItem(string source);
        public static FileSystemItem Create(Directory parent, string type, string name, string key, int index) {
            var node = new XElement(type);
            node.SetAttributeValue("name", name);
            node.SetAttributeValue("key", key);
            node.SetAttributeValue("index", index);
            parent.Node.Add(node);
            var item = GetItemInstance(parent.Db, parent.Project, node);
            item.CreateItem();
            return item;
        }
        public virtual void Delete() {
            DeleteItem();
            Project.Remove(this);
            Node.Remove();
        }
        public virtual void Move(Directory parent) {
            string path = Path;
            Node.Remove();
            parent.Node.Add(Node);
            MoveItem(path);
        }
        public virtual void Rename(string name) {
            Name = name;
        }
        public static FileSystemItem GetItemInstance(ScribsDbContext db, Project project, XElement node) {
            if (node.Name == Directory.Type)
                return new Directory(db, project, node) as FileSystemItem;
            return new File(db, project, node) as FileSystemItem;
        }
    }
}