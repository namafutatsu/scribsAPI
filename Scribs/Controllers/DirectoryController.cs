using System.Web.Http;
using Scribs.Models;
using Scribs.Filters;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class DirectoryController : ItemController {

        [HttpPost]
        public DirectoryModel Get(DirectoryModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var directory = new Directory(user, model.Path);
                return DirectoryModelUtils.CreateDirectoryModel(directory, model.Read ?? false);
            }
        }

        public override IFileSystemItem GetItem(User user, string path) {
            return new Directory(user, path);
        }
    }
}
