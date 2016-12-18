using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WTacticsGameService.Api.GameRuntime
{
    public class GameList
    {
        public List<GameCard> Cards { get; private set; }

        public ListType Kind { get; set; }

        public GameList()
        {
            Cards = new List<GameCard>();
        }
    }
}