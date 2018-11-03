using System.Linq;

namespace ScriboAPI.Models {
    public class TreeNodeModel {
        public string Key { get; set; }
        public string ParentKey { get; set; }
        public string Path { get; set; }
        public string[] Structure { get; set; }
        public int? Type { get; set; }
        public string Description { get; set; }
        public int? Index { get; set; }
        public int Level { get; set; }
        public bool? IsLeaf { get; set; }
        // From PrimeNg :
        public string label { get; set; }
        public string data { get; set; }
        public string icon { get; set; }
        public string expandedIcon { get; set; }
        public string collapsedIcon { get; set; }
        public IOrderedEnumerable<TreeNodeModel> children { get; set; }
        public bool? droppable { get; set; }
        public bool? expanded { get; set; }
        public string type { get; set; }
        public TreeNodeModel parent { get; set; }
        public bool? partialSelected { get; set; }
        public string styleClass { get; set; }
        public bool? draggable { get; set; }
        public bool? selectable { get; set; }
    }
}