using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Scribs.Filters;
using Scribs.Models;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public abstract class ItemController : AccessController {
        public abstract FileSystemItem GetItem(User user, string project, string key);
        public FileSystemItem GetItem(User user, ItemModel model) => GetItem(user, model.Project, model.Key);
        public Action<FileSystemItem> incr = o => o.Index = o.Index + 1;
        public Action<FileSystemItem> decr = o => o.Index = o.Index - 1;

        public void Create(User user, ItemModel model) {
            var item = GetItem(user, model);
            item.Create();
            item.Key = model.Key;
            item.Index = model.Index;
            //UpdateIndexes(item, item.Key, incr, item.Index);
        }

        public void Delete(User user, ItemModel model) {
            var item = GetItem(user, model);
            int index = item.Index;
            item.Delete();
            //UpdateIndexes(item, null, decr, index);
        }

        //public async Task MoveAsync(User user, ItemModel model) {
        //    var item = GetItem(user, model);
        //    var fromItem = item;
        //    var toItem = item;
        //    string key = item.Key;
        //    bool needUpdate = false;
        //    if (!String.IsNullOrEmpty(model.MoveToPath) && model.Path != model.MoveToPath) {
        //        toItem = GetItem(user, model.MoveToPath);
        //        await toItem.CopyFromAsync(item);
        //        //needUpdate = item.Parent.Key != toItem.Parent.Key;
        //        item = toItem;
        //    }
        //    if (model.MoveToIndex.HasValue && model.Index != model.MoveToIndex) {
        //        item.Index = model.MoveToIndex ?? 0;
        //        needUpdate = true;
        //    }
        //    //if (needUpdate) {
        //    //    UpdateIndexes(fromItem, key, decr, model.Index);
        //    //    UpdateIndexes(toItem, key, incr, model.MoveToIndex.HasValue ? model.MoveToIndex.Value : model.Index);
        //    //}
        //}

        //private void UpdateIndexes(FileSystemItem item, string key, Action<FileSystemItem> op, int start) =>
        //    item.Parent.GetItems().Where(o => o.Index >= start && o.Key != key).ToList().ForEach(o => op(o));

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

        //[HttpPost]
        //public async Task<ResultModel> Move(ItemModel model) {
        //    using (var db = new ScribsDbContext()) {
        //        var user = GetUser(db);
        //        await MoveAsync(user, model);
        //        return new ResultModel { Success = true };
        //    }
        //}
    }
}
