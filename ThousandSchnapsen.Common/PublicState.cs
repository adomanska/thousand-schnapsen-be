using System.Collections.Generic;
using System.Linq;

namespace ThousandSchnapsen.Common
{
    public class PublicState
    {
        public PublicState(int[] stock, CardsSet[] playersUsedCards, int[] playersPoints, List<Color> trumpsHistory,
                           int nextPlayerId)
        {
            Stock = stock;
            PlayersUsedCards = playersUsedCards;
            PlayersPoints = playersPoints;
            TrumpsHistory = trumpsHistory;
            NextPlayerId = nextPlayerId;
        }

        public int[] Stock { get; }
        public CardsSet[] PlayersUsedCards { get; }
        public int[] PlayersPoints { get; }
        public List<Color> TrumpsHistory { get; }
        public Color? Trump => TrumpsHistory.LastOrDefault();
        public int NextPlayerId { get; }
    }
}