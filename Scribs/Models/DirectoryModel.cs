using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scribs.Models {
    public class DirectoryModel : ItemModel {
        public List<ItemModel> Items {get;set;}
    }

    public static class DirectoryModelUtils {
        public static async Task<ItemModel> CreateDirectoryModelAsync(Directory directory, bool read = false) {
            var model = new DirectoryModel {
                Path = directory.Path.ToString(),
                Name = directory.Name,
                Items = new List<ItemModel>(),
                Discriminator = Discriminator.Directory,
                Index = directory.Index,
                Key = directory.Key,
            };
            var tasks = directory.Directories.Select(o => CreateDirectoryModelAsync(o, read)).ToList();
            tasks.AddRange(directory.Files.Select(o => FileModelUtils.CreateFileModelAsync(o, read)));
            var results = await Task.WhenAll(tasks);
            model.Items = results.ToList();
            return model;
        }
    }
}