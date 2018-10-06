
namespace Scribs.Models {
    public class ResultModel {
        public bool Success { get; set; }
    }

    public class ResultModel<E>: ResultModel {
        public E Model { get; set; }
        public ResultModel(E model) {
            this.Model = Model;
        }
    }
}