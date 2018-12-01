using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Scribs {

    public class File : FileSystemItem {
        public static bool FromMd = false;

        public static string Type => "file";

        public File(ScribsDbContext db, Project project, XElement xelement) : base(db, project, xelement) { }

        public async Task<string> DownloadTextAsync() {
            using (FileStream stream = System.IO.File.Open(Path, FileMode.Open)) {
                var result = new byte[stream.Length];
                await stream.ReadAsync(result, 0, (int)stream.Length);
                stream.Close();
                string text = System.Text.Encoding.UTF8.GetString(result);
                if (FromMd && Name.EndsWith(".md")) {
                    using (TextReader reader = new StringReader(text)) {
                        using (var writer = new StringWriter()) {
                            CommonMark.CommonMarkConverter.Convert(reader, writer);
                            text = writer.GetStringBuilder().ToString();
                        }
                    }
                }
                return text;
            }
        }

        public async Task UploadTextAsync(string content, DateTime time) {
            using (StreamWriter writer = new StreamWriter(Path.ToString())) {
                await writer.WriteAsync(content);
                writer.Close();
            }
            Time = time;
            Project.Save();
        }

        public override bool ExistsItem() => System.IO.File.Exists(Path.ToString());

        public override void CreateItem() {
            using (StreamWriter writer = new StreamWriter(Path.ToString())) {
                writer.Write(" ");
                writer.Close();
            }
        }

        public override void DeleteItem() => System.IO.File.Delete(Path.ToString());

        public void Move(File source) => System.IO.File.Move(source.Path, Path);

        public override void MoveItem(string source) => System.IO.File.Move(source, Path);

        public override void Rename(string name) {
            string source = Path;
            base.Rename(name);
            System.IO.File.Move(source, Path);
        }
    }
}