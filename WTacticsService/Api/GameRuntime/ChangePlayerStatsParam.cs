using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WTacticsService.Api.GameRuntime
{
    public class ChangePlayerStatsParam
    {
        public Guid PlayerGuid { get; set; }

        public int VictoryPoints { get; set; }

        public GameResources Resources { get; set; }
        
    }
}