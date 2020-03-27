using System.Linq;
using ThousandSchnapsen.Common.Commons;

namespace ThousandSchnapsen.Common.States
{
    public class PublicState
    {
        public StockItem[] Stock { get; set; } = { };

        public CardsSet[] PlayersUsedCards { get; set; } =
            new CardsSet[Constants.PlayersCount].Select(item => new CardsSet()).ToArray();

        public int[] PlayersPoints { get; set; } = new int[Constants.PlayersCount];
        public Color[] TrumpsHistory { get; set; } = { };
        public Color? Trump => TrumpsHistory.Length > 0 ? TrumpsHistory.Last() : (Color?) null;
        public int NextPlayerId { get; set; }
        public int DealerId { get; set; }
    }
}