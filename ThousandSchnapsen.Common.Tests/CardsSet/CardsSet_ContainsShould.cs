using Xunit;

namespace ThousandSchnapsen.Common.Tests
{
    public class CardsSet_ContainsShould
    {
        [Fact]
        public void Contains_CardIdInSet_ReturnTrue()
        {
            CardsSet A = new CardsSet(new int[] { 1, 7, 20});
            int cardId = 7;

            Assert.True(A.Contains(cardId));
        }

        [Fact]
        public void Contains_CardInSet_ReturnTrue()
        {
            CardsSet A = new CardsSet(new int[] { 1, 7, 20});
            Card card = new Card(7);

            Assert.True(A.Contains(card));
        }

        [Fact]
        public void Contains_CardIdOutOfSet_ReturnFalse()
        {
            CardsSet A = new CardsSet(new int[] { 2, 8, 16 });
            int cardId = 7;

            Assert.False(A.Contains(cardId));
        }

        [Fact]
        public void Contains_CardOutOfSet_ReturnFalse()
        {
            CardsSet A = new CardsSet(new int[] { 2, 8, 16 });
            Card card = new Card(7);

            Assert.False(A.Contains(card));
        }
    }
}
