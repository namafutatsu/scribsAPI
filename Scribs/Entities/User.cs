using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Scribs {

    public partial class User {

        public static User GetFromName(string name) {
            using (var db = new ScribsDbContext()) {
                return db.Users.FirstOrDefault(o => o.Name == name);
            }
        }
    }
}