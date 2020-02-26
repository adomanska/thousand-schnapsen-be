using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.Common.Interfaces
{
    public interface IGameState : IPublicState
    {
        CardsSet[] PlayersCards { get; }
        PlayerState GetPlayerState(int playerId);
        void PerformAction(Action action);
        Action[] GetAvailableActions();
    }
}