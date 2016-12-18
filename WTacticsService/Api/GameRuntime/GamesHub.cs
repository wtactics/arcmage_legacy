using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WTacticsService.Api.GameRuntime
{
    [HubName("games")]
    public class GamesHub : Hub
    {

        private readonly GameRepository _gameRepository;

        public GamesHub() :this(GameRepository.Instance)
        {
            
        }

        public GamesHub(GameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task JoinGame(Guid gameGuid, Guid playerGuid)
        {
            var gameAction = _gameRepository.Join(gameGuid, playerGuid);
            if ((bool)gameAction.ActionResult)
            {
                 await Groups.Add(Context.ConnectionId, gameGuid.ToString());
                _gameRepository.PushGameAction(gameAction);
            }
        }

        public Task LeaveGame(Guid gameGuid, Guid playerGuid)
        {
            var gameAction = new GameAction()
            {
                GameGuid = gameGuid,
                PlayerGuid = playerGuid,
                ActionType = GameActionType.LeaveGame,
            };
            _gameRepository.PushAction(gameAction);
            return Groups.Remove(Context.ConnectionId, gameGuid.ToString());
        }

        public void PushAction(GameAction gameAction)
        {
            _gameRepository.PushAction(gameAction);
        }
    }
}