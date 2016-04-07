using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace WTacticsService.Api.Authentication
{
    public static class TokenExtracton
    {

        public static string GetTokenFromCookie(HttpRequest request)
        {
            var cookie = request.Cookies["Authorization"];
            if (cookie == null) return null;

            return DecodeTokenFromCookie(cookie.Value);
        }

        private static string DecodeTokenFromCookie(string cookieValue)
        {
            var token = HttpUtility.UrlDecode(cookieValue);
            if (!token.StartsWith("Bearer ")) return null;   // We don't now how to authenticate agains this scheme

            token = token.Substring("Bearer ".Length);

            return token;
        }
    
    }
}
