using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Scribs.Models;
using Scribs.Filters;
using ScriboAPI.Models;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class ProjectController : AccessController {

        [HttpPost]
        public async Task<TreeNodeModel> Get(ProjectModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var project = user.GetProject(model.Name);
                List<SheetTemplate> templates = null;
                if (model.Read.HasValue && model.Read.Value)
                    templates = db.SheetTemplates.Where(o => o.ProjectKey == project.Key).ToList();
                var result = await ProjectModelUtils.CreateProjectModelAsync(project, model.Read ?? false);
                return ProjectModelUtils.ProjectToTreeItemModel(result);
            }
        }

        [HttpGet]
        public IEnumerable<ProjectModel> GetAll() {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var projects = user.GetProjects();
                return projects.Select(o => new ProjectModel {
                    Name = o.Name,
                    Key = o.Key
                });
            }
        }

        [HttpPost]
        public TreeNodeModel Put(TreeNodeModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                var project = user.GetProject(model.label);
                var models = new Dictionary<string, TreeNodeModel>();
                // Creations/Modifications
                UpdateDirectory(db, model, project, models);
                // Deletions
                var keys = project.GetAllItems().Select(o => o.Key).ToList();
                var deleted = new HashSet<string>();
                foreach (var key in keys) {
                    if (!deleted.Contains(key) && project.TryGetItem(key, null, out FileSystemItem item)) {
                        if (item != null && !models.ContainsKey(key)) {
                            item.Delete();
                            var directory = item as Directory;
                            if (directory != null)
                                foreach (var node in directory.Node.Descendants())
                                    deleted.Add((string)node.Attribute("key"));
                            deleted.Add(item.Key);
                        }
                    }
                }
                project.Save();
                return model;
            }
        }

        private void UpdateDirectory(ScribsDbContext db, TreeNodeModel model, Project project, Dictionary<string, TreeNodeModel> models) {
            models.Add(model.Key, model);
            Directory directory = project;
            Directory parent = null;
            if (model.ParentKey != null)
                project.TryGetDirectory(model.ParentKey, out parent);
            if (model.Key != project.Key) {
                if (!project.TryGetDirectory(model.Key, out directory)) {
                    FileSystemItem.Create(parent, Directory.Type, model.label, model.Key, model.Index);
                } else {
                    if (directory.Parent != parent)
                        directory.Move(parent);
                    if (directory.Index != model.Index)
                        directory.Index = model.Index;
                    if (directory.Name != model.label)
                        directory.Rename(model.label);
                }
            }
            foreach (var child in model.children.Where(o => !o.IsLeaf))
                UpdateDirectory(db, child, project, models);
            foreach (var child in model.children.Where(o => o.IsLeaf))
                UpdateFile(db, child, project, models);
        }

        private void UpdateFile(ScribsDbContext db, TreeNodeModel model, Project project, Dictionary<string, TreeNodeModel> models) {
            models.Add(model.Key, model);
            Directory parent = model.ParentKey == project.Key ? project : project.GetDirectory(model.ParentKey);
            if (!project.TryGetFile(model.Key, out File file)) {
                FileSystemItem.Create(parent, File.Type, model.label, model.Key, model.Index);
            } else {
                if (file.Parent != parent)
                    file.Move(parent);
                if (file.Index != model.Index)
                    file.Index = model.Index;
                if (file.Name != model.label)
                    file.Rename(model.label);
            }
        }

        [HttpPost]
        public async Task<ProjectModel> PostAsync(ProjectModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                int index = user.GetProjects().Count();
                var project = new Project(user, model.Name);
                if (project.ExistsItem())
                    throw new System.Exception("This project already exists");
                project.CreateItem();
                project.Type = (Project.ProjectTypes)model.Type;
                project.Description = model.Description;
                project.Structure = model.Structure.Aggregate((a, b) => a + ";" + b);
                project.Index = 0;
                project.Key = Guid.NewGuid().ToString();

                // Basic sheet templates
                //var charTemplate = SheetTemplate.Factory.CreateInstance(db);
                //charTemplate.User = user;
                //charTemplate.ProjectKey = project.Key;
                //charTemplate.Name = "Characters";
                //int i = 0;
                //foreach (string label in new List<string> {
                //    "Name",
                //    "Description",
                //    "Personality",
                //    "Occupation",
                //    "Objective",
                //    "Habits",
                //    "Conflicts",
                //    "Relatives",
                //    "Notes"
                //}) {
                //    var field = SheetTemplateField.Factory.CreateInstance(db);
                //    field.Index = i++;
                //    field.Label = label;
                //    field.SheetTemplate = charTemplate;
                //    //charTemplate.SheetTemplateFields.Add(field);
                //}
                //var setTemplate = SheetTemplate.Factory.CreateInstance(db);
                //setTemplate.User = user;
                //setTemplate.ProjectKey = project.Key;
                //setTemplate.Name = "Settings";
                //i = 0;
                //foreach (string label in new List<string> {
                //    "Name",
                //    "Description",
                //    "Sights",
                //    "Sounds",
                //    "Smells",
                //    "Notes"
                //}) {
                //    var field = SheetTemplateField.Factory.CreateInstance(db);
                //    field.Index = i++;
                //    field.Label = label;
                //    field.SheetTemplate = setTemplate;
                //    //setTemplate.SheetTemplateFields.Add(field);
                //}
                await db.SaveChangesAsync();

                // Structure generation
                var folders = model.Structure.Length > 1 ? model.Structure.Take(model.Structure.Length - 1) :
                    new List<string> { "folder" };
                var file = model.Structure.Length > 0 ? model.Structure.Last() : "file";
                var directory = project as Directory;
                foreach (string folder in folders) {
                    string directoryName = folder.Substring(0, 1).ToUpper() + folder.Substring(1, folder.Length - 1) + " 1";
                    directory = directory.GetDirectory(directoryName);
                    directory.CreateItem();
                    directory.Index = 0;
                    directory.Key = Guid.NewGuid().ToString();
                }
                string fileName = file.Substring(0, 1).ToUpper() + file.Substring(1, file.Length - 1) + " 1";
                var fileItem = directory.GetFile(fileName);
                fileItem.CreateItem();
                fileItem.Index = 0;
                fileItem.Key = Guid.NewGuid().ToString();

                return new ProjectModel {
                    Name = model.Name,
                    Discriminator = Discriminator.Directory,
                    Key = project.Key
                };
            }
        }
    }
}
