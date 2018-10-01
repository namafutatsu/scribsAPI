using System.Web.Http;
using Scribs.Models;
using Scribs.Filters;
using System;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class FileController : AccessController {

        [HttpPost]
        public FileModel Get(FSItemModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var file = FileSystem.GetFile(user, model.Path);
                if (file == null)
                    throw new Exception("Fichier non trouvé");
                return FileModelUtils.CreateFileModel(file, true);
            }
        }
    }
}
