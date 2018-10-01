using Microsoft.WindowsAzure.Storage.File;
using System.Collections.Generic;
using System.Linq;

namespace Scribs.Models {
    public class DirectoryModel : FSItemModel {
        public IList<FSItemModel> Items {get;set;}
    }

    public static class DirectoryModelUtils {
        public static DirectoryModel CreateDirectoryModel(CloudFileDirectory directory, bool read = false) {
            var model = new DirectoryModel {
                Path = directory.Uri.AbsolutePath,
                Name = directory.Uri.Segments.Last(),
                Items = new List<FSItemModel>()
            };
            foreach (var item in directory.ListFilesAndDirectories()) {
                var file = item as CloudFile;
                if (file != null) {
                    model.Items.Add(FileModelUtils.CreateFileModel(file, read));
                    continue;
                }
                var subdir = item as CloudFileDirectory;
                if (directory != null)
                    model.Items.Add(CreateDirectoryModel(subdir, read));
            }
            return model;
        }
    }
}