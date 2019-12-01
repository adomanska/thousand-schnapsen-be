using System.Linq;

namespace ThousandSchnapsen.Common
{
    public class PublicState
    {
        public PublicState(int[] stock, CardsSet[] playersUsedCards, int[] playersPoints, Color[] trumpsHistory,
                           int nextPlayerId, int dealerId)
        {
            Stock = stock;
            PlayersUsedCards = playersUsedCards;
            PlayersPoints = playersPoints;
            TrumpsHistory = trumpsHistory;
            NextPlayerId = nextPlayerId;
            DealerId = dealerId;
        }

        public int[] Stock { get; }
        public CardsSet[] PlayersUsedCards { get; }
        public int[] PlayersPoints { get; }
        public Color[] TrumpsHistory { get; }
        public Color? Trump => TrumpsHistory.LastOrDefault();
        public int NextPlayerId { get; }
        public int DealerId { get; }

    }
}