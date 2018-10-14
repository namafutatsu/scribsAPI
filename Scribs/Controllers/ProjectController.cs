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
                bool exists = await project.ExistsAsync();
                if (exists)
                    throw new System.Exception("This project already exists");
                await project.CreateDirectoryAsync();
                project.Type = (Project.Types)model.Type;
                project.Template = model.Template;
                var templateSegments = project.Template.Split(';').ToList();
                var folders = templateSegments.Count > 1 ? templateSegments.Take(templateSegments.Count - 1) :
                    new List<string> { "folder" };
                var file = templateSegments.Count > 0 ? templateSegments.Last() : "file";
                var directory = project as Directory;
                foreach (string folder in folders) {
                    string directoryName = folder.Substring(0, 1).ToUpper() + folder.Substring(1, folder.Length - 1) + " 1";
                    directory = directory.GetDirectory(directoryName);
                    await directory.CreateAsync();
                }
                string fileName = file.Substring(0, 1).ToUpper() + file.Substring(1, file.Length - 1) + " 1";
                var fileItem = directory.GetFile(fileName);
                await fileItem.CreateAsync();
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
