using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Hangfire;
using Newtonsoft.Json;
using WTacticsDAL;
using WTacticsLibrary;
using WTacticsLibrary.Assembler;
using WTacticsLibrary.Layout;
using WTacticsLibrary.Model;
using WTacticsService.Api.Authentication;

namespace WTacticsService.Api
{
    public class DecksController : ApiController
    {
       

        [HttpGet]
        public async Task<HttpResponseMessage> Get(Guid id)
        {
            var token = TokenExtracton.GetTokenFromCookie(HttpContext.Current.Request);
            using (var repository = new Repository(token))
            {
                await repository.Context.Factions.LoadAsync();
                await repository.Context.CardTypes.LoadAsync();
                var result = await repository.Context.Decks.Include(x=>x.DeckCards.Select(dc=>dc.Card)).Where(x=>x.Guid == id).FirstOrDefaultAsync();
                if (result == null)
                {
                    Request.CreateResponse(HttpStatusCode.NotFound);
                }

                var deck = result.FromDal(true);
                if (repository.ServiceUser != null)
                {
                    await repository.Context.Entry(result).Reference(x => x.Creator).LoadAsync();
                    deck.IsEditable = repository.ServiceUser.Guid == result.Creator.Guid;
                }
                return Request.CreateResponse(deck);
            }
        }

        [HttpGet]
        public async Task<HttpResponseMessage> Export(Guid id, ExportFormat format)
        {
            Repository.InitPaths();
            var deckFile = "";
            var mediaType = "application/pdf";
            switch (format)
            {
                case ExportFormat.Pdf:
                    deckFile = Repository.GetDeckFile(id);
                    break;
            }

            if (!File.Exists(deckFile)) return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The deck with the specified id does not exist");

            Stream stream = new FileStream(deckFile, FileMode.Open);

            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = $"deck_{id}.pdf"
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            response.Content.Headers.ContentLength = stream.Length;
            return response;
        }

        /// <summary>
        /// Get all the decks for a user
        /// /api/Decks?userId=xx
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetForUser(Guid userId)
        {
            using (var repository = new Repository())
            {
                var query = await repository.Context.Decks.Where(x=>x.Creator.Guid == userId).ToListAsync();
                var result = new ResultList<Deck>(query.Select(x => x.FromDal()).ToList());
                return Request.CreateResponse(result);
            }
        }



        [Authorize]
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] Deck deck)
        {
            var principal = HttpContext.Current.User as Principal;
            using (var repository = new Repository(principal.UserId))
            {

                if (string.IsNullOrWhiteSpace(deck.Name))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The name is required.");
                }
                var deckModel = repository.CreateDeck(deck.Name, Guid.NewGuid());
                return Request.CreateResponse(deckModel.FromDal());
            }
        }

        [HttpDelete]
        public async Task<HttpResponseMessage> Delete([FromUri] Guid id)
        {
            var principal = HttpContext.Current.User as Principal;
            return Request.CreateResponse(HttpStatusCode.NotImplemented);
        }

        [Authorize]
        [HttpPatch]
        public async Task<HttpResponseMessage> Patch([FromUri]Guid id, [FromBody] Deck deck)
        {

            var principal = HttpContext.Current.User as Principal;
            using (var repository = new Repository(principal.UserId))
            {

                await repository.Context.Factions.LoadAsync();
                await repository.Context.CardTypes.LoadAsync();
                var deckModel = await repository.Context.Decks.Include(x => x.DeckCards.Select(dc => dc.Card)).Where(x => x.Guid == id).FirstOrDefaultAsync();
                if (deckModel == null)
                {
                    Request.CreateResponse(HttpStatusCode.NotFound);
                }
                await repository.Context.Entry(deckModel).Reference(x => x.Creator).LoadAsync();
                if (deckModel.Creator.Guid != repository.ServiceUser.Guid)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                }
                deckModel.Patch(deck, repository.ServiceUser);
                deck = deckModel.FromDal(true);
                File.WriteAllText(Repository.GetDeckJsonFile(deck.Guid), JsonConvert.SerializeObject(deck));
                DeckGenerator.GenerateDeck(deck.Guid);

                await repository.Context.SaveChangesAsync();


                

                //BackgroundJob.Schedule(()=>DeckGenerator.GenerateDeck(deck.Guid))

                return Request.CreateResponse(deck);
            }
        }

    }
}
