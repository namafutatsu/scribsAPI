using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Scribs {

    public class Project : Directory {
        public User User { get; set; }
        private Dictionary<string, FileSystemItem> contents = new Dictionary<string, FileSystemItem>();
        public new string Path { get; private set; }
        private string name;
        public string ContentsPath => System.IO.Path.Combine(Path, ".contents.xml");

        public Project(ScribsDbContext db, XElement xelement) : base(db, null, xelement) => throw new NotImplementedException();

        public Project(User user, string name): base(user.db, null, null) {
            if (String.IsNullOrEmpty(name))
                throw new KeyNotFoundException($"Project '{name}' not found for user '{user.Name}'");
            this.name = name;
            User = user;
            Project = this;
            Path = System.IO.Path.Combine(user.Path, name);
            Load();
        }

        private void GenerateTree() {
            node = GenerateNodeDirectory(Path, name);
            node.Name = "project";
            Description = String.Empty;
            Structure = String.Empty;
            Type = Types.Novel;
            Save();
        }

        public void Load() {
            if (System.IO.File.Exists(ContentsPath))
                node = XElement.Load(ContentsPath);
            else
                GenerateTree();
        }

        public void Save() => node.Save(ContentsPath);

        private XElement GenerateNodeFile(string file, int index = 0) {
            var element = new XElement("file");
            FillNode(element, file, index);
            return element;
        }

        private XElement GenerateNodeDirectory(string path, string directory, int index = 0) {
            var element = new XElement("directory");
            FillNode(element, directory, index);
            int childIndex = 0;
            foreach (var child in System.IO.Directory.GetDirectories(path)) {
                string name = System.IO.Path.GetFileName(child);
                element.Add(GenerateNodeDirectory(System.IO.Path.Combine(path, name), name, childIndex++));
            }
            foreach (var child in System.IO.Directory.GetFiles(path)) {
                string name = System.IO.Path.GetFileName(child);
                element.Add(GenerateNodeFile(name, childIndex++));
            }
            return element;
        }

        private void FillNode(XElement element, string name, int index) {
            element.SetAttributeValue("key", Guid.NewGuid().ToString());
            element.SetAttributeValue("index", index.ToString());
            element.SetAttributeValue("name", name);
        }

        public new File GetFile(string key) {
            if (contents.ContainsKey(key))
                return contents[key] as File;
            var element = node.Descendants("file").SingleOrDefault(o => (string)o.Attribute("key") == key);
            if (element == null)
                throw new KeyNotFoundException($"File '{key}' does not exist for project '{Project.name}' of user '{User.Name}'");
            var file = new File(Db, this, element);
            contents.Add(key, file as FileSystemItem);
            return file;
        }

        public new Directory GetDirectory(string key) {
            if (contents.ContainsKey(key))
                return contents[key] as Directory;
            var element = node.Descendants("directory").SingleOrDefault(o => (string)o.Attribute("key") == key);
            if (element == null)
                throw new KeyNotFoundException($"Directory '{key}' does not exist for project '{Project.name}' of user '{User.Name}'");
            var directory = new Directory(Db, this, element);
            contents.Add(key, directory as FileSystemItem);
            return directory;
        }

        public enum Types {
            Novel = 0,
            ShortStory = 1
        }

        public string Description {
            get {
                return (string)node.Attribute("description");
            }
            set {
                node.SetAttributeValue("description", value);
            }
        }

        public string Structure {
            get {
                return (string)node.Attribute("structure");
            }
            set {
                node.SetAttributeValue("structure", value);
            }
        }

        public string[] GetStructure() {
            if (String.IsNullOrWhiteSpace(Structure))
                return new string[0];
            return Structure.Split(';');
        }

        public Types Type {
            get {
                return (Types)Enum.Parse(typeof(Types), (string)node.Attribute("type"));
            }
            set {
                node.SetAttributeValue("type", value.ToString());
            }
        }

        //public static Project GetFromDirectory(ScribsDbContext db, Directory directory) => new Project(db, directory.CloudItem);

        //public Task<bool> CreateDirectoryAsync() => CloudItem.CreateIfNotExistsAsync();

        //public void UdpateIndex() {
        //    int index = User.Directory.Directories.Max(o => o.Index) + 1;
        //    UdpateIndex(index);
        //}
    }
}