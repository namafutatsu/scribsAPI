using System.Web.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scribs.Models;
using Scribs.Filters;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class ProjectController : AccessController {

        [HttpPost]
        public Task<ProjectModel> Get(ProjectModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var project = user.GetProject(model.Name);
                return ProjectModelUtils.CreateProjectModelAsync(project, model.Read ?? false);
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
        public async Task<ProjectModel> Post(ProjectModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var project = new Project(user, model.Name);
                project.Template = model.Template;
                project.Type = (Project.Types)model.Type;
                bool exists = await project.ExistsAsync();
                if (exists)
                    throw new System.Exception("This project already exists");
                await project.CreateDirectoryAsync();
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
