﻿using System.Threading.Tasks;

namespace Scribs.Models {
    public class FileModel : ItemModel {
        public string Text { get; set; }
    }

    public static class FileModelUtils {
        public static async Task<ItemModel> CreateFileModelAsync(File file, bool read = false) {
            var model = new FileModel {
                Url = file.Url,
                Name = file.Name,
                Discriminator = Discriminator.File,
                Index = file.Index,
                Key = file.Key
            };
            if (read)
                model.Text = await file.DownloadTextAsync();
            return model;
        }

        public static TreeNodeModel FileToTreeItemModel(FileModel o, string parentKey, string[] structure, int level) {
            return new TreeNodeModel {
                Key = o.Key,
                ParentKey = parentKey,
                Index = o.Index,
                Level = level,
                Url = o.Url,
                //label = o.Index + "." + o.Name + "." + level,
                label = o.Name,
                droppable = false,
                data = o.Text,
                icon = "fa fa-file-o",
                IsLeaf = true
            };
        }
    }
}