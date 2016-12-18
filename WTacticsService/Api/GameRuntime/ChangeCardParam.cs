using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsService.Api.GameRuntime
{
    public class ChangeCardParam
    {
        public Guid CardId { get; set; }

        public bool? IsFaceDown { get; set; }

        public bool? IsDraggable { get; set; }

        public bool? IsMarked { get; set; }

        public double? Top { get; set; }

        public double? Left { get; set; }

        public int? CounterA { get; set; }

        public int? CounterB { get; set; }
    }
}
