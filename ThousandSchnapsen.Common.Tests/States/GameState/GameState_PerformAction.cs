using System;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;
using Xunit;
using Action = ThousandSchnapsen.Common.Commons.Action;

namespace ThousandSchnapsen.Common.Tests.States
{
    public class GameState_PerformActionShould
    {
        private const int NextPlayerId = 1;
        private const int DealerId = 2;

        private static readonly CardsSet PlayerCards = new CardsSet(new[]
        {
            new Card(Rank.Queen, Color.Clubs),
            new Card(Rank.King, Color.Clubs)
        });

        private readonly CardsSet[] _playersCards =
        {
            new CardsSet(),
            PlayerCards,
            new CardsSet(),
            new CardsSet()
        };

        [Fact]
        public void PerformAction_EmptyStockAndPlayerChecks_MoveCardAndAddNewTrump()
        {
            var state = new GameState(DealerId, NextPlayerId, _playersCards, new (int PlayerId, Card Card)[] { },
                new Color[] { });
            var card = new Card(Rank.Queen, Color.Clubs);
            var action = new Action(NextPlayerId, card);
            var expectedStock = new[] {(PlayerId: NextPlayerId, Card: card)};
            var expectedPlayerCards = new CardsSet(new[] {new Card(Rank.King, Color.Clubs)});

            state.PerformAction(action);

            Assert.Equal(expectedStock, state.Stock);
            Assert.Equal(Color.Clubs, state.Trump);
            Assert.Equal(expectedPlayerCards, state.PlayersCards[NextPlayerId]);
            Assert.Equal(60, state.PlayersPoints[NextPlayerId]);
        }

        [Fact]
        public void PerformAction_OneCardOnStock_MoveCard()
        {
            var state = new GameState(DealerId, NextPlayerId, _playersCards,
                new[] {(PlayerId: NextPlayerId - 1, Card: new Card(Rank.Jack, Color.Clubs))},
                new Color[] { });
            var card = new Card(Rank.Queen, Color.Clubs);
            var action = new Action(NextPlayerId, card);
            var expectedStock = new[]
            {
                (PlayerId: NextPlayerId - 1, Card: new Card(Rank.Jack, Color.Clubs)),
                (PlayerId: NextPlayerId, Card: card)
            };
            var expectedPlayerCards = new CardsSet(new[] {new Card(Rank.King, Color.Clubs)});

            state.PerformAction(action);

            Assert.Equal(expectedStock, state.Stock);
            Assert.Equal(expectedPlayerCards, state.PlayersCards[NextPlayerId]);
        }

        [Fact]
        public void PerformAction_TwoCardsOnStockAndNoTrumps_MoveCardAndEvaluateTurn()
        {
            var stock = new[]
            {
                (PlayerId: 3, Card: new Card(Rank.Jack, Color.Clubs)),
                (PlayerId: 0, Card: new Card(Rank.Ace, Color.Hearts))
            };
            var state = new GameState(DealerId, NextPlayerId, _playersCards, stock,
                new Color[] { });
            var card = new Card(Rank.Queen, Color.Clubs);
            var action = new Action(NextPlayerId, card);
            var expectedStock = new[]
            {
                (PlayerId: 3, Card: new Card(Rank.Jack, Color.Clubs)),
                (PlayerId: 0, Card: new Card(Rank.Ace, Color.Hearts)),
                (PlayerId: NextPlayerId, Card: card)
            };
            var expectedPlayerCards = new CardsSet(new[] {new Card(Rank.King, Color.Clubs)});

            state.PerformAction(action);

            Assert.Equal(expectedStock, state.Stock);
            Assert.Equal(expectedPlayerCards, state.PlayersCards[NextPlayerId]);
            Assert.Equal(new[] {0, 16, 0, 0}, state.PlayersPoints);
        }

        [Fact]
        public void PerformAction_TwoCardsOnStockAndActiveTrumps_MoveCardAndEvaluateTurn()
        {
            var stock = new[]
            {
                (PlayerId: 3, Card: new Card(Rank.Jack, Color.Clubs)),
                (PlayerId: 0, Card: new Card(Rank.Ace, Color.Hearts))
            };
            var state = new GameState(DealerId, NextPlayerId, _playersCards, stock,
                new[] {Color.Hearts});
            var card = new Card(Rank.Queen, Color.Clubs);
            var action = new Action(NextPlayerId, card);
            var expectedStock = new[]
            {
                (PlayerId: 3, Card: new Card(Rank.Jack, Color.Clubs)),
                (PlayerId: 0, Card: new Card(Rank.Ace, Color.Hearts)),
                (PlayerId: NextPlayerId, Card: card)
            };
            var expectedPlayerCards = new CardsSet(new[] {new Card(Rank.King, Color.Clubs)});

            state.PerformAction(action);

            Assert.Equal(expectedStock, state.Stock);
            Assert.Equal(expectedPlayerCards, state.PlayersCards[NextPlayerId]);
            Assert.Equal(new[] {16, 0, 0, 0}, state.PlayersPoints);
        }

        [Fact]
        public void PerformAction_InvalidPlayerId_ThrowException()
        {
            var state = new GameState(DealerId, NextPlayerId, _playersCards, new (int PlayerId, Card Card)[] { },
                new Color[] { });
            var card = new Card(Rank.Queen, Color.Clubs);
            var action = new Action(DealerId, card);

            Assert.Throws<InvalidOperationException>(() => state.PerformAction(action));
        }
    }
}