using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.Common.Interfaces
{
    public interface IAgent
    {
        public int PlayerId { get; }
        (int, byte)[] GetCardsToLet(PlayerState playerState);
        void Init((int, byte)[] cardsToLet, int initializerId, PublicState gameState);
        Action GetAction(PlayerState playerState, Card[] availableCards);
        void UpdateState(Action action, PublicState newState, bool trump);
    }
}