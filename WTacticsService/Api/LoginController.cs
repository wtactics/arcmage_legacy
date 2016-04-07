using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using log4net;
using WTacticsLibrary;
using WTacticsLibrary.Assembler;
using WTacticsLibrary.Model;
using WTacticsService.Api.Authentication;
using WTacticsService.Helpers;

namespace WTacticsService.Api
{
    public class LoginController : ApiController
    {
    
        [HttpPost]
        public HttpResponseMessage Post([FromBody]Login login)
        {

            using (var repository = new Repository())
            {
                if (login == null)
                {
                    var authCookie = HttpContext.Current.Request.Cookies["Authorization"];
                    if (authCookie != null)
                    {

                        var cookieToken = AuthenticationAttribute.DecodeTokenFromCookie(authCookie.Value);
                        if (cookieToken != null)
                        {
                            if (repository.Context.Users.SingleOrDefault(x => x.Token == cookieToken) != null)
                            {
                                return Request.CreateResponse(HttpStatusCode.OK);
                            }

                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                var user = repository.Context.Users.SingleOrDefault(x => x.Email == login.Email);
                if (user  != null && user.Password == login.Password)
                {
                    user.Token = TokenGenerator.GetRandomToken(100);
                    repository.Context.SaveChanges();
                    var expiryDate = DateTime.UtcNow.AddYears(1);

                    var token = "Bearer " + user.Token;
                    var response = Request.CreateResponse(token);
                    var cookie = new CookieHeaderValue("Authorization", token)
                    {
                        HttpOnly = true,
                        Expires = expiryDate,
                    };
                    response.Headers.AddCookies(new CookieHeaderValue[] { cookie });
                    return response;
                }
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }
    }
}
