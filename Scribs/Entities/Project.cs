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
            Node = GenerateNodeDirectory(Path, name);
            Node.Name = "project";
            Description = String.Empty;
            Structure = String.Empty;
            Type = ProjectTypes.Novel;
            Save();
        }
        
        public void Load() {
            if (System.IO.File.Exists(ContentsPath))
                Node = XElement.Load(ContentsPath);
            else
                GenerateTree();
        }

        public void Save() => Node.Save(ContentsPath);

        private XElement GenerateNodeFile(string file, int index = 0) {
            var element = new XElement(File.Type);
            FillNode(element, file, index);
            return element;
        }

        private XElement GenerateNodeDirectory(string path, string directory, int index = 0) {
            var element = new XElement(Directory.Type);
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

        public void Remove(FileSystemItem item) => contents.Remove(item.Key);

        public bool TryGetItem(string key, string type, out FileSystemItem item) {
            item = null;
            if (contents.ContainsKey(key))
                item = contents[key];
            else {
                var element = (type == null ? Node.Descendants() : Node.Descendants(type))
                        .SingleOrDefault(o => (string)o.Attribute("key") == key);
                if (element == null)
                    return false;
                item = GetItemInstance(Db, this, element);
                contents.Add(key, item);
            }
            return true;
        }

        public FileSystemItem GetItem(string key) {
            if (!TryGetItem(key, null, out FileSystemItem item))
                throw new KeyNotFoundException($"Element '{key}' does not exist for project '{Project.name}' of user '{User.Name}'");
            return item;
        }

        public IEnumerable<FileSystemItem> GetAllItems(string type = null) =>
            (type == null ? Node.Descendants() : Node.Descendants(type))
            .Select(o => GetItem((string)o.Attribute("key")));

        public bool TryGetFile(string key, out File file) {
            if (TryGetItem(key, File.Type, out FileSystemItem item)) {
                file = item as File;
                return true;
            }
            file = null;
            return false;
        }

        public new File GetFile(string key) {
            if (!TryGetFile(key, out File file))
                throw new KeyNotFoundException($"File '{key}' does not exist for project '{Project.name}' of user '{User.Name}'");
            return file;
        }

        public IEnumerable<File> GetAllFiles() => GetAllItems(File.Type).Cast<File>();

        public bool TryGetDirectory(string key, out Directory directory) {
            directory = key == Key ? Project : null;
            if (directory != null)
                return true;
            if (TryGetItem(key, Directory.Type, out FileSystemItem item)) {
                directory = item as Directory;
                return true;
            }
            return false;
        }

        public new Directory GetDirectory(string key) {
            if (!TryGetDirectory(key, out Directory directory))
                throw new KeyNotFoundException($"Directory '{key}' does not exist for project '{Project.name}' of user '{User.Name}'");
            return directory;
        }

        public IEnumerable<Directory> GetAllDirectories() => GetAllItems(Directory.Type).Cast<Directory>();

        //public async System.Threading.Tasks.Task<Dictionary<string, string>> GetTextsAsync() {
        //    var texts = new Dictionary<string, string>();
        //    foreach (var kvp in contents) {
        //        var file = kvp.Value as File;
        //        if (file == null)
        //            continue;
        //        texts.Add(kvp.Key, await file.DownloadTextAsync());
        //    }
        //    return texts;
        //}

        public enum ProjectTypes {
            Novel = 0,
            ShortStory = 1
        }

        public string Description {
            get {
                return (string)Node.Attribute("description");
            }
            set {
                Node.SetAttributeValue("description", value);
            }
        }

        public string Structure {
            get {
                return (string)Node.Attribute("structure");
            }
            set {
                Node.SetAttributeValue("structure", value);
            }
        }

        public string[] GetStructure() {
            if (String.IsNullOrWhiteSpace(Structure))
                return new string[0];
            return Structure.Split(';');
        }

        public new ProjectTypes Type {
            get {
                return (ProjectTypes)Enum.Parse(typeof(ProjectTypes), (string)Node.Attribute("type"));
            }
            set {
                Node.SetAttributeValue("type", value.ToString());
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