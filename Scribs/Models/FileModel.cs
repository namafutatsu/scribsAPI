namespace Scribs.Models {
    public class FileModel : ItemModel {
        public string Text { get; set; }
    }

    public static class FileModelUtils {
        public static FileModel CreateFileModel(File file, bool read = false) {
            var model = new FileModel {
                Path = file.Path,
                Name = file.Name,
                Discriminator = Discriminator.File,
                Index = file.Index,
                Key = file.Key
            };
            if (file.CloudItem.Metadata.ContainsKey("Index"))
                model.Index = int.Parse(file.CloudItem.Metadata["Index"]);
            if (read)
                model.Text = file.DownloadTextAsync().Result;
            return model;
        }
    }
}