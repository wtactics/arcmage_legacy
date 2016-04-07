using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsLibrary.Model
{
    public class DeckCard : Base
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public Card Card { get; set; }

        public Deck Deck { get; set; }
    }
}
