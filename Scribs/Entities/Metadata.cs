using System;
using System.ComponentModel;
using Serilog;

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
        public static string EMPTY = "<EMPTY>";
        public static Metadata<string> Key => new Metadata<string>("Key", () => Guid.NewGuid().ToString());
        public static Metadata<int> Index => new Metadata<int>("Index", () => 0);
        public static Metadata<string> Description => new Metadata<string>("Description", () => EMPTY);
        public static Metadata<string> Structure => new Metadata<string>("Structure", () => EMPTY);
        public static Metadata<Project.Types> Type => new Metadata<Project.Types>("Type", () => Project.Types.Novel);

        public static bool HasMetadata<T>(this IFileSystemItem item, Metadata<T> metadata) {
            if (!item.Fetched) {
                item.FetchAttributes();
                item.Fetched = true;
            }
            return item.Metadata.ContainsKey(metadata.Id);
        }

        public static T GetMetadata<T>(this IFileSystemItem item, Metadata<T> metadata) {
            if (!item.Fetched) {
                item.FetchAttributes();
                item.Fetched = true;
            }
            string id = metadata.Id;
            if (item.Metadata.ContainsKey(id)) {
                if (typeof(T) == typeof(string) && item.Metadata[id].ToString() == EMPTY)
                    return ConvertType<T>(String.Empty);
                return ConvertType<T>(item.Metadata[id]);
            }
            T @default = metadata.Default();
            item.Metadata.Add(id, @default.ToString());
            item.SetMetadata();
            if (typeof(T) == typeof(string))
                return ConvertType<T>(String.Empty);
            return @default;
        }

        public static void SetMetadata<T>(this IFileSystemItem item, Metadata<T> metadata, T value) {
            if (!item.Fetched) {
                item.FetchAttributes();
                item.Fetched = true;
            }
            string id = metadata.Id;
            if (typeof(T) == typeof(string) && String.IsNullOrEmpty(value.ToString()))
                value = ConvertType<T>(EMPTY);
            if (item.Metadata.ContainsKey(id))
                item.Metadata[id] = value.ToString();
            else
                item.Metadata.Add(id, value.ToString());
            Log.Error(item.Path + " : " + metadata.Id + " -> " + value);
            item.SetMetadata();
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