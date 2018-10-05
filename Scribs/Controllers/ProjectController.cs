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
        public UpdateModel Update(UpdateModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                foreach (var instruction in model.Instructions) {
                    var item = instruction.Discriminator == Discriminator.File ?
                        (IFileSystemItem)new File(user, instruction.Path) : new Directory(user, instruction.Path);
                    switch (instruction.Type) {
                        case (InstructionType.Create):
                            item.Create();
                            item.Key = instruction.Key;
                            item.Index = instruction.Index;
                            break;
                        case InstructionType.Delete:
                            item.Delete();
                            break;
                        case InstructionType.Move:
                            var newItem = instruction.Discriminator == Discriminator.File ?
                                (IFileSystemItem)new File(user, instruction.MoveToPath) : new Directory(user, instruction.MoveToPath);
                            newItem.CopyFrom(item);
                            break;
                    }
                    instruction.Done = true;
                }
                return model;
            }
        }
    }
}
