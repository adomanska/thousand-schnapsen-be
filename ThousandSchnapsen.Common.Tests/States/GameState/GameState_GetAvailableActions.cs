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
            new Card(Rank.Ace, Color.Diamonds),
            new Card(Rank.Nine, Color.Hearts)
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
            var state = new GameState()
            {
                DealerId = DealerId,
                NextPlayerId = NextPlayerId,
                PlayersCards = _playersCards
            };
            var expected = PlayerCards.GetCards()
                .Select(card => new Action()
                    {
                        PlayerId = NextPlayerId,
                        Card = card
                    }
                )
                .ToArray();

            var result = state.GetAvailableActions();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetAvailableActions_FullStock_ReturnAllActions()
        {
            var state = new GameState()
            {
                DealerId = DealerId,
                NextPlayerId = NextPlayerId,
                PlayersCards = _playersCards,
                Stock = new[]
                {
                    new StockItem(2, new Card(Rank.Ten, Color.Clubs)),
                    new StockItem(3, new Card(Rank.Queen, Color.Clubs)),
                    new StockItem(1, new Card(Rank.King, Color.Clubs)),
                },
            };
            var expected = PlayerCards.GetCards()
                .Select(card => new Action()
                    {
                        PlayerId = NextPlayerId,
                        Card = card
                    }
                )
                .ToArray();

            var result = state.GetAvailableActions();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetAvailableActions_NonEmptyStockAndHaveColorAndCannotBeat_ReturnSuitableActions()
        {
            var state = new GameState()
            {
                DealerId = DealerId,
                NextPlayerId = NextPlayerId,
                PlayersCards = _playersCards,
                Stock = new[]
                {
                    new StockItem(1, new Card(Rank.Ace, Color.Clubs))
                },
                TrumpsHistory = new[]
                {
                    Color.Hearts
                }
            };
            var expected = new[]
            {
                new Action()
                {
                    PlayerId = NextPlayerId,
                    Card = new Card(Rank.Jack, Color.Clubs)
                }
            };

            var result = state.GetAvailableActions();

            Assert.Equal(expected, result);
        }
        
        [Fact]
        public void GetAvailableActions_NonEmptyStockAndHaveColorAndCanBeat_ReturnSuitableActions()
        {
            var state = new GameState()
            {
                DealerId = DealerId,
                NextPlayerId = NextPlayerId,
                PlayersCards = _playersCards,
                Stock = new[]
                {
                    new StockItem(1, new Card(Rank.Queen, Color.Diamonds))
                },
                TrumpsHistory = new[]
                {
                    Color.Hearts
                }
            };
            var expected = new[]
            {
                new Action()
                {
                    PlayerId = NextPlayerId,
                    Card = new Card(Rank.Ace, Color.Diamonds)
                }
            };

            var result = state.GetAvailableActions();

            Assert.Equal(expected, result);
        }
        
        [Fact]
        public void GetAvailableActions_NonEmptyStockAndDontHaveColorAndCanBeat_ReturnSuitableActions()
        {
            var state = new GameState()
            {
                DealerId = DealerId,
                NextPlayerId = NextPlayerId,
                PlayersCards = _playersCards,
                Stock = new[]
                {
                    new StockItem(1, new Card(Rank.Queen, Color.Spades))
                },
                TrumpsHistory = new[]
                {
                    Color.Hearts
                }
            };
            var expected = new[]
            {
                new Action()
                {
                    PlayerId = NextPlayerId,
                    Card = new Card(Rank.Nine, Color.Hearts)
                }
            };

            var result = state.GetAvailableActions();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetAvailableActions_NonEmptyStockAndNoSuitableActions_ReturnAllActions()
        {
            var state = new GameState()
            {
                DealerId = DealerId,
                NextPlayerId = NextPlayerId,
                PlayersCards = _playersCards,
                Stock = new[]
                {
                    new StockItem(1, new Card(Rank.King, Color.Spades))
                }
            };
            var expected = PlayerCards.GetCards()
                .Select(card => new Action()
                {
                    PlayerId = NextPlayerId,
                    Card = card
                })
                .ToArray();

            var result = state.GetAvailableActions();

            Assert.Equal(expected, result);
        }
    }
}