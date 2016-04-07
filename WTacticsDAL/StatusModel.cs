using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsDAL
{
    public class StatusModel : ModelBase
    {
        [Key]
        public int StatusId { get; set; }

        public string Name { get; set; }
    }
}
