using System.Collections.Generic;
using System.Linq;

namespace ThousandSchnapsen.Common
{
    public class GameState : PublicState
    {
        public GameState(int[] stock, CardsSet[] playersUsedCards, int[] playersPoints, List<Color> trumpsHistory,
                         int nextPlayerId, CardsSet[] playersCards) :
                         base(stock, playersUsedCards, playersPoints, trumpsHistory, nextPlayerId)
        {
            PlayersCards = playersCards;
        }
        public CardsSet[] PlayersCards { get; }

        public PlayerState GetPlayerState(int playerId)
        {
            var stock = (int[])this.Stock.Clone();
            var playersUsedCards = (CardsSet[])this.PlayersUsedCards.Clone();
            var playersPoints = (int[])this.PlayersPoints.Clone();
            var trumpsHistory = new List<Color>(this.TrumpsHistory);
            var cards = new CardsSet(this.PlayersCards[playerId].Code);

            return new PlayerState(stock, playersUsedCards, playersPoints, trumpsHistory, this.NextPlayerId, cards, playerId);
        }
    }
}