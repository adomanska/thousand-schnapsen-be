using ThousandSchnapsen.Common.Commons;
using Xunit;

namespace ThousandSchnapsen.Common.Tests.Commons
{
    public class CardsSet_ContainsShould
    {
        [Fact]
        public void Contains_CardIdInSet_ReturnTrue()
        {
            var cardsSetA = new CardsSet(new int[] {1, 7, 20});
            const int cardId = 7;

            Assert.True(cardsSetA.Contains(cardId));
        }

        [Fact]
        public void Contains_CardInSet_ReturnTrue()
        {
            var cardsSetA = new CardsSet(new int[] {1, 7, 20});
            var card = new Card(7);

            Assert.True(cardsSetA.Contains(card));
        }

        [Fact]
        public void Contains_CardIdOutOfSet_ReturnFalse()
        {
            var cardsSetA = new CardsSet(new int[] {2, 8, 16});
            const int cardId = 7;

            Assert.False(cardsSetA.Contains(cardId));
        }

        [Fact]
        public void Contains_CardOutOfSet_ReturnFalse()
        {
            var cardsSetA = new CardsSet(new int[] {2, 8, 16});
            var card = new Card(7);

            Assert.False(cardsSetA.Contains(card));
        }
    }
}