using ThousandSchnapsen.Common.Commons;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Commons
{
    public class CardsSet_DeckShould
    {
        [Fact]
        public void Deck_Always_ReturnDeckCardSet()
        {
            const int expected = 0b111111111111111111111111;

            var deckCardsSet = CardsSet.Deck();

            Assert.Equal(expected, deckCardsSet.Code);
        }
    }
}