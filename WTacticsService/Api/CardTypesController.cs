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
    public class CardTypesController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            using (var repository = new Repository())
            {
                var query = await repository.Context.CardTypes.ToListAsync();
                var result = new ResultList<CardType>(query.Select(x => x.FromDal()).ToList());
                return Request.CreateResponse(result);
            }
        }


        [HttpGet]
        public async Task<HttpResponseMessage> Get(Guid id)
        {
            using (var repository = new Repository())
            {
                var result = await repository.Context.CardTypes.FindByGuidAsync(id);
                return Request.CreateResponse(result.FromDal());
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] CardType cardType)
        {
            var principal = HttpContext.Current.User as Principal;
            using (var repository = new Repository(principal.UserId))
            {

                if (string.IsNullOrWhiteSpace(cardType.Name))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The name is required.");
                }
                var templateInfoModel = await repository.Context.TemplateInfoModels.FindByGuidAsync(cardType.TemplateInfo.Guid);

                var cardTypeModel = repository.CreateCardType(cardType.Name, Guid.NewGuid(), templateInfoModel);
                return Request.CreateResponse(cardTypeModel.FromDal());
            }
        }

        [HttpDelete]
        public async Task<HttpResponseMessage> Delete([FromUri] Guid id)
        {
            var principal = HttpContext.Current.User as Principal;
            return Request.CreateResponse(HttpStatusCode.NotImplemented);
        }


        [HttpPatch]
        public async Task<HttpResponseMessage> Patch([FromUri]Guid id, [FromBody] CardType cardType)
        {
            var principal = HttpContext.Current.User as Principal;
            using (var repository = new Repository(principal.UserId))
            {

                if (string.IsNullOrWhiteSpace(cardType.Name))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The name is required.");
                }
                var cardTypeModel = await repository.Context.CardTypes.FindByGuidAsync(id);
                var templateInfoModel = await repository.Context.TemplateInfoModels.FindByGuidAsync(cardType.TemplateInfo.Guid);

                cardTypeModel.Patch(cardType, templateInfoModel, repository.ServiceUser);
                await repository.Context.SaveChangesAsync();
                return Request.CreateResponse(cardTypeModel.FromDal());
            }
        }

    }
}
