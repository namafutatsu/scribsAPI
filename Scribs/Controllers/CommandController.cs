using System.Web.Http;
using Scribs.Models;
using Scribs.Filters;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class CommandController : AccessController {

        [HttpPost]
        public CommandSetModel Post(CommandSetModel model) {
            using (var db = new ScribsDbContext()) {
                var user = GetUser(db);
                foreach (var command in model.Commands) {
                    ItemController controller = command.Discriminator == Discriminator.File ?
                        new FileController() : (ItemController)new DirectoryController();
                    switch (command.Type) {
                        case (Command.Create):
                            controller.Create(user, command);
                            break;
                        case Command.Delete:
                            controller.Delete(user, command);
                            break;
                        case Command.Move:
                            controller.Move(user, command);
                            break;
                    }
                    command.Done = true;
                }
                return model;
            }
        }
    }
}
