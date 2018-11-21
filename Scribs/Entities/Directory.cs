using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Scribs {

    public class Directory : FileSystemItem {

        public static string Type => "directory";

        public Directory(ScribsDbContext db, Project project, XElement xelement) : base(db, project, xelement) { }

        public IEnumerable<File> Files => Node.Elements().Where(o => o.Name == File.Type).Select(o => Project.GetFile((string)o.Attribute("key")));

        public IEnumerable<Directory> Directories => Node.Elements().Where(o => o.Name == Type).Select(o => Project.GetDirectory((string)o.Attribute("key")));

        public IOrderedEnumerable<FileSystemItem> GetItems() => Files.Cast<FileSystemItem>().Concat(Directories.Cast<FileSystemItem>()).OrderBy(o => o.Index);

        public Directory GetDirectory(string name) => Project.GetDirectory((string)(Node.Elements().SingleOrDefault(o => (string)o.Attribute("name") == name).Attribute("key")));

        public File GetFile(string name) => Project.GetFile((string)(Node.Elements().SingleOrDefault(o => (string)o.Attribute("name") == name).Attribute("key")));

        public bool CreateIfNotExists() {
            if (!ExistsItem()) {
                CreateItem();
                return false;
            }
            return true;
        }

        public override void MoveItem(string source) => System.IO.Directory.Move(source, Path);

        public override bool ExistsItem() => System.IO.Directory.Exists(Path);

        public override void CreateItem() => System.IO.Directory.CreateDirectory(Path);

        public override void DeleteItem() => System.IO.Directory.Delete(Path, true);

        public override void Delete() {
            foreach (var file in Files)
                Project.Remove(file);
            foreach (var directory in Directories)
                Project.Remove(directory);
            base.Delete();
        }

        public override void Rename(string name) {
            string source = Path;
            base.Rename(name);
            System.IO.Directory.Move(source, Path);
        }
    }
}