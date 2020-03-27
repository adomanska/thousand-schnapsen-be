using ThousandSchnapsen.Common.Commons;

namespace ThousandSchnapsen.Simulator.Dto
{
    public class StockItemDto
    {
        public int PlayerId { get; set; }
        public int CardId { get; set; }
    }

    public class StateDto
    {
        public StockItemDto[] Stock { get; set; }
        public int[] PlayersUsedCards { get; set; }
        public int[] PlayersPoints { get; set; }
        public Color[] TrumpsHistory { get; set; }
        public int NextPlayerId { get; set; }
        public int DealerId { get; set; }
        public int Cards { get; set; }
    }
}