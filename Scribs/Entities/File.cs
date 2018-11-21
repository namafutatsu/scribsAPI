using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Scribs {

    public class File : FileSystemItem {

        public static string Type => "file";

        public File(ScribsDbContext db, Project project, XElement xelement) : base(db, project, xelement) { }

        public async Task<string> DownloadTextAsync() {
            using (FileStream reader = System.IO.File.Open(Path, FileMode.Open)) {
                var result = new byte[reader.Length];
                await reader.ReadAsync(result, 0, (int)reader.Length);
                reader.Close();
                return System.Text.Encoding.UTF8.GetString(result);
            }
        }

        public async Task UploadTextAsync(string content) {
            using (StreamWriter writer = new StreamWriter(Path.ToString())) {
                await writer.WriteAsync(content);
                writer.Close();
            }
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