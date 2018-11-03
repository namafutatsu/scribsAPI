using System.Threading.Tasks;
using ScriboAPI.Models;

namespace Scribs.Models {
    public class FileModel : ItemModel {
        public string Text { get; set; }
    }

    public static class FileModelUtils {
        public static async Task<ItemModel> CreateFileModelAsync(File file, bool read = false) {
            var model = new FileModel {
                Path = file.Path.ToString(),
                Name = file.Name,
                Discriminator = Discriminator.File,
                Index = file.Index,
                Key = file.Key
            };
            if (read) {
                string text = await file.DownloadTextAsync();
                // Temp
                //using (TextReader reader = new StringReader(text)) {
                //    using (var writer = new StringWriter()) {
                //        CommonMark.CommonMarkConverter.Convert(reader, writer);
                //        text = writer.GetStringBuilder().ToString();
                //    }
                //}
                model.Text = text;
            }
            return model;
        }

        public static TreeNodeModel FileToTreeItemModel(FileModel o, string parentKey, string[] structure, int level) {
            return new TreeNodeModel {
                Key = o.Key,
                ParentKey = parentKey,
                Index = o.Index,
                Level = level,
                Path = o.Path,
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