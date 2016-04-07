using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsLibrary.Model
{
    public class Role : Base
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Right> Rights { get; set; }

        public Role()
        {
            Rights = new List<Right>();
        }
    }
}
