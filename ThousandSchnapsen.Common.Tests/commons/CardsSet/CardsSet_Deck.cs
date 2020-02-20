using System;
using Xunit;

namespace ThousandSchnapsen.Common.Tests
{
    public class CardsSet_DeckShould
    {
        [Fact]
        public void Deck_Always_ReturnDeckCardSet()
        {
            int expectedCode = 0b111111111111111111111111;

            var deckCardsSet = CardsSet.Deck();

            Assert.Equal(expectedCode, deckCardsSet.Code);
        }
    }
}
