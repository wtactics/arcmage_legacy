using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Newtonsoft.Json;
using WTacticsLibrary;

namespace WTacticsGameService.Api.GameRuntime
{
    public class GameController
    {
       
        public Game Game { get; private set; }

        public GameController(Game game)
        {
            Game = game;
        }
        
        public GameAction ProcessAction(GameAction action)
        {
            Game.LastAction = DateTime.Now;
           
            var player = Game.GetPlayer(action.PlayerGuid);
            switch (action.ActionType)
            {
                case GameActionType.JoinGame:
                    action.ActionResult = JoinGameAction(player, action.PlayerGuid);
                    break;
                case GameActionType.LeaveGame:
                    action.ActionResult = LeaveGameAction(player, action.PlayerGuid);
                    break;
                case GameActionType.LoadDeck:
                    var loadDeckParam = JsonConvert.DeserializeObject<LoadDeckParam>(action.ActionData.ToString());
                    var loaded = LoadDeck(player, loadDeckParam);
                    // start the game if everything is loaded
                    var startaction = StartGame(action);
                    if (startaction != null) { action = startaction;}
                    break;
                case GameActionType.StartGame:
                    break;
                case GameActionType.ShuffleList:
                    var shuffleListParam = JsonConvert.DeserializeObject<ShuffleListParam>(action.ActionData.ToString());
                    action.ActionResult = ShuffleListAction(shuffleListParam);
                    break;
                case GameActionType.UpdateList:
                    var updateListParam = JsonConvert.DeserializeObject<UpdateListParam>(action.ActionData.ToString());
                    action.ActionResult = UpdateListAction(updateListParam);
                    break;
                case GameActionType.DrawCard:
                case GameActionType.DiscardCard:
                case GameActionType.PlayCard:
                case GameActionType.DeckCard:
                case GameActionType.RemoveCard:
                    var moveCardParam = JsonConvert.DeserializeObject<MoveCardParam>(action.ActionData.ToString());
                    action.ActionResult = MoveCardAction(moveCardParam);
                    action.ActionData = moveCardParam;
                    break;
                case GameActionType.ChangeCardState:
                    var changeCardParam = JsonConvert.DeserializeObject<ChangeCardParam>(action.ActionData.ToString());
                    action.ActionResult = ChangeCardState(changeCardParam);
                    break;
                case GameActionType.ChangePlayerStats:
                    var changePlayerStats =
                        JsonConvert.DeserializeObject<ChangePlayerStatsParam>(action.ActionData.ToString());
                    LimitResources(changePlayerStats);
                    LimitVictoryPoints(changePlayerStats);
                    action.ActionData = changePlayerStats;
                    break;
            }

            return action;

        }

       

        private bool ChangeCardState(ChangeCardParam state, bool includeLocation = true)
        {
            var card = Game.Cards.FirstOrDefault(x => x.Id == state.CardId);
            if (card != null)
            {
                if (state.IsMarked != null) card.IsMarked = state.IsMarked.Value;
                if (state.IsDraggable != null) card.IsDraggable = state.IsDraggable.Value;
                if (state.IsFaceDown != null) card.IsFaceDown = state.IsFaceDown.Value;
                if (state.CounterA != null) card.CounterA = Math.Max(0,state.CounterA.Value);
                if (state.CounterB != null) card.CounterB = Math.Max(0, state.CounterB.Value);

                if (state.Top != null && state.Left != null && includeLocation)
                {
                    UpdateCardLocation(card, state.Top.Value, state.Left.Value);
                }
            }
            return true;
        }

        private void UpdateCardLocation(GameCard card, double top, double left)
        {
            var isCardInPlay = Game.Players.SelectMany(x => x.Play.Cards).Any(x => x.Id == card.Id);
            if (isCardInPlay)
            {
                card.Top = top;
                card.Left = left;
            }
            else
            {
                card.Top = 0;
                card.Left = 0;
            }
        }

        private void LimitVictoryPoints(ChangePlayerStatsParam changePlayerStats)
        {
            changePlayerStats.VictoryPoints = Math.Max(0, Math.Min(changePlayerStats.VictoryPoints, 30));
        }

        private void LimitResources(ChangePlayerStatsParam changePlayerStats)
        {
            LimitResource(changePlayerStats.Resources.Black);
            LimitResource(changePlayerStats.Resources.Blue);
            LimitResource(changePlayerStats.Resources.Green);
            LimitResource(changePlayerStats.Resources.Red);
            LimitResource(changePlayerStats.Resources.Yellow);
        }

