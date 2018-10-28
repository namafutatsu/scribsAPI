using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Scribs.Models;
using Scribs.Filters;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public abstract class ItemController : AccessController {
        public abstract IFileSystemItem GetItem(User user, string path);
        public Action<IFileSystemItem> incr = o => o.Index = o.Index + 1;
        public Action<IFileSystemItem> decr = o => o.Index = o.Index - 1;

        public async Task CreateAsync(User user, ItemModel model) {
            var item = GetItem(user, model.Path);
            await item.CreateAsync();
            item.Key = model.Key;
            item.Index = model.Index;
            UpdateIndexes(item, item.Key, incr, item.Index);
        }

        public async Task DeleteAsync(User user, ItemModel model) {
            var item = GetItem(user, model.Path);
            int index = item.Index;
            await item.DeleteAsync();
            UpdateIndexes(item, null, decr, index);
        }

        public async Task MoveAsync(User user, ItemModel model) {
            var item = GetItem(user, model.Path);
            var fromItem = item;
            var toItem = item;
            string key = item.Key;
            bool needUpdate = false;
            if (!String.IsNullOrEmpty(model.MoveToPath) && model.Path != model.MoveToPath) {
                toItem = GetItem(user, model.MoveToPath);
                await toItem.CopyFromAsync(item);
                needUpdate = item.Parent.Key != toItem.Parent.Key;
                item = toItem;
            }
            if (model.MoveToIndex.HasValue && model.Index != model.MoveToIndex) {
                item.Index = model.MoveToIndex ?? 0;
                needUpdate = true;                
            }
            if (needUpdate) {
                UpdateIndexes(fromItem, key, decr, model.Index);
                UpdateIndexes(toItem, key, incr, model.MoveToIndex.HasValue ? model.MoveToIndex.Value : model.Index);
            }
        }

        private void UpdateIndexes(IFileSystemItem item, string key, Action<IFileSystemItem> op, int start) =>
            item.Parent.GetItems().Where(o => o.Index >= start && o.Key != key).ToList().ForEach(o => op(o));

        [HttpPost]
        public async Task<ResultModel> Create(ItemModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                await CreateAsync(user, model);
                return new ResultModel { Success = true };
            }
        }

        [HttpPost]
        public async Task<ResultModel> Delete(ItemModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                await DeleteAsync(user, model);
                return new ResultModel { Success = true };
            }
        }

        [HttpPost]
        public async Task<ResultModel> Move(ItemModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                await MoveAsync(user, model);
                return new ResultModel { Success = true };
            }
        }
    }
}
