using System.Collections.Generic;
using System.Linq;

namespace Scribs.Models {
    public class DirectoryModel : ItemModel {
        public List<ItemModel> Items {get;set;}
    }

    public static class DirectoryModelUtils {
        public static DirectoryModel CreateDirectoryModel(Directory directory, bool read = false) {
            var model = new DirectoryModel {
                Path = directory.Uri.AbsolutePath,
                Name = directory.Uri.Segments.Last(),
                Items = new List<ItemModel>(),
                Discriminator = Discriminator.Directory,
                Index = directory.Index,
                Key = directory.Key,
            };
            model.Items.AddRange(directory.Directories.Select(o => CreateDirectoryModel(o, read)));
            model.Items.AddRange(directory.Files.Select(o => FileModelUtils.CreateFileModel(o, read)));
            return model;
        }
    }
}