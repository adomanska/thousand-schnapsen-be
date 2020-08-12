using Xunit;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.CFR.Utils;
using ThousandSchnapsen.Common.States;

namespace ThousandSchnapsen.CFR.Tests.Utils
{
    public class InfoSet_RawDataShould
    {
        private readonly int[] _opponentsIds = {1, 2};
        private readonly byte[] _cardsLeft = {1, 2, 2};

        private static readonly CardsSet PlayerCards = new CardsSet(new[]
        {
            new Card(Rank.Jack, Color.Clubs),
            new Card(Rank.Nine, Color.Diamonds),
        });
        
        private readonly PlayerState _playerState = new PlayerState()
        {
            Cards = PlayerCards,
            PlayerId = 0,
            Stock = new StockItem[] {}
        };

        [Fact]
        public void RawData_EqualPossibleCardsSets_ReturnProperValue()
        {
            var commonPossibleCardsSet = new CardsSet(new[]
            {
                new Card(Rank.Jack, Color.Diamonds),
                new Card(Rank.Queen, Color.Diamonds),
                new Card(Rank.Jack, Color.Hearts),
                new Card(Rank.Ace, Color.Spades),
            });
            CardsSet[] possibleCardsSets =
            {
                PlayerCards,
                commonPossibleCardsSet,
                commonPossibleCardsSet.Clone()
            };
            CardsSet[] certainCardsSets =
            {
                new CardsSet(),
                new CardsSet(new[] {new Card(Rank.Ten, Color.Hearts)}),
                new CardsSet(new[] {new Card(Rank.Ace, Color.Hearts)}),
            };
            var infoSet = new InfoSet(
                _playerState,
                PlayerCards,
                _opponentsIds,
                possibleCardsSets,
                certainCardsSets,
                _cardsLeft
            );
            var expected = (
                PlayerCards.Code,
                (1, certainCardsSets[1].Code, certainCardsSets[2].Code)
            );

            var result = infoSet.RawData;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void RawData_NotEqualPossibleCardsSets_ReturnProperValue()
        {
            CardsSet[] possibleCardsSets =
            {
                PlayerCards,
                new CardsSet(new[] {new Card(Rank.Ace, Color.Spades)}),
                new CardsSet(new[] {new Card(Rank.Ace, Color.Diamonds)}),
            };
            CardsSet[] certainCardsSets =
            {
                new CardsSet(),
                new CardsSet(new[] {new Card(Rank.Ten, Color.Hearts)}),
                new CardsSet(new[] {new Card(Rank.Ace, Color.Hearts)}),
            };
            var infoSet = new InfoSet(
                _playerState,
                PlayerCards,
                _opponentsIds,
                possibleCardsSets,
                certainCardsSets,
                _cardsLeft
            );
            var expected = (
                PlayerCards.Code,
                (
                    1,
                    (certainCardsSets[1] | new CardsSet(new[] {new Card(Rank.Ace, Color.Spades)})).Code,
                    (certainCardsSets[2] | new CardsSet(new[] {new Card(Rank.Ace, Color.Diamonds)})).Code
                )
            );

            var result = infoSet.RawData;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void RawData_AllOpponentsCardsGiven_ReturnProperValue()
        {
            var commonPossibleCardsSet = new CardsSet(new[]
            {
                new Card(Rank.Jack, Color.Clubs),
                new Card(Rank.Queen, Color.Diamonds),
            });
            CardsSet[] possibleCardsSets =
            {
                PlayerCards,
                commonPossibleCardsSet,
                commonPossibleCardsSet.Clone(),
            };
            CardsSet[] certainCardsSets =
            {
                new CardsSet(),
                new CardsSet(new[] {new Card(Rank.Ten, Color.Hearts)}),
                new CardsSet(new[] {new Card(Rank.Ace, Color.Hearts), new Card(Rank.Nine, Color.Hearts)}),
            };
            var infoSet = new InfoSet(
                _playerState,
                PlayerCards,
                _opponentsIds,
                possibleCardsSets,
                certainCardsSets,
                _cardsLeft
            );
            var expected = (
                PlayerCards.Code,
                (
                    1,
                    (certainCardsSets[1] | new CardsSet(new[] {new Card(Rank.Queen, Color.Diamonds)})).Code,
                    certainCardsSets[2].Code
                )
            );

            var result = infoSet.RawData;

            Assert.Equal(expected, result);
        }
    }
}