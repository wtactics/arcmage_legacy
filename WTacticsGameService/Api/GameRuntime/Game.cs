using System;
using System.Collections.Generic;

namespace WTacticsGameService.Api.GameRuntime
{
    public class Game
    {
        public DateTime LastAction { get; set; }

        public Guid Guid { get; set; }

        public string Name { get; set; }

        public bool CanJoin { get; set; }

        public DateTime? CreateTime { get; set; }

        public List<GamePlayer> Players { get; private set; }

        public List<GameCard> Cards { get; private set; }
        public bool IsStarted { get; set; }

        public Game()
        {
            Players = new List<GamePlayer>();
            Cards = new List<GameCard>();

        }

    }
}
