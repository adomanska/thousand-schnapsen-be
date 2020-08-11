using Xunit;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.CFR.Utils;

namespace ThousandSchnapsen.CFR.Tests.Utils
{
    public class InfoSet_RawDataShould
    {
        private readonly int[] _opponentsIds = {1, 2};
        private readonly byte[] _cardsLeft = {1, 2, 2};

        private readonly CardsSet _playersCards = new CardsSet(new[]
        {
            new Card(Rank.Jack, Color.Clubs),
            new Card(Rank.Nine, Color.Diamonds),
        });

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
                _playersCards,
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
                _playersCards,
                _playersCards,
                _opponentsIds,
                possibleCardsSets,
                certainCardsSets,
                _cardsLeft,
                0,
                0
            );
            var expected = (
                _playersCards.Code,
                (0, certainCardsSets[1].Code, certainCardsSets[2].Code)
            );

            var result = infoSet.RawData;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void RawData_NotEqualPossibleCardsSets_ReturnProperValue()
        {
            CardsSet[] possibleCardsSets =
            {
                _playersCards,
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
                _playersCards,
                _playersCards,
                _opponentsIds,
                possibleCardsSets,
                certainCardsSets,
                _cardsLeft,
                0,
                0
            );
            var expected = (
                _playersCards.Code,
                (
                    0,
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
                _playersCards,
                commonPossibleCardsSet,
                commonPossibleCardsSet.Clone(),
            };
            CardsSet[] certainCardsSets =
            {
                new CardsSet(),
                new CardsSet(new[] {new Card(Rank.Ten, Color.Hearts) }),
                new CardsSet(new[] {new Card(Rank.Ace, Color.Hearts), new Card(Rank.Nine, Color.Hearts)}),
            };
            var infoSet = new InfoSet(
                _playersCards,
                _playersCards,
                _opponentsIds,
                possibleCardsSets,
                certainCardsSets,
                _cardsLeft,
                0,
                0
            );
            var expected = (
                _playersCards.Code,
                (
                    0,
                    (certainCardsSets[1] | new CardsSet(new[] {new Card(Rank.Queen, Color.Diamonds)})).Code,
                    certainCardsSets[2].Code
                )
            );

            var result = infoSet.RawData;

            Assert.Equal(expected, result);
        }
    }
}