using Scribs.Models;
using Scribs.Filters;
using System.Web.Http;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public abstract class ItemController : AccessController {
        public abstract IFileSystemItem GetItem(User user, string path);

        public void Create(User user, ItemModel model) {
            var item = GetItem(user, model.Path);
            item.Create();
            item.Key = model.Key;
            item.Index = model.Index ?? 0;
        }

        public void Delete(User user, ItemModel model) {
            var file = GetItem(user, model.Path);
            file.Delete();
        }

        public void Move(User user, ItemModel model) {
            var item = GetItem(user, model.Path);
            var newItem = GetItem(user, model.MoveToPath);
            newItem.CopyFrom(item);
            newItem.Index = model.MoveToIndex ?? 0;
        }

        [HttpPost]
        public ResultModel Create(ItemModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                Create(user, model);
                return new ResultModel { Success = true };
            }
        }

        [HttpPost]
        public ResultModel Delete(ItemModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                Delete(user, model);
                return new ResultModel { Success = true };
            }
        }

        [HttpPost]
        public ResultModel Move(ItemModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                Move(user, model);
                return new ResultModel { Success = true };
            }
        }
    }
}
