using System.Text.Json.Serialization;
using ThousandSchnapsen.Common.Commons;

namespace ThousandSchnapsen.Common.Interfaces
{
    public interface IPublicState
    {
        StockItem[] Stock { get; }
        CardsSet[] PlayersUsedCards { get; }
        int[] PlayersPoints { get; }
        Color[] TrumpsHistory { get; }
        Color? Trump { get; }
        int NextPlayerId { get; }
        int DealerId { get; }
        bool GameFinished { get; }
    }
}