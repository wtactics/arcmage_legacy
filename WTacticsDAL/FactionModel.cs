using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsDAL
{
    public class FactionModel : ModelBase
    {
        [Key]
        public int FactionId { get; set; }

        public string Name { get; set; }
    }
}
