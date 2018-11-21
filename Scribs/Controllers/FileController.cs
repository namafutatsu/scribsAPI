using System.Threading.Tasks;
using System.Web.Http;
using Scribs.Models;
using Scribs.Filters;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class FileController : AccessController {

        [HttpPost]
        public Task<ItemModel> Get(FileModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var file = GetItem(user, model.Project, model.Key) as File;
                return FileModelUtils.CreateFileModelAsync(file, model.Read ?? false);
            }
        }

        [HttpPost]
        public Task<string> Read(FileModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var file = GetItem(user, model.Project, model.Key) as File;
                return file.DownloadTextAsync();
            }
        }

        public FileSystemItem GetItem(User user, string project, string key) {
            return user.GetProject(project).GetFile(key);
        }
    }
}
