using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scribs.Models {

    public class ProjectModel : DirectoryModel {
        public string[] Structure { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public IList<SheetTemplateModel> Templates { get; set; }
    }

    public static class ProjectModelUtils {
        public static async Task<ProjectModel> CreateProjectModelAsync(Project project, bool read = false, IEnumerable<SheetTemplate> templates = null) {
            var model = new ProjectModel {
                Url = project.Url,
                Name = project.Name,
                Items = new List<ItemModel>(),
                Discriminator = Discriminator.Directory,
                Index = project.Index,
                Key = project.Key,
                Structure = project.GetStructure(),
                Type = (int)project.Type,
                Description = project.Description,
            };
            var dirModel = await DirectoryModelUtils.CreateDirectoryModelAsync(project, read);
            model.Items = (dirModel as DirectoryModel).Items;
            if (templates != null)
                model.Templates = templates.Select(o => SheetModelUtils.GetModel(o, true, true, true)).ToList();
            return model;
        }

        public static TreeNodeModel ProjectToTreeItemModel(ProjectModel o) {
            var node = DirectoryModelUtils.DirectoryToTreeItemModel(o, null, o.Structure, 0);
            node.Structure = o.Structure;
            node.Level = 0;
            node.Type = o.Type;
            node.Description = o.Description;
            node.expanded = true;
            return node;
        }
    }
}