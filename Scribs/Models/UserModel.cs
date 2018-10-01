namespace Scribs.Models {
    public class UserModel : DbModel {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Mail { get; set; }
    }
}