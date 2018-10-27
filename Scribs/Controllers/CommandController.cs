using System.Threading.Tasks;
using System.Web.Http;
using Scribs.Models;
using Scribs.Filters;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class CommandController : AccessController {

        [HttpPost]
        public async Task<CommandModel> Post(CommandModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                ItemController controller = model.Discriminator == Discriminator.File ?
                    new FileController() : (ItemController)new DirectoryController();
                switch (model.Type) {
                    case (Command.Create):
                        await controller.CreateAsync(user, model);
                        break;
                    case Command.Delete:
                        await controller.DeleteAsync(user, model);
                        break;
                    case Command.Move:
                        await controller.MoveAsync(user, model);
                        break;
                }
                return model;
            }
        }
    }
}
