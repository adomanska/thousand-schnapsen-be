
namespace ThousandSchnapsen.Common
{
    public class PlayerState : PublicState
    {
        public PlayerState(int[] stock, CardsSet[] playersUsedCards, int[] playersPoints, Color[] trumpsHistory,
                           int nextPlayerId, CardsSet cards, int playerId, int dealerId) :
                           base(stock, playersUsedCards, playersPoints, trumpsHistory, nextPlayerId, dealerId)
        {
            Cards = cards;
            PlayerId = playerId;
        }

        public int PlayerId { get; }
        public CardsSet Cards { get; }
    }
}