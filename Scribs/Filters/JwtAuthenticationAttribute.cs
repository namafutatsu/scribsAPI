using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Scribs.Filters {

    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter {
        public bool AllowMultiple {
            get { return false; }
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken) {
            var request = context.Request;
            var authorization = request.Headers.Authorization;
            if (authorization == null || authorization.Scheme != "Bearer") {
                context.ErrorResult = new AuthenticationFailureResult("Bah authorization scheme", request);
                return;
            }
            if (string.IsNullOrEmpty(authorization.Parameter)) {
                context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token", request);
                return;
            }
            var token = authorization.Parameter.Replace("\"", "");
            var principal = await AuthenticateJwtToken(token);
            if (principal == null)
                context.ErrorResult = new AuthenticationFailureResult("Invalid token", request);
            else
                context.Principal = principal;
        }

        private static bool ValidateToken(string token, out int? userId) {
            using (var db = new ScribsDbContext()) {
                userId = null;
                try {
                    var success = true;
                    var jwtToken = JwtManager.GetJwtToken(token);
                    ClaimsPrincipal simplePrinciple = JwtManager.GetPrincipal(db, jwtToken, out Access access);
                    if (simplePrinciple == null) {
                        success = false;
                    } else {
                        var identity = simplePrinciple.Identity as ClaimsIdentity;
                        if (identity == null || !identity.IsAuthenticated) {
                            success = false;
                        } else {
                            var agentIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
                            if (agentIdClaim != null)
                                userId = int.Parse(agentIdClaim.Value);
                            if (!userId.HasValue)
                                success = false;
                        }
                    }
                    if (access != null) {
                        if (success) {
                            access.CTime = DateTime.Now;
                        } else {
                            access.Status = Status.Expired;
                        }
                        access.MTime = DateTime.Now;
                        db.SaveChanges();
                    } else {
                        success = false;
                    }
                    return success;
                } catch (Exception) {
                    return false;
                }
            }
        }

        protected Task<IPrincipal> AuthenticateJwtToken(string token) {
            if (ValidateToken(token, out int? userId)) {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()),
                };
                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal agent = new ClaimsPrincipal(identity);

                return Task.FromResult(agent);
            }
            return Task.FromResult<IPrincipal>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken) {
            var challenge = new AuthenticationHeaderValue("Bearer");
            context.Result = new AddChallengeOnUnauthorizedResult(challenge, context.Result);
            return Task.FromResult(0);
        }
    }
}
