using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTacticsDAL;
using WTacticsLibrary.Model;

namespace WTacticsLibrary.Assembler
{
    public static class DeckAssembler
    {
        public static Deck FromDal(this DeckModel deckModel, bool includeCards = false)
        {
            if (deckModel == null) return null;
            var result = new Deck()
            {
                Id = deckModel.DeckId,
                Name = deckModel.Name,
            };
            if (includeCards)
            {
                deckModel.DeckCards.ForEach(x => result.DeckCards.Add(x.FromDal()));
            }
            result.SyncBase(deckModel, true, true);

            result.Pdf = $"/api/Decks/{deckModel.Guid}?format=PDf&modified={result.LastModifiedTime.Value.Ticks}";
            result.IsPdfAvailable = File.Exists(Repository.GetDeckFile(deckModel.Guid));

            return result;
        }

        public static void Patch(this DeckModel deckModel, Deck deck, UserModel user)
        {
            if (deckModel == null) return;
            deckModel.Name = deck.Name;
            deckModel.Patch(user);
        }
    }
}
