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
    public class SeriesController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            using (var repository = new Repository())
            {
                var query = await repository.Context.Series.ToListAsync();
                var result = new ResultList<Serie>(query.Select(x => x.FromDal()).ToList());
                return Request.CreateResponse(result);
            }
        }


        [HttpGet]
        public async Task<HttpResponseMessage> Get(Guid id)
        {
            using (var repository = new Repository())
            {
                var result = await repository.Context.Series.FindByGuidAsync(id);
                return Request.CreateResponse(result.FromDal(true));
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] Serie serie)
        {
            var principal = HttpContext.Current.User as Principal;
            using (var repository = new Repository(principal.UserId))
            {

                if (string.IsNullOrWhiteSpace(serie.Name))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The name is required.");
                }
                var serieModel = repository.CreateSeries(serie.Name, Guid.NewGuid());
                return Request.CreateResponse(serieModel.FromDal());
            }
        }

        [HttpDelete]
        public async Task<HttpResponseMessage> Delete([FromUri] Guid id)
        {
            var principal = HttpContext.Current.User as Principal;
            return Request.CreateResponse(HttpStatusCode.NotImplemented);
        }


        [HttpPatch]
        public async Task<HttpResponseMessage> Patch([FromUri]Guid id, [FromBody] Serie serie)
        {
            var principal = HttpContext.Current.User as Principal;
            using (var repository = new Repository(principal.UserId))
            {

                if (string.IsNullOrWhiteSpace(serie.Name))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The name is required.");
                }
                var serieModel = await repository.Context.Series.FindByGuidAsync(id);
                serieModel.Patch(serie, repository.ServiceUser);
                await repository.Context.SaveChangesAsync();
                return Request.CreateResponse(serieModel.FromDal());
            }
        }

    }
}
