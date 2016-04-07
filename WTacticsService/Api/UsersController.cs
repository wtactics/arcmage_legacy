using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
    
    public class UsersController : ApiController
    {

        /// <summary>
        /// Get details about the user that is currently logged in
        /// To be used by the client like this: http://localhost:49556/api/Users/me 
        /// </summary>
        [Authorize]
        [HttpGet]
        public HttpResponseMessage Get(string id)
        {
            var principal = HttpContext.Current.User as Principal;
            using (var repository = new Repository(principal.UserId))
            {
            
                if (id != "me")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Only 'me' is supported as the id");
                }

                var result = repository.ServiceUser.FromDal();
                
                return Request.CreateResponse(result);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The name is required.");
            }
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The email is required.");
            }
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The password is required.");
            }
            if (string.IsNullOrWhiteSpace(user.Password2))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The confirm password is required.");
            }
            if (user.Password != user.Password2)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The passwords do not match.");
            }

            using (var repository = new Repository())
            {

                var newUser = repository.Context.Users.FirstOrDefault(x => x.Email == user.Email);
                if (newUser != null) Request.CreateResponse(HttpStatusCode.BadRequest, "Email is already taken");
               
                var userModel = repository.CreateUser(user.Name, user.Email, user.Password, Guid.NewGuid());
                userModel.Token = TokenGenerator.GetRandomToken(100);
                repository.Context.SaveChanges();
                var expiryDate = DateTime.UtcNow.AddYears(1);

                var token = "Bearer " + userModel.Token;
                var response = Request.CreateResponse(token);
                var cookie = new CookieHeaderValue("Authorization", token)
                {
                    HttpOnly = true,
                    Expires = expiryDate,
                };
                response.Headers.AddCookies(new CookieHeaderValue[] { cookie });
                return response;
            }
        }

    }
}
