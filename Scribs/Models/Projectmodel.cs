using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scribs.Models {
    public class ProjectModel : DirectoryModel {
    }

    public static class ProjectModelUtils {
        public static async Task<ProjectModel> CreateProjectModelAsync(Directory directory, bool read = false) {
            var model = new ProjectModel {
                Path = directory.Path.ToString(),
                Name = directory.Name,
                Items = new List<ItemModel>(),
                Discriminator = Discriminator.Directory,
                Index = directory.Index,
                Key = directory.Key
            };
            if (directory.CloudItem.Metadata.ContainsKey("Index"))
                model.Index = int.Parse(directory.CloudItem.Metadata["Index"]);
            var dirModel = await DirectoryModelUtils.CreateDirectoryModelAsync(directory, read);
            model.Items = (dirModel as DirectoryModel).Items;
            return model;
        }
    }
}