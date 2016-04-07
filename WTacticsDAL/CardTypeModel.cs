using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsDAL
{
    public class CardTypeModel : ModelBase
    {
        [Key]
        public int CardTypeId { get; set; }

        public string Name { get; set; }

        public TemplateInfoModel TemplateInfo { get; set; }

    }
}
