using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Scribs {

    public class File : FileSystemItem {
        public File(ScribsDbContext db, Project project, XElement xelement) : base(db, project, xelement) { }

        public async Task<string> DownloadTextAsync() {
            using (FileStream reader = System.IO.File.Open(Path, FileMode.Open)) {
                var result = new byte[reader.Length];
                await reader.ReadAsync(result, 0, (int)reader.Length);
                return System.Text.Encoding.UTF8.GetString(result);
            }
        }

        public Task UploadTextAsync(string content) {
            using (StreamWriter writer = new StreamWriter(Path.ToString())) {
                return writer.WriteAsync(content);
            }
        }

        public override bool Exists() => System.IO.File.Exists(Path.ToString());

        public override void Create() => System.IO.File.Create(Path.ToString());

        public override void Delete() => System.IO.File.Delete(Path.ToString());

        public void Move(File source) => System.IO.File.Move(source.Path, Path);

        public override void Move(FileSystemItem source) => Move(source as File);
    }
}