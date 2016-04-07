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
    public class FactionsController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            using (var repository = new Repository())
            {
                var query = await repository.Context.Factions.ToListAsync();
                var result = new ResultList<Faction>(query.Select(x => x.FromDal()).ToList());
                return Request.CreateResponse(result);
            }
        }


        [HttpGet]
        public async Task<HttpResponseMessage> Get(Guid id)
        {
            using (var repository = new Repository())
            {
                var result = await repository.Context.Factions.FindByGuidAsync(id);
                return Request.CreateResponse(result.FromDal());
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] Faction faction)
        {
            var principal = HttpContext.Current.User as Principal;
            using (var repository = new Repository(principal.UserId))
            {

                if (string.IsNullOrWhiteSpace(faction.Name))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The name is required.");
                }
                var factionModel = repository.CreateFaction(faction.Name, Guid.NewGuid());
                return Request.CreateResponse(factionModel.FromDal());
            }
        }

        [HttpDelete]
        public async Task<HttpResponseMessage> Delete([FromUri] Guid id)
        {
            var principal = HttpContext.Current.User as Principal;
            return Request.CreateResponse(HttpStatusCode.NotImplemented);
        }


        [HttpPatch]
        public async Task<HttpResponseMessage> Patch([FromUri]Guid id, [FromBody] Faction faction)
        {
            var principal = HttpContext.Current.User as Principal;
            using (var repository = new Repository(principal.UserId))
            {

                if (string.IsNullOrWhiteSpace(faction.Name))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The name is required.");
                }
                var factionModel = await repository.Context.Factions.FindByGuidAsync(id);
                factionModel.Patch(faction, repository.ServiceUser);
                await repository.Context.SaveChangesAsync();
                return Request.CreateResponse(factionModel.FromDal());
            }
        }

    }
}
