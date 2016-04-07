using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTacticsDAL;
using WTacticsLibrary.Model;

namespace WTacticsLibrary.Assembler
{
    public static class DeckCardAssembler
    {
        public static DeckCard FromDal(this DeckCardModel deckCardModel, bool includeDeck = false)
        {
            if (deckCardModel == null) return null;
            var result = new DeckCard
            {
                Quantity = deckCardModel.Quantity,
                Card =  deckCardModel.Card.FromDal(),
            };
            if (includeDeck)
            {
                result.Deck = deckCardModel.Deck.FromDal();
            }

            return result.SyncBase(deckCardModel);
        }

        public static void Patch(this DeckCardModel deckCardModel, DeckCard deckCard, UserModel user)
        {
            if (deckCardModel == null) return;
            deckCardModel.Quantity = deckCard.Quantity;
            deckCardModel.Patch(user);
        }
    }
}
