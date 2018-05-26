using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsLibrary.Model
{
    public class CardSearchOptions : SearchOptionsBase
    {

        public string Cost { get; set; }

        public CardType CardType { get; set; }

        public Faction Faction { get; set; }

        public Serie Serie { get; set; }

        public RuleSet RuleSet { get; set; }

        public Status Status { get; set; }

        public int? Loyalty { get; set; }

    }
}
