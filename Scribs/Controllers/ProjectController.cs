using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
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
                List<SheetTemplate> templates = null;
                if (model.Read.HasValue && model.Read.Value)
                    templates = db.SheetTemplates.Where(o => o.ProjectKey == project.Key).ToList();
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
                project.Description = model.Description;
                project.Structure = model.Structure;

                // Basic sheet templates
                var charTemplate = SheetTemplate.Factory.CreateInstance(db);
                charTemplate.User = user;
                charTemplate.ProjectKey = project.Key;
                charTemplate.Name = "Characters";
                int i = 0;
                foreach (string label in new List<string> {
                    "Name",
                    "Description",
                    "Personality",
                    "Occupation",
                    "Objective",
                    "Habits",
                    "Conflicts",
                    "Relatives",
                    "Notes"
                }) {
                    var field = SheetTemplateField.Factory.CreateInstance(db);
                    field.Index = i++;
                    field.Label = label;
                    field.SheetTemplate = charTemplate;
                    //charTemplate.SheetTemplateFields.Add(field);
                }
                var setTemplate = SheetTemplate.Factory.CreateInstance(db);
                setTemplate.User = user;
                setTemplate.ProjectKey = project.Key;
                setTemplate.Name = "Settings";
                i = 0;
                foreach (string label in new List<string> {
                    "Name",
                    "Description",
                    "Sights",
                    "Sounds",
                    "Smells",
                    "Notes"
                }) {
                    var field = SheetTemplateField.Factory.CreateInstance(db);
                    field.Index = i++;
                    field.Label = label;
                    field.SheetTemplate = setTemplate;
                    //setTemplate.SheetTemplateFields.Add(field);
                }
                await db.SaveChangesAsync();

                // Structure generation
                var structureSegments = project.Structure.Split(';').ToList();
                var folders = structureSegments.Count > 1 ? structureSegments.Take(structureSegments.Count - 1) :
                    new List<string> { "folder" };
                var file = structureSegments.Count > 0 ? structureSegments.Last() : "file";
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
