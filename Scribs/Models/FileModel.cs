using System.IO;
using System.Text;
using System.Threading.Tasks;

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
            if (file.CloudItem.Metadata.ContainsKey("Index"))
                model.Index = int.Parse(file.CloudItem.Metadata["Index"]);
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
    }
}