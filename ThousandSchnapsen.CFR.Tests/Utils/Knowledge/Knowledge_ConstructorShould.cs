using Xunit;
using ThousandSchnapsen.Common.Commons;
using ThousandSchnapsen.CFR.Utils;
using ThousandSchnapsen.Common.Utils;

namespace ThousandSchnapsen.CFR.Tests.Utils
{
    public class Knowledge_ConstructorShould
    {
        [Fact]
        public void Constructor_GivenInitParams_ProperlySetProperties()
        {
            var dealerCards = new CardsSet(new[]
            {
                new Card(Rank.Jack, Color.Diamonds),
                new Card(Rank.Ace, Color.Hearts),
                new Card(Rank.Nine, Color.Spades),
            });
            var initializerId = 0;
            var cardsToLet = new[]
            {
                (1, new Card(Rank.Jack, Color.Spades).CardId),
                (2, new Card(Rank.Nine, Color.Spades).CardId)
            };
            var expectedCardsLeft = new byte[] {8, 8, 8, 8};
            var expectedCertainCardsSets = new[]
            {
                new CardsSet(new[]
                {
                    new Card(Rank.Jack, Color.Diamonds),
                    new Card(Rank.Ace, Color.Hearts),
                }),
                new CardsSet(new[] {new Card(Rank.Jack, Color.Spades)}),
                new CardsSet(new[] {new Card(Rank.Nine, Color.Spades)}),
                new CardsSet(),
            };
            var expectedPossibleCardsSets = new CardsSet[4]
                .Populate(_ => CardsSet.Deck() - dealerCards - new CardsSet(new[] {new Card(Rank.Jack, Color.Spades)}));
            expectedPossibleCardsSets[3] = CardsSet.Deck();

            var result = new Knowledge(dealerCards, initializerId, cardsToLet);

            Assert.Equal(expectedCardsLeft, result.CardsLeft);
            Assert.Equal(expectedCertainCardsSets, result.CertainCardsSets);
            Assert.Equal(expectedPossibleCardsSets, result.PossibleCardsSets);
        }
    }
}