namespace Scribs.Models {

    public enum Discriminator {
        Directory = 0,
        File
    }

    public abstract class ItemModel {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public bool? Read { get; set; }
        public Discriminator? Discriminator { get; set; }
        public int? Index { get; set; }
        public string MoveToPath { get; set; }
        public int? MoveToIndex { get; set; }
    }
}