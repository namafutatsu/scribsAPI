using ScriboAPI.Models;
using System;
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

        public static TreeNodeModel DirectoryToTreeItemModel(DirectoryModel o, string parentKey, string[] structure, int level) {
            return new TreeNodeModel {
                Key = o.Key,
                ParentKey = parentKey,
                Path = o.Path,
                Index = o.Index,
                //FolderLabel = structure.Any() ? level < structure.Length - 1 ? structure[level] : String.Empty : "Folder",
                //FileLabel = structure.Any() ? level >= structure.Length - 1 ? structure.Last() : String.Empty : "File",
                label = o.Name,
                collapsedIcon = "fa fa-folder",
                expandedIcon = "fa fa-folder-open",
                droppable = o.Discriminator == Discriminator.Directory,
                Level = level,
                children = o.Items.Select(i => i.Discriminator == Discriminator.Directory ?
                    DirectoryToTreeItemModel(i as DirectoryModel, o.Key, structure, level + 1) :
                    FileModelUtils.FileToTreeItemModel(i as FileModel, o.Key, structure, level + 1)).ToArray()
            };
        }
    }
}