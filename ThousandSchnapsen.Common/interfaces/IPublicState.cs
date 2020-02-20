namespace ThousandSchnapsen.Common
{
    public interface IPublicState
    {
        (int PlayerId, Card Card)[] Stock { get; }
        CardsSet[] PlayersUsedCards { get; }
        int[] PlayersPoints { get; }
        Color[] TrumpsHistory { get; }
        Color? Trump { get; }
        int NextPlayerId { get; }
        int DealerId { get; }
        bool GameFinished { get; }
    }
}