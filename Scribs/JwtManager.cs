using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Scribs.Models;

namespace Scribs {
    public static class JwtManager {

        public static string GenerateToken(ScribsDbContext db, User user, UserModel model) {
            var secretBase64 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var secret = Convert.FromBase64String(secretBase64);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name.ToString()),
                    new Claim(ClaimTypes.Email, user.Mail.ToString())
                }),
                Expires = now.AddMonths(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            var access = new Access {
                Token = token,
                Secret = secretBase64,
                User = user,
                CTime = DateTime.Now,
                MTime = DateTime.Now
            };
            db.Accesses.Add(access);
            db.SaveChanges();

            return token;
        }

        public static ClaimsPrincipal GetPrincipal(ScribsDbContext db, JwtSecurityToken jwtToken, out Access access) {
            access = null;
            try {
                var tokenHandler = new JwtSecurityTokenHandler();
                var claims = jwtToken.Claims.ToList();
                var claim = claims.Find(o => o.Type == "nameid");
                var user = User.Factory.GetInstance(db, int.Parse(claim.Value));
                access = db.Accesses.FirstOrDefault(o => o.UserId == user.Id && o.Status == Status.Active);
                if (access == null)
                    return null;
                var symmetricKey = Convert.FromBase64String(access.Secret);

                var validationParameters = new TokenValidationParameters() {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(jwtToken.RawData, validationParameters, out securityToken);

                return principal;
            } catch (Exception) {
                return null;
            }
        }

        internal static JwtSecurityToken GetJwtToken(string token) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            return jwtToken;
        }
    }
}