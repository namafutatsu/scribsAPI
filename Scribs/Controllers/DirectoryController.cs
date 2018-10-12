using System.Web.Http;
using System.Threading.Tasks;
using Scribs.Models;
using Scribs.Filters;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class DirectoryController : ItemController {

        [HttpPost]
        public Task<ItemModel> Get(DirectoryModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var directory = new Directory(user, model.Path);
                return DirectoryModelUtils.CreateDirectoryModelAsync(directory, model.Read ?? false);
            }
        }

        public override IFileSystemItem GetItem(User user, string path) {
            return new Directory(user, path);
        }
    }
}
