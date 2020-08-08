using System;
using System.Linq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.Utils;

namespace ThousandSchnapsen.Common.States
{
    public class PublicState
    {
        public StockItem[] Stock { get; set; } = { };

        public CardsSet[] PlayersUsedCards { get; set; } = new CardsSet[Constants.PlayersCount]
            .Populate(_ => new CardsSet());

        public int[] PlayersPoints { get; set; } = new int[Constants.PlayersCount];
        public Color[] TrumpsHistory { get; set; } = { };
        public Color? Trump => TrumpsHistory.Length > 0 ? TrumpsHistory.Last() : (Color?) null;
        public int NextPlayerId { get; set; }
        public int DealerId { get; set; }
        public bool StockEmpty => Stock.Length == 0 || Stock.Length == Constants.PlayersCount - 1;

        public Color StockColor
        {
            get
            {
                if (StockEmpty)
                    throw new Exception("Stock color cannot be fetched when stock is empty");
                return Stock[0].Card.Color;
            }
        }

        public CardsSet DealerCards;
    }
}