using System.Linq;

namespace ThousandSchnapsen.Common
{
    public class PublicState
    {
        public (int PlayerId, Card Card)[] Stock { get; set; }
        public CardsSet[] PlayersUsedCards { get; set; }
        public int[] PlayersPoints { get; set; }
        public Color[] TrumpsHistory { get; set; }
        public Color? Trump => TrumpsHistory.LastOrDefault();
        public int NextPlayerId { get; set; }
        public int DealerId { get; set; }
    }
}