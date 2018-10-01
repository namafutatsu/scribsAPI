using System.Web.Http;
using Scribs.Models;
using Scribs.Filters;
using System;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class DirectoryController : AccessController {

        [HttpPost]
        public DirectoryModel Get(FSItemModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var directory = FileSystem.GetDirectory(user, model.Path);
                if (directory == null)
                    throw new Exception("Répertoire non trouvé");
                return DirectoryModelUtils.CreateDirectoryModel(directory, model.Read ?? false);
            }
        }
    }
}
