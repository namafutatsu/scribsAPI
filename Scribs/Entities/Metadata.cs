using System;
using System.ComponentModel;

namespace Scribs {

    public class Metadata<T> {
        public string Id { get; }
        public Func<T> Default { get; }
        public Metadata(string id, Func<T> @default) {
            Id = id;
            Default = @default;
        }
    }

    public static class MetadataUtils {
        public static Metadata<string> Key => new Metadata<string>("Key", () => Guid.NewGuid().ToString());
        public static Metadata<int> Index => new Metadata<int>("Index", () => 0);
        public static Metadata<string> Description => new Metadata<string>("Description", () => String.Empty);
        public static Metadata<string> Template => new Metadata<string>("Template", () => String.Empty);
        public static Metadata<Project.Types> Type => new Metadata<Project.Types>("Type", () => Project.Types.Novel);

        public static T GetMetadata<T>(this IFileSystemItem item, Metadata<T> metadata) {
            if (item.Metadata.ContainsKey(metadata.Id))
                return ConvertType<T>(item.Metadata[metadata.Id]);
            T @default = metadata.Default();
            item.Metadata.Add(metadata.Id, @default.ToString());
            return @default;
        }

        public static void SetMetadata<T>(this IFileSystemItem item, Metadata<T> metadata, T value) {
            if (item.Metadata.ContainsKey(metadata.Id))
                item.Metadata[metadata.Id] = value.ToString();
            else
                item.Metadata.Add(metadata.Id, value.ToString());
        }

        public static T ConvertType<T>(string input) {
            T result = default(T);
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
                    result = (T)converter.ConvertFromString(input);
            return result;
        }
    }
}