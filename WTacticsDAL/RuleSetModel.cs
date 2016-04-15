using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsDAL
{
    public class RuleSetModel : ModelBase
    {
        [Key]
        public int RuleSetId { get; set; }

        public string Name { get; set; }
        
        public StatusModel Status { get; set; }
    }
}
