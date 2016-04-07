using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsDAL
{
    public class ModelBase
    {
        public Guid Guid { get; set; }

        public UserModel Creator { get; set; }
        
        public DateTime CreateTime { get; set; }

        public UserModel LastModifiedBy { get; set; }

        public DateTime LastModifiedTime { get; set; }

    }
}
