using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsDAL
{
    public class DeckModel : ModelBase
    {
        [Key]
        public int DeckId { get; set; }

        public string Name { get; set; }

        public string Pdf { get; set; }

        public virtual List<DeckCardModel> DeckCards { get; set; }

    }
}
