using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Scribs {

    public class Directory : FileSystemItem {
        public Directory(ScribsDbContext db, Project project, XElement xelement) : base(db, project, xelement) { }

        public IEnumerable<File> Files => node.Elements().Where(o => o.Name == "file").Select(o => Project.GetFile((string)o.Attribute("key")));

        public IEnumerable<Directory> Directories => node.Elements().Where(o => o.Name == "directory").Select(o => Project.GetDirectory((string)o.Attribute("key")));

        public IOrderedEnumerable<FileSystemItem> GetItems() => Files.Cast<FileSystemItem>().Concat(Directories.Cast<FileSystemItem>()).OrderBy(o => o.Index);

        public Directory GetDirectory(string name) => Project.GetDirectory((string)(node.Elements().SingleOrDefault(o => (string)o.Attribute("name") == name).Attribute("key")));

        public File GetFile(string name) => Project.GetFile((string)(node.Elements().SingleOrDefault(o => (string)o.Attribute("name") == name).Attribute("key")));

        public bool CreateIfNotExists() {
            if (!Exists()) {
                Create();
                return false;
            }
            return true;
        }

        public override void Move(FileSystemItem source) => Move(source as Directory);

        public void Move(Directory source) => System.IO.Directory.Move(source.Path, Path);
            //Create();
            //foreach (var sourceFile in source.Files) {
            //    var file = project.GetFile()
            //    await file.CopyFromAsync(sourceFile);
            //}
            //foreach (var sourceSubDir in source.Directories) {
            //    var subDir = new Directory(User, Path + "/" + sourceSubDir.Name);
            //    await subDir.CopyFromAsync(sourceSubDir);
            //}
            //await source.DeleteAsync();

        public override bool Exists() => System.IO.Directory.Exists(Path);

        public override void Create() => System.IO.Directory.CreateDirectory(Path);

        public override void Delete() => System.IO.Directory.Delete(Path, true);
    }
}