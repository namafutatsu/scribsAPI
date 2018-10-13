﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scribs.Models {

    public class ProjectModel : DirectoryModel {
        public string Template { get; set; }
        public int Type { get; set; }
    }

    public static class ProjectModelUtils {
        public static async Task<ProjectModel> CreateProjectModelAsync(Project project, bool read = false) {
            var model = new ProjectModel {
                Path = project.Path.ToString(),
                Name = project.Name,
                Items = new List<ItemModel>(),
                Discriminator = Discriminator.Directory,
                Index = project.Index,
                Key = project.Key,
                Template = project.Template,
                Type = (int)project.Type
            };
            if (project.CloudItem.Metadata.ContainsKey("Index"))
                model.Index = int.Parse(project.CloudItem.Metadata["Index"]);
            var dirModel = await DirectoryModelUtils.CreateDirectoryModelAsync(project, read);
            model.Items = (dirModel as DirectoryModel).Items;
            return model;
        }
    }
}