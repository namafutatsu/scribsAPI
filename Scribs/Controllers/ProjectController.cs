using System.Web.Http;
using Scribs.Models;
using Scribs.Filters;
using System.Collections.Generic;
using System.Linq;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class ProjectController : AccessController {

        [HttpPost]
        public DirectoryModel Get(DirectoryModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var project = user.GetProject(model.Name);
                return DirectoryModelUtils.CreateDirectoryModel(project, model.Read ?? false);
            }
        }

        [HttpPost]
        public IEnumerable<DirectoryModel> GetAll() {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var projects = user.GetProjects();
                return projects.Select(o => new DirectoryModel {
                    Name = o.Name,
                    Path = o.Path
                });
            }
        }
    }
}
