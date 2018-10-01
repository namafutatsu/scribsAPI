using Microsoft.WindowsAzure.Storage.File;
using System.Linq;

namespace Scribs.Models {
    public class FileModel : FSItemModel {
        public string Text { get; set; }
    }

    public static class FileModelUtils {
        public static FileModel CreateFileModel(CloudFile file, bool read = false) {
            var model = new FileModel {
                Path = file.Uri.AbsolutePath,
                Name = file.Uri.Segments.Last()
            };
            if (read)
                model.Text = file.DownloadTextAsync().Result;
            return model;
        }
    }
}