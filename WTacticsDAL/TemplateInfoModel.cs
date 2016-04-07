using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsDAL
{
    public class TemplateInfoModel: ModelBase
    {
        [Key]
        public int TemplateInfoId { get; set; }
        
        public bool ShowName { get; set; }

        public bool ShowType { get; set; }

        public bool ShowFaction { get; set; }

        public bool ShowGoldCost { get; set; }

        public bool ShowLoyalty { get; set; }

        public bool ShowText { get; set; }

        public bool ShowAttack { get; set; }

        public bool ShowDefense { get; set; }

        public bool ShowDiscipline { get; set; }

        public bool ShowArt { get; set; }

        public bool ShowInfo { get; set; }

        public double MaxTextBoxWidth { get; set; }

        public double MaxTextBoxHeight { get; set; }
       
    }
}
