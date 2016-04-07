using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsLibrary.Model
{
    public class CardType : Base
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public TemplateInfo TemplateInfo { get; set; }
    }
}
