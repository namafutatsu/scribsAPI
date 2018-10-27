using System.Collections.Generic;

namespace Scribs.Models {

    public enum Command {
        Create = 0,
        Move,
        Delete
    }

    public class CommandModel: ItemModel {
        public Command Type { get; set; }
        public string CommandKey { get; set; }
    }
}