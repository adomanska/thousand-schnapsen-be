using System;
using System.Linq;
using System.Text;

namespace ThousandSchnapsen.Common
{
    public class PublicState
    {
        public (int PlayerId, Card Card)[] Stock { get; set; }
        public CardsSet[] PlayersUsedCards { get; set; }
        public int[] PlayersPoints { get; set; }
        public Color[] TrumpsHistory { get; set; }
        public Color? Trump => TrumpsHistory.Length > 0 ? TrumpsHistory.Last() : (Color?)null;
        public int NextPlayerId { get; set; }
        public int DealerId { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("STOCK:");
            sb.AppendLine(String.Join("\n", Stock.Select(stockItem => $"{stockItem.PlayerId + 1}: {stockItem.Card}")));
            sb.AppendLine("\nRESULTS:");
            sb.AppendLine(String.Join('|', Enumerable.Range(1, PlayersPoints.Length).Select(id => String.Format("{0,4}", id))));
            sb.AppendLine(String.Join('|', PlayersPoints.Select(id => String.Format("{0,4}", id))));
            sb.AppendLine($"\nTRUMP: {Trump}");
            return sb.ToString();
        }
    }
}