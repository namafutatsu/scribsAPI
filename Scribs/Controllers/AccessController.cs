using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using Scribs.Filters;
using Scribs.Models;

namespace Scribs.Controllers {

    [JwtAuthentication]
    public class AccessController : ApiController {

        //[HttpGet]
        //public bool ValidateToken() {
        //    return true;
        //}

        //[HttpGet]
        //public UserModel GetUserModel(ScribsDbContext db) {
        //    var user = GetUser(db);
        //    return new UserModel {
        //        Id = user.Id,
        //        Name = user.Name
        //    };
        //}

        public User GetUser(ScribsDbContext db) {
            try {
                var principal = RequestContext.Principal as ClaimsPrincipal;
                var userId = int.Parse(principal.Claims.First(o => o.Type == ClaimTypes.NameIdentifier).Value);
                return db.Users.Find(userId);
            } catch {
                throw new HttpResponseException(HttpStatusCode.ExpectationFailed);
            }
        }
    }

    public class UserController : ApiController {

        [HttpPost]
        public TokenModel SignIn(UserModel model) {
            using (var db = new ScribsDbContext()) {
                if (String.IsNullOrEmpty(model.Username) || String.IsNullOrEmpty(model.Password))
                    throw new Exception("Wrong username and/or password");
                var user = db.Users.SingleOrDefault(o => o.Name == model.Username);
                if (user == null)
                    throw new Exception("This username does not exist");
                if (user.Password != model.Password)
                    throw new Exception("Wrong password");
                var result = new TokenModel();
                var access = db.Accesses.FirstOrDefault(o => o.UserId == user.Id && o.Status == Status.Active);
                result.Token = access != null ? access.Token : JwtManager.GenerateToken(db, user, model);
                return result;
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        [HttpPost]
        public TokenModel SignUp(UserModel model) {
            using (var db = new ScribsDbContext()) {
                if (String.IsNullOrEmpty(model.Username) || String.IsNullOrEmpty(model.Password) || String.IsNullOrEmpty(model.Mail))
                    throw new Exception("Some fields are missing");
                if (model.Username.Length > 40)
                    throw new Exception("This username is too long");
                var user = db.Users.SingleOrDefault(o => o.Name == model.Username);
                if (user != null)
                    throw new Exception("This username is already taken");
                user = db.Users.SingleOrDefault(o => o.Mail == model.Mail);
                if (user != null)
                    throw new Exception("This email is already used");
                user = new User {
                    Name = model.Username,
                    Password = model.Password,
                    Mail = model.Mail
                };
                try {
                    FileSystem.CreateUserDir(user);
                } catch {
                    throw new Exception("Some special characters in your username are not supported");
                }
                db.Users.Add(user);
                db.SaveChanges();
                var result = new TokenModel();
                var access = db.Accesses.FirstOrDefault(o => o.UserId == user.Id && o.Status == Status.Active);
                result.Token = access != null ? access.Token : JwtManager.GenerateToken(db, user, model);
                return result;
            }
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
    }
}
