using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsDAL
{
    public class RoleModel : ModelBase
    {
        [Key]
        public int RoleId { get; set; }

        public string Name { get; set; }

        public virtual List<RightModel> Rights { get; set; }
    }
}
