using System.Collections.Generic;
using System.Linq;

namespace Scribs.Models {
    public class ProjectModel : DirectoryModel {
    }

    public static class ProjectModelUtils {
        public static ProjectModel CreateProjectModel(Directory directory, bool read = false) {
            var model = new ProjectModel {
                Path = directory.Uri.AbsolutePath,
                Name = directory.Uri.Segments.Last(),
                Items = new List<ItemModel>(),
                Discriminator = Discriminator.Directory
            };
            if (directory.CloudItem.Metadata.ContainsKey("Index"))
                model.Index = int.Parse(directory.CloudItem.Metadata["Index"]);
            model.Items.AddRange(directory.Directories.Select(o => DirectoryModelUtils.CreateDirectoryModel(o, read)));
            model.Items.AddRange(directory.Files.Select(o => FileModelUtils.CreateFileModel(o, read)));
            return model;
        }
    }
}