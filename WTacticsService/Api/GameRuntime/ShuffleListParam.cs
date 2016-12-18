using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WTacticsService.Api.GameRuntime
{
    public class ShuffleListParam
    {
        public Guid PlayerGuid { get; set; }

        public ListType Kind { get; set; }
    }
}