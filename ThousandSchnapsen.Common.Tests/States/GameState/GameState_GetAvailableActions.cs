using System.Linq;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.States
{
    public class GameState_GetAvailableActionsShould
    {
        private const int NextPlayerId = 1;
        private const int DealerId = 0;

        private static readonly CardsSet PlayerCards = new CardsSet(new[]
        {
            new Card(Rank.Jack, Color.Clubs),
            new Card(Rank.Ace, Color.Diamonds)
        });

        private readonly CardsSet[] _playersCards =
        {
            new CardsSet(),
            PlayerCards,
            new CardsSet(),
            new CardsSet()
        };

        [Fact]
        public void GetAvailableActions_EmptyStock_ReturnAllActions()
        {
            var state = new GameState(DealerId, NextPlayerId, _playersCards, new (int PlayerId, Card Card)[] { },
                new Color[] { });
            var expected = PlayerCards.GetCards().Select(card => new Action(NextPlayerId, card)).ToArray();

            var result = state.GetAvailableActions();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetAvailableActions_NonEmptyStock_ReturnSuitableActions()
        {
            var state = new GameState(DealerId, NextPlayerId, _playersCards,
                new[] {(PlayerId: 1, Card: new Card(Rank.King, Color.Clubs))}, new Color[] { });
            var expected = new[] {new Action(NextPlayerId, new Card(Rank.Jack, Color.Clubs))};

            var result = state.GetAvailableActions();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetAvailableActions_NonEmptyStockAndNoSuitableActions_ReturnAllActions()
        {
            var state = new GameState(DealerId, NextPlayerId, _playersCards,
                new[] {(PlayerId: 1, Card: new Card(Rank.King, Color.Hearts))}, new Color[] { });
            var expected = PlayerCards.GetCards().Select(card => new Action(NextPlayerId, card)).ToArray();

            var result = state.GetAvailableActions();

            Assert.Equal(expected, result);
        }
    }
}