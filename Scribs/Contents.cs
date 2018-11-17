//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;
//using System.Xml;
//using System.Xml.Linq;

//namespace Scribs {
//    public class Contents {
//        private Project project;
//        private string path;
//        private XElement tree;
//        private Dictionary<string, FileSystemItem> dictionary = new Dictionary<string, FileSystemItem>();
//        private ScribsDbContext db;

//        public Contents(ScribsDbContext db, Project project) {
//            this.db = db;
//            this.project = project;
//            //path = project.Path.ToString() + @"\.contents.xml";
//            path = @"D:\Scribs-Storage\.contents.xml";
//            try {
//                tree = XElement.Load(path);
//            } catch {
//                FromScratch();
//                Save();
//            }
//        }

//        private void FromScratch() {
//            tree = Generate(project);
//            tree.Name = "project";
//        }

//        public void Save() {
//            tree.Save(path);
//        }

//        private XElement Generate(File file, int index = 0) {
//            var element = new XElement("file");
//            FillElement(element, file, index);
//            return element;
//        }

//        private XElement Generate(Directory directory, int index = 0) {
//            var element = new XElement("directory");
//            FillElement(element, directory, index);
//            int childIndex = 0;
//            foreach (var child in directory.Directories)
//                element.Add(Generate(child, childIndex++));
//            foreach (var child in directory.Files)
//                element.Add(Generate(child, childIndex++));
//            return element;
//        }

//        private void FillElement(XElement element, FileSystemItem item, int index) {
//            element.SetAttributeValue("key", Guid.NewGuid().ToString());
//            element.SetAttributeValue("index", index.ToString());
//            element.SetAttributeValue("name", item.Name);
//        }

//        public File GetFile(string key) {
//            if (dictionary.ContainsKey(key))
//                return dictionary[key] as File;
//            var element = tree.Descendants("file").SingleOrDefault(o => (string)o.Attribute("key") == key);
//            var file = new File(db, element);
//            dictionary.Add(key, file as FileSystemItem);
//            return file;
//        }

//        public Directory GetDirectory(string key) {
//            if (dictionary.ContainsKey(key))
//                return dictionary[key] as Directory;
//            var element = tree.Descendants("directory").SingleOrDefault(o => (string)o.Attribute("key") == key);
//            var directory = new Directory(db, element);
//            dictionary.Add(key, directory as FileSystemItem);
//            return directory;
//        }

//        //public async Task TestWriter() {
//        //    XElement tree = new XElement("root");
//        //    XmlWriterSettings settings = new XmlWriterSettings();
//        //    settings.Async = true;
//        //    settings.Indent = true;
//        //    using (XmlWriter writer = XmlWriter.Create(this.path, settings)) {
//        //        await writer.WriteStartElementAsync(null, "root", null);
//        //        await Generate(writer, project);
//        //        await writer.FlushAsync();
//        //    }
//        //}

//        //public async Task Generate(XmlWriter writer, File file, int index = 0) {
//        //    await writer.WriteStartElementAsync(null, "file", null);
//        //    await Generate(writer, file, index);
//        //    await writer.WriteEndElementAsync();
//        //}

//        //public async Task Generate(XmlWriter writer, Directory directory, int index = 0) {
//        //    await writer.WriteStartElementAsync(null, "directory", null);
//        //    await Generate(writer, directory, index);
//        //    int childIndex = 0;
//        //    foreach (var child in directory.Directories)
//        //        await Generate(writer, child, childIndex++);
//        //    foreach (var child in directory.Files)
//        //        await Generate(writer, child, childIndex++);
//        //    await writer.WriteEndElementAsync();
//        //}

//        //public async Task Generate(XmlWriter writer, IFileSystemItem item, int index) {
//        //    await writer.WriteAttributeStringAsync(null, "key", null, Guid.NewGuid().ToString());
//        //    await writer.WriteAttributeStringAsync(null, "index", null, index.ToString());
//        //    await writer.WriteAttributeStringAsync(null, "name", null, item.Name);
//        //}

//        //public async void Read() {
//        //    using (XmlReader reader = XmlReader.Create("perls.xml")) {
//        //        reader.Get
//        //        while (reader.Read()) {
//        //            if (reader.IsStartElement()) {
//        //                switch (reader.Name) {

//        //                }
//        //            }
//        //        }
//        //    }
//        //}
//    }
//}