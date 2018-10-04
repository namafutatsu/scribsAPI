namespace Scribs.Models {

    public enum InstructionType {
        Create = 0,
        Move,
        Delete
    }

    public abstract class InstructionModel {
        public InstructionType Type { get; set; }
        public string Key { get; set; }
        public string Path { get; set; }
        public Discriminator Discriminator { get; set; }
        public int Index { get; set; }
        public string ItemKey { get; set; }
        public string MoveToPath { get; set; }
        public int MoveToIndex { get; set; }
        public bool Done { get; set; }
    }
}