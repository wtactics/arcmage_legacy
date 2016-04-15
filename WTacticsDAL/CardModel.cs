using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsDAL
{
    public class CardModel : ModelBase
    {
        [Key]
        public int CardId { get; set; }
        
        // name
        public string Name { get; set; }

        // first line of name if split up
        public string FirstName { get; set; }

        // second line of name if split up
        public string LastName { get; set; }
        
        // artist
        public string Artist { get; set; }
       
        public string RuleText { get; set; }

        public string FlavorText { get; set; }

        public string SubType { get; set; }

        public CardTypeModel Type { get; set; }
        
        public FactionModel Faction { get; set; }

        public string Cost { get; set; }

        public int Loyalty { get; set; }

        public string Attack { get; set; }

        public string Defense { get; set; }
        
        public string Info { get; set; }
        
        public SerieModel Serie { get; set; }

        public RuleSetModel RuleSet { get; set; }

        public StatusModel Status { get; set; }

        // generation input
        public string LayoutText { get; set; }

        public string PngCreationJobId { get; set; }

    }
}
