using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using WTacticsLibrary;
using WTacticsService.Api.Authentication;
using WTacticsService.Helpers;

namespace WTacticsService.Api
{
    [Authorize]
    public class LogoutController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post()
        {

            var principal = HttpContext.Current.User as Principal;
            using (var repository = new Repository(principal.UserId))
            {
                repository.ServiceUser.Token = null;
                repository.Context.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }
    }
}
