﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scribs.Models {
    public class DirectoryModel : ItemModel {
        public List<ItemModel> Items {get;set;}
    }

    public static class DirectoryModelUtils {
        public static async Task<ItemModel> CreateDirectoryModelAsync(Directory directory, bool read = false) {
            var model = new DirectoryModel {
                Url = directory.Url,
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
            var model = new TreeNodeModel {
                Key = o.Key,
                ParentKey = parentKey,
                Url = o.Url,
                Index = o.Index,
                //label = o.Index + "." + o.Name + "." + level,
                label = o.Name,
                collapsedIcon = "fa fa-folder",
                expandedIcon = "fa fa-folder-open",
                Level = level,
                droppable = true,
                IsLeaf = false,
                children = o.Items.Select(i => i.Discriminator == Discriminator.Directory ?
                    DirectoryToTreeItemModel(i as DirectoryModel, o.Key, structure, level + 1) :
                    FileModelUtils.FileToTreeItemModel(i as FileModel, o.Key, structure, level + 1)).OrderBy(i => i.Index)
            };
            if (o.Name.StartsWith(".")) {
                model.Intern = true;
                model.label = model.label.Substring(1);
                model.draggable = false;
                if (model.label == "Drafts") {
                    model.collapsedIcon = model.expandedIcon = "fa fa-pencil"; // fa fa-clipboard
                    model.Structure = new string[] { "folder", "draft"};
                }
            }
            return model;
        }
    }
}