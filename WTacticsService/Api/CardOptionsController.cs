using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Data.Entity;

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
    public class CardOptionsController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> Get(Guid id)
        {
            var token = TokenExtracton.GetTokenFromCookie(HttpContext.Current.Request);
            using (var repository = new Repository(token))
            {
                var card = await repository.Context.Cards.FindByGuidAsync(id);
                await repository.Context.Entry(card).Reference(x => x.Status).LoadAsync();
                await repository.Context.Entry(card).Reference(x => x.Creator).LoadAsync();

                var cardOptions = new CardOptions();
                if (repository.ServiceUser != null)
                {
                    await repository.Context.Entry(repository.ServiceUser).Reference(x => x.Role).LoadAsync();
                    if (card.Status.Guid != PredefinedGuids.Final)
                    {
                        if (card.Creator.Guid == repository.ServiceUser.Guid) cardOptions.IsEditable = true;
                    }
                    if (repository.ServiceUser.Role.Guid == PredefinedGuids.Developer ||
                        repository.ServiceUser.Role.Guid == PredefinedGuids.Administrator ||
                        repository.ServiceUser.Role.Guid == PredefinedGuids.ServiceUser)
                    {
                        cardOptions.IsEditable = true;
                        cardOptions.IsStatusChangedAllowed = true;
                    }
                }

                cardOptions.Factions = repository.Context.Factions.AsNoTracking().ToList().Select(x => x.FromDal()).ToList();
                cardOptions.Series = repository.Context.Series.AsNoTracking().ToList().Select(x => x.FromDal(false)).ToList();
                cardOptions.RuleSets = repository.Context.RuleSets.AsNoTracking().ToList().Select(x => x.FromDal()).ToList();
                cardOptions.Statuses = repository.Context.Statuses.AsNoTracking().ToList().Select(x => x.FromDal()).ToList();
                cardOptions.CardTypes = repository.Context.CardTypes.Include(x => x.TemplateInfo).AsNoTracking().ToList().Select(x => x.FromDal(true)).ToList();

                return Request.CreateResponse(cardOptions);
            }
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var token = TokenExtracton.GetTokenFromCookie(HttpContext.Current.Request);
            using (var repository = new Repository(token))
            {
           
                var cardOptions = new CardOptions();
                cardOptions.Factions = repository.Context.Factions.AsNoTracking().ToList().Select(x => x.FromDal()).ToList();
                cardOptions.Series = repository.Context.Series.AsNoTracking().ToList().Select(x => x.FromDal(false)).ToList();
                cardOptions.RuleSets = repository.Context.RuleSets.AsNoTracking().ToList().Select(x => x.FromDal()).ToList();
                cardOptions.Statuses = repository.Context.Statuses.AsNoTracking().ToList().Select(x => x.FromDal()).ToList();
                cardOptions.CardTypes = repository.Context.CardTypes.Include(x => x.TemplateInfo).AsNoTracking().ToList().Select(x => x.FromDal(true)).ToList();

                return Request.CreateResponse(cardOptions);
            }
        }
    }
}
