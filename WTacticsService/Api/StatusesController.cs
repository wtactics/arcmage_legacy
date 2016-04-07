using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WTacticsLibrary;
using WTacticsLibrary.Assembler;
using WTacticsLibrary.Model;
using WTacticsService.Api.Authentication;

namespace WTacticsService.Api
{
    [Authorize]
    public class StatusesController : ApiController
    {

        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            using (var repository = new Repository())
            {
                var query = await repository.Context.Statuses.ToListAsync();
                var result = new ResultList<Status>(query.Select(x => x.FromDal()).ToList());
                return Request.CreateResponse(result);
            }
        }


        [HttpGet]
        public async Task<HttpResponseMessage> Get(Guid id)
        {
            using (var repository = new Repository())
            {
                var result = await repository.Context.Statuses.FindByGuidAsync(id);
                return Request.CreateResponse(result.FromDal());
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] Status status)
        {
            var principal = HttpContext.Current.User as Principal;
            using (var repository = new Repository(principal.UserId))
            {

                if (string.IsNullOrWhiteSpace(status.Name))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The name is required.");
                }
                var statusModel = repository.CreateStatus(status.Name, Guid.NewGuid());
                return Request.CreateResponse(statusModel.FromDal());
            }
        }

        [HttpDelete]
        public async Task<HttpResponseMessage> Delete([FromUri] Guid id)
        {
            var principal = HttpContext.Current.User as Principal;
            return Request.CreateResponse(HttpStatusCode.NotImplemented);
        }


        [HttpPatch]
        public async Task<HttpResponseMessage> Patch([FromUri]Guid id, [FromBody] Status status)
        {
            var principal = HttpContext.Current.User as Principal;
            using (var repository = new Repository(principal.UserId))
            {

                if (string.IsNullOrWhiteSpace(status.Name))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The name is required.");
                }
                var statusModel = await repository.Context.Statuses.FindByGuidAsync(id);
                statusModel.Patch(status, repository.ServiceUser);
                await repository.Context.SaveChangesAsync();
                return Request.CreateResponse(statusModel.FromDal());
            }
        }


    }
}
