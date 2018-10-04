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
                    Path = o.Path,
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
                    Path = project.Path,
                    Discriminator = Discriminator.Directory,
                    Key = project.Key
                };
            }
        }

        [HttpPost]
        IEnumerable<InstructionModel> Update(IList<InstructionModel> models) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                foreach (var model in models) {
                    IFileSystemItem item = model.Discriminator == Discriminator.File ?
                        (IFileSystemItem)new File(user, model.Path) : new Directory(user, model.Path);
                    switch (model.Type) {
                        case (InstructionType.Create):
                            item.Create();
                            item.Key = model.Key;
                            item.Index = model.Index;
                            break;
                        case InstructionType.Delete:
                            item.Delete();
                            break;
                        case InstructionType.Move:
                            item.Index = model.Index;
                            break;
                    }
                    model.Done = true;
                }
                return models;
            }
        }
    }
}
