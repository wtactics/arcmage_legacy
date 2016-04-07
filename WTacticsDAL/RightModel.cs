using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsDAL
{
    public class RightModel : ModelBase
    {
        [Key]
        public int RightId { get; set; }

        public string Name { get; set; }

        public virtual List<RoleModel> Roles { get; set; }
    }
}
