using System.Web.Http;
using System.Threading.Tasks;
using Scribs.Models;
using Scribs.Filters;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class CommandController : AccessController {

        [HttpPost]
        public async Task<CommandSetModel> Post(CommandSetModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                foreach (var command in model.Commands) {
                    ItemController controller = command.Discriminator == Discriminator.File ?
                        new FileController() : (ItemController)new DirectoryController();
                    switch (command.Type) {
                        case (Command.Create):
                            await controller.CreateAsync(user, command);
                            break;
                        case Command.Delete:
                            await controller.DeleteAsync(user, command);
                            break;
                        case Command.Move:
                            await controller.MoveAsync(user, command);
                            break;
                    }
                    command.Done = true;
                }
                return model;
            }
        }
    }
}
