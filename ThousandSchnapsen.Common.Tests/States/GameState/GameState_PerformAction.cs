using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.Common.States;
using Xunit;

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
            var state = new GameState()
            {
                DealerId = DealerId,
                NextPlayerId = NextPlayerId,
                PlayersCards = _playersCards
            };
            var card = new Card(Rank.Queen, Color.Clubs);
            var action = new Action()
            {
                PlayerId = NextPlayerId,
                Card = card
            };

            var expectedStock = new[] {new StockItem(NextPlayerId, card)};
            var expectedPlayerCards = new CardsSet(new[] {new Card(Rank.King, Color.Clubs)});

            var resultState = state.PerformAction(action);

            Assert.Equal(expectedStock, resultState.Stock);
            Assert.Equal(Color.Clubs, resultState.Trump);
            Assert.Equal(expectedPlayerCards, resultState.PlayersCards[NextPlayerId]);
            Assert.Equal(60, resultState.PlayersPoints[NextPlayerId]);
        }

        [Fact]
        public void PerformAction_OneCardOnStock_MoveCard()
        {
            var state = new GameState()
            {
                DealerId = DealerId,
                NextPlayerId = NextPlayerId,
                PlayersCards = _playersCards,
                Stock = new[]
                {
                    new StockItem(NextPlayerId - 1, new Card(Rank.Jack, Color.Clubs))
                },
            };
            var card = new Card(Rank.Queen, Color.Clubs);
            var action = new Action()
            {
                PlayerId = NextPlayerId,
                Card = card
            };
            var expectedStock = new[]
            {
                new StockItem(NextPlayerId - 1, new Card(Rank.Jack, Color.Clubs)),
                new StockItem(NextPlayerId, card)
            };
            var expectedPlayerCards = new CardsSet(new[] {new Card(Rank.King, Color.Clubs)});

            var resultState = state.PerformAction(action);

            Assert.Equal(expectedStock, resultState.Stock);
            Assert.Equal(expectedPlayerCards, resultState.PlayersCards[NextPlayerId]);
        }

        [Fact]
        public void PerformAction_TwoCardsOnStockAndNoTrumps_MoveCardAndEvaluateTurn()
        {
            var state = new GameState()
            {
                DealerId = DealerId,
                NextPlayerId = NextPlayerId,
                PlayersCards = _playersCards,
                Stock = new[]
                {
                    new StockItem(3, new Card(Rank.Jack, Color.Clubs)),
                    new StockItem(0, new Card(Rank.Ace, Color.Hearts))
                }
            };
            var card = new Card(Rank.Queen, Color.Clubs);
            var action = new Action()
            {
                PlayerId = NextPlayerId,
                Card = card
            };
            var expectedStock = new[]
            {
                new StockItem(3, new Card(Rank.Jack, Color.Clubs)),
                new StockItem(0, new Card(Rank.Ace, Color.Hearts)),
                new StockItem(NextPlayerId, card)
            };
            var expectedPlayerCards = new CardsSet(new[] {new Card(Rank.King, Color.Clubs)});

            var resultState = state.PerformAction(action);

            Assert.Equal(expectedStock, resultState.Stock);
            Assert.Equal(expectedPlayerCards, resultState.PlayersCards[NextPlayerId]);
            Assert.Equal(new[] {0, 16, 0, 0}, resultState.PlayersPoints);
        }

        [Fact]
        public void PerformAction_TwoCardsOnStockAndActiveTrumps_MoveCardAndEvaluateTurn()
        {
            var state = new GameState()
            {
                DealerId = DealerId,
                NextPlayerId = NextPlayerId,
                PlayersCards = _playersCards,
                Stock = new[]
                {
                    new StockItem(3, new Card(Rank.Jack, Color.Clubs)),
                    new StockItem(0, new Card(Rank.Ace, Color.Hearts))
                },
                TrumpsHistory = new[]
                {
                    Color.Hearts
                }
            };
            var card = new Card(Rank.Queen, Color.Clubs);
            var action = new Action()
            {
                PlayerId = NextPlayerId,
                Card = card
            };
            var expectedStock = new[]
            {
                new StockItem(3, new Card(Rank.Jack, Color.Clubs)),
                new StockItem(0, new Card(Rank.Ace, Color.Hearts)),
                new StockItem(NextPlayerId, card)
            };
            var expectedPlayerCards = new CardsSet(new[] {new Card(Rank.King, Color.Clubs)});

            var resultState = state.PerformAction(action);

            Assert.Equal(expectedStock, resultState.Stock);
            Assert.Equal(expectedPlayerCards, resultState.PlayersCards[NextPlayerId]);
            Assert.Equal(new[] {16, 0, 0, 0}, resultState.PlayersPoints);
        }

        [Fact]
        public void PerformAction_InvalidPlayerId_ThrowException()
        {
            var state = new GameState()
            {
                DealerId = DealerId,
                NextPlayerId = NextPlayerId,
                PlayersCards = _playersCards,
            };
            var card = new Card(Rank.Queen, Color.Clubs);
            var action = new Action()
            {
                PlayerId = DealerId,
                Card = card
            };

            Assert.Throws<System.InvalidOperationException>(() => state.PerformAction(action));
        }
    }
}