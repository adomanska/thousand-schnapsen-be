using ThousandSchnapsen.Common.Commons;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Commons
{
    public class CardsSet_GetHigherCardsSet
    {
        [Fact]
        public void GetHigherCardsSet_CardOfStockColorAndNoTrump_ReturnProperCardsSet()
        {
            var card = new Card(Rank.Jack, Color.Diamonds);
            var stockColor = Color.Diamonds;
            Color? trump = null;
            var expected = 0b000000111100000000000000;

            var result = CardsSet.GetHigherCardsSet(card, stockColor, trump);

            Assert.Equal(expected, result.Code);
        }
        
        [Fact]
        public void GetHigherCardsSet_CardOfStockColorAndTrump_ReturnProperCardsSet()
        {
            var card = new Card(Rank.Jack, Color.Diamonds);
            var stockColor = Color.Diamonds;
            Color? trump = Color.Hearts;
            var expected = 0b111111111100000000000000;

            var result = CardsSet.GetHigherCardsSet(card, stockColor, trump);

            Assert.Equal(expected, result.Code);
        }
        
        [Fact]
        public void GetHigherCardsSet_CardOfTrumpColor_ReturnProperCardsSet()
        {
            var card = new Card(Rank.Jack, Color.Diamonds);
            var stockColor = Color.Clubs;
            Color? trump = Color.Diamonds;
            var expected = 0b000000111100000000000000;

            var result = CardsSet.GetHigherCardsSet(card, stockColor, trump);

            Assert.Equal(expected, result.Code);
        }
        
        [Fact]
        public void GetHigherCardsSet_OtherCardAndNoTrump_ReturnProperCardsSet()
        {
            var card = new Card(Rank.Jack, Color.Diamonds);
            var stockColor = Color.Clubs;
            Color? trump = null;
            var expected = 0b000000000000111111000000;

            var result = CardsSet.GetHigherCardsSet(card, stockColor, trump);

            Assert.Equal(expected, result.Code);
        }
        
        [Fact]
        public void GetHigherCardsSet_OtherCardAndTrump_ReturnProperCardsSet()
        {
            var card = new Card(Rank.Jack, Color.Diamonds);
            var stockColor = Color.Clubs;
            Color? trump = Color.Hearts;
            var expected = 0b111111000000111111000000;

            var result = CardsSet.GetHigherCardsSet(card, stockColor, trump);

            Assert.Equal(expected, result.Code);
        }
    }
}