using Scribs.Models;
using Scribs.Filters;
using System.Web.Http;
using System.Threading.Tasks;

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
            var file = GetItem(user, model.Path);
            return file.DeleteAsync();
        }

        public async Task MoveAsync(User user, ItemModel model) {
            var item = GetItem(user, model.Path);
            var newItem = GetItem(user, model.MoveToPath);
            await newItem.CopyFromAsync(item);
            newItem.Index = model.MoveToIndex ?? 0;
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
