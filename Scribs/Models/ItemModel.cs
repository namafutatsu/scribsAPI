namespace Scribs.Models {

    public enum Discriminator {
        Directory = 0,
        File
    }

    public abstract class ItemModel {
        public string Path { get; set; }
        public string Name { get; set; }
        public bool? Read { get; set; }
        public Discriminator Discriminator { get; set; }
        public int Index { get; set; }
    }
}