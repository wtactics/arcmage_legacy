using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WTacticsService.Api.GameRuntime
{
    public class GameRepository
    {

        private readonly static Lazy<GameRepository> _instance = new Lazy<GameRepository>(() => new GameRepository(GlobalHost.ConnectionManager.GetHubContext<GamesHub>().Clients));

        private readonly List<Game> Games = new List<Game>();

        private GameRepository(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
        }

        public static GameRepository Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        private readonly object _callLock = new object();

        public void PushAction(GameAction gameAction)
        {
            lock (_callLock)
            {
                var game = Games.FirstOrDefault(x => x.Guid == gameAction.GameGuid);
                if (game == null) return;
                if (game.Players.Any(x => x.PlayerGuid == gameAction.PlayerGuid))
                {
                    var gameController = new GameController(game);
                    var result = gameController.ProcessAction(gameAction);
                    PushGameAction(result);
                    if(gameAction.ActionType == GameActionType.LeaveGame)
                    {
                        Games.Remove(game);
                    }
                }
                
            }
        }

        public void PushGameAction(GameAction gameAction)
        {
            lock (_callLock)
            {
                var game = Games.FirstOrDefault(x => x.Guid == gameAction.GameGuid);
                if (game == null) return;
                Clients.Group(game.Guid.ToString()).processAction(gameAction);
            }
        }


        public GameAction Join(Guid gameGuid, Guid playerGuid)
        {
            lock (_callLock)
            {
                var gameAction = new GameAction()
                {
                    GameGuid = gameGuid,
                    PlayerGuid = playerGuid,
                    ActionType = GameActionType.JoinGame,
                    ActionResult = false,
                };
                var game = Games.FirstOrDefault(x => x.Guid == gameGuid);
                if (game != null)
                {
                    var gameController = new GameController(game);
                    gameAction = gameController.ProcessAction(gameAction);
                }
                return gameAction;
            }
        }


        public Game CreateGame(string name)
        {
            lock (_callLock)
            {
                var utcNow = DateTime.UtcNow;
                var game = new Game
                {
                    CreateTime = utcNow,
                    Guid = Guid.NewGuid(),
                    Name = name,
                    CanJoin = true,
                };
                Games.Add(game);
                return game;
            }
        }

        public List<Game> GetGames()
        {
            lock (_callLock)
            {
                return new List<Game>(Games.Select(x=> new Game(){
                    Guid = x.Guid,
                    Name = x.Name,
                    CreateTime = x.CreateTime,
                    CanJoin = x.CanJoin,
                }));
            }
        }


       
    }
}
