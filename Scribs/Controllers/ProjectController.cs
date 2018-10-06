using System.Web.Http;
using Scribs.Models;
using Scribs.Filters;
using System.Collections.Generic;
using System.Linq;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class ProjectController : AccessController {

        [HttpPost]
        public ProjectModel Get(ProjectModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var project = user.GetProject(model.Name);
                return ProjectModelUtils.CreateProjectModel(project, model.Read ?? false);
            }
        }

        [HttpGet]
        public IEnumerable<ProjectModel> GetAll() {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var projects = user.GetProjects();
                return projects.Select(o => new ProjectModel {
                    Name = o.Name,
                    Path = o.Path.ToString(),
                    Key = o.Key
                });
            }
        }

        [HttpPost]
        public ProjectModel Post(ProjectModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var project = new Project(user, model.Name);
                if (project.Exists())
                    throw new System.Exception("This project already exists");
                project.CreateDirectory();
                return new ProjectModel {
                    Name = model.Name,
                    Path = project.Path.ToString(),
                    Discriminator = Discriminator.Directory,
                    Key = project.Key
                };
            }
        }
    }
}
