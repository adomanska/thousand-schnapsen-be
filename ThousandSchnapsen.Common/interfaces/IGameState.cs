namespace ThousandSchnapsen.Common
{
    public interface IGameState: IPublicState
    {
        CardsSet[] PlayersCards { get; }

        PlayerState GetPlayerState(int playerId);
        void PerformAction(Action action);
        Action[] GetAvailableActions();
    }
}