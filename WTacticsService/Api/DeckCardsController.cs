using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WTacticsDAL;
using WTacticsLibrary;
using WTacticsLibrary.Assembler;
using WTacticsLibrary.Model;
using WTacticsService.Api.Authentication;

namespace WTacticsService.Api
{
    [Authorize]
    public class DeckCardsController : ApiController
    {

        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] DeckCard deckCard)
        {
            var principal = HttpContext.Current.User as Principal;
            using (var repository = new Repository(principal.UserId))
            {

                if (deckCard.Card == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The card is required.");
                }
                if (deckCard.Deck == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The deck is required.");
                }

                var deckModel = await repository.Context.Decks.FindByGuidAsync(deckCard.Deck.Guid);
                if (deckModel == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The deck is not found.");
                }

                var cardModel = await repository.Context.Cards.FindByGuidAsync(deckCard.Card.Guid);
                if (cardModel == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The card is not found.");
                }

                await repository.Context.Entry(deckModel).Reference(x => x.Creator).LoadAsync();
                if (repository.ServiceUser.Guid != deckModel.Creator.Guid)
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
                }

                var deckCardModel = await repository.Context.DeckCards.Include(x=>x.Deck).Include(x=>x.Card).FirstOrDefaultAsync(x => x.Card.CardId == cardModel.CardId && x.Deck.DeckId == deckModel.DeckId);
                if (deckCardModel == null)
                {
                    if (deckCard.Quantity > 0)
                    {
                        deckCardModel = repository.CreateDeckCard(deckModel, cardModel, deckCard.Quantity, Guid.NewGuid());
                    }
                    
                }
                else
                {
                    if (deckCard.Quantity <= 0)
                    {
                        deckModel.DeckCards.Remove(deckCardModel);
                        repository.Context.DeckCards.Remove(deckCardModel);
                        await repository.Context.SaveChangesAsync();
                    }
                    else
                    {
                        deckCardModel.Quantity = deckCard.Quantity;
                        await repository.Context.SaveChangesAsync();
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }
      
    }
}
