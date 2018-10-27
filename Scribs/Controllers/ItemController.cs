using System;
using System.Threading.Tasks;
using System.Web.Http;
using Scribs.Models;
using Scribs.Filters;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public abstract class ItemController : AccessController {
        public abstract IFileSystemItem GetItem(User user, string path);

        public async Task CreateAsync(User user, ItemModel model) {
            var item = GetItem(user, model.Path);
            await item.CreateAsync();
            item.Key = model.Key;
            item.Index = model.Index ?? 0;
        }

        public Task DeleteAsync(User user, ItemModel model) {
            var item = GetItem(user, model.Path);
            return item.DeleteAsync();
        }

        public async Task MoveAsync(User user, ItemModel model) {
            var item = GetItem(user, model.Path);
            if (!String.IsNullOrEmpty(model.MoveToPath) && model.Path != model.MoveToPath) {
                var newItem = GetItem(user, model.MoveToPath);
                await newItem.CopyFromAsync(item);
                item = newItem;
            }
            if (model.MoveToIndex.HasValue && model.Index != model.MoveToIndex)
                item.Index = model.MoveToIndex ?? 0;
        }

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