        private void LimitResource(GameResource resource)
        {
            resource.Available = Math.Max(0, Math.Min(resource.Available, 99));
            resource.Used = Math.Max(0, Math.Min(resource.Used, resource.Available));
        }

        private bool MoveCardAction(MoveCardParam moveParam)
        {
            var sourceList = Game.GetList(moveParam.FromPlayerGuid, moveParam.FromKind);
            var targetList = Game.GetList(moveParam.ToPlayerGuid, moveParam.ToKind);
            if (moveParam.CardId.HasValue)
            {
                var card = sourceList.Cards.FirstOrDefault(x => x.Id == moveParam.CardId.Value);
                if (card != null)
                {
                    sourceList.Cards.Remove(card);
                    targetList.Cards.Push(card);
                    moveParam.CardState.CardId = card.Id;
                }
            }
            else
            {
                var card = sourceList.Cards.Pop();
                if (card != null)
                {
                    targetList.Cards.Push(card);
                    moveParam.CardState.CardId = card.Id;
                }
            }


            if (moveParam.ToKind != ListType.Play)
            {
                moveParam.CardState.Left = 0;
                moveParam.CardState.Top = 0;
            }
            else
            {
                // if moving to play, center on battlefield
                if (!moveParam.CardState.Left.HasValue)
                {
                    moveParam.CardState.Left = 907;
                }
                if (!moveParam.CardState.Top.HasValue) { 
                    moveParam.CardState.Top = 800;
                }
            }
            ChangeCardState(moveParam.CardState);
            return true;
        }

        private List<GameCard> ShuffleListAction(ShuffleListParam shuffleListParam)
        {
            var list = Game.GetList(shuffleListParam.PlayerGuid, shuffleListParam.Kind).Cards;
            list.Shuffle();
            return list;
        }

        private List<GameCard> UpdateListAction(UpdateListParam updateListParam)
        {
            var list = Game.GetList(updateListParam.PlayerGuid, updateListParam.Kind).Cards;
            list.Clear();
            foreach (var gameCard in updateListParam.Cards)
            {
                var card = Game.Cards.FirstOrDefault(x => x.Id == gameCard.CardId);
                if (card != null)
                {
                    list.Add(card);
                    ChangeCardState(gameCard, false);
                }
            }
            return list;
        }


        private GameAction StartGame(GameAction action)
        {
            if (!Game.CanJoin && Game.Players.All(x => x.IsDeckLoaded))
            {
                Game.Players.ForEach(x=>Game.Cards.AddRange(x.Deck.Cards));
                Game.IsStarted = true;
                return new GameAction
                {
                    ActionType = GameActionType.StartGame,
                    PlayerGuid = action.PlayerGuid,
                    GameGuid = action.GameGuid,
                    ActionData = null,
                    ActionResult = Game
                };
            }
            return null;
        }

        private bool JoinGameAction(GamePlayer player, Guid playerGuid)
        {
            if (player == null && Game.CanJoin)
            {
                var playernumber = Game.Players.Count() + 1;
                var avatar = playernumber == 1 ? "player.png" : "opponent.png";
                player = new GamePlayer()
                {
                    PlayerGuid = playerGuid,
                    Name = "Player " + playernumber,
                    VictoryPoints = 15,
                    Avatar = avatar,
                };
                Game.Players.Add(player);
                Game.CanJoin = Game.Players.Count < 2;
                return true;
            }
            return false;
        }

        private bool LeaveGameAction(GamePlayer player, Guid playerGuid)
        {
            return true;
        }

        private bool LoadDeck(GamePlayer player, LoadDeckParam loadDeckParam)
        {
            if(player == null) return false;
            if(player.IsDeckLoaded) return true;

            using (var repository = new Repository())
            {
                repository.Context.Factions.Load();
                repository.Context.CardTypes.Load();
                var deckModel = repository.Context.Decks.Include(x => x.DeckCards.Select(dc => dc.Card)).Where(x => x.Guid == loadDeckParam.DeckGuid).FirstOrDefault();
                if (deckModel == null) return false;

                foreach (var deckCard in deckModel.DeckCards)
                {
                    for (int i = 0; i < deckCard.Quantity; i++)
                    {
                        player.Deck.Cards.Push(new GameCard()
                        {
                            Url = $"/WTactics/Cards/{deckCard.Card.Guid}/card.jpg",
                            IsFaceDown = true,
                            IsDraggable = true,
                            CounterA = 0,
                            CounterB = 0
                        });
                    }
                    
                }
                player.Deck.Cards.Shuffle();
                player.IsDeckLoaded = true;
                return true;

            }
        }

     

    
    }
}
