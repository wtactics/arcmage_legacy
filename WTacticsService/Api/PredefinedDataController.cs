using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WTacticsLibrary;


namespace WTacticsService.Api
{
    public class PredefinedDataController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get()
        {
            using (var repository = new Repository())
            {
                repository.FillPredefinedData();
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
        }
    }
}
