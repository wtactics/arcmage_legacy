using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsDAL
{
    public class SerieModel : ModelBase
    {
        [Key]
        public int SerieId { get; set; }

        public string Name { get; set; }

        public UserModel Creator { get; set; }

        public UserModel LastModifiedBy { get; set; }

        public virtual List<CardModel> Cards { get; set; }

        public StatusModel Status { get; set; }

    }
}
