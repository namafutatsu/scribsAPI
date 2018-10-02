using System.Threading.Tasks;
using System.Web.Http;
using Scribs.Models;
using Scribs.Filters;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class FileController : AccessController {

        [HttpPost]
        public FileModel Get(FileModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var file = new File(user, model.Path);
                return FileModelUtils.CreateFileModel(file, model.Read ?? false);
            }
        }

        [HttpPost]
        public Task<string> Read(FileModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var file = new File(user, model.Path);
                return file.DownloadTextAsync();
            }
        }
    }
}
