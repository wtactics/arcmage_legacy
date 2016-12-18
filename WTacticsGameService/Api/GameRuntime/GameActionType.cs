namespace WTacticsGameService.Api.GameRuntime
{
    public enum GameActionType
    {
        JoinGame,
        LeaveGame,
        LoadDeck,
        StartGame,

        ShuffleList,
        UpdateList,

        DrawCard,
        DiscardCard,
        DeckCard,
        PlayCard,
        RemoveCard,

        ChangeCardState,
        ChangePlayerStats,
        
    }
}
