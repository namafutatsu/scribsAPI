using System.Web.Http;
using System.Threading.Tasks;
using Scribs.Models;
using Scribs.Filters;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class DirectoryController : AccessController {

        [HttpPost]
        public Task<ItemModel> Get(DirectoryModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var directory = GetItem(user, model.Project, model.Key) as Directory;
                return DirectoryModelUtils.CreateDirectoryModelAsync(directory, model.Read ?? false);
            }
        }

        public FileSystemItem GetItem(User user, string project, string key) {
            return user.GetProject(project).GetDirectory(key);
        }
    }
}
