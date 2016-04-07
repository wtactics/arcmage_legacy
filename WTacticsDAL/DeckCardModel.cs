using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsDAL
{
    public class DeckCardModel : ModelBase
    {
        [Key]
        public int DeckCardId { get; set; }

        public int Quantity { get; set; }

        public DeckModel Deck { get; set; }

        public CardModel Card { get; set; }

        public string PdfCreationJobId { get; set; }
    }
}
