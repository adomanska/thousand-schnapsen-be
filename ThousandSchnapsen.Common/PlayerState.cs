using System.Collections.Generic;
using System.Linq;

namespace ThousandSchnapsen.Common
{
    public class PlayerState : PublicState
    {
        public PlayerState(int[] stock, CardsSet[] playersUsedCards, int[] playersPoints, List<Color> trumpsHistory,
                           int nextPlayerId, CardsSet cards, int playerId) :
                           base(stock, playersUsedCards, playersPoints, trumpsHistory, nextPlayerId)
        {
            Cards = cards;
            PlayerId = playerId;
        }

        public int PlayerId { get; }
        public CardsSet Cards { get; }
    }
}