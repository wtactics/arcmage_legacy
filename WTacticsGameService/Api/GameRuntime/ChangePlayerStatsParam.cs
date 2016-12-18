using System;

namespace WTacticsGameService.Api.GameRuntime
{
    public class ChangePlayerStatsParam
    {
        public Guid PlayerGuid { get; set; }

        public int VictoryPoints { get; set; }

        public GameResources Resources { get; set; }
        
    }
}