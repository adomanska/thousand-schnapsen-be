using System.Linq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.Interfaces;

namespace ThousandSchnapsen.Common.States
{
    public class PlayerState : IPlayerState
    {
        public (int PlayerId, Card Card)[] Stock { get; set; }
        public CardsSet[] PlayersUsedCards { get; set; }
        public int[] PlayersPoints { get; set; }
        public Color[] TrumpsHistory { get; set; }
        public Color? Trump => TrumpsHistory.Length > 0 ? TrumpsHistory.Last() : (Color?) null;
        public int NextPlayerId { get; set; }
        public int DealerId { get; set; }
        public int PlayerId { get; set; }
        public CardsSet Cards { get; set; }
        public bool GameFinished => (Stock.Length == Constants.PlayersCount - 1) && Cards.IsEmpty;
    }
}