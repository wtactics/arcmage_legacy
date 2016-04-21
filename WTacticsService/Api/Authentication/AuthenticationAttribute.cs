using System;
using System.Data.Entity;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using log4net;
using WTacticsLibrary;
using WTacticsService.Helpers;

namespace WTacticsService.Api.Authentication
{
    //http://www.asp.net/web-api/overview/security/authentication-filters
    public class AuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AuthenticationAttribute));

        public bool AllowMultiple
        {
            get { return false; }
        }

        /// <summary>
        /// Check if the request is authentication and set the principal if it is
        /// </summary>
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            using (_log.LogMethodEnter(new MethodParameter(nameof(context), context.Request.RequestUri)))
            {
                // We look for the token in the header and the cookie (header takes precedence)
                var token = GetToken(context);
                if (token == null)
                {
                    // We cannot authenticate the user
                    return;
                }

                using (Repository repo = new Repository())
                {
                    var user = await repo.Context.Users.FirstOrDefaultAsync(it => it.Token == token);
                    if (user == null)
                    {
                        // No user corresponds with the token
                        return;
                    }

                    var principal = new Principal(user.Guid, user.Email, user.Role.Name);
                    context.Principal = principal;
                }
            }
        }

        private static string GetToken(HttpAuthenticationContext context)
        {
            return GetTokenFromHeader(context) ?? GetTokenFromCookie(context);
        }

        private static string GetTokenFromCookie(HttpAuthenticationContext context)
        {
            var cookie = HttpContext.Current.Request.Cookies["Authorization"];
            if (cookie == null) return null;

            var token = DecodeTokenFromCookie(cookie.Value);

            if (string.IsNullOrEmpty(token))
            {
                // The scheme is correct but the data is not. In that case we return an error to the client.
                context.ErrorResult = new AuthenticationFailureResult("Missing authorization token", context.Request);
                return null;
            }

            return token;
        }

        public static string DecodeTokenFromCookie(string cookieValue)
        {
            var token = HttpUtility.UrlDecode(cookieValue);
            return token;
        }

        private static string GetTokenFromHeader(HttpAuthenticationContext context)
        {
            var header = context.Request.Headers.Authorization;
            if (header == null || header.Scheme != "Bearer")
            {
                return null;
            }

            if (string.IsNullOrEmpty(header.Parameter))
            {
                // The scheme is correct but the data is not. In that case we return an error to the client.
                context.ErrorResult = new AuthenticationFailureResult("Missing authorization token", context.Request);
                return null;
            }

            return header.Parameter;
        }

        /// <summary>
        /// Explain to the client how we expect the authentication to occur
        /// </summary>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var challenge = new AuthenticationHeaderValue("Bearer");
            context.Result = new UnauthorizedChallenge(challenge, context.Result);
            return Task.FromResult(0);
        }
    }
}